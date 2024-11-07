using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class UpSpell : MonoBehaviour
{
    [SerializeField] private Transform splashPos;
    [SerializeField] private GameObject splashVfx;
    [SerializeField] private ParticleSystem bubblevfx;
    [SerializeField] private float delay;
    [SerializeField] private CameraShakeProfile shakeProfile;

    private CinemachineImpulseSource impulseSource; 

    void Start()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
        StartCoroutine(UpSpellEffect(splashVfx, splashPos, Quaternion.identity, delay));
        BubbleEffect();
    }
    private IEnumerator UpSpellEffect(GameObject obj,Transform transform,Quaternion rotation,float delay)
    {
        yield return new WaitForSeconds(delay);
        Instantiate(obj,transform.position,rotation);
        CameraManager.instance.CameraShakeFromProfile(shakeProfile, impulseSource);
    }
    private void BubbleEffect()
    {
        Instantiate(bubblevfx, splashPos.position, Quaternion.identity);
    }
}
