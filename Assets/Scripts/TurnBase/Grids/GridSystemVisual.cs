using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class GridSystemVisual : MonoBehaviour
    {
        public static GridSystemVisual Instance { get; private set; }


        [Serializable]
        public struct GridVisualTypeMaterial
        {
            public GridVisualType gridVisualType;
            public Material material;
        }

        public enum GridVisualType
        {
            White,
            Blue,
            Red,
            RedSoft,
            Yellow,
        }
        [SerializeField] private Transform gridSystemVisualSinglePrefab;
        [SerializeField] private List<GridVisualTypeMaterial> gridVisualTypeMaterialList;

        private GridSystemVisualSingle[,] gridSystemVisualSingleArray;

        private GridPosition gridPositionSelected;

        private GameObject gridSquadVisual;

        private GridSystemVisualSingle lastSelectedGridSystemVisualSingle;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("There's more than one GridSystemVisual! " + transform + " - " + Instance);
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            gridSquadVisual = new GameObject("GridHexVisual");
            gridSystemVisualSingleArray = new GridSystemVisualSingle[
                HexLevelGrid.Instance.GetWidth(),
                HexLevelGrid.Instance.GetHeight()
            ];

            for (int x = 0; x < HexLevelGrid.Instance.GetWidth(); x++)
            {
                for (int z = 0; z < HexLevelGrid.Instance.GetHeight(); z++)
                {
                    GridPosition gridPosition = new GridPosition(x, z);

                    Transform gridSystemVisualSingleTransform =
                        Instantiate(gridSystemVisualSinglePrefab, HexLevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity, gridSquadVisual.transform);

                    gridSystemVisualSingleArray[x, z] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
                }
            }

            UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
            HexLevelGrid.Instance.OnAnyUnitMovedGridPosition += HexLevelGrid_OnAnyUnitMovedGridPosition;

            UpdateGridVisual();
        }

        //Bonus HexPathfinding
        // for (int x = 0; x < HexLevelGrid.Instance.GetWidth(); x++)
        // {
        //     for (int z = 0; z < HexLevelGrid.Instance.GetHeight(); z++)
        //     {
        //         gridSystemVisualSingleArray[x, z].
        //         Show(GetGridVisualTypeMaterial(GridVisualType.White));
        //     }
        // }

        Vector3 mouseWorldPosition;
        GridPosition gridPosition;

        void FixedUpdate()
        {
            if (lastSelectedGridSystemVisualSingle != null)
            {
                lastSelectedGridSystemVisualSingle.HideSelected();
            }
            mouseWorldPosition = MouseWorld.GetPosition();
            gridPosition = HexLevelGrid.Instance.GetGridPosition(mouseWorldPosition);
            if (HexLevelGrid.Instance.IsValidGridPosition(gridPosition))
            {
                lastSelectedGridSystemVisualSingle = gridSystemVisualSingleArray[gridPosition.x, gridPosition.z];
            }
            if (lastSelectedGridSystemVisualSingle != null)
            {
                lastSelectedGridSystemVisualSingle.ShowSelected();
                gridPositionSelected = gridPosition;
            }
        }

        public GridPosition GetGridPositionSelected()
        {
            return gridPositionSelected;
        }

        public void HideAllGridPosition()
        {
            for (int x = 0; x < HexLevelGrid.Instance.GetWidth(); x++)
            {
                for (int z = 0; z < HexLevelGrid.Instance.GetHeight(); z++)
                {
                    gridSystemVisualSingleArray[x, z].Hide();
                }
            }
        }

        private void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualType gridVisualType)
        {
            List<GridPosition> gridPositionList = new List<GridPosition>();

            for (int x = -range; x <= range; x++)
            {
                for (int z = -range; z <= range; z++)
                {
                    GridPosition testGridPosition = gridPosition + new GridPosition(x, z);

                    if (!HexLevelGrid.Instance.IsValidGridPosition(testGridPosition))
                    {
                        continue;
                    }

                    if (!HexPathfinding.Instance.IsWalkableGridPosition(testGridPosition))
                    {
                        continue;
                    }
                    if (!HexPathfinding.Instance.HasPath(gridPosition, testGridPosition))
                    {
                        continue;
                    }

                    Vector3 unitWorldPosition = HexLevelGrid.Instance.GetHexGridSystem().GetWorldPosition(gridPosition);
                    Vector3 testWorldPosition = HexLevelGrid.Instance.GetHexGridSystem().GetWorldPosition(testGridPosition);
                    float distance = Vector3.Distance(unitWorldPosition,testWorldPosition);
                    if (distance > range)
                    {
                        continue;
                    }
                    gridPositionList.Add(testGridPosition);
                }
            }

            ShowGridPositionList(gridPositionList, gridVisualType);
        }

        private void ShowGridPositionRangeSquare(GridPosition gridPosition, int range, GridVisualType gridVisualType)
        {
            List<GridPosition> gridPositionList = new List<GridPosition>();

            for (int x = -range; x <= range; x++)
            {
                for (int z = -range; z <= range; z++)
                {
                    GridPosition testGridPosition = gridPosition + new GridPosition(x, z);

                    if (!HexLevelGrid.Instance.IsValidGridPosition(testGridPosition))
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
                gridSystemVisualSingleArray[gridPosition.x, gridPosition.z].
                    Show(GetGridVisualTypeMaterial(gridVisualType));
            }
        }

        private void UpdateGridVisual()
        {
            Debug.Log("UpdateGridVisual");
            HideAllGridPosition();

            Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
            BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();

            GridVisualType gridVisualType;

            switch (selectedAction)
            {
                default:
                case MoveAction moveAction:
                    gridVisualType = GridVisualType.White;
                    break;
                case SpinAction spinAction:
                    gridVisualType = GridVisualType.Blue;
                    break;
                case ShootAction shootAction:
                    gridVisualType = GridVisualType.Red;

                    ShowGridPositionRange(selectedUnit.GetGridPosition(), shootAction.GetShootDistance(), GridVisualType.RedSoft);
                    break;
                case GrenadeAction grenadeAction:
                    gridVisualType = GridVisualType.Yellow;
                    break;
                // case SwordAction swordAction:
                //     gridVisualType = GridVisualType.Red;

                //     ShowGridPositionRangeSquare(selectedUnit.GetGridPosition(), swordAction.GetMaxSwordDistance(), GridVisualType.RedSoft);
                //     break;
                case InteractAction interactAction:
                    gridVisualType = GridVisualType.Blue;
                    break;
            }

            ShowGridPositionList(
                selectedAction.GetValidActionGridPositionList(), gridVisualType);
        }

        private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
        {
            UpdateGridVisual();
        }

        private void HexLevelGrid_OnAnyUnitMovedGridPosition(object sender, EventArgs e)
        {
            Debug.Log("Update Grid");
            UpdateGridVisual();
        }

        private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType)
        {
            foreach (GridVisualTypeMaterial gridVisualTypeMaterial in gridVisualTypeMaterialList)
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
