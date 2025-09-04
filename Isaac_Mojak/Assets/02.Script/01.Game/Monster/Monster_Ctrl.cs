using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.AI;




public class Monster_Ctrl : MonoBehaviour, IDamage
{
    Room_Ctrl Room_Onwer;


    public float MaxHp = 6;  // MaxHP
    public float Hp = 6;  // HP
    public float Speed = 1f;  // 이동 속도
    public float ArriveDist = 0.03f;  // 도착 판정 거리

    public float HitDelay = 0.3f;
    protected float HitDelayTime = 0.3f;
    public bool IsHit = false;
    public bool IsDie = false;

    public bool IsBoss = false;


    protected NavMeshAgent Agent;
    protected Animator Anim;
    protected Rigidbody2D Rb;
    protected Collider2D Col;


    protected virtual void Awake()
    {
        // 네비메쉬
        {
            Agent = GetComponent<NavMeshAgent>();
            Agent.updateRotation = false;
            Agent.updateUpAxis = false;

            Agent.speed = Speed;
        }


        Col = GetComponent<Collider2D>();
        Rb = GetComponent<Rigidbody2D>();

        // 애니메이션
        Anim = GetComponent<Animator>();


        IsDie = false;

        Vector3 pos = this.transform.position;
        pos.z = -1;
        this.transform.position = pos;

        Hp = MaxHp;
    }

    protected virtual void Start()
    {
        Room_Onwer = transform.parent?.parent?.parent?.gameObject.GetComponent<Room_Ctrl>();
        if(Room_Onwer) Room_Onwer.AddMonster(this);

    }


    protected virtual void Update()
    {
        if (!GlobalValue.IsCanPlay) return;

        if (IsHit)
        {
            if (HitDelay > HitDelayTime)
                HitDelayTime += Time.deltaTime;
            else
            {
                HitDelayTime -= HitDelayTime;
                MonsterHit(false);
            }

        }
            
        MonsterMove();
    }

    protected virtual void MonsterMove()
    {
        
    }

    // 죽었을때
    protected virtual void MonsterDie()
    {
        if (Anim == null) Anim = GetComponent<Animator>();
        if (Col == null) Col = GetComponent<Collider2D>();

        IsDie = true;

        Col.enabled = false;
        Rb.bodyType = RigidbodyType2D.Static;
        Anim?.SetTrigger("Die");
        Room_Onwer?.DieMonster(this);
    }

    protected virtual void MonsterHit(bool isHit)
    {
        if (Anim == null) Anim = GetComponent<Animator>();

        IsHit = isHit;

        Anim.SetBool("Hit", isHit);

        if (isHit)
            HitDelayTime = 0;
    }


    // 죽는 애니메이션에서 호출될 삭제함수
    public virtual void AE_MonsterDieEnd()
    {
        Destroy(this.gameObject);
    }

    
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsDie) return;  // 죽었을때 리턴
        if (collision.gameObject.tag != "Player") return; // 플레이어가 아닌경우 무시

        PlayerHit();
    }

    protected virtual void PlayerHit()
    {
        // 플레이어에게 데미지 넣기
        Mgr_Game.Inst.Player.TakeDamage(1, EDamageType.Normal);

        // 넉백
        Mgr_Game.Inst.Player.KnockBack(this.transform.position, 40);
    }


    // 데미지 처리
    public virtual void TakeDamage(float inDamage, EDamageType inDamageType)
    {
        if (IsDie) return;
        if (inDamage < 0) return; // 데미지가 음수인 경우 리턴 들어왔을때

        Hp -= inDamage;

        // 죽었을때
        if (Hp < 0)
        {
            MonsterDie();
            return;
        }

        MonsterHit(true);
    }

    // 넉백
    public virtual void KnockBack(Vector3 inHitPos, float inPower)
    {
        // 오브젝트와의 거리
        float dist = Vector2.Distance(this.transform.position, inHitPos);

        Vector3 direction = (this.transform.position - inHitPos).normalized;

        Rb.AddForce((direction * inPower), ForceMode2D.Impulse);
    }

}
