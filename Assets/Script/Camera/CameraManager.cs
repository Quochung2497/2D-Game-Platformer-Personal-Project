using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera[] allVirtualCameras;


    private CinemachineVirtualCamera currentCamera;
    private CinemachineFramingTransposer framingTransposer;
    public static CameraManager instance { get; private set; }

    [Header("Y Damping Settings for Player Jump/Fall:")]
    [SerializeField] private float panAmount = 0.1f;
    [SerializeField] private float panTime = 0.2f;
    public float playerFallSpeedThreshold = -10;
    public bool isLerpingYDamping;
    public bool hasLerpedYDamping;

    private CinemachineImpulseDefinition impulseDefinition;
    private float normalYDamp;
    private CameraTarget target;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        for (int i = 0; i < allVirtualCameras.Length; i++)
        {
            if (allVirtualCameras[i].enabled)
            {
                currentCamera = allVirtualCameras[i];

                framingTransposer = currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            }
            var impulseListener = allVirtualCameras[i].GetComponent<CinemachineImpulseListener>();
            if (impulseListener == null)
            {
                impulseListener = allVirtualCameras[i].gameObject.AddComponent<CinemachineImpulseListener>();
            }
        }
        normalYDamp = framingTransposer.m_YDamping;
    }
    private void Start()
    {
        target = GetComponentInChildren<CameraTarget>();
        for (int i = 0; i < allVirtualCameras.Length; i++)
        {
                //allVirtualCameras[i].Follow = PlayerController.Instance.transform;
                allVirtualCameras[i].Follow = target.transform;
        }
    }


    public void SwapCamera(CinemachineVirtualCamera _newCam)
    {
        currentCamera.Priority = 0;
        currentCamera.enabled = false;
        currentCamera = _newCam;
        currentCamera.enabled = true;
        currentCamera.Priority = 10;
    }
    public IEnumerator LerpYDamping(bool _isPlayerFalling)
    {
        isLerpingYDamping = true;
        float _startYDamp = framingTransposer.m_YDamping;
        float _endYDamp = 0;
        if (_isPlayerFalling)
        {
            _endYDamp = panAmount;
            hasLerpedYDamping = true;
        }
        else
        {
            _endYDamp = normalYDamp;
        }
        //lerp panAmount
        float _timer = 0;
        while (_timer < panTime)
        {
            _timer += Time.deltaTime;
            float _lerpedPanAmount = Mathf.Lerp(_startYDamp, _endYDamp, (_timer / panTime));
            framingTransposer.m_YDamping = _lerpedPanAmount;
            yield return null;
        }
        isLerpingYDamping = false;
    }
    public void CameraShakeFromProfile(CameraShakeProfile profile, CinemachineImpulseSource impulseSource)
    {
        //apply settings
        SetupCameraShakeSettings(profile, impulseSource);
        //camerashake
        impulseSource.GenerateImpulseWithForce(profile.impactForce);
    }
    private void SetupCameraShakeSettings(CameraShakeProfile profile, CinemachineImpulseSource impulseSource)
    {
        impulseDefinition = impulseSource.m_ImpulseDefinition;
        //change impulse source settings
        impulseDefinition.m_ImpulseDuration = profile.impactTime;
        impulseSource.m_DefaultVelocity = profile.defaultVelocity;
        impulseDefinition.m_CustomImpulseShape = profile.impulseCurve;

        //change impulse listener settings
        foreach (var virtualCamera in allVirtualCameras)
        {
            if (virtualCamera.enabled)
            {
                var impulseListener = virtualCamera.GetComponent<CinemachineImpulseListener>();
                if (impulseListener != null)
                {
                    impulseListener.m_ReactionSettings.m_AmplitudeGain = profile.listenerAmplitude;
                    impulseListener.m_ReactionSettings.m_FrequencyGain = profile.listenerFrequency;
                    impulseListener.m_ReactionSettings.m_Duration = profile.listenerDuration;
                }
            }
        }
    }
}
