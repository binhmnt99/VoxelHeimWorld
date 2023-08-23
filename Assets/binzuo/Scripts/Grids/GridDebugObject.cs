using UnityEngine;
using TMPro;

namespace binzuo
{
    public class GridDebugObject : MonoBehaviour
    {
        [SerializeField] private TextMeshPro textMeshPro;
        private GridObject gridObject;

        public void SetGridObject(GridObject _gridObject)
        {
            this.gridObject = _gridObject;
        }

        private void Update() {
            textMeshPro.text = gridObject.ToString();
        }
    }

}
