using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public static BGMManager instance;

    private AudioSource source;

    public AudioClip[] clips;
    public float[] settingVolumn;

    private WaitForSeconds waitTime = new WaitForSeconds(0.01f);

    private void Awake()
    {
        if(instance == null) { DontDestroyOnLoad(gameObject); instance = this; }
        else Destroy(this.gameObject);
    }

    void Start()
    {
        source = GetComponent<AudioSource>();
        settingVolumn = new float[clips.Length];
        for (int i = 0; i < settingVolumn.Length; i++)
        {
            if(settingVolumn[i] == 0) settingVolumn[i] = 0.1f;
        }
        Play(0);
    }

    public void Play(int _playMusicTrack)
    {
        source.clip = clips[_playMusicTrack];
        source.volume = settingVolumn[_playMusicTrack];
        source.Play();
    }

    public void SetVolumn(float _volumn)
    {
        source.volume = _volumn;
    }

    public void Pause()
    {
        source.Pause();
    }

    public void Stop()
    {
        source.Stop();
    }

    public void FadeOutMusic()
    {
        StopAllCoroutines();
        StartCoroutine("FadeOutMusicCoroutine");
    }

    IEnumerator FadeOutMusicCoroutine()
    {
        for (float i = source.volume; i >= 0f; i -= 0.01f)
        {
            source.volume = i;
            yield return waitTime;
        }
    }
    public void FadeInMusic(int _playMusicTrack)
    {
        StopAllCoroutines();
        StartCoroutine("FadeInMusicCoroutine", _playMusicTrack);
    }

    IEnumerator FadeInMusicCoroutine(int _playMusicTrack)
    {
        for (float i = 0f; i <= settingVolumn[_playMusicTrack]; i += 0.01f)
        {
            source.volume = i;
            yield return waitTime;
        }
    }
}
