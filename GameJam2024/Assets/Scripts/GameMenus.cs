using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenus : MonoBehaviour {

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject loseScreen;
    [SerializeField] private GameObject Tutorial;
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
        Tutorial.SetActive(true);
        Time.timeScale = 0f;
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
        Tutorial.SetActive(false);
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
