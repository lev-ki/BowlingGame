using System;
using UnityEngine;

public class BallPinCollector : MonoBehaviour
{
    [SerializeField] LayerMask ballMask;
    [SerializeField] LayerMask pinMask;
    [SerializeField] LayerMask bottleMask;
    
    private float bottleCollectTriggerDelay;
    
    private void OnTriggerEnter(Collider other)
    {
        if ((1 << other.gameObject.layer & pinMask) != 0)
        {
            Destroy(other.gameObject);
        }
        if ((1 << other.gameObject.layer & ballMask) != 0)
        {
            Destroy(other.gameObject);
            GameManager.Instance.BallCleaned();
        }
        else if ((1 << other.gameObject.layer & bottleMask) != 0 && bottleCollectTriggerDelay < 0)
        {
            GameManager.Instance.BottleCleaned();
            bottleCollectTriggerDelay = 3;
        }
    }

    private void FixedUpdate()
    {
        bottleCollectTriggerDelay -= Time.fixedDeltaTime;
    }
}