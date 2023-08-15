using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionImpulseDetector : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private float impulseThreshold;
    [SerializeField] private float angularVelocityThreshold;

    [SerializeField] private UnityEvent<Vector3> onThresholdPassedEvent;
    [SerializeField] private LayerMask ballMask;
    [SerializeField] private float ballMultiplier = 2f;

    private void OnCollisionEnter(Collision collision)
    {
        float multiplier = 1;
        bool isBall = (1 << collision.gameObject.layer & ballMask) != 0;
        if(isBall)
        {
            multiplier *= ballMultiplier;
        }
        if (rigidbody.angularVelocity.magnitude * multiplier > angularVelocityThreshold)
        {
            onThresholdPassedEvent.Invoke(collision.impulse);
        }
        if (collision.impulse.magnitude * multiplier > impulseThreshold)
        {
            onThresholdPassedEvent.Invoke(collision.impulse);
        }
    }
}
