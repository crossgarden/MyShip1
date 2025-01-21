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

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // sfxSources = new List<AudioSource>();

        // for(int i = 0 ; i < (int)SFXClip.MAX ; i++){
        //     sfxSources.Add(gameObject.AddComponent<AudioSource>());
        //     sfxSources[i].clip = sfxClips[i];
        // }
        // 뭐가 더 빠른 지 모르겠네...
    }

    public void PlaySFX(SFXClip clip)
    {
        sfxSources.clip = sfxClips[(int)clip];
        sfxSources.Play();
    }

    public void PlayBGM(BGMClip clip)
    {
        bgmSource.clip = bgmClips[(int)clip];
        bgmSource.Play();
    }
}
