using System;
using UnityEngine;
using UnityEngine.UI;

public class Bottle : MonoBehaviour
{
    public PinDragMovementControl dragControl;
    public Rigidbody rb;
    public LayerMask invisibleWallMask;
    public Renderer rend;
    
    public Image liquidLevelImage;
    
    public float liquidLevel;
    private static readonly int FillShaderPropertyID = Shader.PropertyToID("_Fill");

    private void FixedUpdate()
    {
        if (!GameManager.Instance.gameData.gameStarted || GameManager.Instance.gameData.isSandboxMode)
        {
            return;
        }
        float angle = Vector3.Angle(transform.up, Vector3.up);
        if (angle < GameManager.Instance.currentRound.spillThreshold)
        {
            return;
        }

        if (liquidLevel > 0)
        {
            // 89 instead of 90 to eliminate edge case when cos gives negative number so liquid level will rise
            float diffToEdge = angle > 89 ? -1 : Mathf.Cos(Mathf.Deg2Rad * angle) - liquidLevel;
            if (diffToEdge < 0)
            {
                float spill = Time.fixedDeltaTime * GameManager.Instance.currentRound.spillSpeed * diffToEdge;
                liquidLevel += spill;
                if (liquidLevel < 0)
                {
                    liquidLevel = 0;
                    GameManager.Instance.LiquidRanOut();
                }
                liquidLevelImage.fillAmount = liquidLevel;
            }
        }

        GameManager.Instance.gameData.maxBottleAngle = angle > GameManager.Instance.gameData.maxBottleAngle
            ? angle
            : GameManager.Instance.gameData.maxBottleAngle;
        if (angle > GameManager.Instance.currentRound.spillMaxThreshold && !GameManager.Instance.gameData.hasBottleFell)
        {
            GameManager.Instance.BottleFell();
        }
        if (transform.position.sqrMagnitude > 900)
        {
            GameManager.Instance.BottleCleaned();
        }
    }

    private void Update()
    {
        rend.material.SetFloat(FillShaderPropertyID, liquidLevel);
    }

    private void OnCollisionEnter(Collision other)
    {
        if ((1 << other.gameObject.layer & invisibleWallMask) != 0)
        {
            GameManager.Instance.BottleTouchedAWall();
        }
    }
}