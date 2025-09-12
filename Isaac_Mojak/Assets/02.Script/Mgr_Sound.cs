using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mgr_Sound : MonoBehaviour
{
    List<AudioSource> AudioList = new List<AudioSource>();

    public static Mgr_Sound Inst;

    private void Awake()
    {
        if(Inst)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Inst = this;
            DontDestroyOnLoad(this.gameObject);
            AudioList.Clear();
            GlobalValue.LoadData();
        }
    }



    
    public void PlaySound(GameObject obj, AudioClip inAudioClip, bool isLoop = false)
    {
        StartCoroutine(Co_PlaySound(obj, inAudioClip, isLoop));
    }


    // ���带 �� ������Ʈ�� AudioSource�� �������Ѽ� �����ϴ� �ڷ�ƾ
    IEnumerator Co_PlaySound(GameObject obj, AudioClip inAudioClip, bool isLoop = false)
    {
        AudioSource audio = obj.AddComponent<AudioSource>();
        AudioList.Add(audio);
        audio.playOnAwake = false;
        audio.spatialBlend = 0f;  // 2D�� (�� �Ÿ� ���� X). 3D�� �ʿ��ϸ� 1f��.
        audio.loop = isLoop;
        audio.clip = inAudioClip;
        audio.volume = GlobalValue.SoundVolume;
        audio.Play();

        

        if(isLoop)
        {
            while (obj != null)
            {
                yield return null;
            }
        }
        else
        {
            // �ش� Ŭ���� ���������� ���
            yield return new WaitForSeconds(audio.clip.length);
        }
            

        if(audio)
        {
            AudioList.Remove(audio);
            Destroy(audio);
        }
    }

    public void SetSoundVolume(float volume)
    {
        GlobalValue.SoundVolume = volume;
        PlayerPrefs.SetFloat("SoundVolume", GlobalValue.SoundVolume);


        foreach (AudioSource src in AudioList.ToArray())   // ������
        {
            if (src == null) 
            {
                AudioList.Remove(src);
                continue; 
            }
            src.volume = GlobalValue.SoundVolume;
        }
    }
}
