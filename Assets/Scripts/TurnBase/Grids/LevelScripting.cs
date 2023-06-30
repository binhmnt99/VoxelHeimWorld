using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class LevelScripting : MonoBehaviour

    {
        [SerializeField] private List<GameObject> hider1List;
        [SerializeField] private List<GameObject> hider2List;
        [SerializeField] private List<GameObject> hider3List;
        [SerializeField] private List<GameObject> enemy1List;
        [SerializeField] private List<GameObject> enemy2List;
        [SerializeField] private Door door1;
        [SerializeField] private Door door2;

        private bool hasShownFirstHider = false;

        void Start()
        {
            HexLevelGrid.Instance.OnAnyUnitMovedGridPosition += LevelGrid_OnAnyUnitMovedGridPosition;
            door1.OnDoorOpened += (object sender, EventArgs e) =>
            {
                SetActiveGameObjectList(hider2List, false);
            };
            door2.OnDoorOpened += (object sender, EventArgs e) =>
            {
                SetActiveGameObjectList(hider3List, false);
                SetActiveGameObjectList(enemy2List, true);
            };

        }

        private void LevelGrid_OnAnyUnitMovedGridPosition(object sender, HexLevelGrid.OnAnyUnitMovedGridPositionEventArgs e)
        {
            if (e.toGridPosition.z == 5 && !hasShownFirstHider)
            {
                hasShownFirstHider = true;
                SetActiveGameObjectList(hider1List, false);
                SetActiveGameObjectList(enemy1List, true);
            }

        }

        private void SetActiveGameObjectList(List<GameObject> gameObjectList, bool isActive)
        {
            foreach (GameObject gameObject in gameObjectList)
            {
                gameObject.SetActive(isActive);
            }
        }

    }
}