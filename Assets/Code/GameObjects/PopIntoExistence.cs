using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Code.GameObjects
{
    public class PopIntoExistence : MonoBehaviour
    {
        [SerializeField] private AnimationCurve scaleCurve;
        [SerializeField] private Transform transformToManipulate;
        [SerializeField] private Vector3 initialScale;
        [SerializeField] private Vector3 desiredScale;

        public void ScalePopIntoExistence(float duration)
        {
            desiredScale = transformToManipulate.localScale;
            transformToManipulate.localScale = initialScale;
            transformToManipulate.DOScale(desiredScale, duration).SetEase(scaleCurve);
        }
    }
}