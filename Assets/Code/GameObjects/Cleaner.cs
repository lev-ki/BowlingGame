using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Code.GameObjects
{
    public class Cleaner : MonoBehaviour
    {
        [SerializeField] private Transform initialPosition;
        [SerializeField] private Transform extendedPosition;
        [SerializeField] private Transform retractedPosition;

        [SerializeField] private AnimationCurve stepOneCurve;
        [SerializeField] private AnimationCurve stepTwoCurve;
        [SerializeField] private AnimationCurve stepThreeCurve;
        [SerializeField] private AnimationCurve stepFourMoveCurve;
        [SerializeField] private AnimationCurve stepFourRotateCurve;

        [SerializeField] private float stepOneDuration;
        [SerializeField] private float stepTwoDuration;
        [SerializeField] private float stepThreeDuration;
        [SerializeField] private float stepFourDuration;

        [SerializeField] private float pauseDuration;

        public float GetTotalDuration()
        {
            return stepOneDuration + stepTwoDuration + stepThreeDuration + stepFourDuration + pauseDuration * 3;
        }

        public void StartCleaning()
        {
            Sequence sequence = DOTween.Sequence();

            sequence.Append(transform.DOMove(extendedPosition.position, stepOneDuration).SetEase(stepOneCurve));
            sequence.AppendInterval(pauseDuration);
            sequence.Append(transform.DORotateQuaternion(extendedPosition.rotation, stepTwoDuration).SetEase(stepTwoCurve));
            sequence.AppendInterval(pauseDuration);
            sequence.Append(transform.DOMove(retractedPosition.position, stepThreeDuration).SetEase(stepThreeCurve));
            sequence.AppendInterval(pauseDuration);
            sequence.AppendCallback(() =>
            {
                transform.DOMove(initialPosition.position, stepFourDuration).SetEase(stepFourMoveCurve);
                transform.DORotateQuaternion(initialPosition.rotation, stepFourDuration).SetEase(stepFourRotateCurve);
            });
        }
    }
}