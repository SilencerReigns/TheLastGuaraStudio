using System;
using UnityEngine;
using UnityEngine.UI;

namespace AudioSystem
{
    [RequireComponent(typeof(Slider))]
    public class SliderSetup : MonoBehaviour
    {
        //Script para colocar nos sliders de som
        //Atualiza o valor dos sliders automaticamente de acordo 
        // com os valores do mixer salvos no player prefs
        [SerializeField] private string paramName;
        private Slider slider;

        public static event Action<string, float> OnVolumeChanged;

        private void Awake()
        {
            slider = GetComponent<Slider>();

            float value = PlayerPrefs.GetFloat(paramName, 1f);
            slider.value = value;
        }

        public void ChangeVolume(float value)
        {
            OnVolumeChanged?.Invoke(paramName, value);
        }
    }
}