namespace StateMachine
{
    public class StateMachine
    {
        private BaseState currentState;

        public void ChangeState(BaseState newState, bool forceReset = false)
        {
            if (newState != currentState || forceReset)
            {
                if (currentState != null)
                    currentState.StateExit();

                currentState = newState;

                currentState.StateEnter();
            }
        }

        public void PlayStateUpdate()
        {
            if (currentState == null)
                return;

            currentState.StateUpdate();
        }

        public void PlayStateFixedUpdate()
        {
            if (currentState == null)
                return;

            currentState.StateFixedUpdate();
        }
    }
}