using Code.DataContainers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.Bowling
{
    public class SpillLiquid : MonoBehaviour
    {
        public Renderer waterRenderer;
        [SerializeField] private AnimationCurve liquidLevelToSpillAngle;
        [SerializeField] private AnimationCurve liquidDiffToParticleNumber;
        [SerializeField] private ParticleSystem liquidParticleSystem;
        [SerializeField] private ParticleSystem breakBottleParticleSystem;

        private float spillAngle;
        private int emitOverTime;
        private bool bottleBroken = false;

        private float liquidLevel;
        public float LiquidLevel
        {
            get => liquidLevel;
            set
            {
                UIContainer.Instance.liquidLevelImage.SetLevel(value);
                waterRenderer.material.SetFloat(FillShaderPropertyID, value);
                if (value < liquidLevel)
                {
                    if(bottleBroken)
                    {
                        breakBottleParticleSystem.Emit(Mathf.CeilToInt(liquidDiffToParticleNumber.Evaluate(liquidLevel - value)));
                        bottleBroken = false;
                    }
                    else
                    {
                        liquidParticleSystem.Emit(Mathf.CeilToInt(liquidDiffToParticleNumber.Evaluate(liquidLevel - value)));
                    }
                }
                liquidLevel = value;
            }
        }
        private static readonly int FillShaderPropertyID = Shader.PropertyToID("_Fill");

        public void BreakBottle()
        {
            bottleBroken = true;
            LiquidLevel = 0;
        }

        private void FixedUpdate()
        {
            float angle = Vector3.Angle(transform.up, Vector3.up);
            UIContainer.Instance.liquidLevelImage.RotateBottleIndicator(angle);
            if (angle < ProgressionContainer.Instance.CurrentRound.spillThreshold)
            {
                return;
            }

            if (LiquidLevel > 0)
            {
                spillAngle = liquidLevelToSpillAngle.Evaluate(LiquidLevel) * 180;
                if (angle > spillAngle)
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
        }

        // private void Update()
        // {
            // TODO(Alex): check if we do need it here as it's already present in LiquidLevel setter
            // waterRenderer.material.SetFloat(FillShaderPropertyID, LiquidLevel);
        // }
    }
}