using UnityEngine;

namespace CameraSystem
{
    [CreateAssetMenu(fileName = "Camera Data", menuName = "Scriptable Objects/Camera System/Camera Data")]
    public class CameraData : ScriptableObject
    {
        public Vector2 defaultAspectRatio = new(16f, 9f);
        public Color barColor;
    }
}