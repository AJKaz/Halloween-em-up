using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenus : MonoBehaviour {

    [SerializeField] private GameObject pauseMenu;
    
    public static bool bGamePaused;
    
    private void Start() {
        pauseMenu.SetActive(false);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (bGamePaused) {
                ResumeGame();
            }
            else {
                PauseGame();
            }
        }
    }

    private void PauseGame() {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        bGamePaused = true;
    }

    public void ResumeGame() {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        bGamePaused = false;
    }

    public void GoToMainMenu() {
        Time.timeScale = 1f;
        bGamePaused = false;
        SceneManager.LoadScene("MainMenuScene");
    }

    public void Quit() {
        Application.Quit();
    }

}
