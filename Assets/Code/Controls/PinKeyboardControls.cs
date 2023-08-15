using TMPro;
using UnityEngine;

namespace Code.Controls
{
    public class PinKeyboardControls : MonoBehaviour
    {
        [SerializeField] private Rigidbody rb;
        [SerializeField] private Transform forceApplicationPos;
        [SerializeField] private Transform directionSource;

        [SerializeField] private JumpAllowanceTrigger jumpAllowanceTrigger;

        [SerializeField] private float forceMultiplier;

        [SerializeField] private float jumpForce;
        [SerializeField] private float jumpLeaningFactor = 3;
        private Vector3 inputDirection;
        private bool jumpNextFrame;

        #region debugFunctions
#if UNITY_EDITOR
        [SerializeField] private TextMeshProUGUI forceValue;
        public void SetMoveForce(float alpha)
        {
            float min = 1;
            float max = 10;
            forceMultiplier = Mathf.Lerp(min, max, alpha);

            forceValue.text = forceMultiplier.ToString();
        }

        [SerializeField] private TextMeshProUGUI jumpForceValue;
        public void SetJumpForce(float alpha)
        {
            float min = 100;
            float max = 500;
            jumpForce = Mathf.Lerp(min, max, alpha);

            jumpForceValue.text = jumpForce.ToString();
        }

        [SerializeField] private TextMeshProUGUI jumpLeaningValue;
        public void SetJumpLeaning(float alpha)
        {
            float min = 1;
            float max = 5;
            jumpLeaningFactor = Mathf.Lerp(min, max, alpha);

            jumpLeaningValue.text = jumpLeaningFactor.ToString();
        }

        [SerializeField] private TextMeshProUGUI comValue;
        public void SetCenterOfMass(float alpha)
        {
            float min = 0;
            float max = 2.5f;
            Vector3 com = Vector3.zero;
            com.y = Mathf.Lerp(min, max, alpha);

            rb.centerOfMass = com;

            comValue.text = com.y.ToString();
        }
#endif
        #endregion

        void SetInputDirection()
        {
            inputDirection = Vector3.zero;
            inputDirection += directionSource.right * InputManager.Instance.GetHorizontalAxis();
            inputDirection += Vector3.Cross(directionSource.right, Vector3.up) * InputManager.Instance.GetVerticalAxis();
        }

        void Update()
        {
            SetInputDirection();
            if (InputManager.Instance.SecondaryActionActivated())
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
}
