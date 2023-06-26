using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [SerializeField] List<AudioClip> m_AudioClips = new List<AudioClip>();

    AudioSource _audioSource;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        _audioSource= GetComponent<AudioSource>();

        PlayMusic(1);
    }

    public void PlayMusic(int num)
    {
        StopMusic();

        _audioSource.clip = m_AudioClips[num];

        _audioSource.Play();
        _audioSource.loop = true;

        //ProcessClips();
    }

    void StopMusic()
    {
        _audioSource.Stop();
    }

    // Add clip processing if time to get the looping one going
    //void ProcessClips()
    //{
    //    _audioSource.clip.name = "main-menu-intro";


    //}
}