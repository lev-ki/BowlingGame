using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

public class PinKeyboardControls : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform centerOfMass;
    [SerializeField] private Transform forceApplicationPos;
    [SerializeField] private Transform m_directionSource;

    [SerializeField] private JumpAllowanceTrigger jumpAllowanceTrigger;

    [SerializeField] private float forceMultiplier;

    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpLeaningFactor = 3;
    private Vector3 inputDirection;
    private bool jumpNextFrame;

    [SerializeField] private TextMeshProUGUI forceValue;
    public void SetMoveForce(float alpha)
    {
        float min = 1;
        float max = 10;
        forceMultiplier = Mathf.Lerp(min, max, alpha);

        forceValue.text = forceMultiplier.ToString();
    }

    public TextMeshProUGUI jumpForceValue;
    public void SetJumpForce(float alpha)
    {
        float min = 100;
        float max = 500;
        jumpForce = Mathf.Lerp(min, max, alpha);

        jumpForceValue.text = jumpForce.ToString();
    }

    public TextMeshProUGUI jumpLeaningValue;
    public void SetJumpLeaning(float alpha)
    {
        float min = 1;
        float max = 5;
        jumpLeaningFactor = Mathf.Lerp(min, max, alpha);

        jumpLeaningValue.text = jumpLeaningFactor.ToString();
    }

    public TextMeshProUGUI comValue;
    public void SetCenterOfMass(float alpha)
    {
        float min = 0;
        float max = 2.5f;
        Vector3 com = Vector3.zero;
        com.y = Mathf.Lerp(min, max, alpha);

        rb.centerOfMass = com;

        comValue.text = com.y.ToString();
    }


    // Start is called before the first frame update
    void Start()
    {
        //rb.centerOfMass = centerOfMass.position - rb.worldCenterOfMass;
    }

    void SetInputDirection()
    {
        inputDirection = Vector3.zero;
        inputDirection += m_directionSource.right * Input.GetAxis("Horizontal");
        inputDirection += Vector3.Cross(m_directionSource.right, Vector3.up) * Input.GetAxis("Vertical");
    }

    void Update()
    {
        SetInputDirection();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpNextFrame = jumpAllowanceTrigger.IsJumpAllowed;
        }
    }

    private void FixedUpdate()
    {
        rb.AddForceAtPosition(forceMultiplier * inputDirection, forceApplicationPos.position);
        if (jumpNextFrame && jumpAllowanceTrigger.IsJumpAllowed)
        {
            Vector3 jumpDir = rb.transform.up;
            jumpDir.y /= jumpLeaningFactor;
            jumpDir.Normalize();
            rb.AddForce(jumpDir * jumpForce);
            jumpNextFrame = false;
        }
    }
}
