using System.Collections;
using Code.DataContainers;
using Code.GameObjects;
using UnityEngine;

namespace Code.States
{
    [CreateAssetMenu(fileName = "ResetPins", menuName = "SO/GameStates/ResetPins", order = 0)]
    public class ResetPins : BaseState
    {
        [SerializeField] private Pin pinPrefab;

        private int objectsFallen;
        private int objectsTotal;

        public override void OnEnter()
        {
            var resetBottle = ProgressionContainer.Instance.runtimeBottleRoundStartOptions.resetBottle;
            if (!resetBottle
                && !ProgressionContainer.Instance.CurrentRound.resetPins)
            {
                objectsTotal = -1;
                GameManager.Instance.InvokeEvent(EventId.PinsAndBottleSet);
                return;
            }
            foreach (Transform pin in GameObjectsContainer.Instance.pinsParent)
            {
                Destroy(pin.gameObject);
            }
            if (resetBottle)
            {
                GameObjectsContainer.Instance.mainPlayableBottle.gameObject.SetActive(false);
            }
            objectsFallen = 0;
            GameManager.Instance.StartCoroutine(PlacePinsCoroutine());
            base.OnEnter();
        }

        public override void OnExit()
        {
            base.OnExit();
            ProgressionContainer.Instance.runtimeBottleRoundStartOptions.resetBottle = false;
            ProgressionContainer.Instance.runtimeBottleRoundStartOptions.refillBottle = false;
        }

        public override void InvokeEvent(EventId eventId)
        {
            if (eventId == EventId.StartingPinOrBottleHitGround)
            {
                objectsFallen += 1;
                // Debug.Log($"Fell {objectsFallen}/{objectsTotal}");
            }
            if (objectsFallen == objectsTotal)
            {
                objectsTotal = -1;
                GameManager.Instance.InvokeEvent(EventId.PinsAndBottleSet);
            }
            base.InvokeEvent(eventId);
        }

        public IEnumerator PlacePinsCoroutine()
        {
            var level = ProgressionContainer.Instance.CurrentLevel;
            var round = ProgressionContainer.Instance.CurrentRound;
            var goc = GameObjectsContainer.Instance;
            var resetBottle = ProgressionContainer.Instance.runtimeBottleRoundStartOptions.resetBottle || goc.mainPlayableBottle.IsBroken;
            yield return new WaitForSeconds(1f);
            objectsTotal = round.resetPins ? level.pinPositions.Count : 1;
            objectsTotal -= resetBottle ? 0 : 1;
            foreach (int pinPosition in level.pinPositions)
            {
                yield return new WaitForSeconds(0.2f);
                Vector3 fallOffset = Vector3.up * (8 + Random.value);
                PopIntoExistence pie;
                if (pinPosition == level.bottlePosition)
                {
                    if (!resetBottle)
                    {
                        continue;
                    }
                    goc.mainPlayableBottle.ResetBottle();
                    Vector3 startPosition = goc.startingPinPositions[pinPosition].position + fallOffset;
                    goc.mainPlayableBottle.rb.velocity = Vector3.zero;
                    goc.mainPlayableBottle.rb.angularVelocity = Vector3.zero;
                    var bottleTransform = goc.mainPlayableBottle.transform;
                    goc.mainPlayableBottle.rb.isKinematic = true;
                    bottleTransform.SetPositionAndRotation(startPosition, Quaternion.identity);
                    goc.mainPlayableBottle.rb.isKinematic = false;
                    GameObjectsContainer.Instance.mainPlayableBottle.gameObject.SetActive(true);
                    goc.mainPlayableBottle.fallThenNotify.enabled = true;
                    if (ProgressionContainer.Instance.runtimeBottleRoundStartOptions.refillBottle)
                    {
                        goc.mainPlayableBottle.spillLiquid.LiquidLevel = 1;
                    }
                    pie = goc.mainPlayableBottle.popIntoExistence;
                    if (pie)
                    {
                        pie.ScalePopIntoExistence(0.2f);
                        CinematicObjectsContainer.Instance.poppingSoundSource.pitch = Random.Range(0.5f, 1.5f);
                        CinematicObjectsContainer.Instance.poppingSoundSource.Play();
                    }
                    continue;
                }

                var pin = Instantiate(pinPrefab, goc.startingPinPositions[pinPosition].position + fallOffset, Quaternion.identity, goc.pinsParent);
                pie = pin.popIntoExistence;
                if (pie)
                {
                    pie.ScalePopIntoExistence(0.2f);
                    CinematicObjectsContainer.Instance.poppingSoundSource.Play();
                    CinematicObjectsContainer.Instance.poppingSoundSource.pitch = Random.Range(0.5f, 1.5f);
                }
            }
        }
    }
}