using StateMachine;

namespace Player
{
    public class FallState : BaseState<Player.Core, Player.StateController>
    {
        public override void StateEnter()
        {
            core.rb.rotation = 0;

            core.animationHandler.PlayAnimation("Fall");

            core.rb.gravityScale = core.movementData.fallGravity;
        }

        public override void StateUpdate()
        {
            if (core.isGrounded)
                stateController.SetState<LandState>(false);

            else if (core.CheckInput() != InputEnum.None)
            {
                if (core.CheckInput() == InputEnum.Jump)
                {
                    if (core.CheckIfCanJump())
                    {
                        core.ConsumeInput(InputEnum.Jump);
                        stateController.SetState<JumpState>(true);
                    }
                }

                else
                    stateController.SetState<TransitionsState>(false);
            }
        }

        public override void StateFixedUpdate()
        {
            core.rb.linearVelocityX = core.moveDirection.x * core.movementData.moveSpeed;
        }

        public override void StateExit() { }
    }
}