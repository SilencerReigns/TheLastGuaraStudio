using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace LogoSystem
{
    [RequireComponent(typeof(Image))]
    public class LogoBase : MonoBehaviour
    {
        [SerializeField] protected float duration = 2f;
        protected Image image;

        protected virtual void Awake()
        {
            image = GetComponent<Image>();

            if (image == null)
                Debug.LogWarning($"[LogoBase] Image não encontrada em {gameObject.name}");
        }

        public virtual void Play(Action onFinish)
        {
            StartCoroutine(PlayRoutine(onFinish));
        }

        protected virtual IEnumerator PlayRoutine(Action onFinish)
        {
            // exemplo simples: só esperar
            yield return new WaitForSeconds(duration);

            onFinish?.Invoke();
        }
    }
}