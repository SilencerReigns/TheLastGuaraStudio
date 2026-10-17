using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace InteractionSystem
{
    //Script para ir no player que detecta o interagível mais próximo que está dentro do trigger
    [RequireComponent(typeof(CircleCollider2D))]
    public class InteractionDetector : MonoBehaviour
    {
        private List<IInteractable> interactables = new();
        private IInteractable focusedInteractable;

        [SerializeField] private float updateDelay;
        private IInteractable closestInteractable;
        private float closestDistance;

        private Vector2 currentPosition;
        private float currentDistance;

        private void Awake()
        {
            StartCoroutine(UpdateFocusedInteractable());
        }

        private IEnumerator UpdateFocusedInteractable()
        {
            //Atualiza qual o interagível da lista é o mais próximo
            do
            {
                if (interactables.Count() == 0)
                {
                    yield return new WaitForSeconds(updateDelay);
                    continue;
                }

                closestDistance = float.MaxValue;
                closestInteractable = null;

                foreach (IInteractable interactable in interactables)
                {
                    MonoBehaviour interactableBehaviour = interactable as MonoBehaviour;
                    currentPosition = interactableBehaviour.transform.position;

                    currentDistance = Vector2.Distance(transform.position, currentPosition);

                    if (currentDistance < closestDistance)
                    {
                        closestDistance = currentDistance;
                        closestInteractable = interactable;
                    }
                }

                focusedInteractable?.ToggleVisualCue(false);
                focusedInteractable = closestInteractable;
                focusedInteractable.ToggleVisualCue(true);

                yield return new WaitForSeconds(updateDelay);
            } while (true);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {

            if (!collision.TryGetComponent<IInteractable>(out var interactable))
                return;

            if (!interactables.Contains(interactable))
                interactables.Add(interactable);
        }
        private void OnTriggerExit2D(Collider2D collision)
        {

            if (!collision.TryGetComponent<IInteractable>(out var interactable))
                return;

            interactables.Remove(interactable);

            if (focusedInteractable == interactable)
            {
                focusedInteractable.ToggleVisualCue(false);
                focusedInteractable = null;
            }
        }

        public void Interact()
        {
            focusedInteractable?.GetInteracted();
        }
    }
}