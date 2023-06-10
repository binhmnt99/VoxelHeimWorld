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
        [SerializeField] private GameObject enemyTurnVisualGameObject;

        void Awake()
        {
            endTurnButton = GetComponentInChildren<Button>();
            turnNumberText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            enemyTurnVisualGameObject = transform.GetChild(2).gameObject;
        }

        void Start()
        {
            endTurnButton.onClick.AddListener(GoToNextTurn);
            TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
            UpdateTurnText();
            UpdateEnemyTurnVisual();
            UpdateEndTurnButtonVisibility();
        }

        private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
        {
            UpdateTurnText();
            UpdateEnemyTurnVisual();
            UpdateEndTurnButtonVisibility();
        }

        public void GoToNextTurn()
        {
            TurnSystem.Instance.NextTurn();
        }

        private void UpdateTurnText()
        {
            turnNumberText.text = "TURN " +  TurnSystem.Instance.GetTurnNumber();
        }

        private void UpdateEnemyTurnVisual()
        {
            enemyTurnVisualGameObject.SetActive(!TurnSystem.Instance.IsPlayerTurn());
        }

        private void UpdateEndTurnButtonVisibility()
        {
            endTurnButton.gameObject.SetActive(TurnSystem.Instance.IsPlayerTurn());
        }
    }

}