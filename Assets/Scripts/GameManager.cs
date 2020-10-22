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
            pucksToWin = puckSpawnPoints.Length;
        } else {
            Destroy(this);
        }
    }
    #endregion

    public bool playing;
    public bool player1;
    public GameObject puck;
    public Transform[] puckSpawnPoints;
    public List<Puck> pucksTeam1;
    public List<Puck> pucksTeam2;

    List<int> activeTouches;
    List<Puck> selectedPucks;
    float touchRadius = 0.15f;

    public int team1Wins;
    public int team2Wins;
    int pucksToWin;

    #region Setup
    public void SinglePlayer(bool single) {
        player1 = single;
    }
    void CleanBoard() {
        GameObject[] pucks = GameObject.FindGameObjectsWithTag("Puck");
        foreach (GameObject puck in pucks) {
            Destroy(puck);
        }
        UIManager.Instance.HideWinScreen();
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
            AiManager.Instance.StartAIBeforeGame();
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
        if (puck.GetTeam()) {
            // Change to Team 1
            pucksTeam2.Remove(puck); // Removes from team 2 if it's there
            if (!pucksTeam1.Contains(puck)) {
                pucksTeam1.Add(puck);
            }
        } else {
            // Change to Team 2
            pucksTeam1.Remove(puck); // Removes from team 1 if it's there
            if (!pucksTeam2.Contains(puck)) {
                pucksTeam2.Add(puck);
            }
        }
        CheckWin();
    }
    void CheckWin() {
        if (pucksTeam2.Count == pucksToWin) {
            // Team 1 wins
            playing = false;
            team1Wins += 1;
            UIManager.Instance.ShowWinScreen(true);
        } else if (pucksTeam1.Count == pucksToWin) {
            // Team 2 wins
            playing = false;
            team2Wins += 1;
            UIManager.Instance.ShowWinScreen(false);
        }
    }
    #endregion

    void ReportGameFinishedAnalytics(bool team1Won) {
        if (player1) {
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