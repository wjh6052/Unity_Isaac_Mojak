using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ELoadingType
{
    ToGame,
    ToMenu,
    ReGame,
    ToBossRoom
}

public class Mgr_Loading : MonoBehaviour
{
    public LoadingPanel ToGame_Panel;
    public BossRoomPanel ToBossRoom_Panel;

    ELoadingType LodingType;

    public static Mgr_Loading Inst;

    private void Awake()
    {
        Inst = this;

        ToGame_Panel.gameObject.SetActive(false);
        ToBossRoom_Panel.gameObject.SetActive(false);
    }

    public void StartChangeScenes(ELoadingType lodingType)
    {
        LodingType = lodingType;

        
        switch (LodingType)
        {
            case ELoadingType.ToGame:
                {
                    ToGame_Panel.StartLoding();
                    break;
                }
            case ELoadingType.ToMenu:
                {
                    ToGame_Panel.StartLoding();
                    break;
                }
            case ELoadingType.ReGame:
                {
                    ToGame_Panel.StartLoding();
                    break;
                }
            case ELoadingType.ToBossRoom:
                {
                    Time.timeScale = 0;
                    ToBossRoom_Panel.StartLoding();
                    break;
                }
        }

    }

    public void ChangeScenes()
    {
        switch (LodingType)
        {
            case ELoadingType.ToGame:
                {
                    if(SceneManager.GetSceneByName("MenuScene").isLoaded)
                    {
                        SceneManager.UnloadSceneAsync("MenuScene");

                        StartCoroutine(SwapToGameScene());
                    }
                    break;
                }
            case ELoadingType.ToMenu:
                {
                    if (SceneManager.GetSceneByName("GameScene").isLoaded)
                    {
                        SceneManager.UnloadSceneAsync("GameScene");
                        SceneManager.LoadScene("MenuScene", LoadSceneMode.Additive);
                    }
                    break;
                }

            case ELoadingType.ReGame:
                {
                    if (SceneManager.GetSceneByName("GameScene").isLoaded)
                    {
                        SceneManager.UnloadSceneAsync("GameScene");

                        StartCoroutine(SwapToGameScene());
                    }
                    break;
                }
        }
    }

    IEnumerator SwapToGameScene()
    {
        // 1) GameScene�� Additive�� �ε�
        AsyncOperation op = SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Additive);
        yield return op; // �ε� �Ϸ���� ���

        // 2) �ε�� �� ���� �� Ȱ�� ������ ����
        Scene gameScene = SceneManager.GetSceneByName("GameScene");
        SceneManager.SetActiveScene(gameScene);
    }

}
