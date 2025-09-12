using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class OptionMenu_Ctrl : MonoBehaviour
{
    public EOptionType OptionType = 0;

    public GameObject[] Choice_Image = new GameObject[2];

    [Header("사운드")]
    public Image SoundVolume_Image;


    private void Start()
    {
        for(int i = 0; i < Choice_Image.Length; i++)
        {
            Choice_Image[i].SetActive(i == 0);
        }

        SoundVolume_Image.fillAmount = GlobalValue.SoundVolume;

        
    }

    public void MenuMove(Vector2 val)
    {
        // y 상하
        // x 좌우
        if (val == Vector2.zero) return;


        if (val.y != 0.0f) // 상하
        {
            int num = ((int)OptionType + (val.y > 0.0f ? -1 : 1) + Choice_Image.Length) % Choice_Image.Length;
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
        if (OptionType != EOptionType.End) return;

        Mgr_Menu.Inst.SetMenu(EMenuType.GameMenu);
    }

    


    


}
