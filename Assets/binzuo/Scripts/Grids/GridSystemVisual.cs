using System.Collections.Generic;
using UnityEngine;

namespace binzuo
{
    public class GridSystemVisual : Singleton<GridSystemVisual>
    {
        [SerializeField] private Transform gridSystemVisualSinglePrefab;


        private GridSystemVisualSingle[,] gridSystemVisualSingleArray;

        private void Start()
        {
            gridSystemVisualSingleArray = new GridSystemVisualSingle[
                LevelGrid.Instance.GetWidth(),
                LevelGrid.Instance.GetHeight()
            ];

            for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
            {
                for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
                {
                    GridPosition gridPosition = new GridPosition(x, z);

                    Transform gridSystemVisualSingleTransform =
                        Instantiate(gridSystemVisualSinglePrefab, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity, transform);

                    gridSystemVisualSingleArray[x, z] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
                }
            }
        }

        private void Update()
        {
            UpdateGridVisual();
        }

        public void HideAllGridPosition()
        {
            for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
            {
                for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
                {
                    gridSystemVisualSingleArray[x, z].Hide();
                }
            }
        }

        public void ShowGridPositionList(List<GridPosition> gridPositionList)
        {
            foreach (GridPosition gridPosition in gridPositionList)
            {
                gridSystemVisualSingleArray[gridPosition.x, gridPosition.z].Show();
            }
        }

        private void UpdateGridVisual()
        {
            HideAllGridPosition();

            BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();

            ShowGridPositionList(
                selectedAction.GetValidActionGridPositionList());
        }

    }

}
