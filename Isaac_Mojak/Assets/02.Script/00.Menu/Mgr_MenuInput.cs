using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Mgr_MenuInput : MonoBehaviour
{

    void OnMenuMove(InputValue val)
    {
        if (val.Get<Vector2>() == Vector2.zero) return;

        Mgr_Menu.Inst.MenuMove(val.Get<Vector2>());
    }

    void OnMenuChoice(InputValue val)
    {
        Mgr_Menu.Inst.MenuChoice();
    }

    void OnMenuBack(InputValue val)
    {
        Mgr_Menu.Inst.MenuBack();
    }
}
