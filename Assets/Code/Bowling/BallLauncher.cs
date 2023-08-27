using Code.DataContainers;
using Code.Progression;
using UnityEngine;

namespace Code.Bowling
{
    public class BallLauncher : MonoBehaviour
    {
        [SerializeField] private Transform ballsContainer;
        [SerializeField] private Rigidbody ballPrefab;
        [SerializeField] private float laneHalfWidth;
        [SerializeField] private float throwForceMultiplier;
        
        private readonly Vector3 centerTargetPoint = new(7, 0.5f, 0);

        public void Launch(Round.BallLaunch launch)
        {
            Vector3 startPosition = transform.position + Vector3.right * (launch.spawnPositionHorizontal * laneHalfWidth);
            var ballRb = Instantiate(ballPrefab, startPosition, Quaternion.identity, ballsContainer);
            var bottle = GameObjectsContainer.Instance.mainPlayableBottle;

            var targetPosition = bottle.transform.position;
            if (!launch.shouldTargetBottle)
            {
                targetPosition = centerTargetPoint + Vector3.right * (launch.shootingAngleHorizontal * laneHalfWidth);
            }
            Vector3 force = targetPosition - startPosition;
            force *= throwForceMultiplier;
            force *= launch.ballSpeed;
        
            ballRb.AddForce(force, ForceMode.Impulse);
            GameObjectsContainer.Instance.spotlightFollow.AppendTarget(ballRb.transform);
        }
    }
}