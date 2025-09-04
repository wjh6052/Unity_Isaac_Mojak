using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameMenuPanel : MonoBehaviour
{
    
    public GameObject[] ChoiceObj = new GameObject[3];
    public Transform MyItemsList;
    int ChoiceNum = 0;
    public Text Seed_Text;


    public void SetGameMenuPanel(bool isOn)
    {
        if(isOn)
        {
            this.gameObject.SetActive(true);
            ChoiceNum = 0;

            // 선택 삼각형 위치 초기화
            for (int i = 0; i < ChoiceObj.Length; i++)
                ChoiceObj[i].SetActive(i == ChoiceNum);

            // 시드 
            Seed_Text.text = Mgr_Game.Inst.Seed.ToString();

            // 획득 아이템
            foreach (Transform child in MyItemsList)
            {
                if(child)
                    Destroy(child.gameObject);
            }

            Image ImageIcon = Resources.Load<Image>("Item/ItemIcon/ItemIcon_Prefab");
            foreach (EItemType itemType in Mgr_Game.Inst.Player.PlayerHaveItem)
            {
                Sprite[] itemSprites = Resources.LoadAll<Sprite>($"Item/ItemIcon/ItemIcon");
                Sprite sword = itemSprites.FirstOrDefault(s => s.name == $"ItemIcon_{itemType.ToString()}");

                Image img = Instantiate(ImageIcon, MyItemsList);
                img.sprite = sword;
            }

            // 레이아웃 갱신
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)MyItemsList);

            Time.timeScale = 0.0f;
            GetComponent<Animator>().SetTrigger("Start");
        }
        else
        {
            GetComponent<Animator>().SetTrigger("End");
        }
            
    }

    public void SetChoice(bool isUp)
    {
        ChoiceNum = (ChoiceNum + (isUp ? -1 : 1) + ChoiceObj.Length) % ChoiceObj.Length;

        for (int i = 0; i < ChoiceObj.Length; i++)
            ChoiceObj[i].SetActive(i == ChoiceNum);
    }

    public void ChoiceMenu()
    {
        switch(ChoiceNum)
        {
            case 0: // 옵션설정
                {

                    break;
                }
            case 1: // 게임으로 돌아가기
                {
                    Mgr_GameUI.Inst.SetGameMenu(false);
                    break;
                }
            case 2: // 메뉴로 이동
                {
                    Mgr_Loading.Inst.StartChangeScenes(ELoadingType.ToMenu);
                    break;
                }

            default:
                break;


        }
    }


    void AE_EndGameMenu()
    {
        this.gameObject.SetActive(false);
        Time.timeScale = 1.0f;
    }
}
