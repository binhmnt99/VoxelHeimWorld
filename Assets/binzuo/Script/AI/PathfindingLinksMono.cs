using UnityEngine;

namespace binzuo
{
    public class PathfindingLinksMono : MonoBehaviour
    {
        public Vector3 linkPositionA;
        public Vector3 linkPositionB;

        public PathfindingLinks GetPathfindingLinks() => new PathfindingLinks
        {
            gridPositionA = LevelGrid.Instance.GetGridPosition(linkPositionA),
            gridPositionB = LevelGrid.Instance.GetGridPosition(linkPositionB)
        };
    }
}
