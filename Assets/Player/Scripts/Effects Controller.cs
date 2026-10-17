using System;
using UnityEngine;

namespace Player
{
    public class EffectsController : MonoBehaviour
    {
        [Header("Player Objects")]
        [SerializeField] private Rigidbody2D playerRb;
        [SerializeField] private SpriteRenderer playerSr;

        [Header("Run Effects")]
        [SerializeField] private RunParticles runParticles;

        [Header("Jump Effects")]
        [SerializeField] private JumpParticles jumpParticles;

        [Header("Dash Effects")]
        [SerializeField] private AfterImagesManager afterImagesManager;
        [SerializeField] private DashParticles dashParticles;

        public void ToggleRunEffects(bool activate, AudioController audioController)
        {
            if (activate)
                runParticles.StartRunParticles(audioController);

            else
                runParticles.StopRunParticles();
        }

        public void PlayJumpEffects()
        {
            jumpParticles.PlayJumpParticles();
        }

        public void ToggleDashEffects(bool activate)
        {
            if (activate)
            {
                dashParticles.StartDashParticles();
                afterImagesManager.StartAfterImages(playerSr.transform, playerSr);
            }

            else
            {
                dashParticles.StopDashParticles();
                afterImagesManager.StopAfterImages(playerSr.transform);
            }
        }

        public void PlayDeathStartEffects()
        {

        }

        public void PlayDeathFinishEffects()
        {

        }
    }
}