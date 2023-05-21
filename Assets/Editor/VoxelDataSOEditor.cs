using UnityEditor;

namespace Voxel.Editor
{
    [CustomEditor(typeof(VoxelDataSO))]
    public class VoxelDataSOEditor : UnityEditor.Editor
    {
        private SerializedProperty textureSizeXProperty;
        private SerializedProperty textureSizeYProperty;
        private SerializedProperty texturesDataProperty;

        private void OnEnable()
        {
            try
            {
                textureSizeXProperty = serializedObject.FindProperty("textureSizeX");
                textureSizeYProperty = serializedObject.FindProperty("textureSizeY");
                texturesDataProperty = serializedObject.FindProperty("texturesData");
            }
            catch (System.Exception)
            {
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(textureSizeXProperty);
            EditorGUILayout.PropertyField(textureSizeYProperty);
            EditorGUILayout.PropertyField(texturesDataProperty, true);
            bool texturesDataChanged = EditorGUI.EndChangeCheck();

            serializedObject.ApplyModifiedProperties();

            if (texturesDataChanged)
            {
                VoxelDataSO voxelData = (VoxelDataSO)target;
                voxelData.CalculateColumnAndRow();
            }
        }
    }
}
