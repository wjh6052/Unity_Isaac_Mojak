using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class Mgr_Input : MonoBehaviour
{
    public Vector2 MoveDirection;
    public Vector2 AttackDirection;


    public static Mgr_Input Inst;
    private void Awake()
    {
        Inst = this;
    }


    void OnMove(InputValue val)
    {
        MoveDirection = val.Get<Vector2>();

        Mgr_Game.Inst.ClickMove(MoveDirection);
    }


    void OnAttack(InputValue val)
    {
        AttackDirection = val.Get<Vector2>();

        Mgr_Game.Inst.ClickMove(AttackDirection);
    }

    void OnUseBomb(InputValue val)
    {
        if (!GlobalValue.IsCanPlay) return;

        Mgr_Game.Inst.Player.OnUseBomb();
    }

    void OnEsc(InputValue val)
    {
        Mgr_Game.Inst.ClickEsc();
    }

    void OnSpace(InputValue val)
    {
        Mgr_Game.Inst.ClickSpace();
    }

}
