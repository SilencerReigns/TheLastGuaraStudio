using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathCutscene : MonoBehaviour
{
    [Header("Tela YouDied")]
    [SerializeField] private GameObject youDied;          // deixe desativado na cena
    [SerializeField] private float delayBeforeYouDied = 1f; // tempo para a animação Death terminar
    [SerializeField] private float youDiedDuration = 4f;

    [Header("Menu")]
    [SerializeField] private bool loadMenuAtEnd = false;  // ligue quando quiser a troca para o menu
    [SerializeField] private string menuSceneName = "Menu";

    [SerializeField] private string youDiedState = "JustLose";

    private bool started;

    // Este script precisa ficar num objeto que já começa ATIVO na cena.
    private void OnEnable()
    {
        Player.DeathState.OnPlayerDeath += DeathScene;
    }

    private void OnDisable()
    {
        Player.DeathState.OnPlayerDeath -= DeathScene;
    }

    public void DeathScene()
    {
        if (started) return;
        started = true;

        StartCoroutine(Cutscene());
    }

    private IEnumerator Cutscene()
    {
        // a animação "Death" já é tocada pelo DeathState; aqui só esperamos ela terminar
        yield return new WaitForSecondsRealtime(delayBeforeYouDied);

        Time.timeScale = 0f;

        // ao ativar, o Animator do YouDied (Unscaled Time) toca o estado padrão sozinho
        if (youDied != null)
        {
            youDied.SetActive(true);

            Animator anim = youDied.GetComponent<Animator>();
            if (anim != null)
            {
                anim.Play(youDiedState, 0, 0f);
            }
        }

        yield return new WaitForSecondsRealtime(youDiedDuration);

        Time.timeScale = 1f;

        if (loadMenuAtEnd)
        {
            SceneManager.LoadScene(menuSceneName);
        }
    }
}
