using System.Collections.Generic;
using UnityEngine;

namespace Code.Menu
{
    public class RaycastTarget : MonoBehaviour
    {
        [System.Serializable]
        public class HighligthTarget
        {
            public List<Material> defaultMaterials;
            public List<Material> highlightedMaterials;

            public MeshRenderer mesh;
        }

        public List<HighligthTarget> highlightTargets;
        public UIController.OptionType optionType;
        public GameObject lightSource;
        public AudioSource neonAudio;
        private Coroutine lightsCoroutine;

        public void Toggle(bool isHighlighted)
        {
            neonAudio.Play();
        
            foreach (HighligthTarget target in highlightTargets)
            {
                target.mesh.SetMaterials(isHighlighted ? target.highlightedMaterials : target.defaultMaterials);
                lightSource.SetActive(isHighlighted);
            }
        }
    }
}
