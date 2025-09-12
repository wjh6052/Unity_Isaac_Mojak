using UnityEngine;


public enum ESpriteOrder
{
    Room = -2, // ��
    RoomAccessory = -1, // �� �Ǽ�����(������, ��ź ������ ��)
    MapObject = 0,  // �� ������Ʈ, �׺���̼�
    TearOrBomb = 1,  // ����, ��ź
    Character = 2,  // �÷��̾� ����, ����
    Item = 3,  // ������
    Boss = 4,  // ����
    PlayerHead = 5,  // �÷��̾� �Ӹ�
}

public enum EDirection // ������ ��Ÿ���� ������
{
    Idle = 0,
    Up,
    Down,
    left,
    Right
}

public enum EPickUpItemType // �Ⱦ� ������ ����
{
    Gold,
    Bomb,
    Key,
    Heart
}

public enum EHeartType // ��Ʈ ����
{
    Empty = 0,  // �� ��Ʈ
    Half,   // ��ĭ
    Full    // �� ĭ
}

public enum ERoomType // �� Ÿ��
{
    Start,  // �������� ���� ��
    Normal,     // �Ϲ� ��
    Boss,       // ���� ��
    Treasurer   // Ȳ�� ��
}

public enum EBossType
{
    Dingle
}


public enum EItemType // ������ ����
{
    Cricketshead = 0,



    End,
}


public enum EMiniMapIconState // �̴ϸ� �������� ����
{
    Current,    // �÷��� �� (���� ��)
    Visited,    // ���� ���� ����
    Discovered, // ���� �� ���� (���̱� ������ ���� ����x)
    Hidden      // �Ⱥ��� (�̹߰�)

}



public enum EDamageType // ������ Ÿ��
{
    Normal,
    Bomb

}

public enum EOptionType
{
    Sound,
    End
}

// ������ �������̽�
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
