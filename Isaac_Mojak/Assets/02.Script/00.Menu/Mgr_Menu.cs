using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum EMenuType
{
    TitleMenu,
    SaveMenu,
    GameMenu,
    CharacterMenu,
    OptionMenu
}

public class Mgr_Menu : MonoBehaviour
{
    [Header("Camera Transform")]
    public Transform TitleMenu_Transform;
    public Transform SaveMenu_Transform;
    public Transform GameMenu_Transform;
    public Transform CharacterMenu_Transform;
    public Transform OptionMenu_Transform;


    [Header("Menu_Ctrl")]
    public SaveMenu_Ctrl SaveMenu;
    public GameMenu_Ctrl GameMenu;
    public CharacterMenu_Ctrl CharacterMenu;
    public OptionMenu_Ctrl OptionMenu;

    [Header("MenuType")]
    public EMenuType NowMenuType = EMenuType.TitleMenu;


    [Header("Data")]
    public int SaveIndex = 0;


    [Header("Sound")]
    public AudioClip MenuOst_Audio;
    public AudioClip PageTurn_Audio;


    public static Mgr_Menu Inst;

    private void Awake()
    {
        Inst = this;
        Mgr_Camera.Inst.MoveCamera(TitleMenu_Transform);
    }

    private void Start()
    {
        Mgr_Sound.Inst.PlaySound(this.gameObject, MenuOst_Audio);

        if (!SceneManager.GetSceneByName("Loding Scene").isLoaded)
            SceneManager.LoadScene("Loding Scene", LoadSceneMode.Additive);
    }

    // 상태 변경 및 카메라 위치 변경
    public void SetMenu(EMenuType newType)
    {
        NowMenuType = newType;

        switch (NowMenuType)
        {
            case EMenuType.TitleMenu:
            {
                Mgr_Camera.Inst.MoveCamera(TitleMenu_Transform);
                Mgr_Sound.Inst.PlaySound(this.gameObject, PageTurn_Audio);
                break;
            }

            case EMenuType.SaveMenu:
            {
                Mgr_Camera.Inst.MoveCamera(SaveMenu_Transform);
                Mgr_Sound.Inst.PlaySound(this.gameObject, PageTurn_Audio);
                break;
            }

            case EMenuType.GameMenu:
            {
                Mgr_Camera.Inst.MoveCamera(GameMenu_Transform);
                Mgr_Sound.Inst.PlaySound(this.gameObject, PageTurn_Audio);
                break;
            }

            case EMenuType.CharacterMenu:
            {
                Mgr_Camera.Inst.MoveCamera(CharacterMenu_Transform);
                Mgr_Sound.Inst.PlaySound(this.gameObject, PageTurn_Audio);
                break;
            }

            case EMenuType.OptionMenu:
            {
                Mgr_Camera.Inst.MoveCamera(OptionMenu_Transform);
                Mgr_Sound.Inst.PlaySound(this.gameObject, PageTurn_Audio);
                break;
            }

            default:
                break;

        }
    }

    // 메뉴에서 키보드 화살표 클릭
    public void MenuMove(Vector2 value)
    {
        switch (NowMenuType)
        {
            case EMenuType.SaveMenu:
                {
                    SaveMenu.MoveSaveMenu(value.x);
                    break;
                }

            case EMenuType.GameMenu:
                {
                    GameMenu.MoveSaveMenu(value.y);
                    break;
                }

            case EMenuType.OptionMenu:
                {
                    OptionMenu.MenuMove(value);
                    break;
                }

            default:
                break;

        }
    }

    // 키보드 스페이스바를 클릭시 호출
    public void MenuChoice()
    {
        switch (NowMenuType)
        {
            case EMenuType.TitleMenu:
                {
                    SetMenu(EMenuType.SaveMenu);
                    break;
                }

            case EMenuType.SaveMenu:
                {
                    SaveMenu.ChoiceMenu();
                    GameMenu.SetGameMenu();
                    SetMenu(EMenuType.GameMenu);
                    break;
                }

            case EMenuType.GameMenu:
                {
                    GameMenu.ChoiceMenu();
                    break;
                }

            case EMenuType.CharacterMenu:
                {
                    // 데이터 저장
                    CharacterMenu.ChoiceMenu(); // 게임 시작
                    break;
                }

            case EMenuType.OptionMenu:
                {
                    OptionMenu.ChoiceMenu();
                    break;
                }
        }
    }

    // 키보드 esc 클릭시 호출
    public void MenuBack()
    {
        switch (NowMenuType)
        {
            case EMenuType.TitleMenu:
                {
                    Application.Quit();
                    break;
                }

            case EMenuType.SaveMenu:
                {
                    SetMenu(EMenuType.TitleMenu);
                    break;
                }

            case EMenuType.GameMenu:
                {
                    SetMenu(EMenuType.SaveMenu);
                    break;
                }

            case EMenuType.CharacterMenu:
                {
                    SetMenu(EMenuType.GameMenu);
                    break;
                }
            case EMenuType.OptionMenu:
                {
                    SetMenu(EMenuType.GameMenu);
                    break;
                }
        }

    }

}
