using Code.DataContainers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Bowling
{
    public class BallLauncher : MonoBehaviour
    {
        [SerializeField] private Transform ballsContainer;
        [SerializeField] private Rigidbody ballPrefab;
        [SerializeField] private float maxRandomStartDistance;
        [SerializeField] private float maxRandomForce;
        [SerializeField] private float highThrowMultiplier;
        [SerializeField] private float throwForceMultiplier;

        public void Launch()
        {
            Vector3 startPosition = transform.position + Random.insideUnitSphere * maxRandomStartDistance;
            var ballRb = Instantiate(ballPrefab, startPosition, Quaternion.identity, ballsContainer);
            var bottle = GameObjectsContainer.Instance.mainPlayableBottle;
        
            Vector3 force = bottle.transform.position - startPosition;
            force *= throwForceMultiplier;
            force *= ProgressionContainer.Instance.CurrentRound.ballSpeedMultiplier;
            force += Vector3.up * highThrowMultiplier;
            force += Random.insideUnitSphere * maxRandomForce;
        
            ballRb.AddForce(force, ForceMode.Impulse);
            GameObjectsContainer.Instance.spotlightFollow.AppendTarget(ballRb.transform);
        }
    }
}