using System.Collections.Generic;
using UnityEngine;

namespace Code.Controls
{
    public class CustomCameraController : MonoBehaviour
    {
        public enum CameraMode { Fixed, Waypoint, Follow, Orbit };

        [Header("Mode settings")]
        public CameraMode currentCameraMode;
        public CameraMode futureCameraMode;

        [Header("Main settings")]
        public Transform cameraParent;
        public Camera targetCamera;
        public Transform currentTarget;
        public Transform secondaryTarget;
        public Transform defaultOrbitTarget;
        public Vector3 cameraOffset = new Vector3(0, 0, -12);

        [Header("Speed parametres")]
        public float rotationSpeed = 500;
        public float lookFollowSpeed = 60;
        public float moveSpeed = 10;
        public float moveSmoothTime = 0.3f;
        public float zoomSpeed = 5;
        public float scrollSpeed = 50;
        public float touchMultiplier = 0.01f;
        public float controllerMultiplier;

        [Header("Zoom parametres")]
        public float minZoom = 5;
        public float maxZoom = 20;
        public float defaultMinZoom = 5;
        public float defaultMaxZoom = 20;

        [Header("Waypoint parametres")]
        public List<Transform> waypoints;
        public int currentWaypoint;
        public float waypointDistanceTolerance;
        public float waypointAngleTolerance;

        [Header("Collisions paramatres")]
        public LayerMask layerMask;
        public float collisionOffset = 0.25f;
        public float collisionCorrectionSpeed = 50;

        [Header("View angle limits")]
        public float lowVerticalLimit = -15;
        public float highVerticalLimit = 50;

        [Header("Follow Parametres")]
        public float primaryToSecondaryRatio = 0.8f;
        public float primaryToSecondaryRatioWhenAiming = 0.5f;

        private bool secondaryFollow = false;

        protected Plane Plane;

        private float angleX = 0;
        private Vector3 inputVector;
        private bool wasPinchingLastFrame;
        private float lastFrameDistance;
        private RaycastHit camHit;
        private bool inverted = false;

        public void ResetTarget()
        {
            minZoom = defaultMinZoom;
            maxZoom = defaultMaxZoom;
            cameraOffset.z = -maxZoom;
            currentTarget = defaultOrbitTarget;
            secondaryFollow = false;
            inverted = false;
        }

        public void SetMode(CameraMode newMode)
        {
            currentCameraMode = newMode;
        }

        public void SetNewTarget(Transform newTarget, float newMinZoom, float newMaxZoom, bool newInvertedState, bool setSecondaryTarget = false, Transform newSecondaryTarget = null)
        {
            minZoom = newMinZoom;
            maxZoom = newMaxZoom;
            currentTarget = newTarget;

            inverted = newInvertedState;

            if(setSecondaryTarget)
            {
                secondaryFollow = true;
                secondaryTarget = newSecondaryTarget;
            }
        }

        public void SetWayPoints(List<Transform> newWaypoints)
        {
            waypoints = newWaypoints;
            currentCameraMode = CameraMode.Waypoint;
            currentWaypoint = 0;
            currentTarget = waypoints[currentWaypoint];
        }

        public void Toggle()
        {
            if(currentCameraMode == CameraMode.Follow)
            {
                currentCameraMode = CameraMode.Orbit;
            }
            else if(currentCameraMode == CameraMode.Orbit)
            {
                currentCameraMode = CameraMode.Follow;
            }
        }

        public static Vector3 Smooth(Vector3 source, Vector3 target, float rate, float dt)
        {
            return Vector3.Lerp(source, target, 1 - Mathf.Pow(rate, dt));
        }

        public Vector3 SmoothMoveToPos(Transform from, Transform to, Transform secondaryTarget)
        {
            if (GameManager.Instance.IsPaused)
            {
                return transform.position;
            }
            Vector3 followPosition = to.position * primaryToSecondaryRatio + secondaryTarget.position * (1 - primaryToSecondaryRatio);
            return Smooth(transform.position, followPosition, moveSmoothTime, Time.deltaTime); 
        }

        public Quaternion SmoothRotate(Transform from, Transform to)
        {
            if (GameManager.Instance.IsPaused)
            {
                return from.rotation;
            }
            return Quaternion.Lerp(from.rotation, to.rotation, Time.deltaTime * moveSpeed);
        }

        public Quaternion SmoothLookAt(Transform from, Transform to)
        {
            if (GameManager.Instance.IsPaused)
            {
                return from.rotation;
            }
            return Quaternion.RotateTowards(from.rotation, Quaternion.LookRotation(to.position - from.position), Time.deltaTime * lookFollowSpeed);
        }

        public void Zoom(float zoomInput)
        {
            if (GameManager.Instance.IsPaused)
            {
                return;
            }
            cameraOffset.z = Mathf.Clamp(cameraOffset.z - zoomInput, -maxZoom, -minZoom);
            Vector3 newPos = Vector3.Lerp( targetCamera.transform.localPosition, cameraOffset, Time.deltaTime * zoomSpeed );
            targetCamera.transform.localPosition = newPos;
        }

        public bool CalculatePosition()
        {
            if (GameManager.Instance.IsPaused)
            {
                return false;
            }
            Vector3 oldPosition = targetCamera.transform.localPosition;
            Vector3 targetPosition = cameraParent.transform.position - targetCamera.transform.forward * (-cameraOffset.z + collisionOffset);
            if(Physics.Linecast(cameraParent.transform.position, targetPosition, out camHit, layerMask) && !camHit.collider.isTrigger)
            {
                //This gets executed if there's any collider in the way
                targetCamera.transform.position = camHit.point;
                targetCamera.transform.localPosition = new Vector3(oldPosition.x, oldPosition.y, Mathf.Lerp(oldPosition.z, targetCamera.transform.localPosition.z + collisionOffset, Time.deltaTime * collisionCorrectionSpeed / Time.timeScale));
                return true;
            }
            targetCamera.transform.localPosition = new Vector3(oldPosition.x, oldPosition.y, Mathf.Lerp(oldPosition.z, cameraOffset.z, Time.deltaTime * zoomSpeed / Time.timeScale));
            return false;
        }

        public void RotateCamera()
        {
            if (!InputManager.Instance.CameraAdjustmentInputActive())
            {
                return;
            }
            angleX = cameraParent.eulerAngles.x;
            if(angleX > 90)
            {
                angleX -= 360;
            }
            // float clampedAngle =
            if(angleX < 90 && angleX - inputVector.y > highVerticalLimit)
            {
                inputVector.y = -(highVerticalLimit - angleX);
            } 
            if(angleX - inputVector.y < lowVerticalLimit)
                // if(angleX > 90 && angleX + input.y < 350 )
            {
                inputVector.y = -(lowVerticalLimit - angleX);
                // input.x = 350 - cameraParent.eulerAngles.x;
            }
            // if(cameraParent.eulerAngles.x > 90)
            // {
            // input.y = -(350 - cameraParent.eulerAngles.x);
            // }
            cameraParent.eulerAngles = new Vector3(angleX - inputVector.y , cameraParent.eulerAngles.y + inputVector.x, 0);
        }

        // Update is called once per frame
        void LateUpdate()
        {
            inputVector = GetControllerInput();
            if (false)//Input.touchSupported && Application.platform != RuntimePlatform.WebGLPlayer)
            {
                inputVector += GetTouchInput();
            }
            else 
            {
                inputVector += GetMouseInput();
            }
            // text.text = "x:" + input.x + ";\n y:" + input.y + ";\n z:" + input.z;
            inputVector.x *= rotationSpeed * Time.deltaTime / Time.timeScale * (inverted ? -1 : 1);
            inputVector.y *= rotationSpeed * Time.deltaTime / Time.timeScale * (inverted ? -1 : 1);
            switch(currentCameraMode)
            {
                case CameraMode.Fixed:
                    cameraParent.position = SmoothMoveToPos(cameraParent, currentTarget, secondaryTarget);
                    cameraParent.rotation = SmoothRotate(cameraParent, currentTarget);
                    break;

                case CameraMode.Waypoint:
                    cameraParent.position = SmoothMoveToPos(cameraParent, currentTarget, secondaryTarget);
                    cameraParent.rotation = SmoothRotate(cameraParent, currentTarget);
                    if(Vector3.Distance(cameraParent.position, currentTarget.position) < waypointDistanceTolerance
                       && Quaternion.Angle(cameraParent.rotation, currentTarget.rotation) < waypointAngleTolerance)
                    {
                        currentWaypoint++;
                        if(currentWaypoint >= waypoints.Count)
                        {
                            currentCameraMode = futureCameraMode;
                        }
                        else
                        {
                            currentTarget = waypoints[currentWaypoint];
                        }
                    }
                    break;

                case CameraMode.Follow:
                    cameraParent.position = SmoothMoveToPos(cameraParent, currentTarget, secondaryTarget);
                    //cameraParent.rotation = SmoothLookAt(cameraParent, secondaryTarget);
                    Zoom(inputVector.z);
                    break;

                case CameraMode.Orbit:
                    cameraParent.position = SmoothMoveToPos(cameraParent, currentTarget, secondaryTarget);
                    RotateCamera();
                    if(secondaryFollow)
                    {
                        if(inputVector.magnitude > 0)
                        {
                            // secondaryFollow = false;
                        }
                        else
                        {
                            cameraParent.rotation = SmoothLookAt(cameraParent, secondaryTarget);
                        }
                    }
                    Zoom(inputVector.z);
                    CalculatePosition();
                    break;
            }
        }

        Vector3 GetTouchInput()
        {
            Vector3 inputInfo = new Vector3();
            /*
        if (Input.touchCount == 1)
        {
            
                inputInfo.x = Mathf.Clamp(Input.touches[0].deltaPosition.x * touchMultiplier, -1, 1);
                inputInfo.y = Mathf.Clamp(Input.touches[0].deltaPosition.y * touchMultiplier, -1, 1);
            
        }
        if(Input.touchCount == 2)
        {
                inputInfo.x = Mathf.Clamp(Input.touches[1].deltaPosition.x * touchMultiplier, -1, 1);
                inputInfo.y = Mathf.Clamp(Input.touches[1].deltaPosition.y * touchMultiplier, -1, 1);
            
            // Vector2 touch0, touch1;
            // float distance;
            
            // touch0 = Input.GetTouch(0).position;
            // touch1 = Input.GetTouch(1).position;
            // distance = Vector2.Distance(touch0, touch1);

            // if(wasPinchingLastFrame)
            // {
            //     inputInfo.z = Mathf.Clamp(lastFrameDistance - distance, -1, 1 );
            // }

            // wasPinchingLastFrame = true;
            // lastFrameDistance = distance;
        }
        // else
        // {
        //     wasPinchingLastFrame = false;
        //     lastFrameDistance = 0;
        // }*/
            return inputInfo;
        }

        Vector3 GetControllerInput()
        {
            //Vector3 inputInfo = new Vector3(Input.GetAxis("RightStickVertical") * controllerMultiplier, -Input.GetAxis("RightStickHorizontal") * controllerMultiplier, 0);
            //return inputInfo;
            return Vector3.zero;
        }

        protected Vector3 PlanePosition(Vector2 screenPos)
        {
            //position
            var rayNow = targetCamera.ScreenPointToRay(screenPos);
            if (Plane.Raycast(rayNow, out var enterNow))
            {
                return rayNow.GetPoint(enterNow);
            }

            return Vector3.zero;
        }

        Vector3 GetMouseInput()
        {
            Vector3 inputInfo = new Vector3();
            if (GameManager.Instance.IsPaused)
            {
                return inputInfo;
            }
            //if (Input.GetMouseButton(0))
            {

                inputInfo.x = InputManager.Instance.GetCursorDeltaX();
                inputInfo.y = InputManager.Instance.GetCursorDeltaY();

            }
            inputInfo.z = - InputManager.Instance.GetZoomDelta().y * scrollSpeed * Time.deltaTime / Time.timeScale;

            return inputInfo;
        }

        public void SetOrbitTarget( Transform target, Vector3 offset )
        {
            currentTarget = target;
            secondaryTarget = target;
            defaultOrbitTarget = target;
            cameraOffset = offset;
            currentCameraMode = CameraMode.Orbit;
            futureCameraMode = CameraMode.Orbit;
        }

        public void SetFixedTarget( Transform target, Vector3 offset )
        {
            currentTarget = target;
            secondaryTarget = target;
            defaultOrbitTarget = target;
            cameraOffset = offset;
            currentCameraMode = CameraMode.Fixed;
            futureCameraMode = CameraMode.Fixed;
            targetCamera.transform.position = transform.position;
        }
    }
}
