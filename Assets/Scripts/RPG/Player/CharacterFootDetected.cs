using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFootDetected : MonoBehaviour
{
    [SerializeField]
    private Transform characterTransform;
    [SerializeField]
    private LayerMask layer;
    public bool isGrounded { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        if (layer == (layer | (1 << other.gameObject.layer)))
        {
            isGrounded = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (layer == (layer | (1 << other.gameObject.layer)))
        {
            isGrounded = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (layer == (layer | (1 << other.gameObject.layer)))
        {
            isGrounded = false;
        }
    }

    private void LateUpdate()
    {
        transform.rotation = characterTransform.rotation;
    }
}
