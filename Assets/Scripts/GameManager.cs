using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
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
    public AiManager ai;
    public bool playing;
    public bool player1;
    public GameObject puck;
    public Transform[] puckSpawnPoints;
    List<Puck> pucksTeam1;
    public List<Puck> pucksTeam2;

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

        if (player1)
            GetComponent<AiManager>().StartAIBeforeGame();
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
                                Puck puck = collider.GetComponent<Puck>();

                                // Don't allow single player to touch opposite puck
                                if (player1 && !puck.GetTeam())
                                    break;

                                activeTouches.Add(touch.fingerId);
                                selectedPucks.Add(puck);
                                break;
                            }
                        }
                    }
                } else if (touch.phase == TouchPhase.Ended) {
                    int index = activeTouches.IndexOf(touch.fingerId);
                    if (index != -1) {
                        selectedPucks[index].StopMove();
                        activeTouches.RemoveAt(index);
                        selectedPucks.RemoveAt(index);
                    }
                } else if (touch.phase == TouchPhase.Moved) {
                    int index = activeTouches.IndexOf(touch.fingerId);
                    if (index != -1) {
                        Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
                        selectedPucks[index].ChangePos(touchPos);
                    }
                }
            }
        }
    }
    public void PuckChangeTeam(Puck puck) {
        if (puck.GetTeam()) { // Team 1
            pucksTeam2.Remove(puck); // Remove from team 2 if there

            if (!pucksTeam1.Contains(puck)) {
                // Not already in team 1
                pucksTeam1.Add(puck);
                puck.ChangeColor(player1Color);
                CheckWin();
            }
        } else { // Team 2
            pucksTeam1.Remove(puck); // Remove from team 1 if there

            if (!pucksTeam2.Contains(puck)) {
                // Not already in team 2
                pucksTeam2.Add(puck);
                puck.ChangeColor(player2Color);
                CheckWin();
            }
        }
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
        ReportGameFinishedAnalytics(team1Won);
    }

    void ReportGameFinishedAnalytics(bool team1Won) {
        if (player1) {
            Analytics.CustomEvent("1_player_finished", new Dictionary<string, object> { {"difficulty", ai.difficulty} } );
            if (team1Won) {
                Analytics.CustomEvent("1_player_won", new Dictionary<string, object> { {"won", true} } );
            } else {
                Analytics.CustomEvent("1_player_won", new Dictionary<string, object> { {"won", false} } );
            }
        } else {
            Analytics.CustomEvent("2_player_finished", new Dictionary<string, object> { {"finished", true} } );
        }
    }
}