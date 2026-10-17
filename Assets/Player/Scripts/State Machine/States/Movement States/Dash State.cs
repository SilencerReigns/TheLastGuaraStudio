using System.Collections;
using StateMachine;
using UnityEngine;

namespace Player
{
    public class DashState : BaseState<Player.Core, Player.StateController>
    {
        public override void StateEnter()
        {
            core.animationHandler.PlayAnimation("Dash");

            core.effectsController.ToggleDashEffects(true);
            core.audioController.PlayDashSFX();
            
            core.rb.linearVelocity = Vector2.zero;
            core.rb.linearVelocity = core.lastMoveDirection * core.movementData.dashSpeed;

            StartCoroutine(ExitRoutine());
        }

        private IEnumerator ExitRoutine()
        {
            yield return new WaitForSeconds(core.movementData.dashDuration);

            core.effectsController.ToggleDashEffects(false);

            stateController.SetState<IdleState>(false);
        }

        public override void StateUpdate() { }

        public override void StateFixedUpdate() { }

        public override void StateExit() { StopAllCoroutines(); }
    }
}