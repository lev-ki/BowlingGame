using UnityEngine;

public class Reset : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    
    [SerializeField] private KeyCode resetKey;
    public bool useKey;
    
    private Vector3 startPosition;
    private Quaternion startRotation;
    
    private void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
    }
    
    public void SetDefaults(Vector3 position, Quaternion rotation)
    {
        startPosition = position;
        startRotation = rotation;
    }

    private void Update()
    {
        if (Input.GetKeyDown(resetKey) && useKey)
        {
            ResetMe();
        }
    }

    private void ResetMe()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}