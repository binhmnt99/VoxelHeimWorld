using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace binzuo
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float cameraMoveSpeed = 10f;
        [SerializeField] private float cameraRotateSpeed = 100f;
        private void Update()
        {
            Vector3 inputMoveDirection = new Vector3(0, 0, 0);
            if (Input.GetKey(KeyCode.W))
            {
                inputMoveDirection.z = +1f;
            }
            if (Input.GetKey(KeyCode.S))
            {
                inputMoveDirection.z = -1f;
            }
            if (Input.GetKey(KeyCode.A))
            {
                inputMoveDirection.x = -1f;
            }
            if (Input.GetKey(KeyCode.D))
            {
                inputMoveDirection.x = +1f;
            }

            Vector3 moveVector = transform.forward * inputMoveDirection.z + transform.right * inputMoveDirection.x;
            transform.position += moveVector * cameraMoveSpeed * Time.deltaTime;

            Vector3 rotationVector = new Vector3(0, 0, 0);

            if (Input.GetKey(KeyCode.Q))
            {
                rotationVector.y = +1f;
            }
            if (Input.GetKey(KeyCode.E))
            {
                rotationVector.y = -1f;
            }

            transform.eulerAngles += rotationVector * cameraRotateSpeed * Time.deltaTime;
        }
    }

}