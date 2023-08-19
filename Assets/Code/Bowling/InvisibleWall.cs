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
        
        private bool isInWall;
        private float timer;
        public float Timer
        {
            get => timer;
            set
            {
                timer = value;
                UIContainer.Instance.returnTime.text = timer.ToString();
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
                UIContainer.Instance.returnToLane.SetActive(ShownQueue.Count > 0);
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
            }
        }

        private bool IsBottle(Collider other)
        {
            return (1 << other.gameObject.layer & bottleLayerMask) != 0;
        }
    }
}