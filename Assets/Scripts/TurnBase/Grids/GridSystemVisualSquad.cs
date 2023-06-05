using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisualSquad : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    // Start is called before the first frame update
    void Awake()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    public void Show()
    {
        meshRenderer.enabled = true;
    }

    public void Hide()
    {
        meshRenderer.enabled = false;
    }
}
