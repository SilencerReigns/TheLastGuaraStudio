using StateMachine;
using UnityEngine;

namespace Player
{
    //O núcleo da state machine
    //Contém informações que todos os estados, state controller e scripts auxiliares devem acessar
    public class Core : BaseCore
    {
        public Core(
            MovementData movementData,
            InputBufferer inputBufferer,
            AnimationHandler animationHandler,
            EffectsController effectsController,
            AudioController audioController,
            Transform tr,
            Rigidbody2D rb,
            Collider2D col,
            SpriteRenderer sr
        )
        {
            this.movementData = movementData;
            this.inputBufferer = inputBufferer;
            this.animationHandler = animationHandler;
            this.effectsController = effectsController;
            this.audioController = audioController;
            this.tr = tr;
            this.rb = rb;
            this.col = col;
            this.sr = sr;
        }

        [Header("Data")]
        public MovementData movementData;

        [Header("Auxiliary Scripts")]
        public InputBufferer inputBufferer;
        public AnimationHandler animationHandler;
        public EffectsController effectsController;
        public AudioController audioController;

        public InputEnum CheckInput() { return inputBufferer.CheckInput(); }
        public bool ConsumeInput(InputEnum requestedInput)
        {
            bool inputConsumed = inputBufferer.ConsumeInput(requestedInput);
            if (inputConsumed)
                consumedInput = requestedInput;

            return inputConsumed;
        }

        public bool CheckIfCanAnimationCancel() { return animationHandler.CheckIfCanAnimationCancel(); }
        public float GetAnimationLength() { return animationHandler.GetAnimationLength(); }

        [Header("Components")]
        public Transform tr;
        public Rigidbody2D rb;
        public Collider2D col;
        public Animator animator;
        public SpriteRenderer sr;

        [Header("Movement Variables")]
        public Vector2 moveDirection;
        public Vector2 lastMoveDirection = Vector2.right;
        public int facingDirection;
        public Vector3 deathDirection;

        [Header("Jump Variables")]
        public int remainingJumps;
        public float coyoteTimer;
        public bool isGrounded;
        public bool canFlip;

        public bool CheckIfCanJump()
        {
            bool hasCoyoteTime = coyoteTimer >= Mathf.Epsilon;
            bool hasRemainingJumps = remainingJumps > 0;

            return hasCoyoteTime || hasRemainingJumps;
        }

        [Header("Input Variables")]
        public InputEnum consumedInput;
        public bool jumpHeld;
        public bool canPause;
    }
}