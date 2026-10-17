using StateMachine;
using UnityEngine;

namespace Player
{
    public class IdleState : BaseState<Player.Core, Player.StateController>
    {
        public override void StateEnter()
        {
            core.rb.rotation = 0;

            core.animationHandler.PlayAnimation("Idle");

            core.rb.linearVelocity = Vector2.zero;
        }

        public override void StateUpdate()
        {
            if (core.moveDirection != Vector2.zero)
                stateController.SetState<RunState>(false);

            else if (core.CheckInput() != InputEnum.None)
                stateController.SetState<TransitionsState>(false);
        }

        public override void StateFixedUpdate() { }

        public override void StateExit() { }
    }
}