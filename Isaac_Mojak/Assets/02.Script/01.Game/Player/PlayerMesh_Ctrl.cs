using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;




public class PlayerMesh_Ctrl : MonoBehaviour
{
    public Player_Ctrl Player;
    public Animator A_Head;
    public Animator A_Body;
    public SpriteRenderer ItemImage;

    public EDirection HeadValue = 0;
    public EDirection BodyValue = 0;


    private void Awake()
    {
        ItemImage.gameObject.SetActive(false);
    }

    // 플레이어 애니메이션 조정
    public void AnimPlayer(Vector2 inMoveDirection, Vector2 inAttackDirection)
    {
        

        // 몸통 (0 = idle / 1 = U / 2 = D / 3 = L / 4 = R)
        {
            if (inMoveDirection == Vector2.zero) // 이동 입력이 없을때
            {
                BodyValue = EDirection.Idle;
            }
            else if(inMoveDirection.y != 0) // 위&아래 입력이 있을때
            {
                BodyValue = (inMoveDirection.y > 0) ? EDirection.Up : EDirection.Down;
            }
            else // 오른쪽 또는 왼쪽
            {
                BodyValue = (inMoveDirection.x > 0) ? EDirection.Right : EDirection.left;
            }
        }

        // 머리 (0 = idle / 1 = U / 2 = D / 3 = L / 4 = R)
        {
            if (inAttackDirection == Vector2.zero) // 공격 움직임이 없을 때
            {
                if(Mgr_Input.Inst.MoveDirection == Vector2.zero) // 움직임 입력이 없을때
                    HeadValue = EDirection.Idle;
                else
                    HeadValue = BodyValue;
            }
            else // 공격 움직임 입력을 받을 때
            {
                if(inAttackDirection.y > 0) // 위
                {
                    HeadValue = EDirection.Up;
                }
                else if (inAttackDirection.y < 0) // 아래
                {
                    HeadValue = EDirection.Down;
                }
                else if (inAttackDirection.x > 0) // 오른쪽
                {
                    HeadValue = EDirection.Right;
                }
                else // 왼쪽
                {
                    HeadValue = EDirection.left;
                }


            }

        }


        A_Body.SetFloat("Body", (int)BodyValue);
        A_Head.SetFloat("Head", (int)HeadValue);
    }


    // 머리 방향
    public void TearsAttack(Vector2 inAttackDirection)
    {
        EDirection direction = 0;

        if (inAttackDirection == Vector2.zero) // 이동 입력이 없을때
        {
            direction = EDirection.Idle;
        }
        else if (inAttackDirection.y != 0) // 위&아래 입력이 있을때
        {
            direction = (inAttackDirection.y > 0) ? EDirection.Up : EDirection.Down;
        }
        else // 오른쪽 또는 왼쪽
        {
            direction = (inAttackDirection.x > 0) ? EDirection.Right : EDirection.left;
        }


        A_Head.SetFloat("AttackDirection", (int)direction);
        A_Head.SetTrigger("Tears Attack");
    }



    public void AnimHit(bool isHit)
    {
        A_Body.gameObject.SetActive(!isHit);


        A_Head.SetBool("bHit", isHit);
    }

    public void AnimDie()
    {
        A_Body.gameObject.SetActive(false);

        A_Head.SetTrigger("Die");
    }


    public void AnimAddItem(EItemType inItemType)
    {
        A_Body.gameObject.SetActive(false);

        ItemImage.sprite = Resources.Load<Sprite>($"Item/Item/{inItemType.ToString()}");
        ItemImage.gameObject.SetActive(true);

        A_Head.SetTrigger("AddItem");
    }
}
