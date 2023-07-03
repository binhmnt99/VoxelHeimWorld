using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class UnitRagDollSpawner : MonoBehaviour
    {
        [SerializeField] private Transform ragDollPrefab;
        [SerializeField] private Transform originalRootBone;

        private HealthSystem healthSystem;

        void Awake()
        {
            healthSystem = GetComponent<HealthSystem>();

            healthSystem.OnDead += HealthSystem_OnDead;
        }

        private void HealthSystem_OnDead(object sender, EventArgs e)
        {
            
            Transform ragDollTransform = Instantiate(ragDollPrefab, transform.position, transform.rotation);
            UnitRagDoll unitRagDoll = ragDollTransform.GetComponent<UnitRagDoll>();
            unitRagDoll.SetUp(originalRootBone);
        }
    }

}