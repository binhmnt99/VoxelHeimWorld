
using UnityEngine;

namespace binzuo
{
    public class GridSystemVisualSingle : MonoBehaviour
    {
        [SerializeField] private Renderer meshRenderer;

        public void Show(Material material)
        {
            meshRenderer.enabled = true;
            meshRenderer.material = material;
        }

        public void Hide()
        {
            meshRenderer.enabled = false;
        }
    }
}

