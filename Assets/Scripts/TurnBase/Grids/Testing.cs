using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class Testing : MonoBehaviour
    {
        [SerializeField] private Unit unit;
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
                GridPosition startGridPosition = new GridPosition(0, 0);

                List<GridPosition> gridPositionList = Pathfinding.Instance.FindPath(startGridPosition, mouseGridPosition, out int pathLength);
                for (int i = 0; i < gridPositionList.Count - 1; i++)
                {
                    Debug.DrawLine(
                        LevelGrid.Instance.GetWorldPosition(gridPositionList[i]),
                        LevelGrid.Instance.GetWorldPosition(gridPositionList[i + 1]),
                        Color.magenta,
                        10f
                    );
                }

            }
        }
    }

}
