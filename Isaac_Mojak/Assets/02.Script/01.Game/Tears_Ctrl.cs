using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tears_Ctrl : MonoBehaviour
{
    Vector2 StartPos;
    Vector2 Direction;

    float Damage;
    float Range;
    float Speed = 3.0f;          // �ӵ�

    //float FallSpeed = 1f;      // ������ �� y�ӵ� (�������� ������ ������)

    public AudioClip[] TearsStart_Audio = new AudioClip[2];
    public AudioClip[] TearsEnd_Audio = new AudioClip[3];

    bool IsPlayer = false;
    bool IsEnd = false;

    Rigidbody2D rb;
    Animator Anim;

    public void SetTears(Vector3 spawnPos, Vector2 inDirection, float inDamage, float inRange, bool isPlayer = true)
    {
        Direction = inDirection.normalized; // ����

        spawnPos.x += Direction.x * 0.18f;
        spawnPos.y += Direction.y * 0.18f;
        spawnPos.z = -2;
        this.transform.position = spawnPos;

        StartPos = transform.position;
        
        Damage = inDamage;
        Range = inRange;

        IsPlayer = isPlayer;
        IsEnd = false;

        rb = GetComponent<Rigidbody2D>();
        rb.velocity = Direction * Speed;

        Mgr_Sound.Inst.PlaySound(this.gameObject, TearsStart_Audio[Random.Range(0, TearsStart_Audio.Length)]);
    }


    private void Update()
    {
        // �̵��Ÿ� ���
        float dist = Vector2.Distance(StartPos, this.transform.position);

        if (dist >= Range)
            EndTrigger();

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsEnd) return;

        switch(collision.tag)
        {
            case "TearsWall": // ���� ��
                {
                    EndTrigger();
                    break;
                }
            case "BombExplosion": // ������ ��ź
                {
                    EndTrigger();
                    collision.GetComponent<PickUpItem_Bomb>().GetComponent<Rigidbody2D>().AddForce((Direction * 5), ForceMode2D.Impulse);
                    break;
                }
            case "Player":  // �÷��̾�
                {
                    if (IsPlayer) break;
                    EndTrigger();
                    collision.GetComponent<IDamage>().TakeDamage(Damage, EDamageType.Normal);
                    collision.GetComponent<IDamage>().KnockBack(this.transform.position, 50);
                    break;
                }
            case "Monster": // ����
                {
                    if (!IsPlayer) break;
                    EndTrigger();
                    collision.GetComponent<IDamage>().TakeDamage(Damage, EDamageType.Normal);
                    collision.GetComponent<IDamage>().KnockBack(this.transform.position, 10);
                    break;
                }
        }


    }


    // �ִϸ��̼�
    void EndTrigger()
    {
        if (IsEnd) return;

        IsEnd = true;
        rb.bodyType = RigidbodyType2D.Static;
        Anim = GetComponent<Animator>();

        Mgr_Sound.Inst.PlaySound(this.gameObject, TearsEnd_Audio[Random.Range(0, TearsEnd_Audio.Length)]);

        Anim.SetTrigger("End");
    }

    public void DestroyTears()
    {
        Destroy(this.gameObject);
    }

}
