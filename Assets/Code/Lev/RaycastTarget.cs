using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastTarget : MonoBehaviour
{
    [System.Serializable]
    public class HighligthTarget
    {
        public List<Material> defaultMaterials;
        public List<Material> highlightedMaterials;

        public MeshRenderer mesh;
    }

    public List<HighligthTarget> HighligthTargets;
    public MenuController.OptionType OptionType;
    public GameObject LightSource;
    public AudioSource NeonAudio;
    private Coroutine lightsCoroutine;

    public void Toggle(bool isHighlighted)
    {
        NeonAudio.Play();
        
        foreach (HighligthTarget target in HighligthTargets)
        {
            target.mesh.SetMaterials(isHighlighted ? target.highlightedMaterials : target.defaultMaterials);
            LightSource.SetActive(isHighlighted);
        }
        
    }

    private IEnumerator Lights(bool isHighlighted)
    {
        float timer = 0;
        bool toggle = false;
        while(timer < 0.2f)
        {
            foreach (HighligthTarget target in HighligthTargets)
            {
                target.mesh.SetMaterials(toggle ? target.highlightedMaterials : target.defaultMaterials);
                LightSource.SetActive(toggle);
            }
            toggle = !toggle;

            float time = Random.Range(0.02f, 0.07f);
            yield return new WaitForSeconds(time);
            timer += time;
        }
        foreach (HighligthTarget target in HighligthTargets)
        {
            target.mesh.SetMaterials(isHighlighted ? target.highlightedMaterials : target.defaultMaterials);
            LightSource.SetActive(isHighlighted);
        }
    }
}
