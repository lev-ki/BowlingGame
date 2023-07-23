using System;
using UnityEngine;

public class FreezeRotationInSetupFall : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private LayerMask floorBottleMask;
    
    private void Start()
    {
        rb.constraints = RigidbodyConstraints.FreezeAll ^ RigidbodyConstraints.FreezePositionY;
    }

    private void OnCollisionEnter(Collision other)
    {
        if ((1 << other.gameObject.layer & floorBottleMask) != 0)
        {
            rb.constraints = RigidbodyConstraints.None;
            Destroy(this);
        }
    }
}