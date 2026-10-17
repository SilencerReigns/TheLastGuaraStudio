using StateMachine;

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
            if (core.moveDirection.x == 0)
                stateController.SetState<IdleState>(false);

            else if (!core.isGrounded)
                stateController.SetState<FallState>(false);

            else if (core.ConsumeInput(InputEnum.Jump))
                stateController.SetState<JumpState>(false);

            else if (core.CheckInput() != InputEnum.None)
                stateController.SetState<TransitionsState>(false);
        }

        public override void StateFixedUpdate()
        {
            core.rb.linearVelocityX = core.moveDirection.x * core.movementData.moveSpeed;
        }

        public override void StateExit()
        {
            core.effectsController.ToggleRunEffects(false, null);
        }
    }
}