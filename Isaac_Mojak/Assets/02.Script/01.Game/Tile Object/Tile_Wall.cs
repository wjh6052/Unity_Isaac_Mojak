using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile_Wall : MonoBehaviour
{
    public GameObject TearsWall_U;
    public GameObject TearsWall_R;
    public GameObject TearsWall_L;
    public Vector2 Dir;

    void Start()
    {
        Transform parent = transform.parent.parent.parent;
        if (parent != null)
        {
            // ���� ���� �߽ɿ��� � �������� Ȯ��
            Dir = this.transform.position - parent.position;

            TearsWall_U.SetActive(false);
            TearsWall_R.SetActive(false);
            TearsWall_L.SetActive(false);


            if(Dir.y > 1f || Dir.y < -1f) // �� �Ǵ� �Ʒ�
            {
                TearsWall_U.SetActive(true);
                if(Dir.y > 1f && Dir.x > 2f) // �� ������ �밢�� üũ
                    TearsWall_R.SetActive(true);
                else if (Dir.y > 1f && Dir.x < -2f) // �� ���� �밢�� üũ
                    TearsWall_L.SetActive(true);

            }
                
            else if(Dir.x > 1f) // ������
                TearsWall_R.SetActive(true);
            else // ����
                TearsWall_L.SetActive(true);
        }
    }
}
