using System.Collections.Generic;
using Code.DataContainers;
using UnityEngine;

namespace Code.Bowling
{
    public class BottleBreakController : MonoBehaviour
    {
        [SerializeField] private GameObject intactBottle;
        [SerializeField] private float bottleBreakPiecesScatterForce = 5f;
        [SerializeField] private List<GameObject> brokenBottlePrefabs;
        [SerializeField] private AudioSource bottleBreakSound;

        private GameObject brokenBottleInstance;

        public bool IsBroken { get; private set; }
        
        
        public void DestroyPieces(float delay)
        {
            if (brokenBottleInstance)
            {
                Destroy(brokenBottleInstance, delay);
                brokenBottleInstance = null;
            }
        }

        [ContextMenu("Reset Bottle")]
        public void ResetBottle()
        {
            GameObjectsContainer.Instance.mainPlayableBottle.rb.isKinematic = false;
            DestroyPieces(0);
            intactBottle.SetActive(true);
            IsBroken = false;
        }

        public void OnBreakBottle(Vector3 breakImpulse)
        {
            if (IsBroken)
            {
                return;
            }
            bottleBreakSound.Play();
            GameObjectsContainer.Instance.mainPlayableBottle.rb.isKinematic = true;
            Destroy(brokenBottleInstance);
            var cachedTransform = transform;
            GameObject brokenBottlePrefab = brokenBottlePrefabs[Random.Range(0, brokenBottlePrefabs.Count)];
            brokenBottleInstance = Instantiate(brokenBottlePrefab, cachedTransform.position, cachedTransform.rotation, cachedTransform.parent);
            for (int i = 0; i < brokenBottleInstance.transform.childCount; i++)
            {
                Transform child = brokenBottleInstance.transform.GetChild(i);
                Rigidbody childRb = child.GetComponent<Rigidbody>();
                if (childRb)
                {
                    childRb.AddForce((Random.insideUnitSphere + Vector3.up * Random.Range(0, 1)) * bottleBreakPiecesScatterForce);
                    childRb.AddForce(-breakImpulse * 0.1f, ForceMode.Impulse);
                }
            }
            intactBottle.SetActive(false);
            IsBroken = true;
            GameObjectsContainer.Instance.mainPlayableBottle.spillLiquid.BreakBottle();
            GameManager.Instance.InvokeEvent(EventId.BottleBroken);
        }
    }
}