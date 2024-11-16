using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesEffect : MonoBehaviour
{
    [Header("MovementVfx")]
    public ParticleSystem moveVfx;
    [Header("Slash")]
    public ParticleSystem SwordSlashForward;
    public ParticleSystem SwordSlashUp;
    public ParticleSystem SwordSlashDown;
    public ParticleSystem SwordSlashCombo2;
    public ParticleSystem SwordSlashCombo3;
    [Header("WaterVfx")]
    public GameObject WaterFootVFX;
    public GameObject WaterSplashVFX;
    public GameObject WaterJumpSplashVFX;
    [Header("JumpVfx")]
    public GameObject wallJumpVfx;
    public GameObject JumpEffect;
    public GameObject doubleJumpEffect;
    public GameObject landEffect;
    [Header("DashVfx")]
    public GameObject startDashEffect;
    [Header("WingVfx")]
    public GameObject DoubleJumpWing;
    [Header("Spellvfx")]
    public GameObject startSideCastVfx;
    public GameObject chargeOrbVfx;
    public GameObject chargeOrbParticles;
    public GameObject chargeOrbExplosionVfx;
    public GameObject diveVfx;

    [Header("LeafVfx")]
    public ParticleSystem leafVfx;
    public float leafInterval;
    public int spawnFrom;
    public int spawnTo;

    [Header("HitObjectEffect")]
    public GameObject enemyVfx;
    public GameObject HitEnemySplashEffect;
}
