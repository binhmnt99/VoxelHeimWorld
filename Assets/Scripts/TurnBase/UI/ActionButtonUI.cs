using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace TurnBase
{
    public class ActionButtonUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textMeshProUGUI;
        [SerializeField] private Button button;
        [SerializeField] private GameObject selectedGameObject;

        private BaseAction baseAction;

        void Awake()
        {
            textMeshProUGUI = GetComponentInChildren<TextMeshProUGUI>();
            button = GetComponent<Button>();
        }

        public void SetBaseAction(BaseAction baseAction)
        {
            this.baseAction = baseAction;
            textMeshProUGUI.text = baseAction.GetActionName().ToUpper();
            button.onClick.AddListener(() => MoveActionButton_Click(baseAction));
        }

        private void MoveActionButton_Click(BaseAction baseAction)
        {
            UnitActionSystem.Instance.SetSelectedAction(baseAction);
        }

        public void UpdateSelectedVisual()
        {
            BaseAction selectedBaseAction = UnitActionSystem.Instance.GetSelectedAction();
            selectedGameObject.SetActive(selectedBaseAction == baseAction);
        }
    }
}