using System.Collections.Generic;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Serializable]
    private class AudioClipData
    {
        public string Key;
        public AudioClip AudioClip;
    }


    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private AudioSource bgmAudioSource;

    [Header("SFX")]
    [SerializeField] List<AudioClipData> sfxAudioClipData = new();
    [Header("BGM")]
    [SerializeField] List<AudioClipData> bgmAudioClipData = new();

    private void Awake()
    {
        PlayBGM("menu");
    }

    public void PlaySFX(string key)
    {
        AudioClipData audioClipData = sfxAudioClipData.Find(data => data.Key == key);

        if (audioClipData != null)
        {
            sfxAudioSource.PlayOneShot(audioClipData.AudioClip);
        }
        else
            Debug.LogWarning($"[SFX] Can't find audio key: {key}");
    }

    public void StopSFX() => sfxAudioSource.Stop();

    public void PlayBGM(string key)
    {
        AudioClipData audioClipData = bgmAudioClipData.Find(data => data.Key == key);

        if (audioClipData != null)
        {
            bgmAudioSource.Stop();
            bgmAudioSource.clip = audioClipData.AudioClip;
            bgmAudioSource.Play();
        }
        else
            Debug.LogWarning($"[BGM] Can't find audio key: {key}");
    }

}
