using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SplashToPuddle : MonoBehaviour
{
    public ParticleSystem splashParticleSystem; // Ссылка на Particle System для брызг
    public ParticleSystem puddleParticleSystem; // Ссылка на Particle System для лужи

    private ParticleSystem currentPuddle; // Ссылка на текущий активный эффект лужи

    public List<ParticleCollisionEvent> collisionEvents;
    public Vector3 spawnOffset;
    void Start()
    {
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    private void OnParticleCollision(GameObject other)
    {
        // Проверяем, столкнулись ли с партиклом брызг
        if (other == splashParticleSystem.gameObject)
        {
            splashParticleSystem.GetCollisionEvents(gameObject, collisionEvents);

            foreach (var e in collisionEvents)
            {


                // Создаем эффект лужи на позиции столкновения
                currentPuddle = Instantiate(puddleParticleSystem, e.intersection + spawnOffset, Quaternion.identity);
                currentPuddle.Play();

                Destroy(currentPuddle.gameObject, 2);
            }
        }
    }
}