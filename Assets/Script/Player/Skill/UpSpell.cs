using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpSpell : MonoBehaviour
{
    [SerializeField] private Transform splashPos;
    [SerializeField] private GameObject splashVfx;
    [SerializeField] private ParticleSystem bubblevfx;
    [SerializeField] private float delay;

    void Start()
    {
        StartCoroutine(UpSpellEffect(splashVfx, splashPos, Quaternion.identity, delay));
        BubbleEffect();
    }
    private IEnumerator UpSpellEffect(GameObject obj,Transform transform,Quaternion rotation,float delay)
    {
        yield return new WaitForSeconds(delay);
        Instantiate(obj,transform.position,rotation);
    }
    private void BubbleEffect()
    {
        Instantiate(bubblevfx, splashPos.position, Quaternion.identity);
    }
}
