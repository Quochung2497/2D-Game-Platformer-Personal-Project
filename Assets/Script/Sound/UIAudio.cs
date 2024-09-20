using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class UIAudio : MonoBehaviour
{
    [SerializeField] AudioClip click;
    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void SoundOnClick()
    {
        audioSource.PlayOneShot(click);
    }
}
