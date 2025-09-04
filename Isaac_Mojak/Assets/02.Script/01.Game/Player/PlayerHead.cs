using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHead : MonoBehaviour
{

    void AE_PlayerDie()
    {
        Mgr_Game.Inst.PlayerDie();
    }

    void AE_EndAddItem()
    {
        Mgr_Game.Inst.Player.Mesh.A_Body.gameObject.SetActive(true);
        Mgr_Game.Inst.Player.Mesh.ItemImage.gameObject.SetActive(false);
    }
}
