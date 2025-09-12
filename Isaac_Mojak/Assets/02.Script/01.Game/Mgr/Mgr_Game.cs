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

        // �� ���� ����
        StartSpawnRooms();

        // �÷��̾� ����
        {
            Player_Ctrl obj = Resources.Load("Player_Prefab").GetComponent<Player_Ctrl>();
            Player_Ctrl player_Prefab = Instantiate(obj);
            player_Prefab.gameObject.transform.position = Vector3.zero;
            Player = player_Prefab;
        }
        
    }

    void Update()
    {
        // �׽�Ʈ
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

    // EscŬ���� Mgr_Input���� ȣ��
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


    // �÷��̾ �׾����� ȣ��
    public void PlayerDie()
    {
        Time.timeScale = 0;

        Mgr_GameUI.Inst.PlayerDieUI(true);
    }


    // �������� �Ծ����� ���� ��ȭ
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



    #region �� ���� �˰���
    /* �� ���� ��Ģ
    1. ���۹��� (0,0)
    2. �¿� �̵��� x + 4.2, ���� �̵� �� y + 2.4
    3. ��ü �� ������ minRooms ~ maxRooms �������� ����
    4. ��� ���� ���� ��ũ ������� Ȯ��
    5. ����/Ȳ�ݹ��� �ݵ�� 1���� ����
    6. ����/Ȳ�ݹ��� ���ٸ� �濡 ��ġ�ؾ� ��(����� ���� 1����)
     */


    [Header("�� / �÷��� ���� ��")]
    public Vector2Int PlayRoomKey;

    [Header("�� / ��ġ ����")]
    public Transform RoomRoot;  // ������ ����� �θ� ������Ʈ
    float OffSetX = 4f;   // ��/�� �̵� ������
    float OffSetY = 2.2f;   // ��/�� �̵� ������

    [Header("�� / ���� �ɼ�")]
    public int MinRooms = 6;    // �ּ� �� �� (���۹� ���� �ȵ�)
    public int MaxRooms = 12;   // �ִ� �� �� (���۹� ���� �ȵ�)
    public int Seed = 0;        // �õ�

    [Header("�� / ������")]
    public Room_Ctrl StartRoomPrefab;
    public Room_Ctrl[] NormalRoomPrefab;
    public Room_Ctrl[] BossRoomPrefab;
    public Room_Ctrl[] TreasurerRoomPrefab;


    // ������ ����� Ű���� ���� (Vector2�� 2�����迭ó�� Ű������ ����ϱ� ����)
    public Dictionary<Vector2Int, Room_Ctrl> Rooms = new Dictionary<Vector2Int, Room_Ctrl>();
    // �Ϲ� Vector2��뿡�� float�̱⶧���� �ε��Ҽ��� �񱳰� ���� int�� ��ü
    readonly Vector2Int[] kDirs = { Vector2Int.up, Vector2Int.down, Vector2Int.right, Vector2Int.left };


    // �� ���� �����Լ�
    void StartSpawnRooms()
    {
        // �ʱ�ȭ
        Rooms.Clear();


        // �õ� Ȯ��
        if (Seed == 0) // �õ尡 ���� ���
        {
            Seed = System.Environment.TickCount; 
            Random.InitState(System.Environment.TickCount); // ���� �������� ����
        }
        else // �õ尡 �ִ� ���
            Random.InitState(Seed); // ���� ����


        // ���� �� ����
        SpawnRoom(ERoomType.Start, Vector2Int.zero);

        int roomsCount = Random.Range(MinRooms + 1, MaxRooms + 2);
        int guard = 0;
        

        // ���� ��ġ�� �� Ȯ��
        while(Rooms.Count < roomsCount && guard < 1000)
        {
            List<Vector2Int> keys = new List<Vector2Int>(Rooms.Keys);
            Vector2Int pos = keys[Random.Range(0, keys.Count)];

            foreach(Vector2Int key in ShuffledDirs())
            {
                Vector2Int next = pos + key;
                // �������� �޾ƿ� ��ġ Ű���� �ִ��� Ȯ��
                if(!Rooms.ContainsKey(next))
                {
                    // ���ٸ� ����
                    SpawnRoom(ERoomType.Normal, next);
                    break;
                }
            }
            
            // ���� ���� ����
            guard++;
        }


        // Ȳ�ݹ� ����
        SpawnUniqueRoom(ERoomType.Treasurer);
        // ������ ����
        SpawnUniqueRoom(ERoomType.Boss);

        // �ʵ��� ��� ���� �� �� �渶�� �漳�� �ϱ�
        SettingRooms();
    }

    // === ��ġ �Լ� ===
    // ���� ���� �� Rooms�� ����
    void SpawnRoom(ERoomType mapType, Vector2Int grid)
    {
        Vector3 spawnPos = new Vector3(grid.x * OffSetX, grid.y * OffSetY, 0f);
        Room_Ctrl roomObj = Instantiate(GetRoomPrefab(mapType), spawnPos, Quaternion.identity, RoomRoot).GetComponent<Room_Ctrl>();
        roomObj.RoomType = mapType;
        roomObj.RoomId = grid;

        Rooms[grid] = roomObj;
    }

    // �ʿ� �ϳ��� �־���ϴ� ���� üũ �� �����Ѵ�
    void SpawnUniqueRoom(ERoomType mapType)
    {
        bool isBoosRoom = mapType == ERoomType.Boss;
        Vector2Int uniqueKey = new Vector2Int();

        // ���� �ϳ� + ��� �� ã��
        foreach (Vector2Int key in (isBoosRoom)? Rooms.Keys.Reverse() : Rooms.Keys)
        {
            if (Rooms[key].RoomType != ERoomType.Normal) continue; // ���� Ȯ������ ���� Normal�� �ƴѰ��


            // ��ĭ Ȯ��
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

            // �Ʒ�ĭ Ȯ��
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

            // ��ĭ Ȯ��
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

            // ����ĭ Ȯ��
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

    // �ʵ��� ��� ���� �� �� �渶�� �漳��
    void SettingRooms()
    {
        // �׺���̼� ������Ʈ
        Mgr_Navigation.Inst.NavMeshUpdate();

        foreach (Vector2Int pos in Rooms.Keys)
        {
            Room_Ctrl room = Rooms[pos];

            // �� ����
            {
                //��
                room.IsOn_Up = Rooms.ContainsKey(pos + Vector2Int.up);
                if (room.IsOn_Up) room.DoorType_Up = (room.RoomType == ERoomType.Normal || room.RoomType == ERoomType.Start) ? Rooms[pos + Vector2Int.up].RoomType : room.RoomType;

                // �Ʒ�
                room.IsOn_Down = Rooms.ContainsKey(pos + Vector2Int.down);
                if (room.IsOn_Down) room.DoorType_Down = (room.RoomType == ERoomType.Normal || room.RoomType == ERoomType.Start) ? Rooms[pos + Vector2Int.down].RoomType : room.RoomType;

                // ������
                room.IsOn_Right = Rooms.ContainsKey(pos + Vector2Int.right);
                if (room.IsOn_Right) room.DoorType_Right = (room.RoomType == ERoomType.Normal || room.RoomType == ERoomType.Start) ? Rooms[pos + Vector2Int.right].RoomType : room.RoomType;

                // ����
                room.IsOn_Left = Rooms.ContainsKey(pos + Vector2Int.left);
                if (room.IsOn_Left) room.DoorType_Left = (room.RoomType == ERoomType.Normal || room.RoomType == ERoomType.Start) ? Rooms[pos + Vector2Int.left].RoomType : room.RoomType;
            }


            // ó�� ������ġ�� �ƴ� �ʵ��� ������Ʈ ���α�
            room.SetRoom(room.RoomType == ERoomType.Start);

            if (room.RoomType == ERoomType.Start)
            {
                Mgr_Camera.Inst.MoveCamera(room.transform); // ���������� ��� ���� ������ ī�޶� �̵�
                PlayRoomKey = room.RoomId; // �÷��� ���� ������ ����
                Mgr_GameUI.Inst.MiniMapUpdate(Rooms, PlayRoomKey);//UI �̴ϸ� ������Ʈ
            }
        }


        Mgr_Camera.Inst.MoveCamera(Rooms[PlayRoomKey].gameObject.transform);


        Invoke(nameof(StartRoom), 0.5f);
    }


    // �� ������ ���� �� ������ �����ϱ� ���� ȣ��Ǵ� �Լ�
    void StartRoom()
    {
        Rooms[PlayRoomKey].StartRoom();
    }


    // === ��ƿ�Լ� ===

    // �����¿� ���� �迭�� �������� ���� ��ȯ
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

    // key���� ������ �ش� ��ġ �����¿쿡 ���� ����� �������ִ� �Լ�
    int GetNeighborRoomCount(Vector2Int checkkey, bool isBoosRoom)
    {
        // �� ��ó�� �氳�� üũ
        int count = 0;


        // ��
        Vector2Int key = checkkey + Vector2Int.up;
        if (Rooms.ContainsKey(key))
        {
            if (isBoosRoom)
            {
                // �������� ����ָ� ��ó������ ī����
                if (Rooms[key].RoomType == ERoomType.Normal)
                    count++;
                else
                    count = 100;
            }
            else
            {
                // ���� ���� ��� ī����
                if (Rooms[key].RoomType == ERoomType.Normal || Rooms[key].RoomType == ERoomType.Start)
                    count++;
            }
        }

        // �Ʒ�
        key = checkkey + Vector2Int.down;
        if (Rooms.ContainsKey(key))
        {
            if (isBoosRoom)
            {
                // �������� ����ָ� ��ó������ ī����
                if (Rooms[key].RoomType == ERoomType.Normal)
                    count++;
                else
                    count = 100;
            }
            else
            {
                // ���� ���� ��� ī����
                if (Rooms[key].RoomType == ERoomType.Normal || Rooms[key].RoomType == ERoomType.Start)
                    count++;
            }
        }

        // ����
        key = checkkey + Vector2Int.left;
        if (Rooms.ContainsKey(key))
        {
            if (isBoosRoom)
            {
                // �������� ����ָ� ��ó������ ī����
                if (Rooms[key].RoomType == ERoomType.Normal)
                    count++;
                else
                    count = 100;
            }
            else
            {
                // ���� ���� ��� ī����
                if (Rooms[key].RoomType == ERoomType.Normal || Rooms[key].RoomType == ERoomType.Start)
                    count++;
            }
        }

        // ������
        key = checkkey + Vector2Int.right;
        if (Rooms.ContainsKey(key))
        {
            if (isBoosRoom)
            {
                // �������� ����ָ� ��ó������ ī����
                if (Rooms[key].RoomType == ERoomType.Normal)
                    count++;
                else
                    count = 100;
            }
            else
            {
                // ���� ���� ��� ī����
                if (Rooms[key].RoomType == ERoomType.Normal || Rooms[key].RoomType == ERoomType.Start)
                    count++;
            }
        }

        return count;
    }

    // === �ܺ� ȣ�� ===
    // �÷��̾ ���� �̵��Ҷ� ȣ��
    public void MoveRoom(Room_Ctrl room, EDirection direction)
    {
        // EDirection -> Vector2Int ��ȯ
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

       
        Rooms[room.RoomId].SetRoom(false);  // ������ �� ����
        Rooms[room.RoomId + target].SetRoom(true);  // ���Ӱ� �̵��� �� �ѱ�
        PlayRoomKey = room.RoomId + target;

        // �������� ���
        if (Rooms[PlayRoomKey].RoomType == ERoomType.Boss && Rooms[PlayRoomKey].IsClear == false)
        {
            Debug.Log("������ ����");
            Mgr_Loading.Inst.StartChangeScenes(ELoadingType.ToBossRoom);
            Mgr_GameUI.Inst.SetBossHpBarPanel(true);
        }


        Mgr_Camera.Inst.MoveCamera(Rooms[room.RoomId + target].gameObject.transform);
        Invoke(nameof(StartRoom), 0.2f);

        //UI �̴ϸ� ������Ʈ
        Mgr_GameUI.Inst.MiniMapUpdate(Rooms, PlayRoomKey);
    }

    #endregion
}
