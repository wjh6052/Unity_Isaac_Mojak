using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum EGameMenuType
{
    NewRun = 0,
    Continue,
    Options,
    Max
}

public class GameMenu_Ctrl : MonoBehaviour
{
    [Header("불러오기 버튼")]
    public Transform GameMenu_Choice;
    public Transform[] GameMenuArr = new Transform[3];


    [Header("Data")]
    public EGameMenuType GameMenuType = EGameMenuType.NewRun;
    bool Continue = false;

    public int NowIndex = 0;

    private void Start()
    {
        SwitchingMenu(0);
    }


    public void SetGameMenu()
    {
        GameMenuArr[(int)EGameMenuType.Continue].gameObject.SetActive(Continue);
    }

    void SwitchingMenu(int index)
    {
        NowIndex = index;
        GameMenuType = (EGameMenuType)NowIndex;

        Vector3 choicePos = GameMenu_Choice.transform.localPosition;
        choicePos.y = GameMenuArr[NowIndex].localPosition.y;

        GameMenu_Choice.transform.localPosition = choicePos;
    }

    public void MoveSaveMenu(float value)
    {
        int i = (value > 0) ? -1 : 1;
        int max = (int)EGameMenuType.Max;
        int index = (NowIndex + i + max) % max;

        if(index == (int)EGameMenuType.Continue && Continue == false)
            index = (NowIndex + i + i + max) % max;


        SwitchingMenu(index);
    }

    public void ChoiceMenu()
    {
        switch(GameMenuType)
        {
            case EGameMenuType.NewRun: // 뉴게임
                {
                    Mgr_Menu.Inst.SetMenu(EMenuType.CharacterMenu);
                    break;
                }
            case EGameMenuType.Options: // 옵션
                {
                    Mgr_Menu.Inst.SetMenu(EMenuType.OptionMenu);
                    break;
                }
        }
    }
}
