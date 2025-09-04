using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItem_BombMesh : MonoBehaviour
{
    Animator Anim;

    private void Awake()
    {
        Anim = GetComponent<Animator>();
    }

    public void StartExplosion()
    {
        Anim = GetComponent<Animator>();
        this.gameObject.layer = LayerMask.NameToLayer("BombExplosion");

        GetComponent<SpriteRenderer>().sortingOrder = (int)ESpriteOrder.TearOrBomb;
        // �ִϸ��̼� ����
        Anim.SetTrigger("Explosion");
    }

    public void StartPickUp()
    {
        Anim = GetComponent<Animator>();

        // �ִϸ��̼� ����
        Anim.SetTrigger("PickUp");
    }

    public void BombSpawnTrigger()
    {
        bool isParam = false;

        Anim = GetComponent<Animator>();

        foreach (AnimatorControllerParameter param in Anim.parameters)
        {
            if (param.name == "Spawn")
                isParam = true;
        }

        if (!isParam) return;

        Anim.SetTrigger("Spawn");
    }

    // ��ź�� ������ �ִϸ��̼ǿ��� �̺�Ʈ�� ȣ��
    public void Explosion()
    {
        Collider2D  col = GetComponent<Collider2D>();

        ContactFilter2D filter = new ContactFilter2D();
        filter.NoFilter(); // ��� ������Ʈ ����

        Collider2D[] results = new Collider2D[100]; // �ִ� 10������ ����
        int count = col.OverlapCollider(filter, results);

        for (int i = 0; i < count; i++)
        {
            IDamage iDamage = results[i].GetComponent<IDamage>();
            if(iDamage != null)
            {
                iDamage.TakeDamage(10.0f, EDamageType.Bomb);

                // ������Ʈ���� �Ÿ�
                float dist = Vector2.Distance(results[i].transform.position, this.transform.parent.position);
                // ����� ���� �� ���� ������ �б�
                float power = 10f / Mathf.Max(dist, 0.1f);

                // �������̶�� �о��
                iDamage.KnockBack(this.transform.parent.position, power);
            }
            else if(results[i].tag == "Item")
            {
                // ������Ʈ���� �Ÿ�
                float dist = Vector2.Distance(results[i].transform.position, this.transform.parent.position);
                // ����� ���� �� ���� ������ �б�
                float power = 10f / Mathf.Max(dist, 0.1f);

                // �������̶�� �о��
                results[i].GetComponent<Rigidbody2D>().AddForce(((results[i].transform.position - this.transform.parent.position).normalized * power), ForceMode2D.Impulse);
            }
        }

        this.transform.parent.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static; // �̵� ����

        this.transform.parent.GetComponent<Collider2D>().enabled = false; // �浹 ����
    }

    // ������ ����Ʈ �������� ȣ��
    public void End()
    {
        Destroy(this.gameObject.transform.parent.gameObject);
    }

}
