using UnityEngine;

namespace MobilePortSystem
{
    public class PortUI : MonoBehaviour
    {
        [SerializeField] private GameObject rootObject;

        [Header("Movement Controls")]
        [SerializeField] private GameObject movementButtons;
        [SerializeField] private GameObject movementStick;
        [SerializeField] private bool usesStick;

        [Header("Pause Behaviour")]
        [SerializeField] private GameObject[] disableableOnPause;

        private void Awake()
        {
            if (usesStick)
                movementButtons.SetActive(false);
            else
                movementStick.SetActive(false);
        }

        public void ToggleUI(bool isActivating)
        {
            rootObject.SetActive(isActivating);
        }

        public void DisableElementsOnPause(bool isPausing)
        {
            foreach (var uiElement in disableableOnPause)
                uiElement.SetActive(!isPausing);
        }
    }
}