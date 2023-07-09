
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

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

        readonly List<PathNode> openList = new();
        readonly List<PathNode> closedList = new();

        public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition, out int pathLength)
        {
            Profiler.BeginSample("Find Path");
            openList.Clear();
            closedList.Clear();

            PathNode startNode = gridSystem.GetGridObject(startGridPosition);
            PathNode endNode = gridSystem.GetGridObject(endGridPosition);
            openList.Add(startNode);

            for (int x = 0; x < gridSystem.GetWidth(); x++)
            {
                for (int z = 0; z < gridSystem.GetHeight(); z++)
                {
                    GridPosition gridPosition = new(x, z);
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
                Profiler.BeginSample("GetLowestFCostPathNode");
                PathNode currentNode = GetLowestFCostPathNode(openList);
                Profiler.EndSample();

                if (currentNode == endNode)
                {
                    pathLength = endNode.GetFCost();
                    // Reached final node
                    Profiler.EndSample();
                    return CalculatePath(endNode);
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                Profiler.BeginSample("Process NeighborList");
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
                Profiler.EndSample();
            }

            pathLength = 0;
            Profiler.EndSample();

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
            int minCost = int.MaxValue;
            int minIdx = 0;
            for (int i = 0; i < pathNodeList.Count; i++)
            {
                if (pathNodeList[i].GetFCost() < minCost)
                {
                    minCost = pathNodeList[i].GetFCost();
                }    
            }
            return pathNodeList[minIdx];
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
            int width = gridSystem.GetWidth();
            int height = gridSystem.GetHeight();

            int leftX = gridPosition.x - 1;
            int rightX = gridPosition.x + 1;
            int upZ = gridPosition.z + 1;
            int downZ = gridPosition.z - 1;

            if (leftX >= 0)
                neighborList.Add(GetNode(leftX, gridPosition.z));

            if (rightX < width)
                neighborList.Add(GetNode(rightX, gridPosition.z));

            if (downZ >= 0)
                neighborList.Add(GetNode(gridPosition.x, downZ));

            if (upZ < height)
                neighborList.Add(GetNode(gridPosition.x, upZ));

            bool oddRow = gridPosition.z % 2 == 1;

            if (oddRow)
            {
                if (rightX < width && downZ >= 0)
                    neighborList.Add(GetNode(rightX, downZ));

                if (rightX < width && upZ < height)
                    neighborList.Add(GetNode(rightX, upZ));
            }
            else
            {
                if (leftX >= 0 && downZ >= 0)
                    neighborList.Add(GetNode(leftX, downZ));

                if (leftX >= 0 && upZ < height)
                    neighborList.Add(GetNode(leftX, upZ));
            }

            return neighborList;
        }

        List<PathNode> pathNodeList = new();
        List<GridPosition> gridPositionList = new();
    

        private List<GridPosition> CalculatePath(PathNode endNode)
        {
            Profiler.BeginSample("CalculatePath");
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
            Profiler.EndSample();
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