using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(PlayerInput))]
    public class InputReader : MonoBehaviour
    {
        //Precisa de uma referência ao core da state machine do jogador para alterar os valores
        [SerializeField] private StateController stateController;
        private Core core => (Core)stateController.GetCore();

        [SerializeField] private InputData inputData;
        [SerializeField] private InputBufferer inputBufferer;

        private float dashTimer;

        [Header("Booleans")]
        public bool inputsDisabled;
        public bool movementDisabled;
        public bool jumpDisabled;
        public bool dashDisabled;
        public bool pauseDisabled;
        public bool isPaused;

        public static event Action<bool> OnPausePress;

        public void OnMove(InputAction.CallbackContext context)
        {
            if (inputsDisabled || movementDisabled)
            {
                core.moveDirection = Vector2.zero;
                return;
            }

            Vector2 direction = context.ReadValue<Vector2>();

            if (direction.magnitude < inputData.stickDeadzone)
                direction = Vector2.zero;

            core.moveDirection = direction;

            if (direction != Vector2.zero)
                core.lastMoveDirection = direction;
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (inputsDisabled || jumpDisabled)
                return;

            if (context.performed)
            {
                core.jumpHeld = true;
                inputBufferer.BufferInput(InputEnum.Jump);
            }

            else if (context.canceled)
                core.jumpHeld = false;
        }

        public void OnDash(InputAction.CallbackContext context)
        {
            if (inputsDisabled || dashDisabled)
                return;

            if (context.performed && dashTimer <= Mathf.Epsilon)
            {
                dashTimer = inputData.dashCooldown;
                inputBufferer.BufferInput(InputEnum.Dash);
            }
        }

        public void OnPause(InputAction.CallbackContext context)
        {
            if (pauseDisabled)
                return;

            if (!isPaused && Time.timeScale != 1)
                return;

            if (context.performed)
            {
                isPaused = !isPaused;

                inputsDisabled = isPaused;

                OnPausePress?.Invoke(isPaused);
            }
        }

        public void EnableInputs()
        {
            inputsDisabled = false;
            pauseDisabled = false;
        }

        public void EnablePauseInput()
        {
            pauseDisabled = false;
        }

        public void DisableInputs()
        {
            if (inputsDisabled)
                return;

            inputsDisabled = true;
            pauseDisabled = true;

            core.moveDirection.x = 0;
        }

        private void Update()
        {
            if (dashTimer > Mathf.Epsilon)
                dashTimer -= Time.deltaTime;
        }
    }
}