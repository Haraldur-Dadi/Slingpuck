using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour {
    public Animator menuAnim; 
    public GameObject mainMenu;
    public GameObject settingsMenu;
    public Button[] colorBtns;
    public TextMeshProUGUI colorTxt;
    bool players1;
    bool player1Color;

    void Start() {
        MainMenu();
        menuAnim.SetBool("Start", true);
    }

    public void MainMenu() {
        // Display main menu
        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
        menuAnim.SetBool("Start", false);
    }
    public void BackToMenu() {
        AudioManager.Instance.PlayButtonClick();
        MainMenu();
    }
    public void ShowSettings() {
        // Display settings menu
        AudioManager.Instance.PlayButtonClick();
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }
    public void ShowColorScreen(bool nrPlayers1) {
        colorTxt.text = "Player 1 color:";
        players1 = nrPlayers1;
        GameManager.Instance.SinglePlayer(nrPlayers1);
        player1Color = false;
        foreach (Button btn in colorBtns) {
            btn.interactable = true;
        }
        StartCoroutine(SwitchPanels());
    }
    public void PlayerToMainMenu() {
        StartCoroutine(PlayerBackToMainMenu());
    }
    IEnumerator SwitchPanels() {
        AudioManager.Instance.PlayButtonClick();
        menuAnim.SetTrigger("SwitchPanels");
        yield return new WaitForSeconds(.5f);
    }
    IEnumerator PlayerBackToMainMenu() {
        AudioManager.Instance.PlayButtonClick();
        GameManager.Instance.HideWinScreen();
        menuAnim.SetTrigger("BackToMenu");
        yield return new WaitForSeconds(.5f);
        MainMenu();
    }
    public void SetPlayerColor(int btnIndex) {
        AudioManager.Instance.PlayButtonClick();
        if (players1) {
            GameManager.Instance.SetPlayer1Color(colorBtns[btnIndex].GetComponent<Image>().color);
            GameManager.Instance.SetPlayer2Color(Color.grey);
            StartCoroutine(GameReady());
        } else {
            if (!player1Color) {
                GameManager.Instance.SetPlayer1Color(colorBtns[btnIndex].GetComponent<Image>().color);
                colorBtns[btnIndex].interactable = false;
                player1Color = true;
                colorTxt.text = "Player 2 color:";
            } else {
                GameManager.Instance.SetPlayer2Color(colorBtns[btnIndex].GetComponent<Image>().color);
                StartCoroutine(GameReady());
            }
        }
    }
    IEnumerator GameReady() {
        menuAnim.SetTrigger("Play");
        yield return new WaitForSeconds(.5f);
        menuAnim.SetTrigger("Hide");
        GameManager.Instance.StartNewGame();
    }
}
