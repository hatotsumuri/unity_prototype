using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmManager : MonoBehaviour
{
    public AudioClip battle01Clip;
    public AudioClip battle02Clip;
    public AudioClip battle03Clip;
    public AudioClip battle04Clip;
    public AudioClip battle05Clip;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlayBttle01()
    {
        audioSource.clip = battle01Clip;
        audioSource.loop = true;
        //audioSource.Play();
    }

    public void PlayBttle02()
    {
        audioSource.clip = battle02Clip;
        audioSource.loop = true;
        //audioSource.Play();
    }

    public void PlayBttle03()
    {
        audioSource.clip = battle03Clip;
        audioSource.loop = true;
        //audioSource.Play();
    }

    public void PlayBttle04()
    {
        audioSource.clip = battle04Clip;
        audioSource.loop = true;
        //audioSource.Play();
    }

    public void PlayBttle05()
    {
        audioSource.clip = battle05Clip;
        audioSource.loop = true;
        //audioSource.Play();
    }
}
