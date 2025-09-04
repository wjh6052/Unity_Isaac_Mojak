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
        // 애니메이션 실행
        Anim.SetTrigger("Explosion");
    }

    public void StartPickUp()
    {
        Anim = GetComponent<Animator>();

        // 애니메이션 실행
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

    // 폭탄이 터질때 애니메이션에서 이벤트로 호출
    public void Explosion()
    {
        Collider2D  col = GetComponent<Collider2D>();

        ContactFilter2D filter = new ContactFilter2D();
        filter.NoFilter(); // 모든 오브젝트 감지

        Collider2D[] results = new Collider2D[100]; // 최대 10개까지 저장
        int count = col.OverlapCollider(filter, results);

        for (int i = 0; i < count; i++)
        {
            IDamage iDamage = results[i].GetComponent<IDamage>();
            if(iDamage != null)
            {
                iDamage.TakeDamage(10.0f, EDamageType.Bomb);

                // 오브젝트와의 거리
                float dist = Vector2.Distance(results[i].transform.position, this.transform.parent.position);
                // 가까울 수록 더 강한 힘으로 밀기
                float power = 10f / Mathf.Max(dist, 0.1f);

                // 아이템이라면 밀어내기
                iDamage.KnockBack(this.transform.parent.position, power);
            }
            else if(results[i].tag == "Item")
            {
                // 오브젝트와의 거리
                float dist = Vector2.Distance(results[i].transform.position, this.transform.parent.position);
                // 가까울 수록 더 강한 힘으로 밀기
                float power = 10f / Mathf.Max(dist, 0.1f);

                // 아이템이라면 밀어내기
                results[i].GetComponent<Rigidbody2D>().AddForce(((results[i].transform.position - this.transform.parent.position).normalized * power), ForceMode2D.Impulse);
            }
        }

        this.transform.parent.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static; // 이동 제거

        this.transform.parent.GetComponent<Collider2D>().enabled = false; // 충돌 끄기
    }

    // 폭발후 임팩트 마지막에 호출
    public void End()
    {
        Destroy(this.gameObject.transform.parent.gameObject);
    }

}
