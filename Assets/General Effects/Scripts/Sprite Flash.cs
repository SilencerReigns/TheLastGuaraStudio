using System.Collections;
using UnityEngine;

namespace GeneralEffects
{
    public class SpriteFlash : MonoBehaviour
    {
        [SerializeField] private DefaultData defaultData;
        [SerializeField] private Material flashMaterial;

        private Material originalMaterial;

        private Coroutine coroutine;

        public void ApplyFlash
        (
            SpriteRenderer spriteRenderer,
            float? duration = null
        )
        {
            if (coroutine != null)
            {
                spriteRenderer.material = originalMaterial;
                StopCoroutine(coroutine);
            }

            originalMaterial = spriteRenderer.material;
            spriteRenderer.material = flashMaterial;

            coroutine = StartCoroutine(Routine(spriteRenderer, duration ?? defaultData.defaultFlashDuration));
        }

        private IEnumerator Routine(SpriteRenderer spriteRenderer, float duration)
        {
            yield return new WaitForSeconds(duration);

            spriteRenderer.material = originalMaterial;

            coroutine = null;
        }
    }
}