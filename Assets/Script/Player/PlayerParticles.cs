using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParticles : MonoBehaviour
{
    [Header("Slash")]
    [SerializeField] private ParticleSystem SwordSlashForward;
    [SerializeField] private ParticleSystem SwordSlashUp;
    [SerializeField] private ParticleSystem SwordSlashDown;
    [Header("WaterVfx")]
    [SerializeField] private GameObject WaterFootVFX, WaterSplashVFX, WaterJumpSplashVFX;
    [Header("WingVfx")]
    [SerializeField] private GameObject DoubleJumpWing;

    [Header("Variables")]
    [SerializeField] private Transform CharacterWaterVFX;
    [SerializeField] private Transform DoubleWingPos;
    private float FootEffectDelay;
    public int JumpSplashCounted = 0;
    public int DoubleJumpVfxCounted = 0;

    private void start()
    {
        
    }

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
    public void WaterJumpVFX()
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
            if(DoubleJumpVfxCounted == 0)
            {
                Instantiate(DoubleJumpWing, DoubleWingPos.transform);
                DoubleJumpVfxCounted++;
            }
        }
    }
}
