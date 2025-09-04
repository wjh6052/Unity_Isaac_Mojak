using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum EMenuType
{
    TitleMenu,
    SaveMenu,
    GameMenu,
    CharacterMenu
}

public class Mgr_Menu : MonoBehaviour
{
    [Header("Camera Transform")]
    public Transform TitleMenu_Transform;
    public Transform SaveMenu_Transform;
    public Transform GameMenu_Transform;
    public Transform CharacterMenu_Transform;


    [Header("Menu_Ctrl")]
    public SaveMenu_Ctrl SaveMenu;
    public GameMenu_Ctrl GameMenu;
    public CharacterMenu_Ctrl CharacterMenu;

    [Header("MenuType")]
    public EMenuType NowMenuType = EMenuType.TitleMenu;


    [Header("Data")]
    public int SaveIndex = 0;


    public static Mgr_Menu Inst;

    private void Awake()
    {
        Inst = this;
        Mgr_Camera.Inst.MoveCamera(TitleMenu_Transform);
    }

    private void Start()
    {
        if(!SceneManager.GetSceneByName("Loding Scene").isLoaded)
            SceneManager.LoadScene("Loding Scene", LoadSceneMode.Additive);
    }

    // ���� ���� �� ī�޶� ��ġ ����
    public void SetMenu(EMenuType newType)
    {
        NowMenuType = newType;

        switch (NowMenuType)
        {
            case EMenuType.TitleMenu:
            {
                Mgr_Camera.Inst.MoveCamera(TitleMenu_Transform);
                break;
            }

            case EMenuType.SaveMenu:
            {
                Mgr_Camera.Inst.MoveCamera(SaveMenu_Transform);
                break;
            }

            case EMenuType.GameMenu:
            {
                Mgr_Camera.Inst.MoveCamera(GameMenu_Transform);
                break;
            }

            case EMenuType.CharacterMenu:
                {
                    Mgr_Camera.Inst.MoveCamera(CharacterMenu_Transform);
                    break;
                }

            default:
                break;

        }
    }

    // �޴����� Ű���� ȭ��ǥ Ŭ��
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

            default:
                break;

        }
    }

    // Ű���� �����̽��ٸ� Ŭ���� ȣ��
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
                    // ������ ����
                    CharacterMenu.ChoiceMenu(); // ���� ����
                    break;
                }
        }
    }

    // Ű���� esc Ŭ���� ȣ��
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
        }
    }

}
