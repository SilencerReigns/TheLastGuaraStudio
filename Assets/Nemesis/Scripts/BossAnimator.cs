using UnityEngine;

// Toca as animações do boss Nemesis pelo NOME do estado no Animator.
// Os nomes abaixo precisam ser exatamente iguais aos estados que aparecem na janela Animator.
public class BossAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [Header("Nomes dos estados no Animator")]
    [SerializeField] private string idleState = "Nidler";
    [SerializeField] private string attack1State = "1Attack";   // rajada
    [SerializeField] private string attack2State = "2Attack";   // círculo
    [SerializeField] private string attack3State = "3attack";   // invocação
    [SerializeField] private string hurtState = "Nhurt";
    [SerializeField] private string deathState = "Ndeath";

    private bool isDead;
    private float attackLockedUntil;   // enquanto um ataque toca, o Hurt não interrompe
    private float returnToIdleAt = -1f;

    private void Awake()
    {
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        // Animações que não são loop ficam paradas no último quadro: volta para o Idle quando terminam
        if (!isDead && returnToIdleAt > 0f && Time.time >= returnToIdleAt)
        {
            returnToIdleAt = -1f;
            PlayState(idleState);
        }
    }

    // Os métodos de ataque devolvem a duração da animação, para o controller esperar o fim dela
    public float PlayAttack1() => PlayAttack(attack1State);

    public float PlayAttack2() => PlayAttack(attack2State);

    public float PlayAttack3() => PlayAttack(attack3State);

    public void PlayIdle()
    {
        if (isDead) return;

        returnToIdleAt = -1f;
        PlayState(idleState);
    }

    // Só toca se o boss não estiver no meio de um ataque
    public void PlayHurt()
    {
        if (isDead || Time.time < attackLockedUntil) return;

        float length = GetClipLength(hurtState);
        PlayState(hurtState);
        returnToIdleAt = Time.time + length;
    }

    public float PlayDeath()
    {
        isDead = true;
        returnToIdleAt = -1f;
        PlayState(deathState);
        return GetClipLength(deathState);
    }

    private float PlayAttack(string state)
    {
        if (isDead) return 0f;

        float length = GetClipLength(state);
        attackLockedUntil = Time.time + length;
        returnToIdleAt = Time.time + length;
        PlayState(state);
        return length;
    }

    private void PlayState(string state)
    {
        if (animator == null) return;

        animator.Play(state, 0, 0f);
    }

    private float GetClipLength(string clipName)
    {
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
            {
                if (clip.name == clipName) return clip.length;
            }
        }

        Debug.LogWarning("BossAnimator: não achei o clip '" + clipName + "'. Confira se o nome do clip é igual ao do estado.");
        return 1f;
    }
}
