using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class PlayerParticles : MonoBehaviour
{
    [Header("Slash")]
    [SerializeField] private ParticleSystem SwordSlashForward;
    [SerializeField] private ParticleSystem SwordSlashUp;
    [SerializeField] private ParticleSystem SwordSlashDown;
    [SerializeField] private ParticleSystem SwordSlashCombo2;
    [SerializeField] private ParticleSystem SwordSlashCombo3;
    [Header("WaterVfx")]
    [SerializeField] private GameObject WaterFootVFX;
    [SerializeField] private GameObject WaterSplashVFX;
    [SerializeField] private GameObject WaterJumpSplashVFX;
    [Header("JumpVfx")]
    public GameObject wallJumpVfx;
    [Header("DashVfx")]
    [SerializeField] private GameObject startDashEffect;
    [SerializeField] private GameObject dashEffect;
    [Header("WingVfx")]
    [SerializeField] private GameObject DoubleJumpWing;

    [Header("LeafVfx")]
    [SerializeField] private ParticleSystem leafVfx;
    [SerializeField] private float leafInterval;
    [SerializeField] private int spawnFrom;
    [SerializeField] private int spawnTo;

    [Header("HitObjectEffect")]
    [SerializeField] private ParticleSystem EnemyVfx;

    [Header("Variables")]
    [SerializeField] private Transform CharacterWaterVFX;
    [SerializeField] private Transform DoubleWingPos;
    private float FootEffectDelay;
    public int JumpSplashCounted = 0;
    public int DoubleJumpVfxCounted = 0;

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
    public void Combo2Slash()
    {
        SwordSlashCombo2.Play();
    }
    public void Combo3Slash()
    {
        SwordSlashCombo3.Play();
    }

    public void WaterFootPrint()
    {
        FootEffectDelay += Time.deltaTime;
        if (PlayerController.Instance.IsOnWater() && FootEffectDelay >= 0.2f && PlayerController.Instance.rb.velocity.x != 0 && PlayerController.Instance.Grounded())
        {
            FootEffectDelay = 0;
            Debug.Log("isOnWater");
            Instantiate(WaterFootVFX, CharacterWaterVFX.transform.position, Quaternion.identity);
        }
    }
    public void WaterSplash()
    {
       
        if (PlayerController.Instance.IsOnWater())
        {
            GameObject splash = Instantiate(WaterSplashVFX,CharacterWaterVFX.transform.position, Quaternion.identity);
            if (!PlayerController.Instance.isFacingRight)
            {
                Vector3 localScale = splash.transform.localScale;
                localScale.x *= -1; // Lật ngược theo trục X
                splash.transform.localScale = localScale;
            }
        }
    }
    public void WaterJumpVfx()
    {
        if(PlayerController.Instance.IsOnWater() && PlayerController.Instance.rb.velocity.y > 0)
        {
            if (JumpSplashCounted == 0)
            { 
                GameObject JumpVFX = Instantiate(WaterJumpSplashVFX, transform.position, Quaternion.identity);
                JumpSplashCounted++;
            }
        }
    }

    public void WingVfx()
    {
        if(!PlayerController.Instance.Grounded() && PlayerController.Instance.rb.velocity.y > 0)
        {
            if (DoubleJumpVfxCounted == 0)
            {
                Instantiate(DoubleJumpWing, DoubleWingPos.transform);
                DoubleJumpVfxCounted++;
            }
            int randomSpawn = Random.Range(spawnFrom, spawnTo);
            StartCoroutine(LeafSpawn(leafInterval, randomSpawn));
        }
    }

    private IEnumerator LeafSpawn(float seconds, int time)
    {
        for (int i = 0; i < time; i++)
        {
            Instantiate(leafVfx, DoubleWingPos.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(seconds);
        }
    }

    public void HitEnemyVfx(Transform direction)
    {
         Instantiate(EnemyVfx, direction.position, Quaternion.identity);
    }
    public void DashVfx()
    {
        Instantiate(dashEffect, PlayerController.Instance.transform);
    }
    public void StartDashVfx(Transform transform,Quaternion rotation)
    {
        Instantiate(startDashEffect,transform.position, rotation);
    }
    public void PlayVfx(GameObject gameobj,Transform direction,Quaternion rotation)
    {
        Instantiate(gameobj, direction.position, rotation);
    }
}
