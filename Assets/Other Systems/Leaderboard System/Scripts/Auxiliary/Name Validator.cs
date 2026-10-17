using TMPro;
using UnityEngine;

namespace LeaderboardSystem
{
    [RequireComponent(typeof(TMP_InputField))]
    public class NameValidator : MonoBehaviour
    {
        private TMP_InputField nameInput;

        private void Awake()
        {
            nameInput = GetComponent<TMP_InputField>();
        }

        private void Start()
        {
            nameInput.onValidateInput += ValidateChar;
        }

        private void OnDestroy()
        {
            nameInput.onValidateInput -= ValidateChar;
        }

        //Valida se o caractere digitado é apenas um caractere normal (A a Z) e
        //automaticamente o transforma em maíuscula
        private char ValidateChar(string text, int charIndex, char c)
        {
            c = char.ToUpperInvariant(c);

            return (c >= 'A' && c <= 'Z') ? c : '\0';
        }
    }
}