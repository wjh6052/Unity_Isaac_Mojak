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

    // �÷��̾� �ִϸ��̼� ����
    public void AnimPlayer(Vector2 inMoveDirection, Vector2 inAttackDirection)
    {
        

        // ���� (0 = idle / 1 = U / 2 = D / 3 = L / 4 = R)
        {
            if (inMoveDirection == Vector2.zero) // �̵� �Է��� ������
            {
                BodyValue = EDirection.Idle;
            }
            else if(inMoveDirection.y != 0) // ��&�Ʒ� �Է��� ������
            {
                BodyValue = (inMoveDirection.y > 0) ? EDirection.Up : EDirection.Down;
            }
            else // ������ �Ǵ� ����
            {
                BodyValue = (inMoveDirection.x > 0) ? EDirection.Right : EDirection.left;
            }
        }

        // �Ӹ� (0 = idle / 1 = U / 2 = D / 3 = L / 4 = R)
        {
            if (inAttackDirection == Vector2.zero) // ���� �������� ���� ��
            {
                if(Mgr_Input.Inst.MoveDirection == Vector2.zero) // ������ �Է��� ������
                    HeadValue = EDirection.Idle;
                else
                    HeadValue = BodyValue;
            }
            else // ���� ������ �Է��� ���� ��
            {
                if(inAttackDirection.y > 0) // ��
                {
                    HeadValue = EDirection.Up;
                }
                else if (inAttackDirection.y < 0) // �Ʒ�
                {
                    HeadValue = EDirection.Down;
                }
                else if (inAttackDirection.x > 0) // ������
                {
                    HeadValue = EDirection.Right;
                }
                else // ����
                {
                    HeadValue = EDirection.left;
                }


            }

        }


        A_Body.SetFloat("Body", (int)BodyValue);
        A_Head.SetFloat("Head", (int)HeadValue);
    }


    // �Ӹ� ����
    public void TearsAttack(Vector2 inAttackDirection)
    {
        EDirection direction = 0;

        if (inAttackDirection == Vector2.zero) // �̵� �Է��� ������
        {
            direction = EDirection.Idle;
        }
        else if (inAttackDirection.y != 0) // ��&�Ʒ� �Է��� ������
        {
            direction = (inAttackDirection.y > 0) ? EDirection.Up : EDirection.Down;
        }
        else // ������ �Ǵ� ����
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
