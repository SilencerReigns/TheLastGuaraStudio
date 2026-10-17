using UnityEngine;

namespace Player
{
    public class DashParticles : MonoBehaviour
    {
        [SerializeField] private ParticleSystem dashParticlesPrefab;
        [SerializeField] private Transform dashParticlesPoint;
        private ParticleSystem dashParticles;

        private void Awake()
        {
            dashParticles = Instantiate(dashParticlesPrefab, dashParticlesPoint, false);
            dashParticles.transform.position = dashParticlesPoint.position;
        }

        public void StartDashParticles()
        {
            dashParticles.Play();
        }

        public void StopDashParticles()
        {
            dashParticles.Stop();
        }
    }
}