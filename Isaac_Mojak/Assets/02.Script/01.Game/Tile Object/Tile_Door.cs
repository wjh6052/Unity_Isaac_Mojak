using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class Tile_Door : MonoBehaviour
{
    public bool IsOn;
    public bool IsOpen;
    Room_Ctrl Room_Onwer;


    public EDirection DoorDirection;
    public ERoomType DoorType;


    public Sprite[] Close_L_Sprite;
    public Sprite[] Close_R_Sprite;
    public Sprite[] Close_DoorIn;
    public Sprite[] Close_Door;

    public Collider2D Door_Collider;     // �� ���� �ݸ���
    public Collider2D Wall_Collider;     // �� �ݸ���

    public SpriteRenderer Close_L; // ��¦ ����
    public SpriteRenderer Close_R; // ��¦ ������
    public SpriteRenderer DoorIn; // �� ���� �̹���
    public SpriteRenderer Door;   // �� �̹���


    void Start()
    {
        Room_Onwer = transform.parent.parent.parent.gameObject.GetComponent<Room_Ctrl>();

        Room_Onwer.AddDoor(this);
        
    }

    public void SetDoor(EDirection inDoorDirection)
    {
        IsOpen = false;

        DoorDirection = inDoorDirection;

        Vector3 rot = this.transform.rotation.eulerAngles;

        switch (DoorDirection)
        {
            case EDirection.Up:
                {
                    IsOn = Room_Onwer.IsOn_Up;
                    DoorType = Room_Onwer.DoorType_Up;
                    rot.z = 0f;

                    break;
                }
            case EDirection.Down:
                {
                    IsOn = Room_Onwer.IsOn_Down;
                    DoorType = Room_Onwer.DoorType_Down;
                    rot.z = 180f;

                    break;
                }
            case EDirection.Right:
                {
                    IsOn = Room_Onwer.IsOn_Right;
                    DoorType = Room_Onwer.DoorType_Right;
                    rot.z = 270f;

                    break;
                }
            case EDirection.left:
                {
                    IsOn = Room_Onwer.IsOn_Left;
                    DoorType = Room_Onwer.DoorType_Left;
                    rot.z = 90f;

                    break;
                }

        }
        
        this.transform.rotation = Quaternion.Euler(rot);

        Close_L.sprite = Close_L_Sprite[(int)DoorType];
        Close_R.sprite = Close_R_Sprite[(int)DoorType];
        DoorIn.sprite = Close_DoorIn[(int)DoorType];
        Door.sprite = Close_Door[(int)DoorType];

        // ����
        if (IsOn)
        {
            Door_Collider.enabled = true;
            Wall_Collider.enabled = true;

            Close_L.enabled = true;
            Close_R.enabled = true;
            DoorIn.enabled = true;
            Door.enabled = true;
        }
        else // ����
        {
            Door_Collider.enabled = false;
            Wall_Collider.enabled = true;

            Close_L.enabled = false;
            Close_R.enabled = false;
            DoorIn.enabled = false;
            Door.enabled = false;
        }
    }

    // ���ȿ� �ݸ����� ���˽�
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag != "Player") return;

        Mgr_Game.Inst.MoveRoom(Room_Onwer, DoorDirection);
    }

    public void StartOpen()
    {
        if (!IsOn) return;

        GetComponent<Animator>().SetTrigger("Open");
    }

    // ������ �ִϸ��̼ǿ��� ȣ��� �Լ�
    public void EndOpen()
    {
        if (!IsOn) return;

        Close_L.enabled = false;
        Close_R.enabled = false;
        Wall_Collider.enabled = false;
    }

    public void StartClose()
    {
        if (!IsOn) return;

        Close_L.enabled = true;
        Close_R.enabled = true;
        Wall_Collider.enabled = true;

        GetComponent<Animator>().SetTrigger("Close");
    }

}
