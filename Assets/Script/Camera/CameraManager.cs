using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera[] allVirtualCameras;

    private CinemachineVirtualCamera currentCamera;
    private CinemachineFramingTransposer framingTransposer;
    public static CameraManager instance { get; private set; }

    [Header("Y Damping Settings for Player Jump/Fall:")]
    [SerializeField] private float panAmount = 0.1f;
    [SerializeField] private float panTime = 0.2f;
    public float playerFallSpeedThreshold = -10;
    public bool isLerpingYDamping;
    public bool hasLerpedYDamping;

    private float normalYDamp;

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
        }
        normalYDamp = framingTransposer.m_YDamping;
    }
    private void Start()
    {
        for (int i = 0; i < allVirtualCameras.Length; i++)
        {
            allVirtualCameras[i].Follow = PlayerController.Instance.transform;
        }
    }
    public void SwapCamera(CinemachineVirtualCamera _newCam)
    {
        currentCamera.enabled = false;
        currentCamera = _newCam;
        currentCamera.enabled = true;
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
}
