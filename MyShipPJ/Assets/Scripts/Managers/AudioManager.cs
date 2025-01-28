using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameData;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource sfxSources;
    public AudioSource bgmSource;

    public AudioClip[] sfxClips;
    public AudioClip[] bgmClips;

    public enum SFXClip { NONE = -1, FAIL, EATTING, SLIDE, BUY, CLICK, SUCCESS, MAX }
    public enum BGMClip { NONE = -1, MAX }

    public List<AudioSource> tempSources;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

    }

    void Start()
    {
        StartCoroutine(ClearTemp());
        // sfxSources = new List<AudioSource>();

        // for(int i = 0 ; i < (int)SFXClip.MAX ; i++){
        //     sfxSources.Add(gameObject.AddComponent<AudioSource>());
        //     sfxSources[i].clip = sfxClips[i];
        // }
        // 뭐가 더 빠른 지 모르겠네...
    }

    public void PlaySFX(SFXClip clip)
    {
        if (sfxSources.isPlaying)
        {
            tempSources.Add(gameObject.AddComponent<AudioSource>());
            tempSources[tempSources.Count - 1].clip = sfxClips[(int)clip];
            tempSources[tempSources.Count - 1].Play();

        }
        else
        {
            sfxSources.clip = sfxClips[(int)clip];
            sfxSources.Play();
        }
    }

    IEnumerator ClearTemp()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f);
            for (int i = 0; i < tempSources.Count; i++)
            {
                if (!tempSources[i].isPlaying)
                {
                    Destroy(tempSources[i]);
                    tempSources.RemoveAt(i);
                    i--;
                }
            }
        }
    }

    public void PlayBGM(BGMClip clip)
    {
        bgmSource.clip = bgmClips[(int)clip];
        bgmSource.Play();
    }
}
