using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AEA
{
    public class ParallaxBehavior : MonoBehaviour
    {
        [SerializeField] private Vector2 _parallaxEffectMultiplier;

        private Transform _cameraTransform;
        private Vector3 _lastCameraPosition;


        void Start()
        {
            _cameraTransform = Camera.main.transform;
            _lastCameraPosition = _cameraTransform.position;
        }

        private void LateUpdate()
        {
            ParallaxEffect();
        }


        private void ParallaxEffect()
        {
            Vector3 deltaMovement = _cameraTransform.position - _lastCameraPosition;
            transform.position += new Vector3(deltaMovement.x * _parallaxEffectMultiplier.x, deltaMovement.y * _parallaxEffectMultiplier.y);
            _lastCameraPosition = _cameraTransform.position;
        }
    }
}
