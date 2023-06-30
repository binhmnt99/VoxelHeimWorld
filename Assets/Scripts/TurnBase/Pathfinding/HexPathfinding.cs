
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class HexPathfinding : MonoBehaviour
    {
        public static HexPathfinding Instance { get; private set; }

        private const int MOVE_STRAIGHT_COST = 10;
        //private const int MOVE_DIAGONAL_COST = 14;

        [SerializeField] private Transform gridDebugObjectPrefab;
        [SerializeField] private LayerMask obstaclesLayerMask;

        private int width;
        private int height;
        private float cellSize;
        private HexGridSystem<PathNode> gridSystem;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void SetUp(int width, int height, float cellSize)
        {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;

            gridSystem = new HexGridSystem<PathNode>(width, height, cellSize,
                (HexGridSystem<PathNode> g, GridPosition gridPosition) => new PathNode(gridPosition));
            //gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
            GridPosition gridPosition;
            Vector3 worldPosition;
            float rayOffsetDistance = 5f;

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    gridPosition = new GridPosition(x, z);
                    worldPosition = HexLevelGrid.Instance.GetWorldPosition(gridPosition);
                    if (Physics.Raycast(worldPosition + Vector3.down * rayOffsetDistance, Vector3.up, rayOffsetDistance * 2, obstaclesLayerMask))
                    {
                        GetNode(x, z).SetIsWalkable(false);
                    }
                }
            }
        }

        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closedList = new List<PathNode>();

        public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition, out int pathLength)
        {
            openList.Clear();
            closedList.Clear();

            PathNode startNode = gridSystem.GetGridObject(startGridPosition);
            PathNode endNode = gridSystem.GetGridObject(endGridPosition);
            openList.Add(startNode);

            for (int x = 0; x < gridSystem.GetWidth(); x++)
            {
                for (int z = 0; z < gridSystem.GetHeight(); z++)
                {
                    GridPosition gridPosition = new GridPosition(x, z);
                    PathNode pathNode = gridSystem.GetGridObject(gridPosition);

                    pathNode.SetGCost(int.MaxValue);
                    pathNode.SetHCost(0);
                    pathNode.CalculateFCost();
                    pathNode.ResetCameFromPathNode();
                }
            }



            startNode.SetGCost(0);
            startNode.SetHCost(CalculateHeuristicDistance(startGridPosition, endGridPosition));
            startNode.CalculateFCost();

            while (openList.Count > 0)
            {
                PathNode currentNode = GetLowestFCostPathNode(openList);

                if (currentNode == endNode)
                {
                    pathLength = endNode.GetFCost();
                    // Reached final node
                    return CalculatePath(endNode);
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                foreach (PathNode neighborNode in GetNeighborList(currentNode))
                {
                    if (closedList.Contains(neighborNode))
                    {
                        continue;
                    }

                    if (!neighborNode.IsWalkable())
                    {
                        closedList.Add(neighborNode);
                        continue;
                    }

                    int tentativeGCost =
                        currentNode.GetGCost() + CalculateHeuristicDistance(currentNode.GetGridPosition(), neighborNode.GetGridPosition());

                    if (tentativeGCost < neighborNode.GetGCost())
                    {
                        neighborNode.SetCameFromPathNode(currentNode);
                        neighborNode.SetGCost(tentativeGCost);
                        neighborNode.SetHCost(CalculateHeuristicDistance(neighborNode.GetGridPosition(), endGridPosition));
                        neighborNode.CalculateFCost();

                        if (!openList.Contains(neighborNode))
                        {
                            openList.Add(neighborNode);
                        }
                    }
                }
            }
            pathLength = 0;

            // No path found
            return null;
        }

        public int CalculateHeuristicDistance(GridPosition gridPositionA, GridPosition gridPositionB)
        {
            return Mathf.RoundToInt(MOVE_STRAIGHT_COST *
            Vector3.Distance(gridSystem.GetWorldPosition(gridPositionA), gridSystem.GetWorldPosition(gridPositionB)));
        }

        private PathNode GetLowestFCostPathNode(List<PathNode> pathNodeList)
        {
            PathNode lowestFCostPathNode = pathNodeList[0];
            for (int i = 0; i < pathNodeList.Count; i++)
            {
                if (pathNodeList[i].GetFCost() < lowestFCostPathNode.GetFCost())
                {
                    lowestFCostPathNode = pathNodeList[i];
                }
            }
            return lowestFCostPathNode;
        }

        private PathNode GetNode(int x, int z)
        {
            return gridSystem.GetGridObject(new GridPosition(x, z));
        }

        List<PathNode> neighborList = new();
        private List<PathNode> GetNeighborList(PathNode currentNode)
        {
            neighborList.Clear();

            GridPosition gridPosition = currentNode.GetGridPosition();

            if (gridPosition.x - 1 >= 0)
            {
                // Left
                neighborList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 0));
            }

            if (gridPosition.x + 1 < gridSystem.GetWidth())
            {
                // Right
                neighborList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 0));
            }

            if (gridPosition.z - 1 >= 0)
            {
                // Down
                neighborList.Add(GetNode(gridPosition.x + 0, gridPosition.z - 1));
            }
            if (gridPosition.z + 1 < gridSystem.GetHeight())
            {
                // Up
                neighborList.Add(GetNode(gridPosition.x + 0, gridPosition.z + 1));
            }
            bool oddRow = gridPosition.z % 2 == 1;
            if (oddRow)
            {
                if (gridPosition.x + 1 < gridSystem.GetWidth())
                {
                    if (gridPosition.z - 1 >= 0)
                    {
                        neighborList.Add(GetNode(gridPosition.x + 1, gridPosition.z - 1));

                    }
                    if (gridPosition.z + 1 < gridSystem.GetHeight())
                    {
                        neighborList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 1));
                    }
                }

            }
            else
            {
                if (gridPosition.x - 1 >= 0)
                {
                    if (gridPosition.z - 1 >= 0)
                    {
                        neighborList.Add(GetNode(gridPosition.x - 1, gridPosition.z - 1));
                    }
                    if (gridPosition.z + 1 < gridSystem.GetHeight())
                    {
                        neighborList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 1));
                    }
                }

            }
            return neighborList;
        }

        List<PathNode> pathNodeList = new();
        List<GridPosition> gridPositionList = new();

        private List<GridPosition> CalculatePath(PathNode endNode)
        {
            pathNodeList.Clear();
            pathNodeList.Add(endNode);
            PathNode currentNode = endNode;
            while (currentNode.GetCameFromPathNode() != null)
            {
                pathNodeList.Add(currentNode.GetCameFromPathNode());
                currentNode = currentNode.GetCameFromPathNode();
            }

            pathNodeList.Reverse();

            gridPositionList.Clear();
            foreach (PathNode pathNode in pathNodeList)
            {
                gridPositionList.Add(pathNode.GetGridPosition());
            }
            return gridPositionList;
        }

        public void SetIsWalkableGridPosition(GridPosition gridPosition, bool isWalkable)
        {
            gridSystem.GetGridObject(gridPosition).SetIsWalkable(isWalkable);
        }

        public bool IsWalkableGridPosition(GridPosition gridPosition)
        {
            return (gridSystem.GetGridObject(gridPosition) != null) ? gridSystem.GetGridObject(gridPosition).IsWalkable() : false;
        }

        public bool HasPath(GridPosition startGridPosition, GridPosition endGridPosition)
        {
            return FindPath(startGridPosition, endGridPosition, out int pathLength) != null;
        }

        public int GetPathLength(GridPosition startGridPosition, GridPosition endGridPosition)
        {
            FindPath(startGridPosition, endGridPosition, out int pathLength);
            return pathLength;
        }
    }

}