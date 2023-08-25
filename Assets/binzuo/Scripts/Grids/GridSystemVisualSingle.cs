
using UnityEngine;

namespace binzuo
{
    public class GridSystemVisualSingle : MonoBehaviour
    {
        [SerializeField] private Renderer meshRenderer;

        public void Show()
        {
            meshRenderer.enabled = true;
        }

        public void Hide()
        {
            meshRenderer.enabled = false;
        }
    }
}

