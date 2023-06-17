using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace TurnBase
{
    public class ScreenShake : MonoBehaviour
    {
        public static ScreenShake Instance { get; private set; }
        private CinemachineImpulseSource cinemachineImpulseSource;
        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
        }

        void Update()
        {
            // if (Input.GetKeyDown(KeyCode.T))
            // {
            //     cinemachineImpulseSource.GenerateImpulse();
            // }
        }

        public void Shake(float intensity = 1f)
        {
            cinemachineImpulseSource.GenerateImpulse(intensity);
        }
    }

}