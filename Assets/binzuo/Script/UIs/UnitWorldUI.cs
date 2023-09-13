using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace binzuo
{
    public class UnitWorldUI : MonoBehaviour
    {
        [SerializeField] private Unit unit;
        [SerializeField] private Image hitPointImage;
        [SerializeField] private HitPoint hitPoint;

        private void Start() {
            hitPoint.OnDamage += HitPoint_OnDamage;
            UpdateHitPoint();
        }

        private void HitPoint_OnDamage(object sender, EventArgs e)
        {
            UpdateHitPoint();
        }

        private void UpdateHitPoint()
        {
            hitPointImage.fillAmount = hitPoint.GetHitPointNormalized();
        }
    }
}

