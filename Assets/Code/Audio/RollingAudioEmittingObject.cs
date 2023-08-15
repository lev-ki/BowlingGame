using UnityEngine;

namespace Code.Audio
{
    public class RollingAudioEmittingObject : MonoBehaviour
    {
        [SerializeField] private Rigidbody m_rigidbody;

        public AudioSource source;
        public AudioClip clip;

        [SerializeField] private float m_magicNumber;

        public float lerpT = 0.5f;
        public float muteLerpT = 0.5f;
        private int collisions = 0;

        private void FixedUpdate()
        {
            if (collisions == 0)
            {
                source.volume = Mathf.Lerp(source.volume, 0, muteLerpT);
            }
            else
            {
                Vector3 horizontalVel = m_rigidbody.velocity;
                horizontalVel.y = 0;
                source.volume = Mathf.Lerp(source.volume, horizontalVel.magnitude / m_magicNumber, lerpT);
            }

        }

        private void OnCollisionEnter(Collision collision)
        {
            collisions++;
        }

        private void OnCollisionExit(Collision collision)
        {
            collisions--;
        }
    }
}
