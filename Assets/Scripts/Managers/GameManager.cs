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
            pucksToWin = puckSpawnPoints.Length;
            SpawnObjects();
            ChangeFx(true);
        } else {
            Destroy(this);
        }
    }
    #endregion

    public bool playing;
    private int pucksToWin;
    public bool player1;
    public GameObject puck;
    public GameObject rope;
    public Transform[] puckSpawnPoints;
    public List<Puck> pucksTeam1;
    public List<Puck> pucksTeam2;
    private float touchRadius = 0.185f;

    public TextMeshProUGUI finalTxt;
    public TextMeshProUGUI goldEarnedTxt;
    public ParticleSystem snowParticles;
    public TextMeshProUGUI snowAllowed;

    // SlowDown controls
    private float slowDownFactor = 0.05f;
    private float slowDownLength = .1f;

    // Team 1
    private int activeTouchTeam1Id;
    private Puck selectedPuckTeam1;
    // Team 2
    private int activeTouchTeam2Id;
    private Puck selectedPuckTeam2;

    public void SinglePlayer(bool single) {
        player1 = single;
    }
    private void CleanBoard() {
        GameObject[] pucks = GameObject.FindGameObjectsWithTag("Puck");
        foreach (GameObject puck in pucks) {
            Destroy(puck);
        }
        GameObject[] ropes = GameObject.FindGameObjectsWithTag("Rope");
        foreach (GameObject rope in ropes) {
            Destroy(rope);
        }
        SpawnObjects();
    }
    public void ChangePuckAppearance(Sprite s) {
        GameObject[] pucks = GameObject.FindGameObjectsWithTag("Puck");
        foreach (GameObject puck in pucks) {
            puck.GetComponent<SpriteRenderer>().sprite = s; 
        }
    }
    private void SpawnObjects() {
        foreach (Transform spawnPoint in puckSpawnPoints) {
            Instantiate(puck, spawnPoint.position, Quaternion.identity);
        }
        Instantiate(rope, new Vector3(0, 4.25f, 0), Quaternion.identity);
        Instantiate(rope, new Vector3(0, -4.25f, 0), Quaternion.identity);
    }
    public void StartNewGame() {
        UIManager.Instance.HideWinScreen();
        CleanBoard();
        pucksTeam1 = new List<Puck>();
        pucksTeam2 = new List<Puck>();
        ReleasePuck(true);
        ReleasePuck(false);
        playing = true;

        if (player1)
            Ai.Instance.StartAIBeforeGame();
    }
    public void ChangeFx(bool checkPlayerPrefs) {
        if (checkPlayerPrefs) {
            if (PlayerPrefs.GetInt("fxPlaying", 1) == 1) {
                snowParticles.Play();
                snowAllowed.text = "Yes";
            } else {
                snowAllowed.text = "No";
            }
        } else {
            if (snowParticles.isPlaying) {
                // Turn off
                snowParticles.Stop();
                snowParticles.Clear();
                PlayerPrefs.SetInt("fxPlaying", 0);
                snowAllowed.text = "No";
            } else {
                // Turn on
                snowParticles.Play();
                PlayerPrefs.SetInt("fxPlaying", 1);
                snowAllowed.text = "Yes";
            }
        }
    }

    private void Update() {
        if (playing) {
            // Check if we should add another touch to activeTouches or remove
            foreach (Touch touch in Input.touches) {
                if (touch.phase == TouchPhase.Began) {
                    Vector2 rayPos = Camera.main.ScreenToWorldPoint(touch.position);
                    Collider2D[] colliders = Physics2D.OverlapCircleAll(rayPos, touchRadius);
                    foreach (Collider2D collider in colliders) {
                        if (collider.CompareTag("Puck")) {
                            Puck puck = collider.GetComponent<Puck>();
                            // Only allow to hold one puck on each side
                            if (puck.GetTeam() && !selectedPuckTeam1) {
                                activeTouchTeam1Id = touch.fingerId;
                                selectedPuckTeam1 = puck;
                                break;
                            } else if (!puck.GetTeam() && !selectedPuckTeam2 && !player1) {
                                activeTouchTeam2Id = touch.fingerId;
                                selectedPuckTeam2 = puck;
                                break;
                            }
                        }
                    }
                } else if (touch.phase == TouchPhase.Moved) {
                    Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
                    if (touch.fingerId == activeTouchTeam1Id) {
                        selectedPuckTeam1.ChangePos(touchPos);
                    } else if (touch.fingerId == activeTouchTeam2Id) {
                        selectedPuckTeam2.ChangePos(touchPos);
                    }
                } else if (touch.phase == TouchPhase.Ended) {
                    if (touch.fingerId == activeTouchTeam1Id) {
                        ReleasePuck(true);
                    } else if (touch.fingerId == activeTouchTeam2Id) {
                        ReleasePuck(false);
                    }
                }
            }
        }
    }
    public void ReleasePuck(bool team) {
        // Releases and stops moving the puck on either team
        if (team) {
            if (selectedPuckTeam1)
                selectedPuckTeam1.StopMove();
            activeTouchTeam1Id = -1;
            selectedPuckTeam1 = null;
        } else {
            if (selectedPuckTeam2)
                selectedPuckTeam2.StopMove();
            activeTouchTeam2Id = -1;
            selectedPuckTeam2 = null;
        }
    }
    public void PuckChangeTeam(Puck puck) {
        if (puck.GetTeam()) {
            // Change to Team 1
            pucksTeam2.Remove(puck);
            if (!pucksTeam1.Contains(puck))
                pucksTeam1.Add(puck);
        } else {
            // Change to Team 2
            pucksTeam1.Remove(puck);
            if (!pucksTeam2.Contains(puck))
                pucksTeam2.Add(puck);
        }
        CheckWin();
        AudioManager.Instance.ChangeMusicPitch(pucksTeam1.Count, pucksTeam2.Count);
    }
    private void CheckWin() {
        if (pucksTeam2.Count == pucksToWin) {
            // Team 1 wins
            GameOver(true);
        } else if (pucksTeam1.Count == pucksToWin) {
            // Team 2 wins
            GameOver(false);
        }
    }
    private void GameOver(bool team1Won) {
        playing = false;
        if (player1 && team1Won) {
            CurrencyManager.Instance.AddGold(10);
            Ai.Instance.ChangeDifficulty(true);
            finalTxt.text = "You win";
        } else {
            if (player1) {
                Ai.Instance.ChangeDifficulty(team1Won);
                finalTxt.text = "You loose :(";
            } else {
                finalTxt.text = (team1Won) ? "Team 1 wins" : "Team 2 wins";
            }
            CurrencyManager.Instance.AddGold(5);
        }
        goldEarnedTxt.text = (player1 && team1Won) ? "+10" : "+5";
        StartCoroutine(SlowMoEnding(team1Won));
    }
    private IEnumerator SlowMoEnding(bool team1Won) {
        // Slows down time, then speeds it back up
        float initalValue = Time.timeScale;
        Time.timeScale = slowDownFactor;
        Time.fixedDeltaTime = Time.timeScale * .02f;
        yield return new WaitForSeconds(slowDownLength);

        Time.timeScale = initalValue;
        Time.fixedDeltaTime = Time.timeScale * .02f;
        AdManager.Instance.ShowVideoAd();
        UIManager.Instance.ShowWinScreen(team1Won);
    }
}