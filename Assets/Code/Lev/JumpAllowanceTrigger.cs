using UnityEngine;

public class JumpAllowanceTrigger : MonoBehaviour
{
    [SerializeField] private LayerMask m_mask;
    [SerializeField] private float m_raduis;
    public bool IsJumpAllowed { get; private set; }

    private void Update()
    {
        IsJumpAllowed = Physics.CheckSphere(transform.position, m_raduis, m_mask);
    }
}