using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Mgr_Game : MonoBehaviour
{
    public Player_Ctrl Player = null;


    

    public static Mgr_Game Inst;
    private void Awake()
    {
        Inst = this;
    }


    void Start()
    {
        if (!SceneManager.GetSceneByName("Loding Scene").isLoaded)
            SceneManager.LoadScene("Loding Scene", LoadSceneMode.Additive);

        // 맵 스폰 시작
        StartSpawnRooms();

        // 플레이어 스폰
        {
            Player_Ctrl obj = Resources.Load("Player_Prefab").GetComponent<Player_Ctrl>();
            Player_Ctrl player_Prefab = Instantiate(obj);
            player_Prefab.gameObject.transform.position = Vector3.zero;
            Player = player_Prefab;
        }
        
    }

    void Update()
    {
        // 테스트
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // if(Time.timeScale <= 0)
            //     Time.timeScale = 1f;
            // else
            //     Time.timeScale = 0f;

            //Player.TakeDamage(1);

        }
    }

    public void ClickMove(Vector2 val)
    {
        if (Mgr_GameUI.Inst.GameUiType == EGameUiType.Playing || val == Vector2.zero) return;


        Mgr_GameUI.Inst.SetChoice(val);
    }

    // Esc클릭시 Mgr_Input에서 호출
    public void ClickEsc()
    {
        switch (Mgr_GameUI.Inst.GameUiType)
        {
            case EGameUiType.Playing:
                {
                    Mgr_GameUI.Inst.SetGameMenu(true);
                    break;
                }
            case EGameUiType.DieMenu:
                {
                    Time.timeScale = 1;
                    Mgr_Loading.Inst.StartChangeScenes(ELoadingType.ToMenu);
                    break;
                }
            default:
                {
                    Mgr_GameUI.Inst.SetGameMenu(false);
                    break;
                }
        }
    }

    public void ClickSpace()
    {
        switch (Mgr_GameUI.Inst.GameUiType)
        {
            case EGameUiType.Playing:
                {
                    break;
                }
            case EGameUiType.DieMenu:
                {
                    Time.timeScale = 1;
                    Mgr_Loading.Inst.StartChangeScenes(ELoadingType.ReGame);
                    break;
                }

            default:
                {
                    Mgr_GameUI.Inst.ChoiceMenu();
                    break;
                }
                
        }
    }


    // 플레이어가 죽었을때 호출
    public void PlayerDie()
    {
        Time.timeScale = 0;

        Mgr_GameUI.Inst.PlayerDieUI(true);
    }


    // 아이템을 먹었을때 스텟 변화
    public void AddItemStets(EItemType addItemType)
    {
        switch(addItemType)
        {
            case EItemType.Cricketshead:
                {
                    Player.Damage += 2f;
                    break;
                }
        }



    }



    #region 맵 스폰 알고리즘
    /* 맵 생성 규칙
    1. 시작방은 (0,0)
    2. 좌우 이동시 x + 4.2, 상하 이동 시 y + 2.4
    3. 전체 방 개수는 minRooms ~ maxRooms 범위에서 랜덤
    4. 노멀 방은 랜덤 워크 방식으로 확장
    5. 보스/황금방은 반드시 1개씩 보장
    6. 보스/황금방은 막다른 길에 위치해야 함(연결된 방이 1개뿐)
     */


    [Header("방 / 플레이 중인 맵")]
    public Vector2Int PlayRoomKey;

    [Header("방 / 배치 기준")]
    public Transform RoomRoot;  // 생성된 방들의 부모 오브젝트
    float OffSetX = 4f;   // 좌/우 이동 오프셋
    float OffSetY = 2.2f;   // 상/하 이동 오프셋

    [Header("방 / 생성 옵션")]
    public int MinRooms = 6;    // 최소 방 수 (시작방 포함 안됨)
    public int MaxRooms = 12;   // 최대 방 수 (시작방 포함 안됨)
    public int Seed = 0;        // 시드

    [Header("방 / 프리팹")]
    public Room_Ctrl StartRoomPrefab;
    public Room_Ctrl[] NormalRoomPrefab;
    public Room_Ctrl[] BossRoomPrefab;
    public Room_Ctrl[] TreasurerRoomPrefab;


    // 스폰된 방들을 키값에 저장 (Vector2은 2차원배열처럼 키값으로 사용하기 위함)
    public Dictionary<Vector2Int, Room_Ctrl> Rooms = new Dictionary<Vector2Int, Room_Ctrl>();
    // 일반 Vector2사용에는 float이기때문에 부동소수점 비교가 생겨 int로 교체
    readonly Vector2Int[] kDirs = { Vector2Int.up, Vector2Int.down, Vector2Int.right, Vector2Int.left };


    // 맵 스폰 시작함수
    void StartSpawnRooms()
    {
        // 초기화
        Rooms.Clear();


        // 시드 확인
        if (Seed == 0) // 시드가 없는 경우
        {
            Seed = System.Environment.TickCount; 
            Random.InitState(System.Environment.TickCount); // 난수 랜덤으로 설정
        }
        else // 시드가 있는 경우
            Random.InitState(Seed); // 난수 고정


        // 시작 방 고정
        SpawnRoom(ERoomType.Start, Vector2Int.zero);

        int roomsCount = Random.Range(MinRooms + 1, MaxRooms + 2);
        int guard = 0;
        

        // 랜덤 위치에 맵 확장
        while(Rooms.Count < roomsCount && guard < 1000)
        {
            List<Vector2Int> keys = new List<Vector2Int>(Rooms.Keys);
            Vector2Int pos = keys[Random.Range(0, keys.Count)];

            foreach(Vector2Int key in ShuffledDirs())
            {
                Vector2Int next = pos + key;
                // 랜덤으로 받아온 위치 키값이 있는지 확인
                if(!Rooms.ContainsKey(next))
                {
                    // 없다면 스폰
                    SpawnRoom(ERoomType.Normal, next);
                    break;
                }
            }
            
            // 무한 루프 방지
            guard++;
        }


        // 황금방 설정
        SpawnUniqueRoom(ERoomType.Treasurer);
        // 보스방 설정
        SpawnUniqueRoom(ERoomType.Boss);

        // 맵들을 모두 스폰 후 각 방마다 방설정 하기
        SettingRooms();
    }

    // === 배치 함수 ===
    // 방을 스폰 후 Rooms에 저장
    void SpawnRoom(ERoomType mapType, Vector2Int grid)
    {
        Vector3 spawnPos = new Vector3(grid.x * OffSetX, grid.y * OffSetY, 0f);
        Room_Ctrl roomObj = Instantiate(GetRoomPrefab(mapType), spawnPos, Quaternion.identity, RoomRoot).GetComponent<Room_Ctrl>();
        roomObj.RoomType = mapType;
        roomObj.RoomId = grid;

        Rooms[grid] = roomObj;
    }

    // 맵에 하나만 있어야하는 방을 체크 후 생성한다
    void SpawnUniqueRoom(ERoomType mapType)
    {
        bool isBoosRoom = mapType == ERoomType.Boss;
        Vector2Int uniqueKey = new Vector2Int();

        // 문이 하나 + 노멀 방 찾기
        foreach (Vector2Int key in (isBoosRoom)? Rooms.Keys.Reverse() : Rooms.Keys)
        {
            if (Rooms[key].RoomType != ERoomType.Normal) continue; // 현재 확인중인 맵이 Normal이 아닌경우


            // 위칸 확인
            {
                Vector2Int checkkey = key + Vector2Int.up;

                if (!Rooms.ContainsKey(checkkey))
                {
                    int count = GetNeighborRoomCount(checkkey, isBoosRoom);

                    if (count == 1)
                    {
                        uniqueKey = checkkey;
                        break;
                    }
                }
            }

            // 아래칸 확인
            {
                Vector2Int checkkey = key + Vector2Int.down;

                if (!Rooms.ContainsKey(checkkey))
                {
                    int count = GetNeighborRoomCount(checkkey, isBoosRoom);

                    if (count == 1)
                    {
                        uniqueKey = checkkey;
                        break;
                    }
                }
            }

            // 왼칸 확인
            {
                Vector2Int checkkey = key + Vector2Int.left;

                if (!Rooms.ContainsKey(checkkey))
                {
                    int count = GetNeighborRoomCount(checkkey, isBoosRoom);

                    if (count == 1)
                    {
                        uniqueKey = checkkey;
                        break;
                    }
                }
            }

            // 오른칸 확인
            {
                Vector2Int checkkey = key + Vector2Int.right;

                if (!Rooms.ContainsKey(checkkey))
                {
                    int count = GetNeighborRoomCount(checkkey, isBoosRoom);

                    if (count == 1)
                    {
                        uniqueKey = checkkey;
                        break;
                    }
                }
            }

        }


        SpawnRoom(mapType, uniqueKey);
    }

    // 맵들을 모두 스폰 후 각 방마다 방설정
    void SettingRooms()
    {
        // 네비게이션 업데이트
        Mgr_Navigation.Inst.NavMeshUpdate();

        foreach (Vector2Int pos in Rooms.Keys)
        {
            Room_Ctrl room = Rooms[pos];

            // 문 설정
            {
                //위
                room.IsOn_Up = Rooms.ContainsKey(pos + Vector2Int.up);
                if (room.IsOn_Up) room.DoorType_Up = (room.RoomType == ERoomType.Normal || room.RoomType == ERoomType.Start) ? Rooms[pos + Vector2Int.up].RoomType : room.RoomType;

                // 아래
                room.IsOn_Down = Rooms.ContainsKey(pos + Vector2Int.down);
                if (room.IsOn_Down) room.DoorType_Down = (room.RoomType == ERoomType.Normal || room.RoomType == ERoomType.Start) ? Rooms[pos + Vector2Int.down].RoomType : room.RoomType;

                // 오른쪽
                room.IsOn_Right = Rooms.ContainsKey(pos + Vector2Int.right);
                if (room.IsOn_Right) room.DoorType_Right = (room.RoomType == ERoomType.Normal || room.RoomType == ERoomType.Start) ? Rooms[pos + Vector2Int.right].RoomType : room.RoomType;

                // 왼쪽
                room.IsOn_Left = Rooms.ContainsKey(pos + Vector2Int.left);
                if (room.IsOn_Left) room.DoorType_Left = (room.RoomType == ERoomType.Normal || room.RoomType == ERoomType.Start) ? Rooms[pos + Vector2Int.left].RoomType : room.RoomType;
            }


            // 처음 스폰위치가 아닌 맵들은 오브젝트 꺼두기
            room.SetRoom(room.RoomType == ERoomType.Start);

            if (room.RoomType == ERoomType.Start)
            {
                Mgr_Camera.Inst.MoveCamera(room.transform); // 시작지점일 경우 시작 방으로 카메라 이동
                PlayRoomKey = room.RoomId; // 플레이 중인 맵으로 설정
                Mgr_GameUI.Inst.MiniMapUpdate(Rooms, PlayRoomKey);//UI 미니맵 업데이트
            }
        }


        Mgr_Camera.Inst.MoveCamera(Rooms[PlayRoomKey].gameObject.transform);


        Invoke(nameof(StartRoom), 0.5f);
    }


    // 맵 세팅이 끝난 후 게임을 시작하기 위해 호출되는 함수
    void StartRoom()
    {
        Rooms[PlayRoomKey].StartRoom();
    }


    // === 유틸함수 ===

    // 상하좌우 방향 배열을 무작위로 섞어 반환
    Vector2Int[] ShuffledDirs()
    {
        var arr = (Vector2Int[])kDirs.Clone();
        for (int i = 0; i < arr.Length; i++)
        {
            int j = Random.Range(i, arr.Length);
            (arr[i], arr[j]) = (arr[j], arr[i]);
        }
        return arr;
    }

    Room_Ctrl GetRoomPrefab(ERoomType mapType)
    {
        Room_Ctrl prefab = null;
        switch (mapType)
        {
            case ERoomType.Start:
            {
                prefab = StartRoomPrefab;
                break;
            }
            case ERoomType.Normal:
            {
                prefab = NormalRoomPrefab[Random.Range(0, NormalRoomPrefab.Length)];
                break;
            }
            case ERoomType.Boss:
            {
                prefab = BossRoomPrefab[Random.Range(0, BossRoomPrefab.Length)];
                break;
            }
            case ERoomType.Treasurer:
            {
                prefab = TreasurerRoomPrefab[Random.Range(0, TreasurerRoomPrefab.Length)];
                break;
            }
        }

        return prefab;
    }

    // key값을 받으면 해당 위치 상하좌우에 방이 몇개인지 리턴해주는 함수
    int GetNeighborRoomCount(Vector2Int checkkey, bool isBoosRoom)
    {
        // 방 근처의 방개수 체크
        int count = 0;


        // 위
        Vector2Int key = checkkey + Vector2Int.up;
        if (Rooms.ContainsKey(key))
        {
            if (isBoosRoom)
            {
                // 보스맵일 경우노멀맵 근처에서만 카운팅
                if (Rooms[key].RoomType == ERoomType.Normal)
                    count++;
                else
                    count = 100;
            }
            else
            {
                // 방이 있을 경우 카운팅
                if (Rooms[key].RoomType == ERoomType.Normal || Rooms[key].RoomType == ERoomType.Start)
                    count++;
            }
        }

        // 아래
        key = checkkey + Vector2Int.down;
        if (Rooms.ContainsKey(key))
        {
            if (isBoosRoom)
            {
                // 보스맵일 경우노멀맵 근처에서만 카운팅
                if (Rooms[key].RoomType == ERoomType.Normal)
                    count++;
                else
                    count = 100;
            }
            else
            {
                // 방이 있을 경우 카운팅
                if (Rooms[key].RoomType == ERoomType.Normal || Rooms[key].RoomType == ERoomType.Start)
                    count++;
            }
        }

        // 왼쪽
        key = checkkey + Vector2Int.left;
        if (Rooms.ContainsKey(key))
        {
            if (isBoosRoom)
            {
                // 보스맵일 경우노멀맵 근처에서만 카운팅
                if (Rooms[key].RoomType == ERoomType.Normal)
                    count++;
                else
                    count = 100;
            }
            else
            {
                // 방이 있을 경우 카운팅
                if (Rooms[key].RoomType == ERoomType.Normal || Rooms[key].RoomType == ERoomType.Start)
                    count++;
            }
        }

        // 오른쪽
        key = checkkey + Vector2Int.right;
        if (Rooms.ContainsKey(key))
        {
            if (isBoosRoom)
            {
                // 보스맵일 경우노멀맵 근처에서만 카운팅
                if (Rooms[key].RoomType == ERoomType.Normal)
                    count++;
                else
                    count = 100;
            }
            else
            {
                // 방이 있을 경우 카운팅
                if (Rooms[key].RoomType == ERoomType.Normal || Rooms[key].RoomType == ERoomType.Start)
                    count++;
            }
        }

        return count;
    }

    // === 외부 호출 ===
    // 플레이어가 맵을 이동할때 호출
    public void MoveRoom(Room_Ctrl room, EDirection direction)
    {
        // EDirection -> Vector2Int 전환
        Vector2Int target = new Vector2Int();
        switch (direction)
        {
            case EDirection.Up:
            {
                target = Vector2Int.up; 
                break;
            }
            case EDirection.Down:
            {
                target = Vector2Int.down;
                break;
            }
            case EDirection.Right:
            {
                target = Vector2Int.right;
                break;
            }
            case EDirection.left:
            {
                target = Vector2Int.left;
                break;
            }
        }

       
        Rooms[room.RoomId].SetRoom(false);  // 기존의 방 끄기
        Rooms[room.RoomId + target].SetRoom(true);  // 새롭게 이동할 방 켜기
        PlayRoomKey = room.RoomId + target;

        // 보스방인 경우
        if (Rooms[PlayRoomKey].RoomType == ERoomType.Boss && Rooms[PlayRoomKey].IsClear == false)
        {
            Debug.Log("보스방 입장");
            Mgr_Loading.Inst.StartChangeScenes(ELoadingType.ToBossRoom);
            Mgr_GameUI.Inst.SetBossHpBarPanel(true);
        }


        Mgr_Camera.Inst.MoveCamera(Rooms[room.RoomId + target].gameObject.transform);
        Invoke(nameof(StartRoom), 0.2f);

        //UI 미니맵 업데이트
        Mgr_GameUI.Inst.MiniMapUpdate(Rooms, PlayRoomKey);
    }

    #endregion
}
