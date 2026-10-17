using StateMachine;
using UnityEngine;

namespace Player
{
    public class RunState : BaseState<Player.Core, Player.StateController>
    {
        public override void StateEnter()
        {
            core.animationHandler.PlayAnimation("Run");

            core.effectsController.ToggleRunEffects(true, core.audioController);
        }

        public override void StateUpdate()
        {
            if (core.moveDirection == Vector2.zero)
                stateController.SetState<IdleState>(false);

            else if (core.CheckInput() != InputEnum.None)
                stateController.SetState<TransitionsState>(false);
        }

        public override void StateFixedUpdate()
        {
            core.rb.linearVelocity = core.moveDirection * core.movementData.moveSpeed;
        }

        public override void StateExit()
        {
            core.effectsController.ToggleRunEffects(false, null);
        }
    }
}