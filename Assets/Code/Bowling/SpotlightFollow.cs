using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Bowling
{
    public class SpotlightFollow : MonoBehaviour
    {
        [SerializeField] private Transform spotlightParent;
        [SerializeField] private float lerpT;

        private List<Transform> targets;
        private Vector3 targetPosition;

        private void Start()
        {
            targetPosition = Vector3.zero;
            ClearTargets();
        }

        public void ClearTargets()
        {
            targets = new List<Transform>();
        }

        public void AppendTarget(Transform newTarget)
        {
            targets.Add(newTarget);
        }

        private void CalculateTargetPosition()
        {
            if (targets.Count > 0)
            {
                targetPosition = Vector3.zero;
                foreach(Transform target in targets)
                {
                    targetPosition += target.position;
                }
                targetPosition /= targets.Count;
            }
        }

        private void FixedUpdate()
        {
            CalculateTargetPosition();
        }

        void Update()
        {
            if (targets.Count > 0)
            {
                Quaternion rotation = Quaternion.LookRotation(targetPosition - spotlightParent.position , Vector3.up);
                spotlightParent.rotation = Quaternion.Lerp(spotlightParent.rotation, rotation, lerpT);
            }
        }
    }
}