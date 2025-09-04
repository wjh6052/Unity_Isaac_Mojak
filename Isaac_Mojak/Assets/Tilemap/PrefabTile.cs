using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;



[CreateAssetMenu(menuName = "Tilemap/Prefab Tile")]
public class PrefabTile : TileBase
{
    public GameObject Prefab;   // Ÿ���� ���� �� ������ ������(��ũ��Ʈ ���� ����)
    public Sprite Sprite;       // �ȷ�Ʈ/������/Ÿ�ϸʿ��� ���� ��������Ʈ



    // ��/�ȷ�Ʈ �� ���̴� Ÿ���� ���Ѹ���� ����
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        if (!Application.isPlaying)
        {
            tileData.sprite = Sprite;                  // ���� �߿��� �ȷ�Ʈ/���� ����
            tileData.colliderType = Tile.ColliderType.None;

            // �����⿡���� ��������Ʈ ũ�� ����
            if (Sprite != null)
            {
                var tm = tilemap.GetComponent<Tilemap>();
                Vector3 cell3 = (tm != null && tm.layoutGrid != null) ? tm.layoutGrid.cellSize : new Vector3(1f, 1f, 1f);

                Vector2 spriteSize = Sprite.bounds.size; // PPU �ݿ��� ���� ũ��
                float sx = spriteSize.x > 0f ? cell3.x / spriteSize.x : 1f;
                float sy = spriteSize.y > 0f ? cell3.y / spriteSize.y : 1f;

                tileData.transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(sx, sy, 1f));
            }
        }
        else 
        {
            tileData.sprite = null;                    // ��Ÿ��(Play)���� ����
            tileData.colliderType = Tile.ColliderType.None;
        }

        tileData.gameObject = Prefab;                  // �������� �׻� �ڵ� ����/����
        tileData.flags = TileFlags.LockTransform | TileFlags.LockColor;
    }
}
