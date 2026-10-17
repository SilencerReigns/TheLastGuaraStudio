using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    //Controlador base dos estados
    //Feito de um jeito muito fácil de manusear, apenas deve-se herdar dessa classe e definir o estado no qual a entidade começará (normalmente idle)
    //Os estados devem ser adicionados na cena em objetos vazios filhos do objeto que possui o state controller da entidade
    //Os estados devem sempre ser gameObjects filhos do objeto que tem o StateController. daí todo o resto é tratado automaticamente
    //Usa-se um dicionário com o par <Tipo do estado, Estado> para acesso mais rápido dos estados
    //Com esse dicionário, foi possível fazer o par SetState e GetState
    //GetState escolhe um estado do dicionário em tempo constante
    //SetState recebe o tipo do Estado, então usa GetState para buscar o estado em si no dicionário rapidamente
    public abstract class BaseStateController : MonoBehaviour
    {
        protected StateMachine stateMachine;
        protected BaseCore core;
        private readonly Dictionary<System.Type, BaseState> states = new();

        private void Awake()
        {
            stateMachine = new StateMachine();

            var foundStates = GetComponentsInChildren<BaseState>(true);
            foreach (var foundState in foundStates)
                states[foundState.GetType()] = foundState;

            InitializeCore();

            if (core == null)
            {
                Debug.LogWarning("[StateMachine.BaseStateController] Core was not initialized!");
                return;
            }

            foreach (BaseState state in states.Values)
                state.Initialize(core, this);

            OnAwake();
        }
        private void Update()
        {
            stateMachine.PlayStateUpdate();
            OnUpdate();
        }
        private void FixedUpdate()
        {
            stateMachine.PlayStateFixedUpdate();
            OnFixedUpdate();
        }

        protected abstract void InitializeCore();
        protected abstract void OnAwake();
        protected abstract void OnUpdate();
        protected abstract void OnFixedUpdate();

        public void SetState<TState>(bool forced)
            where TState : BaseState
        {
            var state = GetState<TState>();

            stateMachine.ChangeState(state, forced);
        }
        public TState GetState<TState>()
            where TState : BaseState
        {
            var type = typeof(TState);

            return (TState)states[type];
        }

        public BaseCore GetCore() { return core; }
    }
}