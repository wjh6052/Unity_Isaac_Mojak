using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterMenu_Ctrl : MonoBehaviour
{
    public void ChoiceMenu()
    {
        Mgr_Loading.Inst.StartChangeScenes(ELoadingType.ToGame);
    }
}
