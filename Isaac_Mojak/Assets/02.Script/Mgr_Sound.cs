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


    // 사운드를 낼 오브젝트에 AudioSource를 스폰시켜서 관리하는 코루틴
    IEnumerator Co_PlaySound(GameObject obj, AudioClip inAudioClip, bool isLoop = false)
    {
        AudioSource audio = obj.AddComponent<AudioSource>();
        AudioList.Add(audio);
        audio.playOnAwake = false;
        audio.spatialBlend = 0f;  // 2D로 (먼 거리 감쇠 X). 3D가 필요하면 1f로.
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
            // 해당 클립이 끝날때까지 대기
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


        foreach (AudioSource src in AudioList.ToArray())   // 스냅샷
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
