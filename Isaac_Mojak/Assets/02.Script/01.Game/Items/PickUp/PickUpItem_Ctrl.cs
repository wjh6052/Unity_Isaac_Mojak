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

        // 스폰 애니메이션
        SpawnTrigger();
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            GetComponent<Collider2D>().isTrigger = true; // 충돌 끄기

            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null) rb.bodyType = RigidbodyType2D.Static; // 이동 제거

            // 하트인 경우
            if(ItemType == EPickUpItemType.Heart)
            {
                if(Mgr_Game.Inst.Player.CanHeal()) // 힐을 먹을수 있는지 확인
                {
                    // 먹었을때 애니메이션
                    StartPickUp();

                    // 아이템 먹기 작업
                    Mgr_Game.Inst.Player.AddPickUpItem(ItemType, Count);
                }
            }
            else
            {
                // 먹었을때 애니메이션
                StartPickUp();

                // 아이템 먹기 작업
                Mgr_Game.Inst.Player.AddPickUpItem(ItemType, Count);
            }
                
        }
    }

    protected void StartPickUp()
    {
        // 애니메이션 실행
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
