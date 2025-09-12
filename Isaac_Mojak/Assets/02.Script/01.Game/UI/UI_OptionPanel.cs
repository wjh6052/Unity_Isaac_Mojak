using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class UI_OptionPanel : MonoBehaviour
{
   

    public EOptionType OptionType = 0;

    public GameObject[] Choice_Image = new GameObject[2];

    [Header("사운드")]
    public Image SoundVolume_Image;


    public void OnOptionPanel(bool isOn)
    {
        this.gameObject.SetActive(isOn);

        if (isOn)
        {
            OptionType = 0;
            for (int i = 0; i < Choice_Image.Length; i++)
                Choice_Image[i].SetActive(i == (int)OptionType);


            // 사운드 설정
            SoundVolume_Image.fillAmount = GlobalValue.SoundVolume;
        }
    }


    public void SetChoice(Vector2 val)
    {
        // y 상하
        // x 좌우
        if (val == Vector2.zero) return;


        if (val.y != 0.0f) // 상하
        {
            int num = ((int) OptionType +(val.y > 0.0f ? -1 : 1) + Choice_Image.Length) % Choice_Image.Length;
            OptionType = (EOptionType)num;

            for (int i = 0; i < Choice_Image.Length; i++)
                Choice_Image[i].SetActive(i == (int)OptionType);
        }
        else if (val.x != 0.0f) // 좌우
        {
            SetOptionValue(val.x);
        }
    }

    void SetOptionValue(float val)
    {
        if (val == 0.0f) return;

        switch (OptionType)
        {
            case EOptionType.Sound: // 사운드
                {
                    if (SoundVolume_Image.fillAmount + (val / 10.0f) < 0.0f || SoundVolume_Image.fillAmount + (val / 10.0f) > 1.0f) return;
                        
                    SoundVolume_Image.fillAmount += (val / 10.0f);

                    Mgr_Sound.Inst.SetSoundVolume(SoundVolume_Image.fillAmount);
                    break;
                }
            default:
                break;


        }

    }
    public void ChoiceMenu()
    {
        if (OptionType != EOptionType.Sound) return;

        this.gameObject.SetActive(false);
        Mgr_GameUI.Inst.GameUiType = EGameUiType.GameMenu;
        Mgr_GameUI.Inst.GameMenuPanel.gameObject.SetActive(true);


    }
}
