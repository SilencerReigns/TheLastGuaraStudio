using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private bool spriteFacesRight = true;  

    private string currentState;
    private float lockedUntil; 
    private bool isDead;

    public bool IsDead => isDead;

    private void Awake()
    {
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    // Chamado todo frame pelo MeleeMove: andando = Walk, parado = Idle
    public void SetMoving(bool moving)
    {
        if (isDead || Time.time < lockedUntil) return;

        Play(moving ? "Walk" : "Idle");
    }

    public void PlayAttack() => PlayOnce("Attack");

    public void PlayHurt() => PlayOnce("Hurt");

    // Toca a morte e devolve a duração, para quem chamou esperar antes de destruir o inimigo
    public float PlayDeath()
    {
        isDead = true;
        float length = GetClipLength("Death");
        Play("Death", true);
        return length;
    }

    // directionX > 0 = player à direita, < 0 = à esquerda
    public void FaceTowards(float directionX)
    {
        if (spriteRenderer == null || Mathf.Abs(directionX) < 0.01f) return;

        bool lookingLeft = directionX < 0f;
        spriteRenderer.flipX = spriteFacesRight ? lookingLeft : !lookingLeft;
    }

    private void PlayOnce(string state)
    {
        if (isDead) return;

        lockedUntil = Time.time + GetClipLength(state);
        Play(state, true);
    }

    private void Play(string state, bool restart = false)
    {
        if (animator == null) return;
        if (!restart && currentState == state) return;

        currentState = state;
        animator.Play(state, 0, 0f);
    }

    private float GetClipLength(string clipName)
    {
        if (animator == null || animator.runtimeAnimatorController == null) return 0.5f;

        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName) return clip.length;
        }

        return 0.5f;
}   }