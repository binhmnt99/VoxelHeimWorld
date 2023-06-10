using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace TurnBase
{
    public class UnitWorldUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI actionPointsText;
        [SerializeField] private Unit unit;

        void Awake()
        {
            actionPointsText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            unit = transform.GetComponentInParent<Unit>();
            //Debug.Log(unit.GetActionPoints().ToString());
        }

        void Start()
        {
            Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;
            UpdateActionPointText();
        }

        private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
        {
            UpdateActionPointText();
        }

        private void UpdateActionPointText()
        {
            actionPointsText.text = unit.GetActionPoints().ToString();
        }
    }

}
