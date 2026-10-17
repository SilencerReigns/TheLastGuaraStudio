using UnityEngine;

namespace Player
{
    public class AnimationHandler : MonoBehaviour
    {
        //Script para lidar com o play das animações do animator
        //Permite o uso de animation cancelling no jogo
        //Animações deve chamar a função AllowAnimationCancel via um evento dentro delas
        //Estados podem usar CheckIfCanAnimationCancel() para verificar se podem prosseguir para outro estado
        [SerializeField] private Animator animator;

        private bool canAnimationCancel;

        public void PlayAnimation(string animationName)
        {
            animator.Play(animationName, 0, 0f);
            canAnimationCancel = false;
        }

        public void PauseAnimation()
        {
            animator.speed = 0f;
        }
        public void ResumeAnimation()
        {
            animator.speed = 1f;
        }

        public float GetAnimationLength()
        {
            float length = animator.GetCurrentAnimatorStateInfo(0).length;
            return length;
        }

        public void AllowAnimationCanel()
        {
            canAnimationCancel = true;
        }

        public bool CheckIfCanAnimationCancel()
        {
            return canAnimationCancel;
        }

        public void MakeAnimatorUnscaled()
        {
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }
    }
}