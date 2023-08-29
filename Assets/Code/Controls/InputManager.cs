using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Code
{
    public class InputManager : MonoBehaviour
    {
        #region singleton

        public static InputManager Instance;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("Duplicate InputManager found!");
            }
            Instance = this;
        }

        #endregion

        public Vector3 cursorPosition;
        public bool block3DRaycast = false;
        void Start()
        {

        }

        public Vector2 GetZoomDelta()
        {
            return Input.mouseScrollDelta;
        }

        public float GetCursorDeltaX()
        {
            return Input.GetAxis("Mouse X");
        }

        public float GetCursorDeltaY()
        {
            return Input.GetAxis("Mouse Y");
        }

        public float GetHorizontalAxis()
        {
            return Input.GetAxis("Horizontal");
        }

        public float GetVerticalAxis()
        {
            return Input.GetAxis("Vertical");
        }

        // Pause Input
        public bool PauseInputActivated()
        {
            return Input.GetKeyDown(KeyCode.Escape);
        }

        // Jump Pressed
        public bool SecondaryActionActivated()
        {
            return Input.GetKeyDown(KeyCode.Space);
        }

        // Left Mouse Button
        public bool PrimaryActionActivated()
        {
            return Input.GetMouseButtonDown(0);
        }

        public bool PrimaryActionActive()
        {
            return Input.GetMouseButton(0);
        }

        public bool PrimaryActionReleased()
        {
            return Input.GetMouseButtonUp(0);
        }

        public bool CameraAdjustmentInputActive()
        {
            return Input.GetMouseButton(1);
        }

        // Update is called once per frame
        void Update()
        {
            cursorPosition = Input.mousePosition;
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Time.timeScale -= 0.1f;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Time.timeScale += 0.1f;
            }
#endif
        }
    }
}
