using UnityEngine;


public enum ESpriteOrder
{
    Room = -2, // 맵
    RoomAccessory = -1, // 맵 악세서리(돌조각, 폭탄 그을림 등)
    MapObject = 0,  // 맵 오브젝트, 네비게이션
    TearOrBomb = 1,  // 눈물, 폭탄
    Character = 2,  // 플레이어 몸통, 몬스터
    Item = 3,  // 아이템
    Boss = 4,  // 보스
    PlayerHead = 5,  // 플레이어 머리
}

public enum EDirection // 방향을 나타내는 열거형
{
    Idle = 0,
    Up,
    Down,
    left,
    Right
}

public enum EPickUpItemType // 픽업 아이템 종류
{
    Gold,
    Bomb,
    Key,
    Heart
}

public enum EHeartType // 하트 종류
{
    Empty = 0,  // 빈 하트
    Half,   // 반칸
    Full    // 한 칸
}

public enum ERoomType // 방 타입
{
    Start,  // 스테이지 시작 방
    Normal,     // 일반 방
    Boss,       // 보스 방
    Treasurer   // 황금 방
}

public enum EBossType
{
    Dingle
}


public enum EItemType // 아이템 종류
{
    Cricketshead = 0,



    End,
}


public enum EMiniMapIconState // 미니맵 아이콘의 상태
{
    Current,    // 플레이 중 (현재 방)
    Visited,    // 들어갔던 적이 있음
    Discovered, // 가본 적 없음 (보이긴 하지만 아직 입장x)
    Hidden      // 안보임 (미발견)

}



public enum EDamageType // 데미지 타입
{
    Normal,
    Bomb

}

public enum EOptionType
{
    Sound,
    End
}

// 데미지 인터페이스
public interface IDamage
{
    void TakeDamage(float inDamage, EDamageType inDamageType);
    void KnockBack(Vector3 inHitPos, float inPower);
}




public class GlobalValue
{

    public static bool IsCanPlay = true;

    public static float SoundVolume = 1.0f; //(0 ~ 1)


    public static void LoadData()
    {
        SoundVolume = PlayerPrefs.GetFloat("SoundVolume", 1f);
    }


}
