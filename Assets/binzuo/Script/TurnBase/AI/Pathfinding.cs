using System.Collections.Generic;
using UnityEngine;

namespace binzuo
{
    public class Pathfinding : Singleton<Pathfinding>
    {
        private const int MOVE_STRAIGHT_COST = 10;
        private const int MOVE_DIAGONAL_COST = 14;

        [SerializeField] private Transform gridDebugObjectPrefab;
        [SerializeField] private LayerMask obstaclesLayerMask;
        [SerializeField] private LayerMask floorLayerMask;
        [SerializeField] private Transform pathfindingLinksContainer;


        private int width;
        private int height;
        private float cellSize;
        private int floorAmount;
        private List<GridSystem<PathNode>> gridSystemList;
        private List<PathfindingLinks> pathfindingLinkList;

        protected override void Awake()
        {
            base.Awake();
        }

        public void Setup(int width, int height, float cellSize, int floorAmount)
        {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;
            this.floorAmount = floorAmount;

            gridSystemList = new();

            for (int floor = 0; floor < floorAmount; floor++)
            {
                var gridSystem = new GridSystem<PathNode>(width, height, cellSize, floor, LevelGrid.FLOOR_HEIGHT,
                    (GridSystem<PathNode> g, GridPosition gridPosition) => new PathNode(gridPosition));
                gridSystemList.Add(gridSystem);
            }

            //gridSystem.CreateDebugObjects(gridDebugObjectPrefab);

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    for (int floor = 0; floor < floorAmount; floor++)
                    {
                        GetNode(x, z, floor).SetIsWalkable(false);

                        GridPosition gridPosition = new GridPosition(x, z, floor);
                        Vector3 worldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
                        float raycastOffsetDistance = 1f;
                        if (Physics.Raycast(worldPosition + Vector3.up * raycastOffsetDistance, Vector3.down, raycastOffsetDistance * 2, floorLayerMask))
                        {
                            GetNode(x, z, floor).SetIsWalkable(true);
                        }

                        if (Physics.Raycast(worldPosition + Vector3.down * raycastOffsetDistance, Vector3.up, raycastOffsetDistance * 2, obstaclesLayerMask))
                        {
                            GetNode(x, z, floor).SetIsWalkable(false);
                        }
                    }
                }
            }

            pathfindingLinkList = new();
            foreach (Transform pathfindingLinksTransform in pathfindingLinksContainer)
            {
                if (pathfindingLinksTransform.TryGetComponent(out PathfindingLinksMono pathfindingLinksMono))
                {
                    pathfindingLinkList.Add(pathfindingLinksMono.GetPathfindingLinks());
                }
            }
        }

        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closedList = new List<PathNode>();
        public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition, out int pathLength)
        {
            openList.Clear();
            closedList.Clear();

            PathNode startNode = GetGridSystem(startGridPosition.floor).GetGridObject(startGridPosition);
            PathNode endNode = GetGridSystem(endGridPosition.floor).GetGridObject(endGridPosition);
            openList.Add(startNode);

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    for (int floor = 0; floor < floorAmount; floor++)
                    {
                        GridPosition gridPosition = new GridPosition(x, z, floor);
                        PathNode pathNode = GetGridSystem(floor).GetGridObject(gridPosition);

                        pathNode.SetGCost(int.MaxValue);
                        pathNode.SetHCost(0);
                        pathNode.CalculateFCost();
                        pathNode.ResetCameFromPathNode();
                    }
                }
            }

            startNode.SetGCost(0);
            startNode.SetHCost(CalculateDistance(startGridPosition, endGridPosition));
            startNode.CalculateFCost();

            while (openList.Count > 0)
            {
                PathNode currentNode = GetLowestFCostPathNode(openList);

                if (currentNode == endNode)
                {
                    // Reached final node
                    pathLength = endNode.GetFCost();
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
                        currentNode.GetGCost() + CalculateDistance(currentNode.GetGridPosition(), neighborNode.GetGridPosition());

                    if (tentativeGCost < neighborNode.GetGCost())
                    {
                        neighborNode.SetCameFromPathNode(currentNode);
                        neighborNode.SetGCost(tentativeGCost);
                        neighborNode.SetHCost(CalculateDistance(neighborNode.GetGridPosition(), endGridPosition));
                        neighborNode.CalculateFCost();

                        if (!openList.Contains(neighborNode))
                        {
                            openList.Add(neighborNode);
                        }
                    }
                }
            }

            // No path found
            pathLength = 0;
            return null;
        }

        public int CalculateDistance(GridPosition gridPositionA, GridPosition gridPositionB)
        {
            GridPosition gridPositionDistance = gridPositionA - gridPositionB;
            int xDistance = Mathf.Abs(gridPositionDistance.x);
            int zDistance = Mathf.Abs(gridPositionDistance.z);
            int remaining = Mathf.Abs(xDistance - zDistance);
            return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, zDistance) + MOVE_STRAIGHT_COST * remaining;
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

        private GridSystem<PathNode> GetGridSystem(int floor) => gridSystemList[floor];

        private PathNode GetNode(int x, int z, int floor)
        {
            return GetGridSystem(floor).GetGridObject(new GridPosition(x, z, floor));
        }

        List<PathNode> neighborList = new();
        List<PathNode> totalNeighborList = new();
        List<GridPosition> pathfindingLinksGridPositionList = new();
        private List<PathNode> GetNeighborList(PathNode currentNode)
        {
            neighborList.Clear();
            totalNeighborList.Clear();
            pathfindingLinksGridPositionList.Clear();

            GridPosition gridPosition = currentNode.GetGridPosition();

            if (gridPosition.x - 1 >= 0)
            {
                // Left
                neighborList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 0, gridPosition.floor));
            }

            if (gridPosition.x + 1 < width)
            {
                // Right
                neighborList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 0, gridPosition.floor));
            }

            if (gridPosition.z - 1 >= 0)
            {
                // Down
                neighborList.Add(GetNode(gridPosition.x + 0, gridPosition.z - 1, gridPosition.floor));
            }
            if (gridPosition.z + 1 < height)
            {
                // Up
                neighborList.Add(GetNode(gridPosition.x + 0, gridPosition.z + 1, gridPosition.floor));
            }

            totalNeighborList.AddRange(neighborList);

            pathfindingLinksGridPositionList = GetPathfindingLinkConnectedGridPositionList(gridPosition);

            foreach (GridPosition pathfindingLinkGridPosition in pathfindingLinksGridPositionList)
            {
                totalNeighborList.Add(GetNode(pathfindingLinkGridPosition.x, pathfindingLinkGridPosition.z, pathfindingLinkGridPosition.floor));
            }

            return totalNeighborList;

        }

        List<GridPosition> gridPositionConnectedList = new();
private List<GridPosition> GetPathfindingLinkConnectedGridPositionList(GridPosition gridPosition)
    {
        gridPositionConnectedList.Clear();

        foreach (PathfindingLinks pathfindingLink in pathfindingLinkList)
        {
            if (pathfindingLink.gridPositionA == gridPosition)
            {
                gridPositionConnectedList.Add(pathfindingLink.gridPositionB);
            }
            if (pathfindingLink.gridPositionB == gridPosition)
            {
                gridPositionConnectedList.Add(pathfindingLink.gridPositionA);
            }
        }

        return gridPositionConnectedList;
    }


        List<PathNode> pathNodeList = new List<PathNode>();
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

            List<GridPosition> gridPositionList = new List<GridPosition>();
            foreach (PathNode pathNode in pathNodeList)
            {
                gridPositionList.Add(pathNode.GetGridPosition());
            }

            return gridPositionList;
        }

        public void SetWalkableGridPosition(GridPosition gridPosition, bool isWalkable)
        {
            GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).SetIsWalkable(isWalkable);
        }

        public bool IsWalkableGridPosition(GridPosition gridPosition)
        {
            return GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).IsWalkable();
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
