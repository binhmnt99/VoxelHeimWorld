using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class GridSystemVisual : MonoBehaviour
    {
        public static GridSystemVisual Instance { get; private set; }
        [SerializeField] private Transform gridVisualSquadPrefab;
        private GridSystemVisualSquad[,] gridSystemVisualSquadArray;
        private GameObject gridVisualSquad;
        private Unit selectedUnit;

        void Awake()
        {
            if (Instance != null)
            {

                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        // Start is called before the first frame update
        void Start()
        {
            gridSystemVisualSquadArray = new GridSystemVisualSquad[LevelGrid.Instance.GetWidth(), LevelGrid.Instance.GetHeight()];
            gridVisualSquad = new GameObject("GridVisualSquad");
            for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
            {
                for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
                {
                    GridPosition gridPosition = new GridPosition(x, z);
                    Transform gridVisualSquadTransform = Instantiate(gridVisualSquadPrefab, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity);
                    gridVisualSquadTransform.SetParent(gridVisualSquad.transform);
                    gridSystemVisualSquadArray[x, z] = gridVisualSquadTransform.GetComponent<GridSystemVisualSquad>();
                }
            }
        }

        void Update()
        {
            UpdateGridVisual();
        }

        public void HideAllGridPosition()
        {
            for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
            {
                for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
                {
                    gridSystemVisualSquadArray[x, z].Hide();
                }
            }
        }

        public void ShowGridPositionList(List<GridPosition> gridPositionList)
        {
            foreach (GridPosition gridPosition in gridPositionList)
            {
                gridSystemVisualSquadArray[gridPosition.x, gridPosition.z].Show();
            }
        }

        private void UpdateGridVisual()
        {
            HideAllGridPosition();
            if (UnitActionSystem.Instance.GetSelectedUnit())
            {
                selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
                ShowGridPositionList(selectedUnit.GetMoveAction().GetValidGridPositionsList());
            }
        }
    }
}
