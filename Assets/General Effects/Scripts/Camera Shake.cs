using Unity.Cinemachine;
using UnityEngine;

namespace GeneralEffects
{
    [RequireComponent(typeof(CinemachineImpulseSource))]
    public class CameraShake : MonoBehaviour
    {
        [SerializeField] private DefaultData defaultData;

        private CinemachineImpulseSource defaultImpulseSource;
        CinemachineImpulseDefinition impulseDefinition;

        private void Awake()
        {
            defaultImpulseSource = GetComponent<CinemachineImpulseSource>();
        }

        public void PlayCameraShake
        (
            CinemachineImpulseSource impulseSource = null,
            float? shakeForce = null,
            float? shakeTime = null,
            Vector3? shakeVelocity = null
        )
        {
            CinemachineImpulseSource usedSource = impulseSource != null ? impulseSource : defaultImpulseSource;

            impulseDefinition = usedSource.ImpulseDefinition;

            impulseDefinition.ImpulseDuration = shakeTime ?? defaultData.defaultShaketime;
            usedSource.DefaultVelocity = shakeVelocity ?? defaultData.defaultShakeVelocity;

            usedSource.GenerateImpulseWithForce(shakeForce ?? defaultData.defaultShakeForce);
        }
    }
}