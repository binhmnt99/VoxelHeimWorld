using System;
using UnityEngine;

namespace binzuo
{
    public class ActingUI : MonoBehaviour
    {
        private void Start()
        {
            UnitActionSystem.Instance.OnActingChanged += UnitActionSystem_OnActingChanged;
            Hide();
        }

        private void UnitActionSystem_OnActingChanged(object sender, bool acting)
        {
            if (acting)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        private void Show()
        {
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

        
    }
}

