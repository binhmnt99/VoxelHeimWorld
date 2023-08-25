
using System.Collections.Generic;
using UnityEngine;

namespace binzuo
{
    public class GridSystemVisual : Singleton<GridSystemVisual>
    {
        [SerializeField] private Transform gridSystemVisualSinglePrefab;
        private int width;
        private int height;

        private GridSystemVisualSingle[,] gridSystemVisualSingleArray;

        private void Start()
        {
            width = LevelGrid.Instance.GetWidth();
            height = LevelGrid.Instance.GetHeight();
            gridSystemVisualSingleArray = new GridSystemVisualSingle[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    GridPosition gridPosition = new GridPosition(x, z);
                    Vector3 worldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
                    Transform gridSystemVisualTransform = Instantiate(gridSystemVisualSinglePrefab, worldPosition, Quaternion.identity, transform);

                    gridSystemVisualSingleArray[x, z] = gridSystemVisualTransform.GetComponent<GridSystemVisualSingle>();
                }
            }
        }

        private void Update()
        {
            UpdateGridVisual();
        }

        public void HideAllGridVisual()
        {
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    gridSystemVisualSingleArray[x, z].Hide();
                }
            }
        }

        public void ShowAllGridVisual(List<GridPosition> gridPositionList)
        {
            foreach (GridPosition gridPosition in gridPositionList)
            {
                gridSystemVisualSingleArray[gridPosition.x, gridPosition.z].Show();
            }
        }

        public void UpdateGridVisual()
        {
            GridSystemVisual.Instance.HideAllGridVisual();
            Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
            GridSystemVisual.Instance.ShowAllGridVisual(selectedUnit.GetMoveAction().GetValidActionGridPositionList());
        }
    }

}
