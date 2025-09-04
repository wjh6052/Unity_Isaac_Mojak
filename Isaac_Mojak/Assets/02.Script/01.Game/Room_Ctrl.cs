using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room_Ctrl : MonoBehaviour
{
    [Header("���� ����")]
    public ERoomType RoomType; // ���� ����
    public Vector2Int RoomId;

    [Header("���� ����")]
    public bool IsClear = false;

    [Header("Ÿ�ϸ�")]
    public TilemapRenderer TilemapR;

    #region �� ����
    [Header("�� ��")]
    public bool IsOn_Up = true;
    public ERoomType DoorType_Up;
    public Tile_Door DoorObj_Up;

    [Header("�Ʒ� ��")]
    public bool IsOn_Down = true;
    public ERoomType DoorType_Down;
    public Tile_Door DoorObj_Down;

    [Header("������ ��")]
    public bool IsOn_Right = true;
    public ERoomType DoorType_Right;
    public Tile_Door DoorObj_Right;

    [Header("���� ��")]
    public bool IsOn_Left = true;
    public ERoomType DoorType_Left;
    public Tile_Door DoorObj_Left;

    #endregion

    // �濡�� ������ ���͵��� �����ϴ� ����Ʈ
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

    // ���� �޴������� ȣ��(���� ������Ʈ ���� ų�� ���) 
    public void SetRoom(bool IsRoom)
    {
        this.gameObject.SetActive(IsRoom);
    }

    // ���� �޴������� ȣ��(�÷��̾ �濡 ����������)
    public void StartRoom()
    {
        // ���Ͱ� ������
        if (MonsterList.Count <= 0 && BossCtrl == null)
        {
            IsClear = true;
        }

        // Ŭ���� ��
        if (IsClear)
        {
            // ������
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


    // �� ��ũ��Ʈ���� ȣ��
    public void AddDoor(Tile_Door inDoor)
    {
        // ���� ���� �߽ɿ��� � �������� Ȯ��
        Vector2 dir = inDoor.transform.position - this.transform.position;
        

        if (dir.y > 1f) // ��
        {
            DoorObj_Up = inDoor;
            DoorObj_Up.SetDoor(EDirection.Up);
        }
        else if (dir.y < -1f) // �Ʒ�
        {
            DoorObj_Down = inDoor;
            DoorObj_Down.SetDoor(EDirection.Down);
        }
        else if (dir.x > 1f) // ������
        {
            DoorObj_Right = inDoor;
            DoorObj_Right.SetDoor(EDirection.Right);
        }
        else // ����
        {
            DoorObj_Left = inDoor;
            DoorObj_Left.SetDoor(EDirection.left);
        }
    }


    // ���� ��ũ��Ʈ���� ȣ��
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

    // ����� ���Ͱ� �׾����� ȣ��
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

        // ���Ͱ� ��� �׾��� ��
        if(MonsterList.Count <= 0 && BossCtrl == null)
        {
            IsClear = true;
            StartRoom();
        }
    }

}
