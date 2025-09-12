using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class PickUpItem_Bomb : PickUpItem_Ctrl
{
    public PickUpItem_BombMesh BombMesh;
    bool IsExplosion = true;

    public AudioClip[] Explosions_Audio = new AudioClip[3];


    public override void Awake()
    {
        ItemType = EPickUpItemType.Bomb;
        if (Count <= -1)
            Count = 1;

        BombMesh.Onwer = this;
    }


    public void SpawnBomb(Vector2 inspawnPos, bool isExplosion = true)
    {
        SpawnItem(inspawnPos);
        IsExplosion = isExplosion;

        // 아이템이 아닌 폭발을 위한것이라면
        if (!IsExplosion)
        {
            GetComponent<Collider2D>().isTrigger = true;

            this.gameObject.tag = "BombExplosion";
            this.gameObject.layer = LayerMask.NameToLayer("BombExplosion");
            GetComponent<Collider2D>().isTrigger = true;
            BombMesh.StartExplosion();
        }
        else
        {
            BombMesh.BombSpawnTrigger();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsExplosion) return;

        if (collision.collider.tag == "Player")
        {
            GetComponent<Collider2D>().isTrigger = true; // 충돌 끄기

            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null) rb.bodyType = RigidbodyType2D.Static; // 이동 제거

            BombMesh.StartPickUp(); // 먹었을때 애니메이션

            // 폭탄 먹기 작업
            Mgr_Game.Inst.Player.AddPickUpItem(ItemType, Count);
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (IsExplosion) return;

        if (collision.tag == "Player")
            GetComponent<Collider2D>().isTrigger = false;
    }

}
