using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class ParticlesController : MonoBehaviour
{
    [Header("Setting Particles")]
    [SerializeField] private Transform CharacterWaterVFX;
    [SerializeField] private Transform DoubleWingPos;

    private float FootEffectDelay;
    public int JumpEffectCounted = 0;
    public int DoubleJumpVfxCounted = 0;
    private ParticlesEffect particlesEffect;

    private void Start()
    {
        particlesEffect = GetComponentInParent<ParticlesEffect>();    
    }
    public void FootPrint()
    {
        FootEffectDelay += Time.deltaTime;
        if (PlayerController.Instance.rb.velocity.x != 0 && PlayerController.Instance.Grounded() && FootEffectDelay >= 0.2f)
        {
            if (PlayerController.Instance.IsOnWater())
            {
                Instantiate(particlesEffect.WaterFootVFX, CharacterWaterVFX.transform.position, Quaternion.identity);
            }
            else if (!PlayerController.Instance.IsOnWater())
            {
                particlesEffect.moveVfx.Play();
            }
            FootEffectDelay = 0;
        }
    }
    public void StartDashEffect(Quaternion rotation)
    {
        if(PlayerController.Instance.IsOnWater())
        {
            Instantiate(particlesEffect.WaterSplashVFX, CharacterWaterVFX.transform.position, rotation);
        }
        else
        {
            Instantiate(particlesEffect.startDashEffect, PlayerController.Instance.FootCheckPoint.transform.position, rotation);
        }
    }
    public void JumpVfx(Quaternion rotation)
    {
        if (PlayerController.Instance.rb.velocity.y > 0)
        {
            if (JumpEffectCounted == 0)
            {
                if (PlayerController.Instance.IsOnWater())
                {
                    Instantiate(particlesEffect.WaterJumpSplashVFX, transform.position, Quaternion.identity);
                }
                else if(PlayerController.Instance.Grounded())
                {
                    Instantiate(particlesEffect.JumpEffect, PlayerController.Instance.FootCheckPoint.transform.position, rotation);
                }
                JumpEffectCounted++;
            }
        }
        else if (PlayerController.Instance.Grounded() && PlayerController.Instance.rb.velocity.y == 0)
        {
            JumpEffectCounted = 0;
        }
    }

    public void WingVfx()
    {
        if(!PlayerController.Instance.Grounded() && PlayerController.Instance.rb.velocity.y > 0)
        {
            if (DoubleJumpVfxCounted == 0)
            {
                Instantiate(particlesEffect.DoubleJumpWing, DoubleWingPos.transform);
                DoubleJumpVfxCounted++;
            }
            int randomSpawn = Random.Range(particlesEffect.spawnFrom, particlesEffect.spawnTo);
            StartCoroutine(LeafSpawn(particlesEffect.leafInterval, randomSpawn));
        }
    }

    private IEnumerator LeafSpawn(float seconds, int time)
    {
        for (int i = 0; i < time; i++)
        {
            Instantiate(particlesEffect.leafVfx, DoubleWingPos.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(seconds);
        }
    }

}
