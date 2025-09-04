using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Ctrl : Monster_Ctrl
{
    public EBossType BossType;

    [Header("보스로딩 이미지")]
    public Sprite BossName_Sprite;
    public Sprite Boss_Sprite;

    protected override void Awake()
    {
        base.Awake();

        IsBoss = true;
    }


}
