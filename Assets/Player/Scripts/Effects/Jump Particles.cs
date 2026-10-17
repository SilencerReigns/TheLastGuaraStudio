using UnityEngine;

namespace Player
{
    public class JumpParticles : MonoBehaviour
    {
        [SerializeField] private ParticleSystem jumpParticlesPrefab;
        [SerializeField] private Transform jumpParticlesPoint;
        private ParticleSystem jumpParticles;

        private void Awake()
        {
            jumpParticles = Instantiate(jumpParticlesPrefab, jumpParticlesPoint, false);
            jumpParticles.transform.position = jumpParticlesPoint.position;
        }

        public void PlayJumpParticles()
        {
            jumpParticles.Play();
        }
    }
}