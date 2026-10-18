using StateMachine;
using System.Collections;
using UnityEngine;

namespace Player
{
    public class ParryState : BaseState<Player.Core, Player.StateController>
    {
        public override void StateEnter()
        {
            core.canParry = false;
            core.rb.linearVelocity = Vector2.zero;

            core.animationHandler.PlayAnimation("Parry");

            StartCoroutine(ExitRoutine());
        }

        private IEnumerator ExitRoutine()
        {
            yield return new WaitForEndOfFrame();

            yield return new WaitForSeconds(core.GetAnimationLength());

            if (core.CheckInput() != InputEnum.None)
                stateController.SetState<TransitionsState>(false);
            else
                stateController.SetState<IdleState>(false);
        }

        public override void StateUpdate() { }

        public override void StateFixedUpdate() { }

        public override void StateExit() { }
    }
}