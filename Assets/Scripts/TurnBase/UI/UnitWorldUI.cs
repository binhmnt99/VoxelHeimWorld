using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

namespace TurnBase
{
    public class UnitWorldUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI actionPointsText;
        [SerializeField] private Unit unit;
        [SerializeField] private Image healthBarImage;
        [SerializeField] private HealthSystem healthSystem;


        void Start()
        {
            Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;
            healthSystem.OnDamage += HealthSystem_OnDamage;
            UpdateActionPointText();
            UpdateHealthBar();
        }

        private void HealthSystem_OnDamage(object sender, EventArgs e)
        {
            UpdateHealthBar();
        }

        private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
        {
            UpdateActionPointText();
        }

        private void UpdateActionPointText()
        {
            actionPointsText.text = unit.GetActionPoints().ToString();
        }

        private void UpdateHealthBar()
        {
            healthBarImage.fillAmount = healthSystem.GetHealthNormalized();
        }
    }

}
