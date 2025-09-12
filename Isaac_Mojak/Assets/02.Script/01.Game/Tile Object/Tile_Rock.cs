using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile_Rock : MonoBehaviour, IDamage
{
    public AudioClip RockEnd_Audio;

    public void TakeDamage(float inDamage, EDamageType inDamageType)
    {
        // ������ �ƴ϶�� ����
        if (inDamageType != EDamageType.Bomb) return;

        Mgr_Sound.Inst.PlaySound(this.gameObject, RockEnd_Audio);
        Destroy(this.gameObject);
    }

    public void KnockBack(Vector3 inHitObj, float inPower)
    {
        // �����
    }
}
