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
    #region 게임플레이 UI
    [Header("게임플레이 UI")]
    public Text Gold_Text;
    public Text Bomb_Text;
    public Text Key_Text;

    public Transform HeartPanel;
    public Sprite[] HeartImage; // EHeartType에 따라서 맞출예정
    Image[] HeartImageArr = new Image[12];
    #endregion

    [Header("게임중 메뉴")]
    public UI_GameMenuPanel GameMenuPanel;

    [Header("플레이어 사망 메뉴")]
    public GameObject PlayerDiePanel;


    [Header("보스 체력바")]
    public GameObject BossHpBarPanel;
    public Image BossHpBar_Image;
    public bool IsBossRoom = false;

    [Header("UI 상태값")]
    public EGameUiType GameUiType = EGameUiType.Playing;

    public static Mgr_GameUI Inst;

    private void Awake()
    {
        Inst = this;

        GameUiType = EGameUiType.Playing;
        GameMenuPanel.gameObject.SetActive(false);
        PlayerDiePanel.gameObject.SetActive(false);
        BossHpBarPanel.gameObject.SetActive(false);

        // 채력
        {
            for (int i = 0; i < HeartPanel.childCount; i++)
                HeartImageArr[i] = HeartPanel.GetChild(i).GetComponent<Image>();
        }

        // 미니맵
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
        // 하트 설정
        {
            int hp = Mgr_Game.Inst.Player.Hp;
            float maxhp = Mgr_Game.Inst.Player.MaxHp;

            // 꽉찬 하트
            int fullHeart = hp / 2;

            // 반칸 하트가 있는지 확인
            bool hasHert = (hp % 2) == 1;

            for (int i = 0; i < 12; i++)
            {
                HeartImageArr[i].gameObject.SetActive(true);

                if (fullHeart > 0) // 풀칸 하트 부터 추가
                {
                    HeartImageArr[i].sprite = HeartImage[(int)EHeartType.Full];
                    fullHeart--;
                }
                else if (hasHert) // 반칸
                {
                    HeartImageArr[i].sprite = HeartImage[(int)EHeartType.Half];
                    hasHert = false;
                }
                else if (maxhp / 2f > i) // 빈칸
                {
                    HeartImageArr[i].sprite = HeartImage[(int)EHeartType.Empty];
                }
                else // 비우기
                {
                    HeartImageArr[i].gameObject.SetActive(false);
                }
            }
        }

        // 골드
        Gold_Text.text = ToTwoDigitString(Mgr_Game.Inst.Player.GoldCount);

        // 폭탄
        Bomb_Text.text = ToTwoDigitString(Mgr_Game.Inst.Player.BombCount);

        // 열쇠
        Key_Text.text = ToTwoDigitString(Mgr_Game.Inst.Player.KeyCount);

    }

    string ToTwoDigitString(int num) => num.ToString("D2");


    #region 게임중 메뉴
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

    // 게임 중단 메뉴
    public void SetGameMenu(bool isOn)
    {
        GlobalValue.IsCanPlay = !isOn;

        GameUiType = isOn ? EGameUiType.GameMenu : EGameUiType.Playing;

        GameMenuPanel.SetGameMenuPanel(isOn);
    }


    // 죽었을때 메뉴
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


    #region 미니맵

    [Header("--- 미니맵 ---")]
    public Transform MiniMapRoot;       // 아이콘들을 붙일 루트

    public UI_MiniMapIcon UI_MiniMapIcon_Prefab; // 미니맵 아이콘 프리팹

    float CellSizeX = 40f;  // 아이콘간의 간격 X
    float CellSizeY = 40f;  // 아이콘간의 간격 Y

    public Dictionary<Vector2Int, UI_MiniMapIcon> MiniMapRooms = new Dictionary<Vector2Int, UI_MiniMapIcon>();
    

    // 맵 이동후 미니맵 업데이트
    public void MiniMapUpdate(Dictionary<Vector2Int, Room_Ctrl> Rooms, Vector2Int playRoomKey)
    {
        // 아이콘 전체 생성
        SpawnMiniMapIcons(Rooms);

        // 플레이 중인 방은 발겼했다고 체크
        {
            MiniMapRooms[playRoomKey].bCurrent = true;
            MiniMapRooms[playRoomKey].bVisited = true;
        }

        // 플레이 중인 방의 상하좌우 방을 발견했다고 체크
        {
            if (MiniMapRooms.ContainsKey(playRoomKey + Vector2Int.up))   // 상
                MiniMapRooms[playRoomKey + Vector2Int.up].bCurrent = true;
            if (MiniMapRooms.ContainsKey(playRoomKey + Vector2Int.down))  // 하
                MiniMapRooms[playRoomKey + Vector2Int.down].bCurrent = true;
            if (MiniMapRooms.ContainsKey(playRoomKey + Vector2Int.left))  // 좌
                MiniMapRooms[playRoomKey + Vector2Int.left].bCurrent = true;
            if (MiniMapRooms.ContainsKey(playRoomKey + Vector2Int.right))  // 우
                MiniMapRooms[playRoomKey + Vector2Int.right].bCurrent = true;
        }
        

        // 각 아이콘들을 설정
        foreach (Vector2Int key in Rooms.Keys)
        {
            // 아이콘 위치 설정
            MiniMapRooms[key].gameObject.GetComponent<RectTransform>().anchoredPosition = 
                new Vector2(key.x * CellSizeX, key.y * CellSizeY);

            // 아이콘의 상태 설정
            EMiniMapIconState iconState = EMiniMapIconState.Hidden;

            // 현재 확인중인 방이 플레이어가 위치한 방인 경우
            if (key == playRoomKey)
            {
                iconState = EMiniMapIconState.Current;
            }
            else if (MiniMapRooms[key].bVisited) // 플레이를 했었던 방
            {
                iconState = EMiniMapIconState.Visited;
            }
            else if(MiniMapRooms[key].bCurrent)// 플레이 한적이 없지만 찾은적은 있는 방
            {
                iconState = EMiniMapIconState.Discovered;
            }
            else  // 한번도 확인하지 못한 방
            {
                iconState = EMiniMapIconState.Hidden;
            }


            MiniMapRooms[key].SetMiniMapIcon(iconState, Rooms[key].RoomType);
        }

        // 미니맵 배경 사이즈 설정
        SetMiniMapPanelSize();
    }

    // 아이콘 생성 및 저장
    void SpawnMiniMapIcons(Dictionary<Vector2Int, Room_Ctrl> Rooms)
    {
        // 프리팹이 없다면 가져오기
        if (UI_MiniMapIcon_Prefab == null)
        {
            UI_MiniMapIcon_Prefab = Resources.Load("UI/UI_MiniMapIcon_Prefab").GetComponent<UI_MiniMapIcon>();
        }

        foreach (Vector2Int key in Rooms.Keys)
        {
            // 기존에 없던 아이콘이라면 생성후 저장
            if (!MiniMapRooms.ContainsKey(key))
            {
                UI_MiniMapIcon icon = Instantiate(UI_MiniMapIcon_Prefab, MiniMapRoot);
                MiniMapRooms[key] = icon;
            }
        }

    }
   

    // 미니맵 배경 사이즈 설정
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
        
        // 셀 간격이 곧 아이콘 중심으로 계산
        float width = (maxX ) * (CellSizeY);
        float height = (maxY ) * (CellSizeX);
        
        float w = Mathf.Ceil(width * 2f);
        float h = Mathf.Ceil(height * 2f);
        root.sizeDelta = new Vector2(w, h);
    }

    #endregion
}
