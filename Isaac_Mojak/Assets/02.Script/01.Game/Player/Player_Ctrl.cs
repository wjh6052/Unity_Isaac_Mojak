using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;



public class Player_Ctrl : MonoBehaviour, IDamage
{
    #region ����
    [Header("����")]
    public int Hp = 6;  // ���� HP
    public int MaxHp = 24;     // �ִ� ä��

    public float Damage = 1;    // ������
    public float Range = 1;     // ��Ÿ�
    public float Speed = 1.5f;  // �̵� �ӵ�

    public float AttackSpeed = 0.5f;    // ���ݼӵ�
    float AttackSpeedTime = 0;

    public bool IsPlayerDie = false;
    #endregion


    #region ���� ������
    [Header("���� ������")]
    public int GoldCount = 50;  // ���� ���
    public int BombCount = 50;  // ���� ��ź
    public int KeyCount = 50;   // ���� ����
    public int MaxCount = 99;   // �ִ� ���� ����

    float BombDelay = 1.0f; // ��ź ������ �ð�
    float BombDelayTime = 0;


    // �������� ������ ����Ʈ
    public List<EItemType> PlayerHaveItem = new List<EItemType>();
    #endregion

    bool IsInvincible = false;
    float InvincibleTime = 0.75f;

    #region ������ ����
    [Header("������")]
    public float Acceleration = 20.0f;  // ���ӵ� (�������� �Է½� ���� �ӵ��� ����)
    public float Deceleration = 5f;    // ���ӵ� (�������� �Է��� ���� �� ��� �̲�����)
    public float StopThreshold = 0.01f; // �ּ� �ӵ� ���� (�� ���Ϸ� �������� ���� ��ġó��)

    Vector2 Velocity = Vector2.zero;    // ���������� �����ϴ� ���� �ӵ�
    #endregion


    #region ������
    public Tears_Ctrl Tears_Prefab; // ���� ������
    public PickUpItem_Bomb Bomb_Prefab;   // ��ź ������
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

    // �÷��̾��� ������
    void PlayerMove()
    {
        // ������
        Vector2 target = Vector2.zero;

        if (Mgr_Input.Inst.MoveDirection.sqrMagnitude > 0.0001f)
        {
            // �Է��� ���� ��: ��ǥ �ӵ��� ���� * �ִ�ӵ�
            target = Mgr_Input.Inst.MoveDirection * Speed;

            // ���� �ӵ��� ��ǥ �ӵ��� �����ؼ� ������Ű�� (���� �ִ� ����)
            Velocity = Vector2.MoveTowards(
                rb.velocity,
                target,
                Acceleration * Time.fixedDeltaTime
            );
        }
        else
        {
            // �Է��� ���� ��: 0���� ������ ���� �� �̲����� ����
            Velocity = Vector2.MoveTowards(
                rb.velocity,
                Vector2.zero,
                Deceleration * Time.fixedDeltaTime
            );

            // ���� ���� ���� ����ϰ� 0���� �����ؼ� ���� ����
            if (Velocity.magnitude < StopThreshold)
                Velocity = Vector2.zero;
        }

        rb.velocity = Velocity;


        // �ִϸ��̼�
        if (Mesh)
            Mesh.AnimPlayer(Velocity, Mgr_Input.Inst.AttackDirection);

    }

    // �÷��̾� ����
    void PlayerAttack()
    {
        if (AttackSpeed > AttackSpeedTime)
        {
            AttackSpeedTime += Time.fixedDeltaTime;
            return;
        }

        if (Mgr_Input.Inst.AttackDirection == Vector2.zero) return;


        AttackSpeedTime = 0;

        // ���� ����
        SpawnTears();

        // �ִϸ��̼�
        if (Mesh)
            Mesh.TearsAttack(Mgr_Input.Inst.AttackDirection);
    }

    // ���� ����
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

    // ��ź ���
    public void OnUseBomb()
    {
        if (IsPlayerDie) return;
        if (BombCount <= 0) return; // ����� ��ź�� ���ٸ� ����
        if (BombDelayTime < BombDelay) return;

        if (Bomb_Prefab == null)
        {
            Bomb_Prefab = Resources.Load("Item/PickUp/Item_Bomb_Prefab").GetComponent<PickUpItem_Bomb>();

            if (Bomb_Prefab == null) return;
        }

        PickUpItem_Bomb bomb = Instantiate(Bomb_Prefab);
        Vector2 spawnPos = (Mesh.A_Head.gameObject.transform.position + Mesh.A_Body.gameObject.transform.position) * 0.5f;
        bomb.SpawnBomb(spawnPos, false);

        BombCount--; // ��ź ���� ���̱�
        BombDelayTime -= BombDelay; // ������ �ð� �ֱ�

        Mgr_GameUI.Inst.GameUIUpdate();
    }


    // �������� ȹ��
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



    // �������� �޴� �Լ�
    public void TakeDamage(float inDamage, EDamageType inDamageType = EDamageType.Normal)
    {
        if (IsPlayerDie) return;
        if (IsInvincible) return; // ������ ��� ������ ����
        if (inDamage < 0) return; // �������� ������ ��� ���� ��������


        Hp--;


        // �׾�����
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

    // �������� �޾�����
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

    // �˹�
    public void KnockBack(Vector3 inHitPos, float inPower)
    {
        // ������Ʈ���� �Ÿ�
        float dist = Vector2.Distance(this.transform.position, inHitPos);

        Vector3 direction = (this.transform.position - inHitPos).normalized;

        GetComponent<Rigidbody2D>().AddForce((direction * inPower), ForceMode2D.Impulse);
    }

    // ��Ʈ�� �Ծ����� ��
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

    // ���� ������ �ִ� �������� Ȯ��
    public bool CanHeal()
    {
        return Hp <= MaxHp;
    }

}
