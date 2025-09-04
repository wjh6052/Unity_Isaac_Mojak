using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItem_Ctrl : MonoBehaviour
{
    public EPickUpItemType ItemType;
    public int Count = -1;



    public virtual void Awake()
    {
        this.gameObject.tag = "Item";
        this.gameObject.layer = LayerMask.NameToLayer("Item");


    }

    public virtual void SpawnItem(Vector2 inspawnPos)
    {
        this.tag = "Item";

        this.transform.position = new Vector3(inspawnPos.x, inspawnPos.y, 0);

        // ���� �ִϸ��̼�
        SpawnTrigger();
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            GetComponent<Collider2D>().isTrigger = true; // �浹 ����

            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null) rb.bodyType = RigidbodyType2D.Static; // �̵� ����

            // ��Ʈ�� ���
            if(ItemType == EPickUpItemType.Heart)
            {
                if(Mgr_Game.Inst.Player.CanHeal()) // ���� ������ �ִ��� Ȯ��
                {
                    // �Ծ����� �ִϸ��̼�
                    StartPickUp();

                    // ������ �Ա� �۾�
                    Mgr_Game.Inst.Player.AddPickUpItem(ItemType, Count);
                }
            }
            else
            {
                // �Ծ����� �ִϸ��̼�
                StartPickUp();

                // ������ �Ա� �۾�
                Mgr_Game.Inst.Player.AddPickUpItem(ItemType, Count);
            }
                
        }
    }

    protected void StartPickUp()
    {
        // �ִϸ��̼� ����
        GetComponent<Animator>().SetTrigger("PickUp");
    }

    protected void SpawnTrigger()
    {
        if (GetComponent<Animator>() == null) return;

        bool isParam = false;

        foreach (AnimatorControllerParameter param in GetComponent<Animator>().parameters)
        {
            if (param.name == "Spawn")
                isParam = true;
        }

        if (!isParam) return;

        GetComponent<Animator>().SetTrigger("Spawn");
    }

    public void EndPickUp()
    {
        Destroy(this.gameObject);
    }
}
