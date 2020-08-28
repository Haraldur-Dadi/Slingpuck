using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour {
    #region Instance
    public static GameManager Instance;
    void Awake() {
        if (Instance == null) {
            Instance = this;
            playing = false;
            HideWinScreen();
        } else {
            Destroy(this);
        }
    }
    #endregion
    bool playing;
    bool player1;
    public GameObject puck;
    public Transform[] puckSpawnPoints;
    List<Puck> pucksTeam1;
    List<Puck> pucksTeam2;

    List<int> activeTouches;
    List<Puck> selectedPucks;
    float touchRadius = 0.15f;

    public GameObject winScreen;
    public TextMeshProUGUI winTxt;
    public TextMeshProUGUI nrWinsTxt;
    int team1Wins;
    int team2Wins;

    Color player1Color;
    Color player2Color;

    #region Setup
    public void SetPlayer1Color(Color color) {
        player1Color = color;
    }
    public void SetPlayer2Color(Color color) {
        player2Color = color;
    }
    public void SinglePlayer(bool single) {
        player1 = single;
    }
    void CleanBoard() {
        GameObject[] pucks = GameObject.FindGameObjectsWithTag("Puck");
        foreach (GameObject puck in pucks) {
            Destroy(puck);
        }
        HideWinScreen();
    }
    public void StartNewGame() {
        team1Wins = 0;
        team2Wins = 0;
        SetUpBoard();
    }
    public void Rematch() {
        SetUpBoard();
    }
    void SetUpBoard() {
        CleanBoard();
        activeTouches = new List<int>();
        selectedPucks = new List<Puck>();
        pucksTeam1 = new List<Puck>();
        pucksTeam2 = new List<Puck>();
        
        foreach (Transform spawnPoint in puckSpawnPoints) {
            Instantiate(puck, spawnPoint.position, Quaternion.identity);
        }
        playing = true;
    }
    #endregion
    #region Gameplay
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
                    Collider2D[] colliders = Physics2D.OverlapCircleAll(rayPos, touchRadius);
                    if (colliders.Length > 0) {
                        foreach (Collider2D collider in colliders){
                            if (collider.CompareTag("Puck")) {
                                if (player1) {
                                    // Don't allow single player to touch opposite puck
                                    Puck puck = collider.GetComponent<Puck>();
                                    if (puck.GetTeam()) {
                                        activeTouches.Add(touch.fingerId);
                                        selectedPucks.Add(puck);
                                        break;
                                    }
                                } else {
                                    activeTouches.Add(touch.fingerId);
                                    selectedPucks.Add(collider.GetComponent<Puck>());
                                    break;
                                }
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
            puck.ChangeColor(player1Color);
        } else {
            // Team 2
            if (pucksTeam1.Contains(puck))
                pucksTeam1.Remove(puck);
            pucksTeam2.Add(puck);
            puck.ChangeColor(player2Color);
        }

        if (playing)
            CheckWin();
    }
    void CheckWin() {
        if (pucksTeam2.Count == 10) {
            // Team 1 wins
            playing = false;
            ShowWinScreen(true);
        } else if (pucksTeam1.Count == 10) {
            // Team 2 wins
            playing = false;
            ShowWinScreen(false);
        }
    }
    #endregion
    public void HideWinScreen() {
        winScreen.SetActive(false);
    }
    void ShowWinScreen(bool team1Won) {
        winScreen.SetActive(true);
        if (team1Won) {
            winTxt.text = "Team 1 won";
            team1Wins += 1;
        } else {
            winTxt.text = "Team 2 won";
            team2Wins += 1;
        }
        nrWinsTxt.text = team1Wins + " - " + team2Wins;
    }
}