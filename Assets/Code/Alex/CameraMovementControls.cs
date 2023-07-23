using System;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraMovementControls : MonoBehaviour
{
    [SerializeField] private float speed = 0.01f;
    [SerializeField] private float rotationSpeed = 0.1f;
    
    [SerializeField] private Transform container;
    
    [SerializeField] private bool isMouseControllingRotation;
    
    public void MouseControlRotationChange(bool newValue)
    {
        isMouseControllingRotation = newValue;
    }
    
    private void Update()
    {
        if (isMouseControllingRotation)
        {
            if (Input.GetButton("Fire2"))
            {
                Vector3 inputDirection = Vector3.zero;
                inputDirection.z = Input.GetAxis("Mouse Y");
                container.Translate(-inputDirection * speed, Space.Self);
                float rotation = Input.GetAxis("Mouse X");
                container.Rotate(Vector3.up, -rotation * rotationSpeed * 100, Space.World);
            }
        }
        else
        {
            if (Input.GetButton("Fire2"))
            {
                Vector3 inputDirection = Vector3.zero;
                inputDirection.x = Input.GetAxis("Mouse X");
                inputDirection.z = Input.GetAxis("Mouse Y");
                container.Translate(-inputDirection * speed, Space.Self);
            }
            float rotation = 0;
            if (Input.GetKey(KeyCode.Q))
            {
                rotation = -1;
            }
            else if (Input.GetKey(KeyCode.E))
            {
                rotation = 1;
            }
            container.Rotate(Vector3.up, rotation * rotationSpeed, Space.World);
        }
    }
 }