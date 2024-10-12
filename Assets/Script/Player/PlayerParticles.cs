using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParticles : MonoBehaviour
{
    [Header("Slash")]
    [SerializeField] private ParticleSystem SwordSlashForward;
    [SerializeField] private ParticleSystem SwordSlashUp;
    [SerializeField] private ParticleSystem SwordSlashDown;
    [SerializeField] private GameObject WaterFootVFX, WaterSplashVFX, WaterJumpSplashVFX;
    private float FootEffectDelay;

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
        if (PlayerController.Instance.IsOnWater() && FootEffectDelay >= 0.2f && PlayerController.Instance.rb.velocity.x != 0)
        {
            FootEffectDelay = 0;
            Debug.Log("isOnWater");
            Instantiate(WaterFootVFX, PlayerController.Instance.CharacterWaterVFX.transform.position, Quaternion.identity);
        }
    }
    public void WaterSplash()
    {
       
        if (PlayerController.Instance.IsOnWater())
        {
            GameObject splash = Instantiate(WaterSplashVFX, PlayerController.Instance.CharacterWaterVFX.transform.position, Quaternion.identity);
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

    }
}
