﻿/*#==========================================================================#*/
/*#    SoundManager.cs                                                       #*/
/*#                                                                          #*/
/*#    Summary    :    Resourcesフォルダに存在するAudioをいつでも再生・停止  #*/
/*#                    できる状態にしておく                                  #*/
/*#                                                                          #*/
/*#    How        :    1. Resources/Audio/BGM or SEにmp3ファイルを入れる     #*/
/*#                    2. SoundManager.Instance.メソッド名で呼び出す         #*/
/*#                                                                          #*/
/*#==========================================================================#*/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    [SerializeField, Range(0, 1), Tooltip("マスタ音量")]
    float volume = 1;
    [SerializeField, Range(0, 1), Tooltip("BGMの音量")]
    float bgmVolume = 1;
    [SerializeField, Range(0, 1), Tooltip("SEの音量")]
    float seVolume = 1;
    AudioClip[] bgm;
    AudioClip[] se;

    public int AudioLevel { set; get; }
    Dictionary<string, int> bgmIndex = new Dictionary<string, int>();
    Dictionary<string, int> seIndex = new Dictionary<string, int>();
    AudioSource bgmAudioSource;
    AudioSource seAudioSource;
    public float Volume
    {
        set
        {
            volume = Mathf.Clamp01(value);
            bgmAudioSource.volume = bgmVolume * volume * (AudioLevel / 3);
            seAudioSource.volume = seVolume * volume * (AudioLevel / 3);
        }
        get
        {
            return volume;
        }
    }
    public float BgmVolume
    {
        set
        {
            bgmVolume = Mathf.Clamp01(value);
            bgmAudioSource.volume = bgmVolume * volume;
        }
        get
        {
            return bgmVolume;
        }
    }
    public float SeVolume
    {
        set
        {
            seVolume = Mathf.Clamp01(value);
            seAudioSource.volume = seVolume * volume;
        }
        get
        {
            return seVolume;
        }
    }
    public new void Awake()
    {
        if (this != Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        AudioLevel = 3;
        bgmAudioSource = gameObject.AddComponent<AudioSource>();
        seAudioSource = gameObject.GetComponent<AudioSource>();
        bgm = Resources.LoadAll<AudioClip>("Audio/BGM");
        se = Resources.LoadAll<AudioClip>("Audio/SE");
        for (int i = 0; i < bgm.Length; i++)
        {
            bgmIndex.Add(bgm[i].name, i);
        }
        for (int i = 0; i < se.Length; i++)
        {
            seIndex.Add(se[i].name, i);
        }
    }
    public int GetBgmIndex(string name)
    {
        if (bgmIndex.ContainsKey(name))
        {
            return bgmIndex[name];
        }
        else
        {
            Debug.LogError("指定された名前のBGMファイルが存在しません。");
            return 0;
        }
    }
    public int GetSeIndex(string name)
    {
        if (seIndex.ContainsKey(name))
        {
            return seIndex[name];
        }
        else
        {
            Debug.LogError("指定された名前のSEファイルが存在しません。");
            return 0;
        }
    }

    //BGM再生
    public void PlayBgm(int index)
    {
        index = Mathf.Clamp(index, 0, bgm.Length);
        bgmAudioSource.clip = bgm[index];
        bgmAudioSource.loop = true;
        bgmAudioSource.volume = BgmVolume * Volume;
        bgmAudioSource.pitch = 1.0f;
        bgmAudioSource.Play();
    }
    public void PlayBgmByName(string name)
    {
        PlayBgm(GetBgmIndex(name));
    }
    public void StopBgm()
    {
        bgmAudioSource.Stop();
        bgmAudioSource.clip = null;
    }
    public void SetBGMPitchUP()
    {
        bgmAudioSource.pitch = 1.2f;
    }
    //SE再生
    public void PlaySe(int index)
    {
        index = Mathf.Clamp(index, 0, se.Length);
        seAudioSource.PlayOneShot(se[index], SeVolume * Volume);
    }
    public void PlaySeByName(string name)
    {
        PlaySe(GetSeIndex(name));
    }
    public void StopSe()
    {
        seAudioSource.Stop();
        seAudioSource.clip = null;
    }

}