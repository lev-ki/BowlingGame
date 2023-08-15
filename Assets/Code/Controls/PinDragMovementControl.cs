using UnityEngine;
using UnityEngine.UI;

namespace Code.Controls
{
    public class PinDragMovementControl : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private LineRenderer dragLine;
    
        [SerializeField] private float forceMultiplier = 100.0f;

        private int bottleLayerMask;

        private Vector3 grabPoint;
        private float grabDistance;

        private bool isDragging;

        [SerializeField] private Transform alternativeStartTarget;
        [SerializeField] private Transform currentDragPoint;
        public bool alternativeMode = true;
        [SerializeField] private Image fillImage1, fillImage2;
        private float timer = -1;
        public float Timer
        {
            get => timer;
            set
            {
                timer = value;
                fillImage1.fillAmount = fillImage2.fillAmount = timer > 0 ? timer : 0;
            }
        }
        
        public float maxForce;

        private void Start()
        {
            bottleLayerMask = LayerMask.GetMask("Bottle");
        }

        private void Update()
        {
            Timer -= Time.deltaTime;
            if(InputManager.Instance.block3DRaycast || Timer > 0)
            {
                if (dragLine.positionCount != 0)
                {
                    dragLine.positionCount = 0;
                }
                return;
            }
            if (InputManager.Instance.PrimaryActionActivated())
            {
                bool didHit = Physics.Raycast(mainCamera.ScreenPointToRay(InputManager.Instance.cursorPosition), out var hitInfo, 1000, bottleLayerMask);
                if (!didHit)
                {
                    if (!alternativeMode)
                    {
                        return;
                    }
                    float distance = Vector3.Distance(mainCamera.transform.position,
                        alternativeStartTarget.position);
                    if (distance > 22)
                    {
                        return;
                    }
                    grabPoint = alternativeStartTarget.position;
                    isDragging = true;
                    grabDistance = distance;
                }
                else
                {
                    isDragging = true;
                    grabPoint = hitInfo.point;
                    grabDistance = hitInfo.distance;
                }
                currentDragPoint.position = grabPoint;
            }
            else if (InputManager.Instance.PrimaryActionReleased() && isDragging) // else here to prevent same frame fast click
            {
                Vector3 releasePoint = mainCamera.ScreenPointToRay(InputManager.Instance.cursorPosition).GetPoint(grabDistance);
                Vector3 force = grabPoint - releasePoint;
                if (force.sqrMagnitude > 0.01)
                {
                    force *= forceMultiplier;
                    if (force.sqrMagnitude > maxForce * maxForce)
                    {
                        force.Normalize();
                        force *= maxForce;
                    }
                    rb.AddForceAtPosition(force, grabPoint, ForceMode.Impulse);
                    Timer = 2;
                }
                isDragging = false;
                dragLine.positionCount = 0;
            }
        
            // draw
            if (InputManager.Instance.PrimaryActionActive() && isDragging)
            {
                grabPoint = currentDragPoint.position;
                dragLine.positionCount = 2;
                dragLine.SetPositions(new [] // TODO(Alex): limit line length according to maxForce
                {
                    grabPoint,
                    mainCamera.ScreenPointToRay(InputManager.Instance.cursorPosition).GetPoint(grabDistance)
                });
            }
        }
    }
}