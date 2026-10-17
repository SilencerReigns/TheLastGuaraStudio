using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class AfterImagesManager : MonoBehaviour
    {
        [SerializeField] private EffectsData effectsData;
        [SerializeField] private AfterImage afterImagePrefab;

        private Queue<AfterImage> pool = new();
        private Dictionary<Transform, Coroutine> activeCoroutines = new();

        private void Awake()
        {
            for (int i = 0; i < 20; i++)
            {
                AfterImage newImage = Instantiate(afterImagePrefab, transform);

                newImage.Initialize(this);

                pool.Enqueue(newImage);
            }
        }

        public void StartAfterImages(Transform entity, SpriteRenderer spriteRenderer)
        {
            if (activeCoroutines.ContainsKey(entity))
                return;

            Coroutine newRoutine = StartCoroutine(SpawnRoutine(entity, spriteRenderer));

            activeCoroutines.Add(entity, newRoutine);
        }

        public void StopAfterImages(Transform entity)
        {
            if (activeCoroutines.TryGetValue(entity, out Coroutine coroutine))
            {
                StopCoroutine(coroutine);

                activeCoroutines.Remove(entity);
            }
        }

        private IEnumerator SpawnRoutine(Transform entity, SpriteRenderer spriteRenderer)
        {
            while (true)
            {
                if (entity == null)
                {
                    StopAfterImages(entity);
                    yield return null;
                }

                AfterImage afterImage = pool.Count > 0 ? pool.Dequeue() : Instantiate(afterImagePrefab, transform);

                afterImage.Initialize(this);

                //Aplica a mesma escala da entidade
                afterImage.transform.localScale = entity.lossyScale;

                //Aplica a rotação
                afterImage.transform.rotation = entity.rotation;

                afterImage.ApplyEffect(entity.transform.position, spriteRenderer.sprite);

                yield return new WaitForSeconds(effectsData.delayBetweenImages);
            }
        }

        public void ReturnImage(AfterImage image)
        {
            pool.Enqueue(image);
        }
    }
}