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

    public Collider2D Door_Collider;     // 문 안쪽 콜리더
    public Collider2D Wall_Collider;     // 벽 콜리더

    public SpriteRenderer Close_L; // 문짝 왼쪽
    public SpriteRenderer Close_R; // 문짝 오른쪽
    public SpriteRenderer DoorIn; // 문 안쪽 이미지
    public SpriteRenderer Door;   // 문 이미지


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

        // 켜짐
        if (IsOn)
        {
            Door_Collider.enabled = true;
            Wall_Collider.enabled = true;

            Close_L.enabled = true;
            Close_R.enabled = true;
            DoorIn.enabled = true;
            Door.enabled = true;
        }
        else // 꺼짐
        {
            Door_Collider.enabled = false;
            Wall_Collider.enabled = true;

            Close_L.enabled = false;
            Close_R.enabled = false;
            DoorIn.enabled = false;
            Door.enabled = false;
        }
    }

    // 문안에 콜리더와 접촉시
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

    // 열리는 애니메이션에서 호출될 함수
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
