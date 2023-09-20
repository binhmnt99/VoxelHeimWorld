using System;
using System.Collections.Generic;
using UnityEngine;

namespace binzuo
{
    public class GridSystemVisual : Singleton<GridSystemVisual>
    {
        [Serializable]
        public struct GridVisualTypeMaterial
        {
            public GridVisualType gridVisualType;
            public Material material;
        }
        public enum GridVisualType
        {
            White,
            Red,
            RedSoft,
            Yellow
        }
        [SerializeField] private Transform gridSystemVisualSinglePrefab;
        [SerializeField] private List<GridVisualTypeMaterial> gridSystemVisualTypeMaterialList;

        private GridSystemVisualSingle[,,] gridSystemVisualSingleArray;

        private void Start()
        {
            gridSystemVisualSingleArray = new GridSystemVisualSingle[
                LevelGrid.Instance.GetWidth(),
                LevelGrid.Instance.GetHeight(),
                LevelGrid.Instance.GetFloorAmount()
            ];

            for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
            {
                for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
                {
                    for (int floor = 0; floor < LevelGrid.Instance.GetFloorAmount(); floor++)
                    {
                        GridPosition gridPosition = new GridPosition(x, z, floor);

                        Transform gridSystemVisualSingleTransform =
                            Instantiate(gridSystemVisualSinglePrefab, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity, transform);

                        gridSystemVisualSingleArray[x, z, floor] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
                    }
                }
            }
            UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
            LevelGrid.Instance.OnAnyUnitMovedGridPosition += LevelGrid_OnAnyUnitMoveGridPosition;
            UpdateGridVisual();
        }

        private void LevelGrid_OnAnyUnitMoveGridPosition(object sender, EventArgs e)
        {
            UpdateGridVisual();
        }

        private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
        {
            UpdateGridVisual();
        }

        public void HideAllGridPosition()
        {
            for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
            {
                for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
                {
                    for (int floor = 0; floor < LevelGrid.Instance.GetFloorAmount(); floor++)
                    {
                        gridSystemVisualSingleArray[x, z, floor].Hide();
                    }
                }
            }
        }

        public void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualType gridVisualType)
        {
            List<GridPosition> gridPositionList = new List<GridPosition>();

            for (int x = -range; x <= range; x++)
            {
                for (int z = -range; z <= range; z++)
                {
                    GridPosition testGridPosition = gridPosition + new GridPosition(x, z, 0);

                    if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                    {
                        continue;
                    }

                    int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                    if (testDistance > range)
                    {
                        continue;
                    }

                    gridPositionList.Add(testGridPosition);
                }
            }

            ShowGridPositionList(gridPositionList, gridVisualType);

        }

        public void ShowGridPositionList(List<GridPosition> gridPositionList, GridVisualType gridVisualType)
        {
            foreach (GridPosition gridPosition in gridPositionList)
            {

                gridSystemVisualSingleArray[gridPosition.x, gridPosition.z, gridPosition.floor].Show(GetGridVisualTypeMaterial(gridVisualType));
            }
        }

        private void UpdateGridVisual()
        {
            HideAllGridPosition();
            Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
            BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();
            if (selectedUnit == null && selectedAction == null)
            {
                return;
            }
            GridVisualType gridVisualType;
            switch (selectedAction)
            {
                default:
                case MoveAction moveAction:
                    gridVisualType = GridVisualType.White;
                    break;
                case ShootAction shootAction:
                    gridVisualType = GridVisualType.Red;
                    ShowGridPositionRange(selectedUnit.GetGridPosition(), shootAction.GetMaxShootDistance(), GridVisualType.RedSoft);
                    break;
                case InteractAction interactAction:
                    gridVisualType = GridVisualType.Yellow;
                    break;
            }
            ShowGridPositionList(
                selectedAction.GetValidActionGridPositionList(), gridVisualType);
        }

        private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType)
        {
            foreach (GridVisualTypeMaterial gridVisualTypeMaterial in gridSystemVisualTypeMaterialList)
            {
                if (gridVisualTypeMaterial.gridVisualType == gridVisualType)
                {
                    return gridVisualTypeMaterial.material;
                }
            }

            Debug.LogError("Could not find GridVisualTypeMaterial for GridVisualType " + gridVisualType);
            return null;
        }


    }

}
