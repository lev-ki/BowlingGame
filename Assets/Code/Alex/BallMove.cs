using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class BallMove : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;

    [SerializeField] private float rideBackTimer;

    private float destroyTimer = 30;

    private void FixedUpdate()
    {
        destroyTimer -= Time.fixedDeltaTime;
        if (destroyTimer < 0)
        {
            Destroy(gameObject);
            GameManager.Instance.BallCleaned();
        }
        rideBackTimer -= Time.fixedDeltaTime;
        if (rb.velocity.z > -0.1f || rideBackTimer < 0)
        {
            rb.AddForce(Vector3.back * 5);
        }
    }
}