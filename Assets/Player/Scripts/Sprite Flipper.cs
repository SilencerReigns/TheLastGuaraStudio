using UnityEngine;

namespace Player
{
    public class SpriteFlipper : MonoBehaviour
    {
        [SerializeField] private StateController stateController;
        private Core core;

        [SerializeField] private Transform playerBody;
        private bool isFacingRight;

        private void Start()
        {
            core = (Core)stateController.GetCore();

            isFacingRight = true;

            core.facingDirection = 1;
            core.canFlip = true;
        }

        private void Update()
        {
            if (!core.canFlip)
                return;

            if
            (
                isFacingRight && core.moveDirection.x < 0 ||
                !isFacingRight && core.moveDirection.x > 0
            )
            {
                isFacingRight = !isFacingRight;
                core.facingDirection *= -1;

                playerBody.localScale *= new Vector2(-1, 1);
            }
        }
    }
}