using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace binzuo
{
    public class UnitActionSystemUI : MonoBehaviour
    {
        [SerializeField] private Transform actionButtonPrefab;
        [SerializeField] private Transform actionButtonContainerTransform;
        [SerializeField] private TextMeshProUGUI actionPointsText;
        private List<ActionButtonUI> actionButtonUIList;

        private void Awake()
        {
            actionButtonUIList = new();
        }
        private void Start()
        {
            UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
            UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
            UnitActionSystem.Instance.OnActingChanged += UnitActionSystem_OnActingChanged;
            UnitActionSystem.Instance.OnActionStarted += UnitActionSystem_OnActionStarted;

            UpdateActionPoints();
            CreateUnitActionButtons();
            UpdateSelectedVisual();
        }

        private void UnitActionSystem_OnActionStarted(object sender, EventArgs e)
        {
            UpdateActionPoints();
        }

        private void UnitActionSystem_OnActingChanged(object sender, bool acting)
        {
            if (acting)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        private void Show()
        {
            actionButtonContainerTransform.gameObject.SetActive(true);
        }

        private void Hide()
        {
            actionButtonContainerTransform.gameObject.SetActive(false);
        }

        private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
        {
            UpdateSelectedVisual();
        }

        private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e)
        {
            UpdateActionPoints();
            CreateUnitActionButtons();
            UpdateSelectedVisual();
        }

        private void CreateUnitActionButtons()
        {
            foreach (Transform buttonTransform in actionButtonContainerTransform)
            {
                Destroy(buttonTransform.gameObject);
            }
            actionButtonUIList.Clear();
            Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
            foreach (BaseAction action in selectedUnit.GetBaseActions())
            {
                Transform actionButtonTransform = Instantiate(actionButtonPrefab, actionButtonContainerTransform);
                ActionButtonUI actionButtonUI = actionButtonTransform.GetComponent<ActionButtonUI>();
                actionButtonUI.SetBaseAction(action);
                actionButtonUIList.Add(actionButtonUI);
            }
        }

        private void UpdateSelectedVisual()
        {
            foreach (ActionButtonUI actionButtonUI in actionButtonUIList)
            {
                actionButtonUI.UpdateSelectedVisual();
            }
        }

        private void UpdateActionPoints()
        {
            Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
            actionPointsText.text = selectedUnit.GetActionPoints().ToString();
        }
    }

}
