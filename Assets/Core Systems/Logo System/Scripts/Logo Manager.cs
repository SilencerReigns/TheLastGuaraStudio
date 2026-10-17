using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LogoSystem
{
    public class LogoManager : MonoBehaviour
    {
        [SerializeField] private List<LogoBase> logos;
        private int index = 0;

        [SerializeField] private string nextSceneName;

        //Booleana que dita se o próprio script de logo irá carregar a próxima cena
        //Caso falso, o script emitirá um sinal que poderá ser usado por um sistema de transição de cenas
        public bool loadNextScene;
        public static event Action OnLogosFinish;

        void Awake()
        {
            // garante que todas come�am desligadas
            foreach (var logo in logos)
                logo.gameObject.SetActive(false);

            StartCoroutine(RunSequence());
        }

        IEnumerator RunSequence()
        {
            while (index < logos.Count)
            {
                LogoBase logo = logos[index];

                logo.gameObject.SetActive(true);

                bool finished = false;

                logo.Play(() =>
                {
                    finished = true;
                });

                yield return new WaitUntil(() => finished);

                logo.gameObject.SetActive(false);

                index++;
            }

            Debug.Log("Todas as logos terminaram");

            LoadNextScene();
        }

        void LoadNextScene()
        {
            if (string.IsNullOrWhiteSpace(nextSceneName))
            {
                Debug.LogError("[LogoManager] nextSceneName is empty or null!");
                return;
            }

            if (!Application.CanStreamedLevelBeLoaded(nextSceneName))
            {
                Debug.LogError($"[LogoManager] Scene '{nextSceneName}' isn't in the build settings!");
                return;
            }

            if (loadNextScene)
                SceneManager.LoadScene(nextSceneName);
            else
                OnLogosFinish?.Invoke();
        }
    }
}