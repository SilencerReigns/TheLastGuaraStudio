using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Mapa");
    }

    public void QuitGame()
    {
        Debug.Log("O jogo foi fechado!");
        Application.Quit();
    }
}