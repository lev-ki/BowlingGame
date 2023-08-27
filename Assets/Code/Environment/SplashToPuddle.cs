using System.Collections.Generic;
using UnityEngine;

public class SplashToPuddle : MonoBehaviour
{
    [SerializeField] private ParticleSystem splashParticleSystem;
    [SerializeField] private ParticleSystem bottleBreakParticleSystem;
    [SerializeField] private ParticleSystem puddleParticleSystem;
    [SerializeField] private Vector3 spawnOffset;

    private List<ParticleCollisionEvent> collisionEvents;
    
    void Start()
    {
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other != splashParticleSystem.gameObject && other != bottleBreakParticleSystem.gameObject)
        {
            return;
        }
        bool isBottleBreak = other == bottleBreakParticleSystem.gameObject;
        var affectedPs = (isBottleBreak ? bottleBreakParticleSystem : splashParticleSystem);
        affectedPs.GetCollisionEvents(gameObject, collisionEvents);

        foreach (var e in collisionEvents)
        {
            var currentPuddle = Instantiate(puddleParticleSystem, e.intersection + spawnOffset, Quaternion.identity);
            currentPuddle.Play();

            Destroy(currentPuddle.gameObject, 2);
        }
    }
}