using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TurnBase
{
    public class UnitActionSystemUI : MonoBehaviour
    {
        [SerializeField] private Transform actionButtonPrefabTransform;
        [SerializeField] private Transform actionButtonContainerTransform;
        [SerializeField] private TextMeshProUGUI actionPointText;
        [SerializeField] private TextMeshProUGUI movePointText;
        private List<ActionButtonUI> actionButtonUIList;
        void Awake()
        {
            actionButtonUIList = new List<ActionButtonUI>();

            actionButtonContainerTransform = transform.GetChild(0).GetComponent<Transform>();
            actionPointText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            movePointText = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        }
        void Start()
        {
            UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
            UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
            UnitActionSystem.Instance.OnActionStarted += UnitActionSystem_OnActionStarted;
            TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
            Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;
            Unit.OnAnyMovePointsChanged += Unit_OnAnyMovePointsChanged;

            UpdateActionPoints();
            UpdateMovePoints();
            CreateUnitActionButton();
            UpdateSelectedVisual();
        }

        private void CreateUnitActionButton()
        {
            foreach (Transform buttonTransform in actionButtonContainerTransform)
            {
                Destroy(buttonTransform.gameObject);
            }

            actionButtonUIList.Clear();

            Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
            if (selectedUnit)
            {
                foreach (BaseAction baseAction in selectedUnit.GetBaseActionArray())
                {
                    if (baseAction != selectedUnit.GetAction<MoveAction>())
                    {
                        Transform actionButtonTransform = Instantiate(actionButtonPrefabTransform, actionButtonContainerTransform);
                        ActionButtonUI actionButtonUI = actionButtonTransform.GetComponent<ActionButtonUI>();
                        actionButtonUI.SetBaseAction(baseAction);
                        actionButtonUIList.Add(actionButtonUI);
                    }
                }
            }
        }

        private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e)
        {
            CreateUnitActionButton();
            UpdateSelectedVisual();
            UpdateActionPoints();
            UpdateMovePoints();
        }

        private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
        {
            UpdateSelectedVisual();
        }

        private void UnitActionSystem_OnActionStarted(object sender, EventArgs e)
        {
            UpdateActionPoints();
            UpdateMovePoints();
        }

        private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
        {
            UpdateActionPoints();
            UpdateMovePoints();
        }

        private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
        {
            UpdateActionPoints();
        }

        private void Unit_OnAnyMovePointsChanged(object sender, EventArgs e)
        {
            UpdateMovePoints();
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

            actionPointText.text = (selectedUnit ? "Action Points: " + selectedUnit.GetActionPoints() : "");
        }

        private void UpdateMovePoints()
        {
            Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();

            movePointText.text = (selectedUnit ? "Move Points: " + selectedUnit.GetMovePoints() : "");
        }
    }

}