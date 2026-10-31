using System.Collections;
using UnityEngine;

// Ataque 2 do boss: círculo no chão com canalização.
// Quando a canalização termina, se o player estiver dentro da área ele é derrotado na hora.
// A área de morte é uma elipse que acompanha o desenho do sprite (o círculo tem perspectiva).
// O círculo pode ter animações (canalização, loop, se desfazendo) ou, sem Animator, só faz um fade.
public class BossCircleAttack : MonoBehaviour
{
    [Header("Círculo")]
    [Tooltip("Filho CircleZone (começa desativado)")]
    [SerializeField] private SpriteRenderer circleZone;
    [Tooltip("Tempo total de canalização, em segundos. A animação do boss se ajusta para durar o mesmo tempo.")]
    [SerializeField] private float channelTime = 2.5f;
    [Tooltip("Tamanho do desenho dentro da imagem (largura, altura) para a área de morte. Confira pelo contorno amarelo na Scene.")]
    [SerializeField] private Vector2 drawingFill = new Vector2(0.9f, 0.46f);

    [Header("Animação do círculo (opcional)")]
    [Tooltip("Animator do CircleZone. Se vazio, usa o que estiver no próprio CircleZone. Sem Animator, o círculo só faz fade.")]
    [SerializeField] private Animator circleAnimator;
    [Tooltip("Entrada/canalização: toca uma vez, no início")]
    [SerializeField] private string channelState = "Channel";
    [Tooltip("Fica repetindo até a canalização completar")]
    [SerializeField] private string loopState = "Loop";
    [Tooltip("Círculo se desfazendo: toca depois que a canalização completa")]
    [SerializeField] private string dissolveState = "Dissolve";

    [Header("Sem animação (fade)")]
    [Tooltip("Transparência do círculo quando ele aparece (sobe até 1 durante a canalização)")]
    [SerializeField] private float startAlpha = 0.25f;
    [Tooltip("Quanto tempo o círculo fica cheio depois de completar, antes de sumir")]
    [SerializeField] private float endHoldTime = 0.25f;

    private BossAnimator bossAnimator;
    private Transform playerPosition;
    private HealthPlayer playerHealth;

    private void Awake()
    {
        bossAnimator = GetComponent<BossAnimator>();

        if (circleAnimator == null && circleZone != null)
        {
            circleAnimator = circleZone.GetComponent<Animator>();
        }
    }

    private void Start()
    {
        GameObject targetObj = GameObject.Find("PlayerRF/Body/Hitbox");

        if (targetObj != null)
        {
            playerPosition = targetObj.transform;
            playerHealth = targetObj.GetComponentInParent<HealthPlayer>();
        }
        else
        {
            Debug.LogError("BossCircleAttack: O caminho 'PlayerRF/Body/Hitbox' não foi encontrado na cena!");
        }
    }

    // Faz o ataque inteiro e só termina quando o círculo some
    public IEnumerator Execute()
    {
        bossAnimator.PlayAttack2(channelTime);

        if (circleZone == null)
        {
            Debug.LogError("BossCircleAttack: falta arrastar o filho CircleZone em 'Circle Zone'.");
            yield return new WaitForSeconds(channelTime);
            yield break;
        }

        circleZone.gameObject.SetActive(true);
        bool useAnimation = circleAnimator != null;

        // Canalização
        if (useAnimation)
        {
            SetAlpha(1f);
            yield return AnimatedChannelPhase();
        }
        else
        {
            yield return FadeChannelPhase();
        }

        // Completou: quem estiver dentro é derrotado na hora
        if (IsPlayerInside() && playerHealth != null && !playerHealth.IsDead)
        {
            Debug.Log("[Boss] Círculo completou com o player dentro: derrota instantânea");
            playerHealth.Die((Vector2)circleZone.transform.position);
        }
        else
        {
            Debug.Log("[Boss] Círculo completou: o player escapou");
        }

        // O círculo se desfaz
        if (useAnimation)
        {
            circleAnimator.speed = 1f;
            circleAnimator.Play(dissolveState, 0, 0f);
            yield return new WaitForSeconds(GetClipLength(dissolveState));
        }
        else
        {
            SetAlpha(1f);
            yield return new WaitForSeconds(endHoldTime);
        }

        HideZone();
    }

    // Chamado quando o boss morre no meio do ataque
    public void Cancel()
    {
        HideZone();
    }

    // Toca a animação de canalização e depois fica no loop até completar o tempo total
    private IEnumerator AnimatedChannelPhase()
    {
        // a animação de canalização nunca passa do tempo total (se for mais longa, ela acelera)
        float clipLength = GetClipLength(channelState);
        float duration = Mathf.Min(clipLength, channelTime);

        circleAnimator.speed = clipLength / duration;
        circleAnimator.Play(channelState, 0, 0f);
        yield return new WaitForSeconds(duration);

        // o resto da canalização fica em loop
        circleAnimator.speed = 1f;
        circleAnimator.Play(loopState, 0, 0f);

        float remaining = channelTime - duration;
        if (remaining > 0f) yield return new WaitForSeconds(remaining);
    }

    // Sem animação: o círculo vai ficando mais forte até completar
    private IEnumerator FadeChannelPhase()
    {
        SetAlpha(startAlpha);

        float elapsed = 0f;

        while (elapsed < channelTime)
        {
            elapsed += Time.deltaTime;
            SetAlpha(Mathf.Lerp(startAlpha, 1f, elapsed / channelTime));
            yield return null;
        }

        SetAlpha(1f);
    }

    private float GetClipLength(string clipName)
    {
        if (circleAnimator != null && circleAnimator.runtimeAnimatorController != null)
        {
            foreach (AnimationClip clip in circleAnimator.runtimeAnimatorController.animationClips)
            {
                if (clip.name == clipName) return clip.length;
            }
        }

        Debug.LogWarning("BossCircleAttack: não achei o clip '" + clipName + "'. Confira se o nome do clip é igual ao do estado.");
        return 0.5f;
    }

    private bool IsPlayerInside()
    {
        if (playerPosition == null || circleZone == null || circleZone.sprite == null) return false;

        Vector2 radii = GetZoneRadii();
        if (radii.x <= 0f || radii.y <= 0f) return false;

        // Teste de elipse: (dx/rx)² + (dy/ry)² <= 1
        Vector2 offset = (Vector2)playerPosition.position - (Vector2)circleZone.transform.position;
        float nx = offset.x / radii.x;
        float ny = offset.y / radii.y;

        return nx * nx + ny * ny <= 1f;
    }

    // Meio-eixos da elipse em unidades do mundo (considera a Scale do CircleZone e do boss)
    private Vector2 GetZoneRadii()
    {
        Vector3 spriteSize = circleZone.sprite.bounds.size;
        Vector3 scale = circleZone.transform.lossyScale;

        return new Vector2(
            spriteSize.x * Mathf.Abs(scale.x) * 0.5f * drawingFill.x,
            spriteSize.y * Mathf.Abs(scale.y) * 0.5f * drawingFill.y);
    }

    private void SetAlpha(float alpha)
    {
        if (circleZone == null) return;

        Color color = circleZone.color;
        color.a = alpha;
        circleZone.color = color;
    }

    private void HideZone()
    {
        if (circleAnimator != null) circleAnimator.speed = 1f;
        if (circleZone != null) circleZone.gameObject.SetActive(false);
    }

    // Mostra a área de morte (amarelo) na Scene quando o boss está selecionado
    private void OnDrawGizmosSelected()
    {
        if (circleZone == null || circleZone.sprite == null) return;

        Vector2 radii = GetZoneRadii();
        Vector3 center = circleZone.transform.position;

        Gizmos.color = Color.yellow;
        Vector3 previous = center + new Vector3(radii.x, 0f, 0f);

        for (int i = 1; i <= 48; i++)
        {
            float angle = i / 48f * Mathf.PI * 2f;
            Vector3 point = center + new Vector3(Mathf.Cos(angle) * radii.x, Mathf.Sin(angle) * radii.y, 0f);
            Gizmos.DrawLine(previous, point);
            previous = point;
        }
    }
}
