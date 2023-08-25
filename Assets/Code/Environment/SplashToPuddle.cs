using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SplashToPuddle : MonoBehaviour
{
    public ParticleSystem splashParticleSystem;
    public ParticleSystem bottleBreakParticleSystem;
    public ParticleSystem puddleParticleSystem;

    private ParticleSystem currentPuddle;

    public List<ParticleCollisionEvent> collisionEvents;
    public Vector3 spawnOffset;
    void Start()
    {
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other == splashParticleSystem.gameObject || other == bottleBreakParticleSystem.gameObject )
        {
            bool isBottleBreak = other == bottleBreakParticleSystem.gameObject;
            (isBottleBreak? bottleBreakParticleSystem : splashParticleSystem).GetCollisionEvents(gameObject, collisionEvents);

            foreach (var e in collisionEvents)
            {
                currentPuddle = Instantiate(puddleParticleSystem, e.intersection + spawnOffset, Quaternion.identity);
                currentPuddle.Play();

                Destroy(currentPuddle.gameObject, 2);
            }
        }
    }
}