using StateMachine;
using UnityEngine;

namespace Player
{
    public class StateController : BaseStateController
    {
        [Header("States Data")]
        [SerializeField] private MovementData movementData;

        [Header("Components")]
        [SerializeField] private InputBufferer inputBufferer;
        [SerializeField] private AnimationHandler animationHandler;
        [SerializeField] private EffectsController effectsController;
        [SerializeField] private AudioController audioController;
        [SerializeField] private Transform tr;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Collider2D col;
        [SerializeField] private SpriteRenderer sr;

        protected override void InitializeCore()
        {
            core = new Core
            (
                movementData,
                inputBufferer,
                animationHandler,
                effectsController,
                audioController,
                tr,
                rb,
                col,
                sr
            );
        }

        protected override void OnAwake()
        {
            SetState<IdleState>(true);
        }
        protected override void OnUpdate() { }
        protected override void OnFixedUpdate() { }
    }
}