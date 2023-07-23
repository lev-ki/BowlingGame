using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioEmittingObject : MonoBehaviour
{
    [SerializeField] private Rigidbody m_rigidbody;
    [System.Serializable]
    public class SoundSource
    {
        public AudioSource source;
        public List<AudioClip> clips;

        public void PlayRandom(float volume)
        {
            source.clip = clips[Random.Range(0, clips.Capacity - 1)];
            source.volume = volume;
            source.Play();
        }
    }

    [SerializeField] private SoundSource m_impactSource;
    [SerializeField] private SoundSource m_secondaryImpactSource;

    [SerializeField] private float m_impulseThreshold;
    [SerializeField] private float m_angularVelocityThreshold;
    [SerializeField] private float m_magicNumber;
    [SerializeField] private float m_magicNumberElectricBoogaloo;

    private void FixedUpdate()
    {
    }

    private void PlayCollision(float volume)
    {
        if (!m_impactSource.source.isPlaying)
        {
            m_impactSource.PlayRandom(Mathf.Min(volume, 1));
        }
        else if (!m_secondaryImpactSource.source.isPlaying)
        {
            m_secondaryImpactSource.PlayRandom(Mathf.Min(volume, 1));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (m_rigidbody.angularVelocity.magnitude > m_angularVelocityThreshold)
        {
            PlayCollision(m_rigidbody.angularVelocity.magnitude / m_magicNumber);
        }
        if (collision.impulse.magnitude > m_impulseThreshold)
        {
            PlayCollision(collision.impulse.magnitude / m_magicNumberElectricBoogaloo);
        }
    }
}
