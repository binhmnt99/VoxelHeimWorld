using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TurnBase
{
    public class UnitActionSystem : MonoBehaviour
    {
        public static UnitActionSystem Instance { get; private set; }

        private Ray ray;
        private RaycastHit raycastHit;
        public UnitInput unitInput { get; private set; }
        public event EventHandler OnSelectedUnitChanged;
        [SerializeField] private Units selectedUnit;
        [SerializeField] private LayerMask layerMask;
        // Start is called before the first frame update
        void Awake()
        {
            if (Instance != null)
            {

                Destroy(gameObject);
                return;
            }
            Instance = this;
            unitInput = GetComponent<UnitInput>();
        }

        // Update is called once per frame
        void Update()
        {

            if (unitInput.isLeftClicked)
            {
                if (TryHandleUnitSelection())
                {
                    return;
                }
                if (selectedUnit)
                {
                    selectedUnit.Move(MouseWorld.GetPosition());
                }
            }
        }

        private bool TryHandleUnitSelection()
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out raycastHit, float.MaxValue, layerMask))
            {
                if (raycastHit.transform.TryGetComponent<Units>(out Units units))
                {
                    SetSelectedUnit(units);
                    return true;
                }

            }
            return false;
        }

        private void SetSelectedUnit(Units units)
        {
            selectedUnit = units;
            OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
        }

        public Units GetSelectedUnit()
        {
            return selectedUnit;
        }
    }
}

