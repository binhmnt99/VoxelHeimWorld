using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class UnitSelectedVisual : MonoBehaviour
    {
        [SerializeField] private Unit units;
        private MeshRenderer meshRenderer;
        private Quaternion initialRotation;

        // Start is called before the first frame update
        void Awake()
        {
            units = GetComponentInParent<Unit>();
            meshRenderer = GetComponent<MeshRenderer>();
            initialRotation = transform.localRotation;
        }

        void Start()
        {
            UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
            UpdateVisual();
        }

        void LateUpdate()
        {
            transform.rotation = initialRotation;
        }

        private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs empty)
        {
            UpdateVisual();
        }
        // Update is called once per frame
        private void UpdateVisual()
        {
            if (UnitActionSystem.Instance.GetSelectedUnit() == units)
            {
                meshRenderer.enabled = true;
            }
            else
            {
                meshRenderer.enabled = false;
            }
        }

        void OnDestroy()
        {
            UnitActionSystem.Instance.OnSelectedUnitChanged -= UnitActionSystem_OnSelectedUnitChanged;
        }
    }

}
