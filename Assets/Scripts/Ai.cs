﻿using System.Collections;
using UnityEngine;

public class Ai : MonoBehaviour {
    #region Instance
    public static Ai Instance;
    private void Awake() {
        if (Instance == null) {
            Instance = this;
            PlayerInfo info = SaveManager.LoadPlayerInfo();
            skillRank = (info is null) ? 0.0f : info.skillRank;
        } else {
            Destroy(this);
        }
    }
    #endregion

    private bool canPickup;
    public float skillRank;
    private float moveInterval;
    private float moveIntervalMax = 1f;
    private Vector2 startPos;
    private Vector2 movePos;
    private Puck selectedPuck;
    public LayerMask puckMask;

    // Base values
    private Vector2 baseOptimalShotZone = new Vector2(0.65f, 4.5f);
    private float baseMoveSpeed = 3.5f;
    private float baseSecBetweenMoves = 3.5f;
    // Determined by skill rank
    public Vector2 optimalShotZone;
    public float moveSpeed;
    public float secBetweenMoves;

    public float SkillRank() { return skillRank; }
    public void ChangeDifficulty(bool playerWon) {
        // Change skill rank by +-0.2 based on if he won/lost
        skillRank += (playerWon) ? 0.2f : -0.2f;
        skillRank = Mathf.Clamp(skillRank, 0f, 10f);
        SaveManager.SavePlayerInfo();
    }
    public void SelectDifficulty() {
        // Skill rank is earned by beating the ai, and can be at most 10
        optimalShotZone = new Vector2(baseOptimalShotZone.x - ((skillRank/10f)*5f), baseOptimalShotZone.y + (skillRank/10f));
        moveSpeed = baseMoveSpeed + (skillRank/4f);
        secBetweenMoves = baseSecBetweenMoves - (skillRank/5f);
    }
    public void StartAIBeforeGame() {
        SelectDifficulty();
        ReleasePuck();
    }
    private Puck PuckInWay(Vector2 startPos, Vector2 endPos) {
        RaycastHit2D hit = Physics2D.Linecast(startPos, endPos, puckMask);
        if (hit) {
            Puck hitPuck = hit.collider.GetComponent<Puck>();
            if (GameManager.Instance.pucksTeam2.Contains(hitPuck))
                return hitPuck;
        }
        return null;
    }
    private void ChoosePosToMove() {
        // Chooses position to move the puck towards
        movePos = optimalShotZone;
        movePos.x = Random.Range(-optimalShotZone.x, optimalShotZone.x);
    }
    private void FixedUpdate() {
        // AI for singleplayer
        if (GameManager.Instance.playing && GameManager.Instance.player1 && canPickup) {
            if (selectedPuck == null) {
                ChoosePosToMove();
                PickUpPuck();
                moveInterval = 0f;
            } else {
                if (moveInterval <= moveIntervalMax) {
                    MovePuck();
                } else {
                    ReleasePuck();
                }
            }
        }
    }
    private void PickUpPuck() {
        // Choose a puck on ai half
        if (GameManager.Instance.pucksTeam2.Count > 0) {
            // Check if there is a puck blocking the middle, then choose it
            Puck tmpPuck = PuckInWay(Vector2.zero, movePos);
            if (tmpPuck != null) {
                selectedPuck = tmpPuck;
            } else {
                // Pick a random puck
                int index = Random.Range(0, GameManager.Instance.pucksTeam2.Count - 1);
                Puck tmpSelectedPuck = GameManager.Instance.pucksTeam2[index];
                Puck puckInWay = PuckInWay(movePos, tmpSelectedPuck.GetPos());

                if (tmpSelectedPuck == puckInWay) {
                    selectedPuck = tmpSelectedPuck;
                } else {
                    selectedPuck = puckInWay;
                }
            }
            startPos = selectedPuck.GetPos();
        }
    }
    private void MovePuck() {
        // Moves selected puck towards movePos
        Vector2 tmpMovePos = Vector2.Lerp(startPos, movePos, moveInterval);
        moveInterval += Time.fixedDeltaTime * moveSpeed;
        selectedPuck.ChangePos(tmpMovePos);
    }
    private void ReleasePuck() {
        // Releases puck and starts wait between moves
        if (selectedPuck)
            selectedPuck.StopMove();
        canPickup = false;
        selectedPuck = null;
        StartCoroutine(WaitBetweenMoves());
    }
    private IEnumerator WaitBetweenMoves() {
        yield return new WaitForSeconds(secBetweenMoves);
        canPickup = true;
    }
}