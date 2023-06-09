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
        private Unit unit;
        [SerializeField] private Image healthBarImage;
        [SerializeField] private HealthSystem healthSystem;


        void Start()
        {
            unit = GetComponentInParent<Unit>();
            Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;
            healthSystem.OnDamage += HealthSystem_OnDamage;
            healthSystem.OnHealth += healthSystem_OnHealth;
            UpdateActionPointText();
            UpdateHealthBar();
        }

        private void healthSystem_OnHealth(object sender, EventArgs e)
        {
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
