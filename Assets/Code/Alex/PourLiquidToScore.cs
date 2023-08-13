using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PourLiquidToScore : MonoBehaviour
{
    [SerializeField] private Renderer source;
    [SerializeField] private Renderer destination;
    [SerializeField] private ParticleSystem waterFlow;
    [SerializeField] private Animator anim;
    
    [SerializeField] private Image liquidLevelImage;

    [SerializeField] private float particleFadeoutTime = 1.0f;
    [SerializeField] private float deltaPour = 0.5f;
    [SerializeField] private float totalPourTime = 3f;
    private static readonly int FillShaderProperty = Shader.PropertyToID("_Fill");

    [SerializeField] private float sourceFill;
    [SerializeField] private float destinationFill;

    public Transform waterFlowRoot;
    public ParticleSystem ParticlePrefab;

    public void Setup(float score)
    {
        float topWater = score / 90;
        sourceFill = topWater;
        destinationFill = 0;
        source.material.SetFloat(FillShaderProperty, topWater);
        destination.material.SetFloat(FillShaderProperty, 0);
    }

    public void ShowScore(float score)
    {
        waterFlow = Instantiate(ParticlePrefab, waterFlowRoot);
        waterFlow.transform.position = waterFlowRoot.position;
        waterFlow.transform.rotation = waterFlowRoot.rotation;
        StartCoroutine(PouringCinematic(score));
    }
    
    public IEnumerator PouringCinematic(float score)
    {
        float topWater = score / 90;
        
        anim.Play("BottlePouring");
        
        // magic one to prevent lagging pouring during bottle raising 
        yield return new WaitForSeconds(1);
        
        waterFlow.Play();
        for (float waterPoured = 0; waterPoured < topWater; waterPoured += deltaPour)
        {
            sourceFill -= deltaPour;
            destinationFill += deltaPour;
            source.material.SetFloat(FillShaderProperty, sourceFill);
            destination.material.SetFloat(FillShaderProperty, destinationFill);
            liquidLevelImage.fillAmount = sourceFill;
            yield return new WaitForSeconds(deltaPour * totalPourTime);
        }
        
        var waterFlowEmission = waterFlow.emission;
        waterFlowEmission.enabled = false;
        yield return new WaitForSeconds(particleFadeoutTime);
        waterFlow.Stop();
        Destroy(waterFlow.gameObject);
        
        anim.Play("BottleBack");
        yield return new WaitForSeconds(2);
        OldGameManager.Instance.menuController.OpenScoresPanel(score);
        OldGameManager.Instance.menuController.ScoreToGameplay();
    }
}