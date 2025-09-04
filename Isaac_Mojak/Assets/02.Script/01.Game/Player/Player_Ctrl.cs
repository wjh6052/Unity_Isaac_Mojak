using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;



public class Player_Ctrl : MonoBehaviour, IDamage
{
    #region 스텟
    [Header("스텟")]
    public int Hp = 6;  // 현재 HP
    public int MaxHp = 24;     // 최대 채력

    public float Damage = 1;    // 데미지
    public float Range = 1;     // 사거리
    public float Speed = 1.5f;  // 이동 속도

    public float AttackSpeed = 0.5f;    // 공격속도
    float AttackSpeedTime = 0;

    public bool IsPlayerDie = false;
    #endregion


    #region 보유 아이템
    [Header("보유 아이템")]
    public int GoldCount = 50;  // 보유 골드
    public int BombCount = 50;  // 보유 폭탄
    public int KeyCount = 50;   // 보유 열쇠
    public int MaxCount = 99;   // 최대 보유 개수

    float BombDelay = 1.0f; // 폭탄 딜레이 시간
    float BombDelayTime = 0;


    // 보유중인 아이템 리스트
    public List<EItemType> PlayerHaveItem = new List<EItemType>();
    #endregion

    bool IsInvincible = false;
    float InvincibleTime = 0.75f;

    #region 움직임 변수
    [Header("움직임")]
    public float Acceleration = 20.0f;  // 가속도 (높을수록 입력시 빨리 속도가 붙음)
    public float Deceleration = 5f;    // 감속도 (낮을수록 입력을 떼고도 더 길게 미끄러짐)
    public float StopThreshold = 0.01f; // 최소 속도 스냅 (값 이하로 느려지면 완전 정치처리)

    Vector2 Velocity = Vector2.zero;    // 내부적으로 관리하는 현재 속도
    #endregion


    #region 프리팹
    public Tears_Ctrl Tears_Prefab; // 눈물 프리팹
    public PickUpItem_Bomb Bomb_Prefab;   // 폭탄 프리팹
    #endregion



    public PlayerMesh_Ctrl Mesh;
    public Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0.0f;

        IsPlayerDie = false;
    }

    private void Start()
    {
        Mgr_GameUI.Inst.GameUIUpdate();

        BombDelayTime = BombDelay;
    }

    private void Update()
    {
        if (IsPlayerDie) return;

        if (BombDelayTime < BombDelay)
            BombDelayTime += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (IsPlayerDie) return;
        if (GlobalValue.IsCanPlay == false) return;

        PlayerMove();
        PlayerAttack();
    }

    // 플레이어의 움직임
    void PlayerMove()
    {
        // 움직임
        Vector2 target = Vector2.zero;

        if (Mgr_Input.Inst.MoveDirection.sqrMagnitude > 0.0001f)
        {
            // 입력이 있을 때: 목표 속도는 방향 * 최대속도
            target = Mgr_Input.Inst.MoveDirection * Speed;

            // 현재 속도를 목표 속도로 가속해서 근접시키기 (관성 있는 느낌)
            Velocity = Vector2.MoveTowards(
                rb.velocity,
                target,
                Acceleration * Time.fixedDeltaTime
            );
        }
        else
        {
            // 입력이 없을 때: 0으로 서서히 감속 → 미끄러짐 느낌
            Velocity = Vector2.MoveTowards(
                rb.velocity,
                Vector2.zero,
                Deceleration * Time.fixedDeltaTime
            );

            // 아주 느린 값은 깔끔하게 0으로 스냅해서 떨림 방지
            if (Velocity.magnitude < StopThreshold)
                Velocity = Vector2.zero;
        }

        rb.velocity = Velocity;


        // 애니메이션
        if (Mesh)
            Mesh.AnimPlayer(Velocity, Mgr_Input.Inst.AttackDirection);

    }

    // 플레이어 공격
    void PlayerAttack()
    {
        if (AttackSpeed > AttackSpeedTime)
        {
            AttackSpeedTime += Time.fixedDeltaTime;
            return;
        }

        if (Mgr_Input.Inst.AttackDirection == Vector2.zero) return;


        AttackSpeedTime = 0;

        // 눈물 스폰
        SpawnTears();

        // 애니메이션
        if (Mesh)
            Mesh.TearsAttack(Mgr_Input.Inst.AttackDirection);
    }

    // 눈물 스폰
    void SpawnTears()
    {
        if (IsPlayerDie) return;
        if (Tears_Prefab == null)
            Tears_Prefab = Resources.Load("Tears/Tears_Prefab").GetComponent<Tears_Ctrl>();

        Vector2 direction = new Vector2();
        switch (Mesh.HeadValue)
        {
            case EDirection.Up:
                direction.y = 1;
                break;
            case EDirection.Down:
                direction.y = -1;
                break;
            case EDirection.Right:
                direction.x = 1;
                break;
            case EDirection.left:
                direction.x = -1;
                break;
        }

        Tears_Ctrl tears = Instantiate(Tears_Prefab);
        tears.SetTears(Mesh.A_Head.transform.position, direction, Damage, Range);
    }

    // 폭탄 사용
    public void OnUseBomb()
    {
        if (IsPlayerDie) return;
        if (BombCount <= 0) return; // 사용할 폭탄이 없다면 리턴
        if (BombDelayTime < BombDelay) return;

        if (Bomb_Prefab == null)
        {
            Bomb_Prefab = Resources.Load("Item/PickUp/Item_Bomb_Prefab").GetComponent<PickUpItem_Bomb>();

            if (Bomb_Prefab == null) return;
        }

        PickUpItem_Bomb bomb = Instantiate(Bomb_Prefab);
        Vector2 spawnPos = (Mesh.A_Head.gameObject.transform.position + Mesh.A_Body.gameObject.transform.position) * 0.5f;
        bomb.SpawnBomb(spawnPos, false);

        BombCount--; // 폭탄 갯수 줄이기
        BombDelayTime -= BombDelay; // 딜레이 시간 넣기

        Mgr_GameUI.Inst.GameUIUpdate();
    }


    // 아이템을 획득
    public void AddPickUpItem(EPickUpItemType inItemType, int addNum)
    {
        if (IsPlayerDie) return;
        switch (inItemType)
        {
            case EPickUpItemType.Gold:
                {
                    if (GoldCount + addNum < MaxCount)
                        GoldCount += addNum;
                    break;
                }
                
            case EPickUpItemType.Bomb:
                {
                    if (BombCount + addNum < MaxCount)
                        BombCount += addNum;
                    break;
                }
                
            case EPickUpItemType.Key:
                {
                    if (KeyCount + addNum < MaxCount)
                        KeyCount += addNum;
                    break;
                }
            case EPickUpItemType.Heart:
                {
                    Heal(addNum);
                    break;
                }
        }

        Mgr_GameUI.Inst.GameUIUpdate();

    }


    public void AddItem(EItemType addItemType)
    {
        Mgr_Game.Inst.AddItemStets(addItemType);
        PlayerHaveItem.Add(addItemType);



        Mesh.AnimAddItem(addItemType);
    }



    // 데미지를 받는 함수
    public void TakeDamage(float inDamage, EDamageType inDamageType = EDamageType.Normal)
    {
        if (IsPlayerDie) return;
        if (IsInvincible) return; // 무적일 경우 데미지 무시
        if (inDamage < 0) return; // 데미지가 음수인 경우 리턴 들어왔을때


        Hp--;


        // 죽었을때
        if(Hp <= 0)
        {
            PlayerDie();
        }
        else
        {
            PlayerHit();
        }

        Mgr_GameUI.Inst.GameUIUpdate();
    }

    // 데미지를 받았을때
    void PlayerHit()
    {
        IsInvincible = true;
        StartCoroutine(Co_AnimHit());
    }

    IEnumerator Co_AnimHit()
    {
        Mesh.AnimHit(true);

        yield return new WaitForSeconds(InvincibleTime);

        IsInvincible = false;
        Mesh.AnimHit(IsInvincible);
    }

    void PlayerDie()
    {
        IsInvincible = true;
        IsPlayerDie = true;

        Mesh.AnimDie();
    }

    // 넉백
    public void KnockBack(Vector3 inHitPos, float inPower)
    {
        // 오브젝트와의 거리
        float dist = Vector2.Distance(this.transform.position, inHitPos);

        Vector3 direction = (this.transform.position - inHitPos).normalized;

        GetComponent<Rigidbody2D>().AddForce((direction * inPower), ForceMode2D.Impulse);
    }

    // 하트를 먹었을때 힐
    public void Heal(int inHeal)
    {
        if(Hp + inHeal >= MaxHp)
        {
            Hp = MaxHp;
        }
        else
        {
            Hp += inHeal;
        }

        Mgr_GameUI.Inst.GameUIUpdate();
    }

    // 힐을 먹을수 있는 상태인지 확인
    public bool CanHeal()
    {
        return Hp <= MaxHp;
    }

}
