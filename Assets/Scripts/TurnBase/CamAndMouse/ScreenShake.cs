using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace TurnBase
{
    public class ScreenShake : Singleton<ScreenShake>
    {
        private CinemachineImpulseSource cinemachineImpulseSource;
        protected override void Awake()
        {
            base.Awake();
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