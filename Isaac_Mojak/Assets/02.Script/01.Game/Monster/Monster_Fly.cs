using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_Fly : Monster_Ctrl
{
    

    protected override void Awake()
    {
        base.Awake();

        if (Agent)
            Agent.enabled = false;
    }

    protected override void MonsterMove()
    {
        if (IsDie)
        {
            Rb.bodyType = RigidbodyType2D.Static;
            return;
        }

        if (IsHit) return;

        Vector2 pos = this.transform.position;
        Vector2 target = Mgr_Game.Inst.Player.transform.position;
        Vector2 move = (target - pos).normalized;

        

        if (move.sqrMagnitude > ArriveDist * ArriveDist)
        {
            Rb.velocity = move * Speed;
        }
        else
        {
            Rb.velocity = Vector2.zero;        // µµ¬¯ Ω√ ∏ÿ√„(º±≈√)
        }
    }

}
