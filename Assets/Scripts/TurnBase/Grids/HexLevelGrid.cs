using System;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class HexLevelGrid : MonoBehaviour
    {
        public static HexLevelGrid Instance { get; private set; }
        public event EventHandler<OnAnyUnitMovedGridPositionEventArgs> OnAnyUnitMovedGridPosition;
        public class OnAnyUnitMovedGridPositionEventArgs : EventArgs
        {
            public Unit unit;
            public GridPosition fromGridPosition;
            public GridPosition toGridPosition;
        }
        [SerializeField] private Transform gridDebugObjectPrefab;
        [SerializeField] private float cellSize;
        [SerializeField] private List<GameObject> mapList;
        private List<Vector3> mapPositionList;
        private float minX, minZ;
        private float maxX, maxZ;

        private int width;
        private int height;

        HexGridObject hexGridObject;

        private HexGridSystem<HexGridObject> gridSystem;
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            mapPositionList = new List<Vector3>();
            foreach (GameObject map in mapList)
            {
                foreach (Transform item in map.transform)
                {
                    mapPositionList.Add(new Vector3(map.transform.localPosition.x + item.localPosition.x, 0, map.transform.localPosition.z + item.localPosition.z));
                }
            }
            //mapPositionList.Sort(CompareVector);
            FindMinMax();
            CalculateWidthAndHeight();
            gridSystem = new HexGridSystem<HexGridObject>(mapPositionList,width, height, cellSize,
            (HexGridSystem<HexGridObject> g, GridPosition gridPosition) => new HexGridObject(g, gridPosition));
            //gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
        }
        // Start is called before the first frame update
        void Start()
        {
            //HexPathfinding.Instance.SetUp(mapPositionList,width, height, cellSize);
        }

        // Update is called once per frame
        void Update()
        {

        }
        private int CompareVector(Vector3 a, Vector3 b)
        {
            // Compare the Y component of the vectors
            if (a.z < b.z)
            {
                return -1; // a comes before b
            }
            else if (a.z > b.z)
            {
                return 1; // b comes before a
            }
            else
            {
                // If the Y components are equal, compare the X components
                if (a.x < b.x)
                {
                    return -1; // a comes before b
                }
                else if (a.x > b.x)
                {
                    return 1; // b comes before a
                }
                else
                {
                    return 0; // a and b are equal
                }
            }
        }
        private void FindMinMax()
        {
            minX = mapPositionList[0].x;
            minZ = mapPositionList[0].z;

            maxX = mapPositionList[0].x;
            maxZ = mapPositionList[0].z;

            foreach (Vector3 vector in mapPositionList)
            {
                if (vector.x < minX)
                {
                    minX = vector.x;
                }
                if (vector.z < minZ)
                {
                    minZ = vector.z;
                }

                if (vector.x > maxX)
                {
                    maxX = vector.x;
                }
                if (vector.z > maxZ)
                {
                    maxZ = vector.z;
                }
            }
        }
        public int GetAbsoluteValue(float number)
        {
            if (number < 0)
            {
                return Mathf.RoundToInt(-number);
            }
            else
            {
                return Mathf.RoundToInt(number);
            }
        }
        private void CalculateWidthAndHeight()
        {
            width = GetAbsoluteValue(minX) + GetAbsoluteValue(maxX);
            height = GetAbsoluteValue(minZ) + GetAbsoluteValue(maxZ);
        }

        public List<Vector3> GetMapPositionList()
        {
            return mapPositionList;
        }
        public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit)
        {
            hexGridObject = gridSystem.GetGridObject(gridPosition);
            hexGridObject.AddUnit(unit);
        }

        public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition)
        {
            hexGridObject = gridSystem.GetGridObject(gridPosition);
            return hexGridObject.GetUnitList();
        }

        public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit)
        {
            hexGridObject = gridSystem.GetGridObject(gridPosition);
            hexGridObject.RemoveUnit(unit);
        }

        public void UnitMovedGridPosition(Unit unit, GridPosition fromGridPosition, GridPosition toGridPosition)
        {
            RemoveUnitAtGridPosition(fromGridPosition, unit);

            AddUnitAtGridPosition(toGridPosition, unit);

            OnAnyUnitMovedGridPosition?.Invoke(this, new OnAnyUnitMovedGridPositionEventArgs
            {
                unit = unit,
                fromGridPosition = fromGridPosition,
                toGridPosition = toGridPosition,
            });
        }

        public GridPosition GetGridPosition(Vector3 worldPosition) => gridSystem.GetGridPosition(worldPosition);

        public Vector3 GetWorldPosition(GridPosition gridPosition) => gridSystem.GetWorldPosition(gridPosition);

        public bool IsValidGridPosition(GridPosition gridPosition) => gridSystem.IsValidGridPosition(gridPosition);

        public int GetWidth() => gridSystem.GetWidth();
        public int GetHeight() => gridSystem.GetHeight();


        public bool HasAnyUnitOnGridPosition(GridPosition gridPosition)
        {
            hexGridObject = gridSystem.GetGridObject(gridPosition);
            return hexGridObject.HasAnyUnit();
        }

        public Unit GetUnitAtGridPosition(GridPosition gridPosition)
        {
            hexGridObject = gridSystem.GetGridObject(gridPosition);
            return hexGridObject.GetUnit();
        }

        public IInteractable GetInteractableAtGridPosition(GridPosition gridPosition)
        {
            hexGridObject = gridSystem.GetGridObject(gridPosition);
            return hexGridObject.GetInteractable();
        }

        public void SetInteractableAtGridPosition(GridPosition gridPosition, IInteractable interactable)
        {
            hexGridObject = gridSystem.GetGridObject(gridPosition);
            hexGridObject.SetInteractable(interactable);
        }

        public void ClearInteractableAtGridPosition(GridPosition gridPosition)
        {
            hexGridObject = gridSystem.GetGridObject(gridPosition);
            hexGridObject.ClearInteractable();
        }
    }

}