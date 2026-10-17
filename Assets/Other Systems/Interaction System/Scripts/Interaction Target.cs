using UnityEngine;
using UnityEngine.Events;

namespace InteractionSystem
{
    //Script que funciona como um componente que pode ser adicionado a qualquer entidade que seja interagível
    [RequireComponent(typeof(BoxCollider2D))]
    public class InteractionTarget : MonoBehaviour, IInteractable
    {
        [SerializeField] private UnityEvent OnInteractionHappened;
        [SerializeField] private GameObject visualCue;

        private BoxCollider2D col;

        private void Awake()
        {
            col = GetComponent<BoxCollider2D>();
            col.isTrigger = true;

            visualCue.SetActive(false);
        }

        public void ToggleVisualCue(bool activate)
        {
            visualCue.SetActive(activate);
        }

        public void GetInteracted()
        {
            OnInteractionHappened?.Invoke();
        }
    }
}