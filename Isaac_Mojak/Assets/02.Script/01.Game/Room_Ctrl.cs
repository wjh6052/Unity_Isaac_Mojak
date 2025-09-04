using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room_Ctrl : MonoBehaviour
{
    [Header("방의 종류")]
    public ERoomType RoomType; // 맵의 종류
    public Vector2Int RoomId;

    [Header("방의 상태")]
    public bool IsClear = false;

    [Header("타일맵")]
    public TilemapRenderer TilemapR;

    #region 문 변수
    [Header("위 문")]
    public bool IsOn_Up = true;
    public ERoomType DoorType_Up;
    public Tile_Door DoorObj_Up;

    [Header("아래 문")]
    public bool IsOn_Down = true;
    public ERoomType DoorType_Down;
    public Tile_Door DoorObj_Down;

    [Header("오른쪽 문")]
    public bool IsOn_Right = true;
    public ERoomType DoorType_Right;
    public Tile_Door DoorObj_Right;

    [Header("왼쪽 문")]
    public bool IsOn_Left = true;
    public ERoomType DoorType_Left;
    public Tile_Door DoorObj_Left;

    #endregion

    // 방에서 스폰한 몬스터들을 관리하는 리스트
    List<Monster_Ctrl> MonsterList = new List<Monster_Ctrl>();

    public Boss_Ctrl BossCtrl;

    public Trapdoor_Ctrl Trapdoor;
    public Item_Ctrl Item;


    private void Awake()
    {
        MonsterList.Clear();
    }

    void Start()
    {
        TilemapR.enabled = false;
    }

    // 게임 메니저에서 호출(방의 오브젝트 껐다 킬때 사용) 
    public void SetRoom(bool IsRoom)
    {
        this.gameObject.SetActive(IsRoom);
    }

    // 게임 메니저에서 호출(플레이어가 방에 입장했을때)
    public void StartRoom()
    {
        // 몬스터가 없을때
        if (MonsterList.Count <= 0 && BossCtrl == null)
        {
            IsClear = true;
        }

        // 클리어 시
        if (IsClear)
        {
            // 문열기
            {
                DoorObj_Up?.StartOpen();
                DoorObj_Down?.StartOpen();
                DoorObj_Right?.StartOpen();
                DoorObj_Left?.StartOpen();

                Item?.gameObject.SetActive(true);
                Trapdoor?.gameObject.SetActive(true);
            }

            return;
        }
    }


    // 문 스크립트에서 호출
    public void AddDoor(Tile_Door inDoor)
    {
        // 벽의 방의 중심에서 어떤 방향인지 확인
        Vector2 dir = inDoor.transform.position - this.transform.position;
        

        if (dir.y > 1f) // 위
        {
            DoorObj_Up = inDoor;
            DoorObj_Up.SetDoor(EDirection.Up);
        }
        else if (dir.y < -1f) // 아래
        {
            DoorObj_Down = inDoor;
            DoorObj_Down.SetDoor(EDirection.Down);
        }
        else if (dir.x > 1f) // 오른쪽
        {
            DoorObj_Right = inDoor;
            DoorObj_Right.SetDoor(EDirection.Right);
        }
        else // 왼쪽
        {
            DoorObj_Left = inDoor;
            DoorObj_Left.SetDoor(EDirection.left);
        }
    }


    // 몬스터 스크립트에서 호출
    public void AddMonster(Monster_Ctrl inMonster)
    {
        if(inMonster.IsBoss)
        {
            BossCtrl = (Boss_Ctrl)inMonster;
        }
        else
        {
            MonsterList.Add(inMonster);
        }
    }

    // 방안의 몬스터가 죽었을때 호출
    public void DieMonster(Monster_Ctrl inMonster)
    {
        if (inMonster.IsBoss)
        {
            BossCtrl = null;
        }
        else
        {
            MonsterList.Remove(inMonster);
        }

        // 몬스터가 모두 죽었을 때
        if(MonsterList.Count <= 0 && BossCtrl == null)
        {
            IsClear = true;
            StartRoom();
        }
    }

}
