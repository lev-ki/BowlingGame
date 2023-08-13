using System.Collections;
using System.Collections.Generic;
using Code.GameObjects;
using UnityEngine;

public class BallzInMaFace : MonoBehaviour
{
    public List<GameObject> Variants;

    private void Start()
    {
        var selected = Variants[Random.Range(0, Variants.Count - 1)];
        selected.SetActive(true);
    }
}
