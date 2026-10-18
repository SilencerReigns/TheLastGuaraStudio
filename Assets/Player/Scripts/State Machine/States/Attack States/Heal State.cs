using StateMachine;
using System.Collections;
using UnityEngine;

namespace Player
{
    public class HealState : BaseState<Player.Core, Player.StateController>
    {
        public override void StateEnter()
        {
            core.rb.linearVelocity = Vector2.zero;
            core.canheal = false;

            core.animationHandler.PlayAnimation("Heal");

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