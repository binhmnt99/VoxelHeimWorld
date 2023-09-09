using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace binzuo
{
    public class Pathfinding : Singleton<Pathfinding>
    {
        private const int MOVE_STRAIGHT_COST = 10;
        private const int MOVE_DIAGONAL_COST = 14;
        [SerializeField] private Transform pathFindingGridDebugObjectPrefab;

        private int width;
        private int height;
        private float callSize;

        private GridSystem<PathNode> gridSystem;

        protected override void Awake()
        {
            base.Awake();
            gridSystem = new GridSystem<PathNode>(10, 10, 2.5f, (GridSystem<PathNode> g, GridPosition gridPosition) => new PathNode(gridPosition));
            gridSystem.CreateDebugObjects(pathFindingGridDebugObjectPrefab);
        }


        List<PathNode> openList = new();
        List<PathNode> closedList = new();
        public List<GridPosition> FindPath(GridPosition startGrid, GridPosition endGrid)
        {
            openList.Clear();
            closedList.Clear();

            PathNode startNode = gridSystem.GetGridObject(startGrid);
            PathNode endNode = gridSystem.GetGridObject(endGrid);

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
                    pathNode.ResetCameFormPathNode();
                }
            }

            startNode.SetGCost(0);
            startNode.SetHCost(CalculateDistance(startGrid, endGrid));
            startNode.CalculateFCost();

            while (openList.Count > 0)
            {
                PathNode currentNode = GetLowestFCostPathNode(openList);

                if (currentNode == endNode)
                {
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

                    int tentativeGCost = currentNode.GetGCost() + CalculateDistance(currentNode.GetGridPosition(), neighborNode.GetGridPosition());

                    if (tentativeGCost < neighborNode.GetGCost())
                    {
                        neighborNode.SetCameFormPathNode(currentNode);
                        neighborNode.SetGCost(tentativeGCost);
                        neighborNode.SetHCost(CalculateDistance(neighborNode.GetGridPosition(), endGrid));
                        neighborNode.CalculateFCost();

                        if (!openList.Contains(neighborNode))
                        {
                            openList.Add(neighborNode);
                        }
                    }
                }
            }

            return null;
        }

        public int CalculateDistance(GridPosition gridA, GridPosition gridB)
        {
            GridPosition gridDistance = gridA - gridB;
            int xDistance = Mathf.Abs(gridDistance.x);
            int zDistance = Mathf.Abs(gridDistance.z);
            int remaining = Mathf.Abs(xDistance - zDistance);
            return MOVE_DIAGONAL_COST * Mathf.Min(xDistance,zDistance) + MOVE_STRAIGHT_COST * remaining;
        }

        private PathNode GetLowestFCostPathNode(List<PathNode> pathNodeList)
        {
            PathNode lowestFCostPathNode = pathNodeList[0];
            foreach (PathNode pathNode in pathNodeList)
            {
                if (pathNode.GetFCost() < lowestFCostPathNode.GetFCost())
                {
                    lowestFCostPathNode = pathNode;
                }
            }
            return lowestFCostPathNode;
        }

        List<PathNode> pathNodeList = new();
        List<GridPosition> gridPositionList = new();
        private List<GridPosition> CalculatePath(PathNode endNode)
        {
            pathNodeList.Clear();
            gridPositionList.Clear();
            pathNodeList.Add(endNode);
            PathNode currentNode = endNode;
            while (currentNode.GetCameFormPathNode() != null)
            {
                pathNodeList.Add(currentNode.GetCameFormPathNode());
                currentNode = currentNode.GetCameFormPathNode();
            }

            pathNodeList.Reverse();

            foreach (PathNode pathNode in pathNodeList)
            {
                gridPositionList.Add(pathNode.GetGridPosition());
            }

            return gridPositionList;
        }

        List<PathNode> neighborNodeList = new();
        private List<PathNode> GetNeighborList(PathNode currentNode)
        {
            neighborNodeList.Clear();

            GridPosition gridPosition = currentNode.GetGridPosition();

            if (gridPosition.x - 1 >= 0)
            {
                //Left
                neighborNodeList.Add(GetNode(gridPosition.x - 1, gridPosition.z));
            }

            if (gridPosition.x + 1 >= width)
            {
                //Right
                neighborNodeList.Add(GetNode(gridPosition.x + 1, gridPosition.z));
            }

            if (gridPosition.z + 1 >= height)
            {
                //Up
                neighborNodeList.Add(GetNode(gridPosition.x, gridPosition.z + 1));
            }

            if (gridPosition.z - 1 >= 0)
            {
                //Down
                neighborNodeList.Add(GetNode(gridPosition.x, gridPosition.z - 1));
            }

            return neighborNodeList;
        }

        private PathNode GetNode(int x, int z)
        {
            return gridSystem.GetGridObject(new GridPosition(x, z));
        }
    }

}
