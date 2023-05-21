namespace Voxel
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "NoiseSettings", menuName = "Voxel/NoiseSettings", order = 0)]
    public class NoiseSettings : ScriptableObject
    {
        public float noiseZoom;
        public int octaves;
        public Vector2Int offest;
        public Vector2Int worldOffset;
        public float persistance;
        public float redistributionModifier;
        public float exponent;
    }
}

