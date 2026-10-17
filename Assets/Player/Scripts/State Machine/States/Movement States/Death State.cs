using System;
using DG.Tweening;
using StateMachine;
using UnityEngine;

namespace Player
{
    public class DeathState : BaseState<Player.Core, Player.StateController>
    {
        [SerializeField] private LayerMask groundLayer;

        public static event Action OnPlayerDeath;

        public override void StateEnter()
        {
            core.animationHandler.PlayAnimation("Death");
            core.effectsController.PlayDeathStartEffects();

            core.rb.linearVelocity = Vector2.zero;
            core.rb.gravityScale = 0;

            Vector3 direction = -core.deathDirection;

            RaycastHit2D hit = Physics2D.Raycast(core.tr.position, direction, core.movementData.knockbackDistance, groundLayer);

            Vector3 endPos;

            //Se há uma parede no caminho, para antes
            if (hit.collider != null)
                endPos = hit.point - (Vector2)direction * core.movementData.wallDetectedOffset;
            else
                endPos = core.tr.position + core.movementData.knockbackDistance * direction;

            core.tr.DOMove(endPos, core.movementData.deathDuration)
                .SetEase(core.movementData.deathEase)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    core.effectsController.PlayDeathFinishEffects();
                    OnPlayerDeath?.Invoke();
                });
        }

        public override void StateUpdate() { }

        public override void StateFixedUpdate() { }

        public override void StateExit() { }
    }
}