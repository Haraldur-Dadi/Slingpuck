using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AiManager : MonoBehaviour {
    bool canPickup;
    Puck selectedPuck;
    Vector2 optimalShotZone;
    Vector2 startPos;
    Vector2 movePos;
    public LayerMask puckMask;
    float secBetweenMoves;
    float moveInterval;
    float moveIntervalMax = 1f;
    float moveSpeed;
    public string difficulty;

    public Button easyDifficultyBtn;
    public Button mediumDifficultyBtn;
    public Button hardDifficultyBtn;

    #region Setup
    private void Start() {
        // Prevent button click to play at start of the game
        SelectEasyDifficulty();
        easyDifficultyBtn.onClick.AddListener(delegate() { AudioManager.Instance.PlayButtonClick(); });
        mediumDifficultyBtn.onClick.AddListener(delegate() { AudioManager.Instance.PlayButtonClick(); });
        hardDifficultyBtn.onClick.AddListener(delegate() { AudioManager.Instance.PlayButtonClick(); });
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
        ReleasePuck();
    }
    #endregion

    #region GamePlay
    Puck PuckInWay(Vector2 startPos, Vector2 endPos) {
        RaycastHit2D hit = Physics2D.Linecast(startPos, endPos, puckMask);
        if (hit) {
            Puck hitPuck = hit.collider.GetComponent<Puck>();
            if (GameManager.Instance.pucksTeam2.Contains(hitPuck))
                return hitPuck;
        }
        return null;
    }
    void ChoosePosToMove() {
        // Chooses position to move the puck towards
        movePos = optimalShotZone;
        movePos.x = Random.Range(-optimalShotZone.x, optimalShotZone.x);
    }
    void FixedUpdate() {
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
    void PickUpPuck() {
        // Randomly chooses a puck on his half
        Puck tmpPuck = PuckInWay(Vector2.zero, movePos);
        if (tmpPuck != null) {
            // Check if there is a puck blocking the middle, then choose it
            selectedPuck = tmpPuck;
        } else {
            // Pick a random puck
            int index = Random.Range(0, GameManager.Instance.pucksTeam2.Count - 1);
            Puck tmpSelectedPuck = GameManager.Instance.pucksTeam2[index];
            Puck puckInWay = PuckInWay(movePos, tmpSelectedPuck.transform.position);
            if (tmpSelectedPuck == puckInWay) {
                selectedPuck = tmpSelectedPuck;
            } else {
                selectedPuck = puckInWay;
            }
        }
        startPos = selectedPuck.GetPos();
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