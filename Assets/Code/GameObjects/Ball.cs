using Code.Bowling;
using Code.DataContainers;
using UnityEngine;

namespace Code.GameObjects
{
    public class Ball : MonoBehaviour
    {
        private void OnEnable()
        {
            GameObjectsContainer.Instance.balls.Add(this);
        }

        private void OnDisable()
        {
            GameObjectsContainer.Instance.balls.Remove(this);
        }
    }
}