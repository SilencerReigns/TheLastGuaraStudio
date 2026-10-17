using UnityEngine;

namespace Player
{
    public class GroundedChecker : MonoBehaviour
    {
        [SerializeField] private StateController stateController;
        private Core core;

        [Header("Ground Checking")]
        [SerializeField] private Transform groundCheckPoint;
        [SerializeField] private Vector2 groundCheckBoxSize;
        [SerializeField] private LayerMask groundLayerMask;

        private bool isGrounded;

        private void Start()
        {
            //Referência ao core precisa ser feita no Start, pois é inicializado no Awake
            core = (Player.Core)stateController.GetCore();
        }

        private void Update()
        {
            core.isGrounded = isGrounded;
            core.remainingJumps = isGrounded ? core.movementData.extraJumps : core.remainingJumps;
            core.coyoteTimer = isGrounded ? core.movementData.coyoteTime : core.coyoteTimer - Time.deltaTime;
        }

        private void FixedUpdate()
        {
            isGrounded = Physics2D.OverlapBox(groundCheckPoint.position, groundCheckBoxSize, 0f, groundLayerMask);
        }
    }
}