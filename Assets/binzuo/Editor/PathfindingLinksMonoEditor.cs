using UnityEngine;
using UnityEditor;

namespace binzuo
{
    [CustomEditor(typeof(PathfindingLinksMono))]
    public class PathfindingLinksMonoEditor : Editor
    {
        private void OnSceneGUI()
        {
            PathfindingLinksMono pathfindingLinksMono = (PathfindingLinksMono)target;

            EditorGUI.BeginChangeCheck();
            Vector3 newLinkPositionA = Handles.PositionHandle(pathfindingLinksMono.linkPositionA, Quaternion.identity);
            Vector3 newLinkPositionB = Handles.PositionHandle(pathfindingLinksMono.linkPositionB, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(pathfindingLinksMono, "Change Link Position");
                pathfindingLinksMono.linkPositionA = newLinkPositionA;
                pathfindingLinksMono.linkPositionB = newLinkPositionB;
            }
        }
    }
}

