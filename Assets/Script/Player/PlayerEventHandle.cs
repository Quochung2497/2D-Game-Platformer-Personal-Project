using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEventHandle : MonoBehaviour
{
   public void AttackForward(int index)
    {
        PlayerController.Instance.attackForward(index);
    }
    public void AttackUp()
    {
        PlayerController.Instance.attackUp();
    }
    public void AttackDown()
    {
        PlayerController.Instance.attackDown();
    }
    public void ChargeOrbEffect()
    {
        PlayerController.Instance.ChargeOrb();
    }
    public void ChargeOrbExplosion()
    {
        PlayerController.Instance.ChargeOrbExplode();
    }
    public void DiveFireExplosion()
    {
        PlayerController.Instance.DiveFireExplode();
    }
    public void ChargeOrbPartciles(int index)
    {
        if (index == 1)
        { 
            PlayerController.Instance.ChargeOrbParticles(true); 
        }
        if (index == 0)
        {
            PlayerController.Instance.ChargeOrbParticles(false);
        }
    }
}
