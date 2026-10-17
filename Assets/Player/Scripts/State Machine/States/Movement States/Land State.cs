using System.Collections;
using StateMachine;
using UnityEngine;

namespace Player
{
    public class LandState : BaseState<Player.Core, Player.StateController>
    {
        public override void StateEnter()
        {
            core.animationHandler.PlayAnimation("Land");

            core.audioController.PlayLandSFX();

            core.rb.gravityScale = core.movementData.baseGravity;

            core.remainingJumps = core.movementData.extraJumps;

            StartCoroutine(ExitRoutine());
        }

        private IEnumerator ExitRoutine()
        {
            float elapsedTime = 0;

            while (elapsedTime < core.GetAnimationLength())
            {
                if (core.CheckIfCanAnimationCancel())
                {
                    if (core.moveDirection.x != 0)
                        stateController.SetState<RunState>(false);

                    else if (core.ConsumeInput(InputEnum.Jump))
                        stateController.SetState<JumpState>(false);

                    else if (core.CheckInput() != InputEnum.None)
                        stateController.SetState<TransitionsState>(false);
                }

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            stateController.SetState<IdleState>(false);
        }

        public override void StateUpdate() { }

        public override void StateFixedUpdate() { }

        public override void StateExit() { StopAllCoroutines(); }
    }
}