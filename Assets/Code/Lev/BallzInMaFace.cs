using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallzInMaFace : MonoBehaviour
{
    public List<GameObject> Variants;

    private void Start()
    {
        Variants[Random.Range(0, Variants.Count - 1)].SetActive(true);
    }
}
