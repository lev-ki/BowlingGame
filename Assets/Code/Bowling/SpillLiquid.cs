using Code.DataContainers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.Bowling
{
    public class SpillLiquid : MonoBehaviour
    {
        public Renderer waterRenderer;
        
        private float liquidLevel;
        public float LiquidLevel
        {
            get => liquidLevel;
            set
            {
                UIContainer.Instance.liquidLevelImage.SetLevel(value);
                waterRenderer.material.SetFloat(FillShaderPropertyID, liquidLevel);
                liquidLevel = value;
            }
        }
        private static readonly int FillShaderPropertyID = Shader.PropertyToID("_Fill");

        private void FixedUpdate()
        {
            float angle = Vector3.Angle(transform.up, Vector3.up);
            if (angle < ProgressionContainer.Instance.CurrentRound.spillThreshold)
            {
                return;
            }

            if (LiquidLevel > 0)
            {
                // 89 instead of 90 to eliminate edge case when cos gives negative number so liquid level will rise
                float diffToEdge = angle > 89 ? -1 : Mathf.Cos(Mathf.Deg2Rad * angle) - LiquidLevel;
                if (diffToEdge < 0)
                {
                    float spill = Time.fixedDeltaTime * ProgressionContainer.Instance.CurrentRound.spillSpeed * diffToEdge;
                    LiquidLevel += spill;
                    if (LiquidLevel < 0)
                    {
                        LiquidLevel = 0;
                        GameManager.Instance.InvokeEvent(EventId.BottleDrained);
                    }
                }
            }
        }

        // private void Update()
        // {
            // TODO(Alex): check if we do need it here as it's already present in LiquidLevel setter
            // waterRenderer.material.SetFloat(FillShaderPropertyID, LiquidLevel);
        // }
    }
}