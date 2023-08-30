using System;
using System.Collections;
using System.Collections.Generic;
using Code.DataContainers;
using UnityEngine;

namespace Code.Bowling
{
    public class InvisibleWall : MonoBehaviour
    {
        [SerializeField] LayerMask bottleLayerMask;

        [SerializeField] private float timeTillLose;
        
        private static readonly Queue<InvisibleWall> ShownQueue = new();

        public static bool TryShowTimer => ShownQueue.Count > 0;
        
        private bool isInWall;
        private float timer;
        public float Timer
        {
            get => timer;
            set
            {
                timer = value;
                UIContainer.Instance.returnTime.text = $"{(int)timer}";
            }
        }

        public bool IsInWall
        {
            get => isInWall;
            set
            {
                Timer = timeTillLose;
                isInWall = value;
                if (value)
                {
                    ShownQueue.Enqueue(this);
                }
                else if (ShownQueue.Count > 0)
                {
                    ShownQueue.Dequeue();
                }
            }
        }

        private void OnEnable()
        {
            timer = timeTillLose;
        }
        
        private void OnDisable()
        {
            IsInWall = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsBottle(other))
            {
                return;
            }
            IsInWall = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!IsBottle(other))
            {
                return;
            }
            IsInWall = false;
        }

        private void FixedUpdate()
        {
            if (!IsInWall)
            {
                return;
            }
            Timer -= Time.fixedDeltaTime;
            if (Timer < 0)
            {
                GameManager.Instance.InvokeEvent(EventId.BottleFell);
                Timer = timeTillLose;
                UIContainer.Instance.UIController.ShowRoundResult("Stay on the lane");
            }
        }

        private bool IsBottle(Collider other)
        {
            return (1 << other.gameObject.layer & bottleLayerMask) != 0;
        }
    }
}