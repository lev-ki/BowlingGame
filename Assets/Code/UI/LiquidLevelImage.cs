using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class LiquidLevelImage : MonoBehaviour
    {
        public Image liquidLevelImage;
        public GameObject container;

        
        public void SetLevel(float value)
        {
            liquidLevelImage.fillAmount = value;
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