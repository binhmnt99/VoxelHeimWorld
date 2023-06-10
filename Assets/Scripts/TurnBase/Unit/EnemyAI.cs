using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class EnemyAI : MonoBehaviour
    {
        private float timer;
        // Start is called before the first frame update
        void Start()
        {
            TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        }

        private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
        {
            timer = 2f;
        }

        // Update is called once per frame
        void Update()
        {
            if (TurnSystem.Instance.IsPlayerTurn())
            {
                return;
            }

            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                TurnSystem.Instance.NextTurn();
            }
        }
    }
}
