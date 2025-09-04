using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveMenu_Ctrl : MonoBehaviour
{
    public Animator[] FileArr = new Animator[3];

    public int NowIndex = 0;



    void Start()
    {
        NowIndex = 0;

        for (int i = 0; i < FileArr.Length; i++)
        {
            FileArr[i].SetBool("IsChoice", NowIndex == i);
        }
    }

    void SwitchingFile(int index)
    {
        FileArr[NowIndex].SetBool("IsChoice", false);
        FileArr[index].SetBool("IsChoice", true);

        NowIndex = index;
        
    }

    public void MoveSaveMenu(float value)
    {
        bool isLeft = (value < 0) ? true : false;

        int i = isLeft ? -1 : 1;
        int max = FileArr.Length;
        int index = (NowIndex + i + max) % max;


        SwitchingFile(index);
    }

    public void ChoiceMenu()
    {
        Mgr_Menu.Inst.SaveIndex = NowIndex;
    }

}
