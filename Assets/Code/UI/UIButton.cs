using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Code.Menu
{
    public class UIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public GameObject defaultImage;
        public GameObject pressedImage;

        private bool isHovered = false;
        private bool pressedState = false;

        private void Start()
        {
            isHovered = false;
            pressedState = false;
            defaultImage.SetActive(true);
            pressedImage.SetActive(false);
        }
        private void OnDisable()
        {
            isHovered = false;
            pressedState = false;
            defaultImage.SetActive(true);
            pressedImage.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isHovered = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isHovered = false;
            if (pressedState)
            {
                pressedState = false;
                defaultImage.SetActive(true);
                pressedImage.SetActive(false);
            }
        }

        private void Update()
        {
            if(isHovered && !pressedState && InputManager.Instance.PrimaryActionActivated() )
            {
                defaultImage.SetActive(false);
                pressedImage.SetActive(true);
                pressedState = true;
            }

            if(pressedState && InputManager.Instance.PrimaryActionReleased())
            {
                defaultImage.SetActive(true);
                pressedImage.SetActive(false);
            }
        }
    }
}