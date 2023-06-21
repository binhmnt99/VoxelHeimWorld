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
            public Material gridMaterial;
        }

        public enum GridVisualType
        {
            Red,
            RedSoft,
            Blue,
            Yellow,
            White
        }
        [SerializeField] private Transform gridSystemVisualSinglePrefab;
        [SerializeField] private List<GridVisualTypeMaterial> gridVisualTypeMaterialList;

        private GameObject gridSquadVisual;
        private GridSystemVisualSingle[,] gridSystemVisualSingleArray;
        private GridSystemVisualSingle lastSelectedGridSystemVisualSingle;


        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        void Start()
        {
            gridSquadVisual = new GameObject("UnitGridSquadVisual");
            gridSystemVisualSingleArray = new GridSystemVisualSingle[
                LevelGrid.Instance.GetWidth(),
                LevelGrid.Instance.GetHeight()
            ];

            for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
            {
                for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
                {
                    GridPosition gridPosition = new GridPosition(x, z, 0);

                    Transform gridSystemVisualSingleTransform =
                        Instantiate(gridSystemVisualSinglePrefab, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity, gridSquadVisual.transform);

                    gridSystemVisualSingleArray[x, z] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
                }
            }

            UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
            LevelGrid.Instance.OnAnyUnitMovedGridPosition += LevelGrid_OnAnyUnitMovedGridPosition;

            UpdateGridVisual();

            //Bonus HexPathfinding
            // for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
            // {
            //     for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            //     {
            //         gridSystemVisualSingleArray[x, z].
            //         Show(GetGridVisualTypeMaterial(GridVisualType.White));
            //     }
            // }
        }

        void Update()
        {
            if (lastSelectedGridSystemVisualSingle != null)
            {
                lastSelectedGridSystemVisualSingle.HideSelected();
            }
            Vector3 mouseWorldPosition = MouseWorld.GetPosition();
            GridPosition gridPosition = LevelGrid.Instance.GetGridPosition(mouseWorldPosition);
            if (LevelGrid.Instance.IsValidGridPosition(gridPosition))
            {
                lastSelectedGridSystemVisualSingle = gridSystemVisualSingleArray[gridPosition.x, gridPosition.z];
            }
            if (lastSelectedGridSystemVisualSingle != null)
            {
                lastSelectedGridSystemVisualSingle.ShowSelected();
            }
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

        private void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualType gridVisualType)
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
                gridSystemVisualSingleArray[gridPosition.x, gridPosition.z].
                    Show(GetGridVisualTypeMaterial(gridVisualType));
            }
        }

        private void UpdateGridVisual()
        {
            HideAllGridPosition();

            Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
            if (selectedUnit)
            {
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
                    case InteractAction interactAction:
                        gridVisualType = GridVisualType.Blue;
                        break;
                }

                ShowGridPositionList(
                    selectedAction.GetValidActionGridPositionList(), gridVisualType);
            }

        }

        private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
        {
            UpdateGridVisual();
        }

        private void LevelGrid_OnAnyUnitMovedGridPosition(object sender, EventArgs e)
        {
            UpdateGridVisual();
        }

        private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType)
        {
            foreach (GridVisualTypeMaterial gridVisualTypeMaterial in gridVisualTypeMaterialList)
            {
                if (gridVisualTypeMaterial.gridVisualType == gridVisualType)
                {
                    return gridVisualTypeMaterial.gridMaterial;
                }
            }

            return null;
        }

    }
}
