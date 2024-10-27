using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenus : MonoBehaviour {

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject loseScreen;

    [SerializeField] private TMP_Text candyStolenText;

    [SerializeField] private Transform[] panelsToDisable;

    public static GameMenus Instance;

    public static bool bGamePaused;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        pauseMenu.SetActive(false);
        loseScreen.SetActive(false);
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

    public void EnableLoseScreen() {
        Time.timeScale = 0f;
        
        foreach (Transform t in panelsToDisable) {
            t.gameObject.SetActive(false);
        }

        candyStolenText.text = GameManager.Instance.score.ToString("D7");

        bGamePaused = true;
        loseScreen.SetActive(true);
    }

}
