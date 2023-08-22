using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SplashToPuddle : MonoBehaviour
{
    public ParticleSystem splashParticleSystem; // ������ �� Particle System ��� �����
    public ParticleSystem puddleParticleSystem; // ������ �� Particle System ��� ����

    private ParticleSystem currentPuddle; // ������ �� ������� �������� ������ ����

    public List<ParticleCollisionEvent> collisionEvents;
    public Vector3 spawnOffset;
    void Start()
    {
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    private void OnParticleCollision(GameObject other)
    {
        // ���������, ����������� �� � ��������� �����
        if (other == splashParticleSystem.gameObject)
        {
            splashParticleSystem.GetCollisionEvents(gameObject, collisionEvents);

            foreach (var e in collisionEvents)
            {


                // ������� ������ ���� �� ������� ������������
                currentPuddle = Instantiate(puddleParticleSystem, e.intersection + spawnOffset, Quaternion.identity);
                currentPuddle.Play();

                Destroy(currentPuddle.gameObject, 2);
            }
        }
    }
}