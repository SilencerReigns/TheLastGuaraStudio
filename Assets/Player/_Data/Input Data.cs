using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "InputData", menuName = "Scriptable Objects/Player/InputData")]
    public class InputData : ScriptableObject
    {
        [Header("Joystick")]
        public float stickDeadzone;

        [Header("Buffering")]
        public float comboWindow;
        public int comboSize;
        public float bufferDuration;

        [Header("Dashing")]
        public float dashCooldown;
    }
}