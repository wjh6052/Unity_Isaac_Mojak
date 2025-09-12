using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class BossRoomPanel : LoadingPanel
{
    public Image Boss_Image;
    public Image BossName_Image;

    public AudioClip InBossRoom_Audio;

    private void Update()
    {
        if(Mgr_Game.Inst.Rooms[Mgr_Game.Inst.PlayRoomKey].BossCtrl != null)
        {
            BossName_Image.sprite = Mgr_Game.Inst.Rooms[Mgr_Game.Inst.PlayRoomKey].BossCtrl.BossName_Sprite;
            Boss_Image.sprite = Mgr_Game.Inst.Rooms[Mgr_Game.Inst.PlayRoomKey].BossCtrl.Boss_Sprite;
        }
        
    }


    protected override void AE_EndChangeScenes()
    {
        Time.timeScale = 1;

        base.AE_EndChangeScenes();
    }

    void AE_InBossRoomSound()
    {
        Mgr_Sound.Inst.PlaySound(this.gameObject, InBossRoom_Audio);
    }
}