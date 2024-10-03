using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParticles : MonoBehaviour
{
    [Header("Slash")]
    [SerializeField] ParticleSystem SwordSlashForward;
    [SerializeField] ParticleSystem SwordSlashUp;
    [SerializeField] ParticleSystem SwordSlashDown;
    public void ForwardSlash()
    {
        SwordSlashForward.Play();
    }
    public void UpSlash()
    {
        SwordSlashUp.Play();
    }
    public void DownSlash()
    {
        SwordSlashDown.Play();
    }
}
