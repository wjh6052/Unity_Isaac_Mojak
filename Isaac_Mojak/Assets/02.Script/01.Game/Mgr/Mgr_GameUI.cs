using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public enum EGameUiType
{
    Playing,
    GameMenu,
    DieMenu
}


public class Mgr_GameUI : MonoBehaviour
{
    #region �����÷��� UI
    [Header("�����÷��� UI")]
    public Text Gold_Text;
    public Text Bomb_Text;
    public Text Key_Text;

    public Transform HeartPanel;
    public Sprite[] HeartImage; // EHeartType�� ���� ���⿹��
    Image[] HeartImageArr = new Image[12];
    #endregion

    [Header("������ �޴�")]
    public UI_GameMenuPanel GameMenuPanel;

    [Header("�÷��̾� ��� �޴�")]
    public GameObject PlayerDiePanel;


    [Header("���� ü�¹�")]
    public GameObject BossHpBarPanel;
    public Image BossHpBar_Image;
    public bool IsBossRoom = false;

    [Header("UI ���°�")]
    public EGameUiType GameUiType = EGameUiType.Playing;

    public static Mgr_GameUI Inst;

    private void Awake()
    {
        Inst = this;

        GameUiType = EGameUiType.Playing;
        GameMenuPanel.gameObject.SetActive(false);
        PlayerDiePanel.gameObject.SetActive(false);
        BossHpBarPanel.gameObject.SetActive(false);

        // ä��
        {
            for (int i = 0; i < HeartPanel.childCount; i++)
                HeartImageArr[i] = HeartPanel.GetChild(i).GetComponent<Image>();
        }

        // �̴ϸ�
        {
            foreach (Transform child in MiniMapRoot)
                Destroy(child.gameObject);
        }
        
    }


    private void Update()
    {
        if (IsBossRoom)
        {
            if (Mgr_Game.Inst.Rooms[Mgr_Game.Inst.PlayRoomKey].BossCtrl != null)
            {
                BossHpBar_Image.fillAmount = Mgr_Game.Inst.Rooms[Mgr_Game.Inst.PlayRoomKey].BossCtrl.Hp / Mgr_Game.Inst.Rooms[Mgr_Game.Inst.PlayRoomKey].BossCtrl.MaxHp;
            }
            else
                SetBossHpBarPanel(false);
        }
        
    }


    public void GameUIUpdate()
    {
        // ��Ʈ ����
        {
            int hp = Mgr_Game.Inst.Player.Hp;
            float maxhp = Mgr_Game.Inst.Player.MaxHp;

            // ���� ��Ʈ
            int fullHeart = hp / 2;

            // ��ĭ ��Ʈ�� �ִ��� Ȯ��
            bool hasHert = (hp % 2) == 1;

            for (int i = 0; i < 12; i++)
            {
                HeartImageArr[i].gameObject.SetActive(true);

                if (fullHeart > 0) // Ǯĭ ��Ʈ ���� �߰�
                {
                    HeartImageArr[i].sprite = HeartImage[(int)EHeartType.Full];
                    fullHeart--;
                }
                else if (hasHert) // ��ĭ
                {
                    HeartImageArr[i].sprite = HeartImage[(int)EHeartType.Half];
                    hasHert = false;
                }
                else if (maxhp / 2f > i) // ��ĭ
                {
                    HeartImageArr[i].sprite = HeartImage[(int)EHeartType.Empty];
                }
                else // ����
                {
                    HeartImageArr[i].gameObject.SetActive(false);
                }
            }
        }

        // ���
        Gold_Text.text = ToTwoDigitString(Mgr_Game.Inst.Player.GoldCount);

        // ��ź
        Bomb_Text.text = ToTwoDigitString(Mgr_Game.Inst.Player.BombCount);

        // ����
        Key_Text.text = ToTwoDigitString(Mgr_Game.Inst.Player.KeyCount);

    }

    string ToTwoDigitString(int num) => num.ToString("D2");


    #region ������ �޴�
    public void SetChoice(Vector2 val)
    {
        if (val.y == 0) return;

        bool isUp = (val.y > 0);
        switch (GameUiType)
        {
            case EGameUiType.GameMenu:
                {
                    GameMenuPanel.SetChoice(isUp);
                    break;
                }
            case EGameUiType.DieMenu:
                {

                    break;
                }
            default:
                break;
        }


    }

    public void ChoiceMenu()
    {
        switch (GameUiType)
        {
            case EGameUiType.GameMenu:
                {
                    GameMenuPanel.ChoiceMenu();
                    break;
                }
            case EGameUiType.DieMenu:
                {

                    break;
                }
            default:
                break;
        }
    }

    // ���� �ߴ� �޴�
    public void SetGameMenu(bool isOn)
    {
        GlobalValue.IsCanPlay = !isOn;

        GameUiType = isOn ? EGameUiType.GameMenu : EGameUiType.Playing;

        GameMenuPanel.SetGameMenuPanel(isOn);
    }


    // �׾����� �޴�
    public void PlayerDieUI(bool isOn)
    {
        PlayerDiePanel.gameObject.SetActive(isOn);

        GlobalValue.IsCanPlay = !isOn;
        GameUiType = isOn ? EGameUiType.DieMenu : EGameUiType.Playing;
    }



    #endregion


    public void SetBossHpBarPanel(bool isOn)
    {
        IsBossRoom = isOn;
        BossHpBarPanel.gameObject.SetActive(isOn);
    }


    #region �̴ϸ�

    [Header("--- �̴ϸ� ---")]
    public Transform MiniMapRoot;       // �����ܵ��� ���� ��Ʈ

    public UI_MiniMapIcon UI_MiniMapIcon_Prefab; // �̴ϸ� ������ ������

    float CellSizeX = 40f;  // �����ܰ��� ���� X
    float CellSizeY = 40f;  // �����ܰ��� ���� Y

    public Dictionary<Vector2Int, UI_MiniMapIcon> MiniMapRooms = new Dictionary<Vector2Int, UI_MiniMapIcon>();
    

    // �� �̵��� �̴ϸ� ������Ʈ
    public void MiniMapUpdate(Dictionary<Vector2Int, Room_Ctrl> Rooms, Vector2Int playRoomKey)
    {
        // ������ ��ü ����
        SpawnMiniMapIcons(Rooms);

        // �÷��� ���� ���� �߰��ߴٰ� üũ
        {
            MiniMapRooms[playRoomKey].bCurrent = true;
            MiniMapRooms[playRoomKey].bVisited = true;
        }

        // �÷��� ���� ���� �����¿� ���� �߰��ߴٰ� üũ
        {
            if (MiniMapRooms.ContainsKey(playRoomKey + Vector2Int.up))   // ��
                MiniMapRooms[playRoomKey + Vector2Int.up].bCurrent = true;
            if (MiniMapRooms.ContainsKey(playRoomKey + Vector2Int.down))  // ��
                MiniMapRooms[playRoomKey + Vector2Int.down].bCurrent = true;
            if (MiniMapRooms.ContainsKey(playRoomKey + Vector2Int.left))  // ��
                MiniMapRooms[playRoomKey + Vector2Int.left].bCurrent = true;
            if (MiniMapRooms.ContainsKey(playRoomKey + Vector2Int.right))  // ��
                MiniMapRooms[playRoomKey + Vector2Int.right].bCurrent = true;
        }
        

        // �� �����ܵ��� ����
        foreach (Vector2Int key in Rooms.Keys)
        {
            // ������ ��ġ ����
            MiniMapRooms[key].gameObject.GetComponent<RectTransform>().anchoredPosition = 
                new Vector2(key.x * CellSizeX, key.y * CellSizeY);

            // �������� ���� ����
            EMiniMapIconState iconState = EMiniMapIconState.Hidden;

            // ���� Ȯ������ ���� �÷��̾ ��ġ�� ���� ���
            if (key == playRoomKey)
            {
                iconState = EMiniMapIconState.Current;
            }
            else if (MiniMapRooms[key].bVisited) // �÷��̸� �߾��� ��
            {
                iconState = EMiniMapIconState.Visited;
            }
            else if(MiniMapRooms[key].bCurrent)// �÷��� ������ ������ ã������ �ִ� ��
            {
                iconState = EMiniMapIconState.Discovered;
            }
            else  // �ѹ��� Ȯ������ ���� ��
            {
                iconState = EMiniMapIconState.Hidden;
            }


            MiniMapRooms[key].SetMiniMapIcon(iconState, Rooms[key].RoomType);
        }

        // �̴ϸ� ��� ������ ����
        SetMiniMapPanelSize();
    }

    // ������ ���� �� ����
    void SpawnMiniMapIcons(Dictionary<Vector2Int, Room_Ctrl> Rooms)
    {
        // �������� ���ٸ� ��������
        if (UI_MiniMapIcon_Prefab == null)
        {
            UI_MiniMapIcon_Prefab = Resources.Load("UI/UI_MiniMapIcon_Prefab").GetComponent<UI_MiniMapIcon>();
        }

        foreach (Vector2Int key in Rooms.Keys)
        {
            // ������ ���� �������̶�� ������ ����
            if (!MiniMapRooms.ContainsKey(key))
            {
                UI_MiniMapIcon icon = Instantiate(UI_MiniMapIcon_Prefab, MiniMapRoot);
                MiniMapRooms[key] = icon;
            }
        }

    }
   

    // �̴ϸ� ��� ������ ����
    void SetMiniMapPanelSize()
    {
        RectTransform root = MiniMapRoot.GetComponent<RectTransform>();
        
        bool any = false;
        int maxX = int.MinValue, maxY = int.MinValue;
        
        foreach (Vector2Int key in MiniMapRooms.Keys)
        {
            if (!(MiniMapRooms[key].bCurrent || MiniMapRooms[key].bVisited)) continue;
        
            any = true;
            if (key.x > maxX) maxX = key.x;
            if (key.y > maxY) maxY = key.y;
        }
        
        if (!any) { root.sizeDelta = Vector2.zero; return; }
        
        // �� ������ �� ������ �߽����� ���
        float width = (maxX ) * (CellSizeY);
        float height = (maxY ) * (CellSizeX);
        
        float w = Mathf.Ceil(width * 2f);
        float h = Mathf.Ceil(height * 2f);
        root.sizeDelta = new Vector2(w, h);
    }

    #endregion
}
