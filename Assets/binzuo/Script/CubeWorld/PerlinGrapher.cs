using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace binzuo
{
    [ExecuteInEditMode]
    public class PerlinGrapher : MonoBehaviour
    {
        private LineRenderer lineRenderer;

        [SerializeField] private float heightScale = 10f;
        [SerializeField] private float widthScale = .001f;
        [SerializeField] private int octaves = 8;
        [SerializeField] private float heightOffset = -33f;

        private void Start()
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = 100;
            Graph();
        }

        private void OnValidate()
        {
            Graph();
        }

        private float FractalBrownianMotion(float x, float z)
        {
            float total = 0;
            float frequency = 1;
            for (int i = 0; i < octaves; i++)
            {
                total += Mathf.PerlinNoise(x * widthScale * frequency, z * widthScale * frequency) * heightScale;
                frequency *= 2;
            }
            return total;
        }

        private void Graph()
        {
            int z = 11;
            Vector3[] positions = new Vector3[lineRenderer.positionCount];
            for (int x = 0; x < lineRenderer.positionCount; x++)
            {
                float y = FractalBrownianMotion(x, z) + heightOffset;
                positions[x] = new Vector3(x, y, z);
            }
            lineRenderer.SetPositions(positions);
        }
    }

}
