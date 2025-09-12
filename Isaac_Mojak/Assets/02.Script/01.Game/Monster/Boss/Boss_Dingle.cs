using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class Boss_Dingle : Boss_Ctrl
{
    enum EDingleState
    {
        Idle,
        Die,
        Desh,
        Shoot
    }
    EDingleState DingleState = EDingleState.Idle;

    public float DeshCoolDown = 7f;
    public float DeshCoolDownTime = 0f;
    bool IsDesh = false;

    public float ShootCoolDown = 2f;
    public float ShootCoolDownTime = 2f;

    public int DeshCount = 4;
    int NowDeshCount = 0;

    protected override void Awake()
    {
        base.Awake();

        BossType = EBossType.Dingle;
    }


    protected override void Update()
    {
        base.Update();

        if (IsDie) return;  // �׾����� ����

        if (DingleState == EDingleState.Idle)
        {
            DeshCoolDownTime += Time.deltaTime;
            ShootCoolDownTime += Time.deltaTime;

            if(DeshCoolDown <= DeshCoolDownTime)
            {
                DeshCoolDownTime = 0;
                StartDesh();
            }
            else if (ShootCoolDown <= ShootCoolDownTime)
            {
                ShootCoolDownTime = 0;
                Shoot();
            }
        }
    }


    void StartDesh()
    {
        if (IsDie) return;  // �׾����� ����

        if (DingleState != EDingleState.Idle) return;

        DingleState = EDingleState.Desh;

        DeshCount = Random.Range(2, 5);

        Anim?.SetTrigger("DeshStart");
    }


    void Shoot()
    {
        if (IsDie) return;  // �׾����� ����

        DingleState = EDingleState.Shoot;

        Anim?.SetTrigger("Shoot");
    }

    


    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsDie) return;  // �׾����� ����
        if (collision.gameObject.tag != "Player") return; // �÷��̾ �ƴѰ��

        if (IsDesh)
        {
            Rb.excludeLayers = 1 << LayerMask.NameToLayer("Player");
        }
        PlayerHit();
    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        if (IsDie) return;  // �׾����� ����
        if (collision.gameObject.tag != "Player") return; // �÷��̾ �ƴѰ��

        //if (IsDesh)
        //{
        //    Rb.excludeLayers = 0;
        //}
    }

    public override void KnockBack(Vector3 inHitPos, float inPower)
    {

    }


    #region �ִϸ��̼� �̺�Ʈ �Լ�

    void AE_Desh()
    {
        if (IsDie) return;  // �׾����� ����

        NowDeshCount++;
        if (DeshCount >= NowDeshCount) // �뽬 Ƚ���� ������ ��
        {
            IsDesh = true;
            Vector3 direction = (Mgr_Game.Inst.Player.transform.position - this.transform.position).normalized;
            Rb.AddForce((direction * 50), ForceMode2D.Impulse);

            Anim?.SetTrigger("Desh");
        }
        else
        {
            Anim?.SetTrigger("DeshGroggy");
        }
    }

    void AE_DeshEnd()
    {
        if (IsDie) return;  // �׾����� ����

        Rb.velocity = Vector3.zero;
        IsDesh = false;
        Rb.excludeLayers = 0;

        if (DeshCount >= NowDeshCount) // �뽬 Ƚ���� ������ ��
        {
            Anim?.SetTrigger("DeshDelay");
        }
        else
        {
            Anim?.SetTrigger("DeshGroggy");
        }
    }

    void AE_DeshGroggy()
    {
        if (IsDie) return;  // �׾����� ����

        NowDeshCount = 0;
        DingleState = EDingleState.Idle;
    }

    void AE_Shoot()
    {
        if (IsDie) return;  // �׾����� ����

        Rb.velocity = Vector3.zero;

        Tears_Ctrl tears_Prefab = Resources.Load("Tears/Tears_Prefab").GetComponent<Tears_Ctrl>();

        Vector3 direction = (Mgr_Game.Inst.Player.transform.position - this.transform.position).normalized;

        Tears_Ctrl tears = Instantiate(tears_Prefab);
        tears.SetTears(transform.position, direction, 1, 20, false);

        Vector3 right = ShootRotate(direction, +15f);
        tears = Instantiate(tears_Prefab);
        tears.SetTears(transform.position, right, 1, 20, false);

        Vector3 left = ShootRotate(direction, -15f);
        tears = Instantiate(tears_Prefab);
        tears.SetTears(transform.position, left, 1, 20, false);


        DingleState = EDingleState.Idle;

    }

    #endregion

    Vector3 ShootRotate(Vector3 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        return new Vector3(v.x * cos - v.y * sin, v.x * sin, v.y * cos + v.z);
    }
}
