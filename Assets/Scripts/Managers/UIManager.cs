using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {
    #region Instance
    public static UIManager Instance;
    private void Awake() {
        if (Instance == null) {
            Instance = this;
            if (PlayerPrefs.GetString("version", "1.0.0") != currVersion) {
                whatsNewPanel.SetActive(true);
                settingsBtn.gameObject.SetActive(false);
            } else {
                MainMenu();
            }
        } else {
            Destroy(this);
        }
    }
    #endregion

    string currVersion = "1.0.2";

    public Animator menuAnim;
    public GameObject mainMenu;
    public GameObject settingsMenu;
    public GameObject shopMenu;
    public GameObject winScreen;
    public GameObject whatsNewPanel;

    public Image settingsBtn;
    public Sprite settingsSprite;
    public Sprite pauseSprite;
    public GameObject backToMenuBtn;

    public void CloseWathsNewPanel() { 
        AudioManager.Instance.PlayButtonClick();
        whatsNewPanel.SetActive(false);
        PlayerPrefs.SetString("version", currVersion);
        settingsBtn.gameObject.SetActive(true);
        MainMenu();
    }
    public void MainMenu() {
        // Display main menu
        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
        shopMenu.SetActive(false);
        settingsBtn.sprite = settingsSprite;
        backToMenuBtn.SetActive(false);
        HideWinScreen();
    }
    public void DisplaySettings() {
        // Display/hide settings menu
        AudioManager.Instance.PlayButtonClick();
        settingsMenu.SetActive(!settingsMenu.activeSelf);

        if (backToMenuBtn.activeSelf)
            GameManager.Instance.playing = !GameManager.Instance.playing;
    }
    public void DisplayShop() {
        // Display/hide shop menu
        AudioManager.Instance.PlayButtonClick();
        mainMenu.SetActive(!mainMenu.activeSelf);
        shopMenu.SetActive(!shopMenu.activeSelf);
    }
    public void StartGame(bool player1) {
        AudioManager.Instance.PlayButtonClick();
        GameManager.Instance.SinglePlayer(player1);
        settingsBtn.sprite = pauseSprite;
        backToMenuBtn.SetActive(true);
        StartCoroutine(GameReady());
    }
    public void ShowWinScreen(bool team1Won) {
        winScreen.SetActive(true);
    }
    public void HideWinScreen() {
        winScreen.SetActive(false);
    }

    IEnumerator GameReady() {
        menuAnim.SetTrigger("Play");
        yield return new WaitForSeconds(.5f);
        mainMenu.SetActive(false);
        GameManager.Instance.StartNewGame();
    }
}
