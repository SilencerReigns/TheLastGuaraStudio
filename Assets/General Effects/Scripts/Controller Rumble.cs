using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GeneralEffects
{
    public class ControllerRumble : MonoBehaviour
    {
        [SerializeField] private DefaultData defaultData;

        private Gamepad gamepad;
        private Coroutine coroutine;

        private void OnDisable()
        {
            gamepad.SetMotorSpeeds(0, 0);

            StopAllCoroutines();
        }

        public void PlayControllerRumble
        (
            float? lowFreq = null,
            float? highFreq = null,
            float? duration = null
        )
        {
            gamepad = Gamepad.current;

            if (gamepad != null)
            {
                gamepad.SetMotorSpeeds
                (
                    lowFreq ?? defaultData.defaultLowFrequency,
                    highFreq ?? defaultData.defaultHighFrequency
                );

                if (coroutine != null)
                    StopCoroutine(coroutine);

                coroutine = StartCoroutine(Routine(duration ?? defaultData.defaultRumbleDuration));
            }
        }

        IEnumerator Routine(float duration)
        {
            yield return new WaitForSeconds(duration);

            gamepad.SetMotorSpeeds(0, 0);

            coroutine = null;
        }
    }
}