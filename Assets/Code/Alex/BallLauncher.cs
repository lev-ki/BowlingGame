using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BallLauncher : MonoBehaviour
{
    [SerializeField] private Transform ballsContainer;
    [SerializeField] private Rigidbody ballPrefab;
    [SerializeField] private float maxRandomStartDistance;
    [SerializeField] private float maxRandomForce;
    [SerializeField] private float highThrowMultiplier;
    [SerializeField] private float throwForceMultiplier;

    [SerializeField] private Transform bottle;
    
    public bool useKey;
    
    public void Launch()
    {
        Vector3 startPosition = transform.position + Random.insideUnitSphere * maxRandomStartDistance;
        var ballRb = Instantiate(ballPrefab, startPosition, Quaternion.identity, ballsContainer);
        
        Vector3 force = bottle.position - startPosition;
        force *= throwForceMultiplier;
        force *= GameManager.Instance.currentRound.ballSpeedMultiplier;
        force += Vector3.up * highThrowMultiplier;
        force += Random.insideUnitSphere * maxRandomForce;
        
        ballRb.AddForce(force, ForceMode.Impulse);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && useKey)
        {
            Launch();
        }
    }
}