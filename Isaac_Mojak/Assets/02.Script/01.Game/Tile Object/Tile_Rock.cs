using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile_Rock : MonoBehaviour, IDamage
{
    public void TakeDamage(float inDamage, EDamageType inDamageType)
    {
        // ������ �ƴ϶�� ����
        if (inDamageType != EDamageType.Bomb) return;


        Destroy(this.gameObject);
    }

    public void KnockBack(Vector3 inHitObj, float inPower)
    {
        // �����
    }
}
