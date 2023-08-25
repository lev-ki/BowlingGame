using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class LiquidLevelImage : MonoBehaviour
    {
        public Image liquidLevelImage;
        public GameObject container;
        [SerializeField] private RectTransform liquidIndicatorParent;
        [SerializeField] private RectTransform liquidIndicatorMask;
        [SerializeField] private RectTransform liquidIndicator;
        [SerializeField] private AnimationCurve minFromAngle;
        [SerializeField] private AnimationCurve maxFromAngle;
        [SerializeField] private float lerpT;

        private float zero = 0;
        private float max = 1;
        private float realValue;
        private float adjustedValue;
        private float targetValue;

        public void RotateBottleIndicator(float newAngle)
        {
            Vector3 currentRotation = liquidIndicatorParent.localEulerAngles;

            zero = minFromAngle.Evaluate(newAngle);
            max = maxFromAngle.Evaluate(newAngle);

            currentRotation.z = newAngle;
            liquidIndicatorParent.localEulerAngles = currentRotation;
            liquidIndicator.localEulerAngles = currentRotation;
            currentRotation.z = -newAngle;
            liquidIndicatorMask.localEulerAngles = currentRotation;

        }

        public void SetLevel(float value)
        {
            realValue = value;
            adjustedValue = Mathf.Lerp(zero, max, value);
        }

        private void Update()
        {
            liquidLevelImage.fillAmount = Mathf.Lerp(liquidLevelImage.fillAmount, adjustedValue, lerpT * Time.deltaTime);
        }

        public void Show()
        {
            container.SetActive(true);
        }
        
        public void Hide()
        {
            container.SetActive(false);
        }
    }
}