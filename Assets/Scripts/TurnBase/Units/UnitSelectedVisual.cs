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

        // Start is called before the first frame update
        void Awake()
        {
            units = GetComponentInParent<Unit>();
            meshRenderer = GetComponent<MeshRenderer>();
        }

        void Start()
        {
            UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
            UpdateVisual();
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
