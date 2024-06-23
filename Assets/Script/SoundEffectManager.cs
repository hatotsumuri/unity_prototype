using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectManager : MonoBehaviour
{
    public AudioClip attack01Clip;
    public AudioClip break01Clip;
    public AudioClip direct01Clip;
    private AudioSource audioSourceAttack;
    private AudioSource audioSourceBreak;
    private AudioSource audioSourceDirect;

    void Awake()
    {
        audioSourceAttack = gameObject.AddComponent<AudioSource>();
        audioSourceAttack.clip = attack01Clip;

        audioSourceBreak = gameObject.AddComponent<AudioSource>();
        audioSourceBreak.clip = break01Clip;

        audioSourceDirect = gameObject.AddComponent<AudioSource>();
        audioSourceDirect.clip = direct01Clip;
    }

    public void PlayAttack1()
    {
        audioSourceAttack.Play();
    }

    public void PlayBreak01()
    {
        audioSourceBreak.Play();
    }

    public void PlayDirect01()
    {
        audioSourceDirect.Play();
    }
}
