using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile_Rock : MonoBehaviour, IDamage
{
    public void TakeDamage(float inDamage, EDamageType inDamageType)
    {
        // 폭발이 아니라면 리턴
        if (inDamageType != EDamageType.Bomb) return;


        Destroy(this.gameObject);
    }

    public void KnockBack(Vector3 inHitObj, float inPower)
    {
        // 비워둠
    }
}
