using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    #region Instance
    public static GameManager Instance;
    void Awake() {
        if (Instance == null) {
            Instance = this;
            playing = false;
            menuAnim.SetBool("Start", true);
            MainMenu();
        } else {
            Destroy(this);
        }
    }
    #endregion
    public Animator menuAnim; 
    public bool playing;
    public GameObject mainMenu;
    public GameObject settingsMenu;
    public GameObject player2Screen;
    public GameObject player1Screen;

    public GameObject puck;
    public Transform[] puckSpawnPoints;
    public List<Puck> pucksTeam1;
    public List<Puck> pucksTeam2;

    public List<int> activeTouches;
    public List<Puck> selectedPucks;

    #region Main Menu
    public void MainMenu() {
        // Display main menu
        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
        player2Screen.SetActive(false);
        player1Screen.SetActive(false);
    }
    public void BackToMenu() {
        MainMenu();
        menuAnim.SetBool("Start", false);
    }
    public void ShowSettings() {
        // Display settings menu
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }
    public void Player2() {
        // Show screen for player vs player
        AudioManager.Instance.PlayButtonClick();
        StartCoroutine(SwitchPanels());
        player2Screen.SetActive(true);
    }
    public void Player1() {
        // Show screen for player vs ai
        AudioManager.Instance.PlayButtonClick();
        StartCoroutine(SwitchPanels());
        player1Screen.SetActive(true);
    }
    public void PlayerToMainMenu() {
        StartCoroutine(PlayerBackToMainMenu());
    }
    IEnumerator SwitchPanels() {
        menuAnim.SetTrigger("SwitchPanels");
        yield return new WaitForSeconds(.5f);
    }
    IEnumerator PlayerBackToMainMenu() {
        menuAnim.SetTrigger("BackToMenu");
        yield return new WaitForSeconds(.5f);
        BackToMenu();
    }
    public void StartGame() {
        StartCoroutine(GameReady());
    }
    IEnumerator GameReady() {
        menuAnim.SetTrigger("Play");
        yield return new WaitForSeconds(.5f);
        SetUpBoard();
        mainMenu.SetActive(false);
        playing = true;
    }
    #endregion
    #region Gameplay
    void SetUpBoard() {
        foreach (Transform spawnPoint in puckSpawnPoints) {
            Instantiate(puck, spawnPoint.position, Quaternion.identity);
        }

        activeTouches = new List<int>();
        selectedPucks = new List<Puck>();
    }
    void Update() {
        if (playing)
            InputHandler();
    }
    void InputHandler() {
        if (Input.touchCount > 0) {
            // Check if we should add another touch to activeTouches or remove
            foreach (Touch touch in Input.touches) {
                if (touch.phase == TouchPhase.Began) {
                    Vector2 rayPos = Camera.main.ScreenToWorldPoint(touch.position);
                    RaycastHit2D[] hits = Physics2D.RaycastAll(rayPos, Vector2.zero);
                    if (hits.Length > 0) {
                        foreach (RaycastHit2D hit in hits){
                            if (hit.collider.CompareTag("Puck")) {
                                activeTouches.Add(touch.fingerId);
                                selectedPucks.Add(hit.collider.GetComponent<Puck>());
                            }
                        }
                    }
                } else if (touch.phase == TouchPhase.Ended) {
                    if (activeTouches.Contains(touch.fingerId)) {
                        int index = activeTouches.IndexOf(touch.fingerId);
                        selectedPucks[index].StopMove();
                        activeTouches.RemoveAt(index);
                        selectedPucks.RemoveAt(index);
                    }
                } else if (touch.phase == TouchPhase.Moved) {
                    if (activeTouches.Contains(touch.fingerId)) {
                        int index = activeTouches.IndexOf(touch.fingerId);
                        Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
                        selectedPucks[index].ChangePos(touchPos);
                    }
                }
            }
        }
    }
    public void PuckChangeTeam(Puck puck) {
        if (puck.GetTeam()) {
            // Team 1
            if (pucksTeam2.Contains(puck))
                pucksTeam2.Remove(puck);
            pucksTeam1.Add(puck);
        } else {
            // Team 2
            if (pucksTeam1.Contains(puck))
                pucksTeam1.Remove(puck);
            pucksTeam2.Add(puck);
        }
    }
    #endregion
}