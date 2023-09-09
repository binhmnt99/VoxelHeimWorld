using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace binzuo
{
    public class Testing : MonoBehaviour
    {
        [SerializeField] private Unit unit;

        // private void Start() {

        // }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.T))
            {
                GridPosition mouseGrid = LevelGrid.Instance.GetGridPosition(MousePosition.GetPoint());
                GridPosition startGrid = new GridPosition(0,0);

                List<GridPosition> gridPositionList = Pathfinding.Instance.FindPath(startGrid,mouseGrid);
                print(gridPositionList.Count);

                for (int i = 0; i < gridPositionList.Count - 1; i++)
                {
                    Debug.DrawLine(LevelGrid.Instance.GetWorldPosition(gridPositionList[i]) + new Vector3(0,.5f,0),
                     LevelGrid.Instance.GetWorldPosition(gridPositionList[i + 1]) + new Vector3(0,.5f,0),
                     Color.white, 10f);
                }
            }
        }
    }

}
