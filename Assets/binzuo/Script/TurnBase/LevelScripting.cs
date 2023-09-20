using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace binzuo
{
    public class LevelScripting : MonoBehaviour
    {
        [SerializeField] private List<GameObject> hider1List;
        [SerializeField] private List<GameObject> hider2List;
        [SerializeField] private List<GameObject> hider3List;
        [SerializeField] private List<GameObject> hider4List;
        [SerializeField] private List<GameObject> hider5List;


        [SerializeField] private List<GameObject> enemy1List;
        [SerializeField] private List<GameObject> enemy2List;
        [SerializeField] private List<GameObject> enemy3List;
        [SerializeField] private List<GameObject> enemy4List;
        [SerializeField] private List<GameObject> enemy5List;
        [SerializeField] private Door door1;
        [SerializeField] private Door door2;
        [SerializeField] private Door door3;
        [SerializeField] private Door door4;
        [SerializeField] private Door door5;

        private bool hasShownFirstHider = false;

        private void Start()
        {
            LevelGrid.Instance.OnAnyUnitMovedGridPosition += LevelGrid_OnAnyUnitMovedGridPosition;
            door1.OnDoorOpened += (object sender, EventArgs e) =>
            {
                SetActiveGameObjectList(hider1List, false);
            };
            door2.OnDoorOpened += (object sender, EventArgs e) =>
            {
                SetActiveGameObjectList(hider2List, false);
                SetActiveGameObjectList(enemy2List, true);
            };
            door3.OnDoorOpened += (object sender, EventArgs e) =>
            {
                SetActiveGameObjectList(hider3List, false);
                SetActiveGameObjectList(enemy3List, true);
            };
            door4.OnDoorOpened += (object sender, EventArgs e) =>
            {
                SetActiveGameObjectList(hider4List, false);
                SetActiveGameObjectList(enemy4List, true);
            };
            door5.OnDoorOpened += (object sender, EventArgs e) =>
            {
                SetActiveGameObjectList(hider5List, false);
                SetActiveGameObjectList(enemy5List, true);
            };
        }

        private void LevelGrid_OnAnyUnitMovedGridPosition(object sender, LevelGrid.OnAnyUnitMovedGridPositionEventArgs e)
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
