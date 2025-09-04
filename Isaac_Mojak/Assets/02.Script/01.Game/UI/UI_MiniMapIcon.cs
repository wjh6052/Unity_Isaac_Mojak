using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;



public class UI_MiniMapIcon : MonoBehaviour
{
    public Image Background_Image;
    public Image Icon_Image;

    [Header("아이콘 이미지")]
    public Sprite BossIcon_Sprite; 
    public Sprite TreasurerIcon_Sprite;



    public bool bCurrent = false;
    public bool bVisited = false;


    public void SetMiniMapIcon(EMiniMapIconState iconState, ERoomType roomType)
    {
        this.gameObject.SetActive(true);
        switch (iconState)
        {
            case EMiniMapIconState.Current: // 플레이 중 (현재 방)
                {
                    Background_Image.color = new Color32(255, 255, 255, 255);// 아이콘 배경 색 설정
                    break;
                }
            case EMiniMapIconState.Visited: // 들어갔던 적이 있음
                {
                    Background_Image.color = new Color32(150, 150, 150, 255);// 아이콘 배경 색 설정
                    break;
                }
            case EMiniMapIconState.Discovered: // 가본 적 없음 (보이긴 하지만 아직 입장x)
                {
                    Background_Image.color = new Color32(50, 50, 50, 255);// 아이콘 배경 색 설정
                    break;
                }
            case EMiniMapIconState.Hidden: // 안보임 (미발견)
                {
                    this.gameObject.SetActive(false);
                    break;
                }
        }



        // 아이콘 이미지 설정
        SetIcon(roomType);
    }

    // 아이콘 이미지 설정
    void SetIcon(ERoomType roomType)
    {
        switch (roomType)
        {
            case ERoomType.Boss:
            {
                Icon_Image.gameObject.SetActive(true);
                Icon_Image.sprite = BossIcon_Sprite;
                break;
            }
            case ERoomType.Treasurer:
            {
                Icon_Image.gameObject.SetActive(true);
                Icon_Image.sprite = TreasurerIcon_Sprite;
                break;
            }
            default:
            {
                Icon_Image.gameObject.SetActive(false);
                break;
            }
        }
    }
}
