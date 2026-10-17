using DG.Tweening;
using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "MovementData", menuName = "Scriptable Objects/Player/MovementData")]
    public class MovementData : ScriptableObject
    {
        [Header("Movement")]
        public float moveSpeed;

        [Header("Gravity")]
        public float baseGravity;
        public float fallGravity;

        [Header("Jump")]
        public float jumpForce;
        [Range(0, 1)] public float jumpCutMultiplier;
        public int extraJumps;
        public float coyoteTime;
        public float jumpBuffer;

        [Header("Dash")]
        public float dashSpeed;
        public float dashDuration;

        [Header("Death")]
        public float knockbackDistance;
        public float wallDetectedOffset;
        public float deathDuration;
        public Ease deathEase;
    }
}