using UnityEngine;
using UnityEngine.Events;

namespace Voxel
{
    public class WorldGenerate : MonoBehaviour
{
        public UnityEvent onStart;
        // Start is called before the first frame update
        void Start()
        {
            onStart.Invoke();
        }

        // Update is called once per frame
        void Update()
        {

        }
}
}

