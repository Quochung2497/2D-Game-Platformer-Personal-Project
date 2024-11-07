using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName ="CameraShake/New Profile")]
public class CameraShakeProfile : ScriptableObject
{
    [Header("Impulse Source Settings")]
    public float impactTime = 0.2f;
    public float impactForce = 1f;
    public Vector3 defaultVelocity = new Vector3(0f,-1f,0f);
    public AnimationCurve impulseCurve;

    [Header("Impulse Listener Settings")]
    public float listenerAmplitude = 1f;
    public float listenerFrequency = 1f;
    public float listenerDuration = 1f;

}
