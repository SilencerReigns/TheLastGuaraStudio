using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class InputBufferer : MonoBehaviour
    {
        //Script que trata do buffering de inputs
        //Também oferece uma janela de combo (normalmente 0.1), durante a qual inputs que precisam de múltiplos botões apertados simultâneamente
        //podem ser processados para gerar a ação desejada
        //Caso não haja inputs complexos no jogo, pode-se setar bufferWindow no InputData como 0, ou só ignorar o valor mesmo
        [SerializeField] private InputData inputData;

        private List<InputEnum> bufferList = new();
        private InputEnum nextInput;
        private float bufferTimer;

        private Coroutine comboWindowRoutine;
        private Coroutine bufferRoutine;

        public void BufferInput(InputEnum bufferedInput)
        {
            //Adiciona um novo input à lista, mantendo o tamanho desta de acordo com o comboSize do inputData
            //Depois que detectar um input, se já não houver, inicializa uma corrotina para a janela de combo
            if (bufferList.Count == inputData.comboSize)
                bufferList.RemoveAt(0);

            bufferList.Add(bufferedInput);

            comboWindowRoutine ??= StartCoroutine(ComboWindowRoutine());
        }

        private IEnumerator ComboWindowRoutine()
        {
            //Corrotina para esperar o tempo comboWindow do inputData
            //Esse tempo serve para o jogador poder inputar múltiplos botões "simultaneamente"
            //Usada quando houver ações que usam mais de um input ao mesmo tempo
            //Caso não haja esse tipo de ação no jogo, basta definir comboWindow como 0 e comboSize como 1 no inputData
            yield return new WaitForSeconds(inputData.comboWindow);

            //É aqui que o código processa a combinação de inputs que existir na lista
            //Varia de jogo para jogo. Jogos que não têm ações multi-inputs devem pegar o último elemento da lista apenas
            nextInput = bufferList[^1];

            //Limpa a lista, já que ela já foi consumida
            bufferList.Clear();

            bufferTimer = inputData.bufferDuration;   //Reinicia o tempo do buffer;

            bufferRoutine ??= StartCoroutine(BufferRoutine());

            comboWindowRoutine = null;
        }

        private IEnumerator BufferRoutine()
        {
            //Corrotina do buffer em si
            //Espera o tempo de buffer para que o input seja consumido
            //No final do tempo, reseta o nextInput
            while (bufferTimer > Mathf.Epsilon)
            {
                bufferTimer -= Time.deltaTime;

                yield return null;
            }

            nextInput = InputEnum.None;

            bufferRoutine = null;
        }

        public InputEnum CheckInput()
        {
            return nextInput;
        }

        public bool ConsumeInput(InputEnum requestedInput)
        {
            //Se conseguiu consumir o input, reseta tudo e retorna true
            if (nextInput == requestedInput)
            {
                bufferList.Clear();
                nextInput = InputEnum.None;

                StopAllCoroutines();

                bufferRoutine = null;
                comboWindowRoutine = null;

                return true;
            }

            else
                return false;
        }
    }
}