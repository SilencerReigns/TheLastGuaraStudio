using StateMachine;

namespace Player
{
    //Estado responsável por fazer o roteamento de inputs para o estado correto
    //Lógica de sequência de ataques, por exemplo, deve ser implementada aqui
    public class TransitionsState : BaseState<Player.Core, Player.StateController>
    {
        public override void StateEnter()
        {
            if (core.ConsumeInput(InputEnum.Dash))
                stateController.SetState<DashState>(false);
        }

        public override void StateExit() { core.consumedInput = InputEnum.None; }

        public override void StateFixedUpdate() { }

        public override void StateUpdate() { }
    }
}