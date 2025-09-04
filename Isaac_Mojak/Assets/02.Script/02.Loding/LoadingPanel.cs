using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class LoadingPanel : MonoBehaviour
{

    CanvasGroup PanelCanvasGroup;

    public void StartLoding()
    {
        if(PanelCanvasGroup == null) PanelCanvasGroup = GetComponent<CanvasGroup>();

        GlobalValue.IsCanPlay = false;
        this.gameObject.SetActive(true);


    }

    protected void AE_ChangeScenes()
    {
        Mgr_Loading.Inst.ChangeScenes();
    }

    protected virtual void AE_EndChangeScenes()
    {
        GlobalValue.IsCanPlay = true;
        this.gameObject.SetActive(false);
    }
}
