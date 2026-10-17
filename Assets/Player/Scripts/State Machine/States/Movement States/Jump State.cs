using StateMachine;
using UnityEngine;

namespace Player
{
    public class JumpState : BaseState<Player.Core, Player.StateController>
    {
        private bool jumpCutted;

        public override void StateEnter()
        {
            core.animationHandler.PlayAnimation("Jump");

            core.effectsController.PlayJumpEffects();
            core.audioController.PlayJumpSFX();

            core.rb.linearVelocityY = 0;
            core.rb.gravityScale = core.movementData.baseGravity;

            core.rb.AddForceY(core.movementData.jumpForce, ForceMode2D.Impulse);

            if (!core.isGrounded && core.coyoteTimer <= Mathf.Epsilon)
                core.remainingJumps--;

            jumpCutted = false;
        }

        public override void StateUpdate()
        {
            //Jump Cut
            if (!core.jumpHeld)
            {
                core.coyoteTimer = -1;

                if (!jumpCutted)
                {
                    jumpCutted = true;
                    core.rb.linearVelocityY *= core.movementData.jumpCutMultiplier;
                }
            }

            //Transições
            if (core.rb.linearVelocityY < 0.1f)
                stateController.SetState<FallState>(false);

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