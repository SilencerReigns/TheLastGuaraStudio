using Unity.Cinemachine;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(CinemachineConfiner2D))]
    public class CameraHandler : MonoBehaviour
    {
        private CinemachineConfiner2D confinerComponent;
        private PolygonCollider2D cameraConfiner;

        private void Awake()
        {
            confinerComponent = GetComponent<CinemachineConfiner2D>();
        }

        private void Start()
        {
            GameObject confinerObject = GameObject.FindWithTag("CameraConfiner");

            if (confinerObject == null)
            {
                Debug.LogWarning("[Player.CameraController] Camera Confiner was not found!");
                return;
            }

            cameraConfiner = confinerObject.GetComponent<PolygonCollider2D>();

            confinerComponent.BoundingShape2D = cameraConfiner;
            confinerComponent.InvalidateBoundingShapeCache();
        }
    }
}