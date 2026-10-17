using System.Collections;
using UnityEngine;

namespace Player
{
    public class RunParticles : MonoBehaviour
    {
        [SerializeField] private EffectsData effectsData;
        [SerializeField] private ParticleSystem runParticlesPrefab;
        [SerializeField] private Transform runParticlesPoint;
        private ParticleSystem runParticles;

        private Coroutine runRoutine;

        private void Awake()
        {
            runParticles = Instantiate(runParticlesPrefab, runParticlesPoint, false);
            runParticles.transform.position = runParticlesPoint.position;
        }

        public void StartRunParticles(AudioController audioController)
        {
            runRoutine ??= StartCoroutine(RunRoutine(audioController));
        }
        private IEnumerator RunRoutine(AudioController audioController)
        {
            while (true)
            {
                runParticles.Play();

                audioController.PlayWalkSFX();

                yield return new WaitForSeconds(effectsData.runParticlesCooldown);
            }
        }

        public void StopRunParticles()
        {
            StopCoroutine(runRoutine);

            runRoutine = null;
        }
    }
}