using System;
using UnityEngine;
using UnityEngine.UI;

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

    public Transform AlternativeStartTarget;
    public bool AlternativeMode = true;
    public float timer = -1;
    public Image fillImage1, fillImage2;
    public float maxForce;

    private void Start()
    {
        bottleLayerMask = LayerMask.GetMask("Bottle");
    }

    private void Update()
    {
        if (GameManager.Instance.menuController.isPaused
            || GameManager.Instance.menuController.block3dRaycast
            || !GameManager.Instance.menuController.inGameplay)
        {
            return;
        }
        if(timer < 0)
        {
            fillImage1.fillAmount = 0;
            fillImage2.fillAmount = 0;
            return;
        }
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            fillImage1.fillAmount = timer / 2;
            fillImage2.fillAmount = timer / 2;
            if (timer < 0)
            {
                timer = 0;
            }
        }
        if (timer > 0)
        {
            return;
        }
        if (Input.GetButtonDown("Fire1"))
        {
            bool didHit = Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out var hitInfo, 1000, bottleLayerMask);
            if (!didHit)
            {
                if (AlternativeMode)
                {
                    float distance = Vector3.Distance(mainCamera.transform.position, AlternativeStartTarget.position);
                    if (distance < 20)
                    {
                        grabPoint = AlternativeStartTarget.position;
                        isDragging = true;
                        grabDistance = distance;
                    }
                }
                else
                {
                    return;
                }
            }
            else
            {
                isDragging = true;
                grabPoint = hitInfo.point;
                grabDistance = hitInfo.distance;
            }
        }
        else if (Input.GetButtonUp("Fire1") && isDragging) // else here to prevent same frame fast click
        {
            Vector3 releasePoint = mainCamera.ScreenPointToRay(Input.mousePosition).GetPoint(grabDistance);
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
                timer = 2;
            }
            isDragging = false;
            dragLine.positionCount = 0;
        }
        
        
        if (Input.GetButton("Fire1") && isDragging)
        {
            dragLine.positionCount = 2;
            dragLine.SetPositions(new [] // TODO: limit line lenght according to maxForce
            {
                grabPoint,
                mainCamera.ScreenPointToRay(Input.mousePosition).GetPoint(grabDistance)
            });
        }
    }
}