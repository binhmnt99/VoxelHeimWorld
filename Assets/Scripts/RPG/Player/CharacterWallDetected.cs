using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterWallDetected : MonoBehaviour
{
    [SerializeField]
    private Transform characterTransform;
    [SerializeField]
    private LayerMask layer;
    public bool isWall { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        if (layer == (layer | (1 << other.gameObject.layer)))
        {
            isWall = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (layer == (layer | (1 << other.gameObject.layer)))
        {
            isWall = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (layer == (layer | (1 << other.gameObject.layer)))
        {
            isWall = false;
        }
    }

    private void LateUpdate()
    {
        transform.rotation = characterTransform.rotation;
    }
}
