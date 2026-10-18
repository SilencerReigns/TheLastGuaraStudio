using StateMachine;

namespace Player
{
    //Estado responsável por fazer o roteamento de inputs para o estado correto
    //Lógica de sequência de ataques, por exemplo, deve ser implementada aqui
    public class TransitionsState : BaseState<Player.Core, Player.StateController>
    {
        private int lightSequence = -1;

        public override void StateEnter()
        {
            if (core.ConsumeInput(InputEnum.Dash))
                stateController.SetState<DashState>(false);

            else if (core.ConsumeInput(InputEnum.Light_Attack))
            {
                lightSequence = (lightSequence + 1) % 4;

                switch (lightSequence)
                {
                    case 0:
                        stateController.SetState<Light1State>(false);
                        break;
                    case 1:
                        stateController.SetState<Light2State>(false);
                        break;
                    case 2:
                        stateController.SetState<Light3State>(false);
                        break;
                    case 3:
                        stateController.SetState<Light4State>(false);
                        break;
                }
            }

            else if (core.ConsumeInput(InputEnum.Heavy_Attack))
                stateController.SetState<HeavyState>(false);

            else if (core.ConsumeInput(InputEnum.Heal))
            {
                if (core.canheal)
                    stateController.SetState<HealState>(false);
                else
                    stateController.SetState<IdleState>(false);
            }
            else if (core.ConsumeInput(InputEnum.Parry))
            {
                if (core.canParry)
                    stateController.SetState<ParryState>(false);
                else
                    stateController.SetState<IdleState>(false);
            }
        }

        public override void StateExit() { core.consumedInput = InputEnum.None; }

        public override void StateFixedUpdate() { }

        public override void StateUpdate() { }
    }
}