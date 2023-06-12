using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class Pathfinding : MonoBehaviour
    {
        [SerializeField] private Transform gridDebugObjectPrefab;
        [SerializeField] private int width = 10;
        [SerializeField] private int height = 10;
        private float cellSize = 2f;
        private GridSystem<PathNode> gridSystem;

        void Awake()
        {
            gridSystem = new GridSystem<PathNode>(width,height,cellSize, (GridSystem<PathNode> g, GridPosition gridPosition) => new PathNode(gridPosition));
            gridSystem.CreateDebugObject(gridDebugObjectPrefab);
        }
    }

}