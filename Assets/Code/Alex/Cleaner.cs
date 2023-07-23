using System;
using UnityEngine;

public class Cleaner : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Vector3 start;
    [SerializeField] private Vector3 finish;
    [SerializeField] private float totalTime;
    
    private float time;

    private void FixedUpdate()
    {
        time += Time.fixedDeltaTime;
        rb.MovePosition(Vector3.Lerp(start, finish, time / totalTime));
        if (time > totalTime)
        {
            Destroy(gameObject);
        }
    }
    
    public void Start()
    {
        transform.position = start;
        time = 0;
        GameManager.Instance.gameData.isCleanerPresent = true;
    }

    private void OnDestroy()
    {
        GameManager.Instance.gameData.isCleanerPresent = false;
    }
}