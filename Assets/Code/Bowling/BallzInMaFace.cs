using System.Collections.Generic;
using UnityEngine;

namespace Code.Bowling
{
    public class BallzInMaFace : MonoBehaviour
    {
        public List<GameObject> Variants;

        private void Start()
        {
            var selected = Variants[Random.Range(0, Variants.Count - 1)];
            selected.SetActive(true);
        }
    }
}
