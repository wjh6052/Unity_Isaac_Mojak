using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;



public class UI_MiniMapIcon : MonoBehaviour
{
    public Image Background_Image;
    public Image Icon_Image;

    [Header("������ �̹���")]
    public Sprite BossIcon_Sprite; 
    public Sprite TreasurerIcon_Sprite;



    public bool bCurrent = false;
    public bool bVisited = false;


    public void SetMiniMapIcon(EMiniMapIconState iconState, ERoomType roomType)
    {
        this.gameObject.SetActive(true);
        switch (iconState)
        {
            case EMiniMapIconState.Current: // �÷��� �� (���� ��)
                {
                    Background_Image.color = new Color32(255, 255, 255, 255);// ������ ��� �� ����
                    break;
                }
            case EMiniMapIconState.Visited: // ���� ���� ����
                {
                    Background_Image.color = new Color32(150, 150, 150, 255);// ������ ��� �� ����
                    break;
                }
            case EMiniMapIconState.Discovered: // ���� �� ���� (���̱� ������ ���� ����x)
                {
                    Background_Image.color = new Color32(50, 50, 50, 255);// ������ ��� �� ����
                    break;
                }
            case EMiniMapIconState.Hidden: // �Ⱥ��� (�̹߰�)
                {
                    this.gameObject.SetActive(false);
                    break;
                }
        }



        // ������ �̹��� ����
        SetIcon(roomType);
    }

    // ������ �̹��� ����
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
