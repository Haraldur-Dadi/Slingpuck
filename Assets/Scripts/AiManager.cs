using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AiManager : MonoBehaviour {
    bool canPickup;
    bool start;
    Vector2 optimalShotZone;
    float secBetweenMoves;
    float moveInterval;
    float moveIntervalMax = 1f;
    float moveSpeed;
    Vector2 startPos;
    Vector2 movePos;
    Puck selectedPuck;
    public string difficulty;

    public Button easyDifficultyBtn;
    public Button mediumDifficultyBtn;
    public Button hardDifficultyBtn;

    #region Setup
    private void Start() {
        SelectEasyDifficulty();
    }
    public void SelectEasyDifficulty() {
        difficulty = "easy";

        optimalShotZone = new Vector2(0.65f, 4.55f);
        optimalShotZone.x = 0.75f;
        moveSpeed = 3.75f;
        secBetweenMoves = 3.5f;
        easyDifficultyBtn.interactable = false;
        mediumDifficultyBtn.interactable = true;
        hardDifficultyBtn.interactable = true;
    }
    public void SelectMediumDifficulty() {
        difficulty = "medium";

        optimalShotZone = new Vector2(0.5f, 4.75f);
        moveSpeed = 5f;
        secBetweenMoves = 2.75f;
        easyDifficultyBtn.interactable = true;
        mediumDifficultyBtn.interactable = false;
        hardDifficultyBtn.interactable = true;
    }
    public void SelectHardDifficulty() {
        difficulty = "hard";

        optimalShotZone = new Vector2(0.25f, 5f);
        moveSpeed = 6.5f;
        secBetweenMoves = 2f;
        easyDifficultyBtn.interactable = true;
        mediumDifficultyBtn.interactable = true;
        hardDifficultyBtn.interactable = false;
    }
    public void StartAIBeforeGame() {
        start = true;
        ReleasePuck();
    }
    #endregion

    #region GamePlay
    void FixedUpdate() {
        // AI for singleplayer
        if (GameManager.Instance.playing && GameManager.Instance.player1 && canPickup) {
            if (selectedPuck == null) {
                PickUpPuck();
            } else {
                if (movePos == Vector2.zero) {
                    ChoosePosToMove();
                } else if (moveInterval <= moveIntervalMax) {
                    MovePuck();
                } else {
                    ReleasePuck();
                }
            }
        }
    }
    void PickUpPuck() {
        // Randomly chooses a puck on his half
        if (GameManager.Instance.pucksTeam2.Count > 0) {
            // Pucks available
            if (start) {
                // Pick middle puck when starting
                start = false;
                selectedPuck = GameManager.Instance.pucksTeam2[2];
            } else {
                // Pick random puck on ai half of board
                int index = Random.Range(0, GameManager.Instance.pucksTeam2.Count - 1);
                selectedPuck = GameManager.Instance.pucksTeam2[index];
            }
            // Setup
            startPos = selectedPuck.GetPos();
            movePos = Vector2.zero;
        }
    }
    void ChoosePosToMove() {
        // Chooses position to move the puck towards
        movePos = optimalShotZone;
        movePos.x = Random.Range(-optimalShotZone.x, optimalShotZone.x);
        moveInterval = 0f;
    }
    void MovePuck() {
        // Moves selected puck towards movePos
        Vector2 tmpMovePos = Vector2.Lerp(startPos, movePos, moveInterval);
        moveInterval += Time.fixedDeltaTime * moveSpeed;
        selectedPuck.ChangePos(tmpMovePos);
    }
    void ReleasePuck() {
        // Releases puck and starts wait between moves
        if (selectedPuck)
            selectedPuck.StopMove();
        canPickup = false;
        selectedPuck = null;
        StartCoroutine(WaitBetweenMoves());
    }
    IEnumerator WaitBetweenMoves() {
        yield return new WaitForSeconds(secBetweenMoves);
        canPickup = true;
    }
    #endregion
}
