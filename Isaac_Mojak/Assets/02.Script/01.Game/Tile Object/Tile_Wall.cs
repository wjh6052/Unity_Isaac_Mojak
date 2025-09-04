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
            // 벽의 방의 중심에서 어떤 방향인지 확인
            Dir = this.transform.position - parent.position;

            TearsWall_U.SetActive(false);
            TearsWall_R.SetActive(false);
            TearsWall_L.SetActive(false);


            if(Dir.y > 1f || Dir.y < -1f) // 위 또는 아래
            {
                TearsWall_U.SetActive(true);
                if(Dir.y > 1f && Dir.x > 2f) // 위 오른쪽 대각선 체크
                    TearsWall_R.SetActive(true);
                else if (Dir.y > 1f && Dir.x < -2f) // 위 왼쪽 대각선 체크
                    TearsWall_L.SetActive(true);

            }
                
            else if(Dir.x > 1f) // 오른쪽
                TearsWall_R.SetActive(true);
            else // 왼쪽
                TearsWall_L.SetActive(true);
        }
    }
}
