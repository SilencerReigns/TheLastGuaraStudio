using UnityEngine;

namespace StateMachine
{
    public abstract class BaseState : MonoBehaviour
    {
        public abstract void Initialize(BaseCore newCore, BaseStateController newController);

        public abstract void StateEnter();
        public abstract void StateUpdate();
        public abstract void StateFixedUpdate();
        public abstract void StateExit();
    }

    //Estados específicos devem herdam dessa classe, a aprtir da qual é possível tipar o Core e o state controller, que devem ser 
    //específicos para cada entidade
    public abstract class BaseState<TCore, TStateController> : BaseState
        where TCore : BaseCore
        where TStateController : BaseStateController
    {
        protected TCore core;
        protected TStateController stateController;

        public override void Initialize(BaseCore newCore, BaseStateController newController)
        {
            core = (TCore)newCore;
            stateController = (TStateController)newController;
        }
    }
}