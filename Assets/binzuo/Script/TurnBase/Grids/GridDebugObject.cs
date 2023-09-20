using UnityEngine;
using TMPro;

namespace binzuo
{
    public class GridDebugObject : MonoBehaviour
    {
        [SerializeField] private TextMeshPro textMeshPro;
        private object gridObject;

        public virtual void SetGridObject(object _gridObject)
        {
            this.gridObject = _gridObject;
        }

        protected virtual void Update() {
            textMeshPro.text = gridObject.ToString();
        }
    }

}
