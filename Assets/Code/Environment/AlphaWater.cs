using UnityEngine;
using UnityEngine.Serialization;
using static UnityEngine.ParticleSystem;

namespace Code.Environment
{
    public class AlphaWater : MonoBehaviour
    {
        [FormerlySerializedAs("jopa")] [SerializeField] private ParticleSystem ps;
        [FormerlySerializedAs("AlphaCurve")] [SerializeField] private AnimationCurve alphaCurve;
        
        private Particle[] particles;
        private ParticleSystemRenderer pr;
        
        private static readonly int AlphaShaderPropertyID = Shader.PropertyToID("_AlphaControlTest");
        
        void Start()
        {
            pr = ps.GetComponent<ParticleSystemRenderer>();
            particles = new Particle[10];
        }

        void Update()
        {
            ps.GetParticles(particles);
            float alpha = alphaCurve.Evaluate(1 - particles[0].remainingLifetime / particles[0].startLifetime); 
            pr.material.SetFloat(AlphaShaderPropertyID, alpha);
        }
    }
}
