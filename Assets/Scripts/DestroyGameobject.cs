using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class DestroyGameobject : MonoBehaviour
    {
        [SerializeField] private float waitTime = 3f;
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(SelfDestruct());
        }

        private IEnumerator SelfDestruct()
        {
            yield return new WaitForSeconds(waitTime);
            Destroy(gameObject);
        }
    }

}