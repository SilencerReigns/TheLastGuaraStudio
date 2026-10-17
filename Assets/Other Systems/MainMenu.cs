using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Carrega a próxima cena da fila de Build (geralmente a cena do jogo)
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Fecha o jogo (funciona apenas na versão final compilada)
    public void QuitGame()
    {
        Debug.Log("O jogo foi fechado!"); // Feedback visual no editor
        Application.Quit();
    }
}