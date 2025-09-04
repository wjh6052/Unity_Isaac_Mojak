using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;



[CreateAssetMenu(menuName = "Tilemap/Prefab Tile")]
public class PrefabTile : TileBase
{
    public GameObject Prefab;   // 타일을 찍을 때 생성할 프리팹(스크립트 포함 가능)
    public Sprite Sprite;       // 팔레트/에디터/타일맵에서 보일 스프라이트



    // 씬/팔레트 상에 보이는 타일의 “겉모습” 정의
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        if (!Application.isPlaying)
        {
            tileData.sprite = Sprite;                  // 편집 중에는 팔레트/씬에 보임
            tileData.colliderType = Tile.ColliderType.None;

            // 편집기에서만 스프라이트 크기 보정
            if (Sprite != null)
            {
                var tm = tilemap.GetComponent<Tilemap>();
                Vector3 cell3 = (tm != null && tm.layoutGrid != null) ? tm.layoutGrid.cellSize : new Vector3(1f, 1f, 1f);

                Vector2 spriteSize = Sprite.bounds.size; // PPU 반영된 월드 크기
                float sx = spriteSize.x > 0f ? cell3.x / spriteSize.x : 1f;
                float sy = spriteSize.y > 0f ? cell3.y / spriteSize.y : 1f;

                tileData.transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(sx, sy, 1f));
            }
        }
        else 
        {
            tileData.sprite = null;                    // 런타임(Play)에는 숨김
            tileData.colliderType = Tile.ColliderType.None;
        }

        tileData.gameObject = Prefab;                  // 프리팹은 항상 자동 스폰/삭제
        tileData.flags = TileFlags.LockTransform | TileFlags.LockColor;
    }
}
