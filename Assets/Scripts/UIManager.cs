using System.Collections;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour {
    #region Instance
    public static UIManager Instance;
    private void Awake() {
        if (Instance == null) {
            Instance = this;
            menuAnim.SetTrigger("Start");
            MainMenu();
        } else {
            Destroy(this);
        }
    }
    #endregion

    public Animator menuAnim;
    public GameObject mainMenu;
    public GameObject settingsMenu;
    public GameObject shopMenu;
    public GameObject winScreen;
    public TextMeshProUGUI winTxt;
    public TextMeshProUGUI nrWinsTxt;

    public void MainMenu() {
        // Display main menu
        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
        shopMenu.SetActive(false);
        HideWinScreen();
    }
    public void DisplaySettings() {
        // Display/hide settings menu
        AudioManager.Instance.PlayButtonClick();
        settingsMenu.SetActive(!settingsMenu.activeSelf);
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
        StartCoroutine(GameReady());
    }
    public void ShowWinScreen(bool team1Won) {
        winScreen.SetActive(true);
        if (team1Won) {
            winTxt.text = "Team 1 won";
        } else {
            winTxt.text = "Team 2 won";
        }
        nrWinsTxt.text = GameManager.Instance.team1Wins + " - " + GameManager.Instance.team2Wins;
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
