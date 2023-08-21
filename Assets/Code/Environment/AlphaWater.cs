using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class AlphaWater : MonoBehaviour
{
    [SerializeField] private ParticleSystem jopa;
    [SerializeField] private AnimationCurve AlphaCurve;
    private Particle[] particles;
    private ParticleSystemRenderer pr;
    private static readonly int AlphaShaderPropertyID = Shader.PropertyToID("_AlphaControlTest");
    void Start()
    {
        pr = GetComponent<ParticleSystemRenderer>();
        particles = new Particle[10];
    }

    void Update()
    {
        jopa.GetParticles(particles);
        float alpha = AlphaCurve.Evaluate(1 - particles[0].remainingLifetime / particles[0].startLifetime); 
        pr.material.SetFloat(AlphaShaderPropertyID, alpha);
    }
}
