using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace TurnBase
{
    public class TurnSystemUI : MonoBehaviour
    {
        [SerializeField] private Button endTurnButton;
        [SerializeField] private TextMeshProUGUI turnNumberText;

        void Awake()
        {
            endTurnButton = GetComponentInChildren<Button>();
            turnNumberText = GetComponentInChildren<TextMeshProUGUI>();
        }

        void Start()
        {
            endTurnButton.onClick.AddListener(GoToNextTurn);
            TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
            UpdateTurnText();
        }

        private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
        {
            UpdateTurnText();
        }

        public void GoToNextTurn()
        {
            TurnSystem.Instance.NextTurn();
        }

        private void UpdateTurnText()
        {
            turnNumberText.text = "TURN " +  TurnSystem.Instance.GetTurnNumber();
        }
    }

}