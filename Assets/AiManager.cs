using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AiManager : MonoBehaviour {
    bool canPickup;
    bool start;
    Vector2 optimalShotZone;
    public float waitSec;
    public float moveInterval;
    public float moveIntervalMax;
    public float moveSpeed;
    public float shotSpeed;
    Vector2 startPos;
    Vector2 movePos;
    Puck selectedPuck;

    public Button easyDifficultyBtn;
    public Button mediumDifficultyBtn;
    public Button hardDifficultyBtn;

    private void Start() {
        SelectEasyDifficulty();
    }

    public void SelectEasyDifficulty() {
        optimalShotZone = new Vector2(0.75f, 4.5f);
        moveSpeed = 3.5f;
        moveIntervalMax = 2.25f;
        waitSec = 2.75f;
        shotSpeed = 0.25f;
        easyDifficultyBtn.interactable = false;
        mediumDifficultyBtn.interactable = true;
        hardDifficultyBtn.interactable = true;
    }
    public void SelectMediumDifficulty() {
        optimalShotZone = new Vector2(0.5f, 4.5f);
        moveSpeed = 5f;
        moveIntervalMax = 1.85f;
        waitSec = 2.45f;
        shotSpeed = 0.35f;
        easyDifficultyBtn.interactable = true;
        mediumDifficultyBtn.interactable = false;
        hardDifficultyBtn.interactable = true;
    }
    public void SelectHardDifficulty() {
        optimalShotZone = new Vector2(0.25f, 4.5f);
        moveSpeed = 6.5f;
        moveIntervalMax = 1.25f;
        waitSec = 1.75f;
        shotSpeed = 0.4f;
        easyDifficultyBtn.interactable = true;
        mediumDifficultyBtn.interactable = true;
        hardDifficultyBtn.interactable = false;
    }

    public void StartAIBeforeGame() {
        start = true;
        canPickup = false;
        StartCoroutine(WaitBetweenMoves());
    }

    void FixedUpdate() {
        // AI for singleplayer
        if (GameManager.Instance.playing && GameManager.Instance.player1 && canPickup) {
            if (selectedPuck == null) {
                PickUpPuck();
            } else {
                if (movePos == Vector2.zero) {
                    ChoosePosToMove();
                } else {
                    MovePuck();
                }
            }
        }
    }

    void PickUpPuck() {
        // Randomly chooses a puck on his half
        if (GameManager.Instance.pucksTeam2.Count > 0) {
            if (start) {
                start = false;
                selectedPuck = GameManager.Instance.pucksTeam2[2]; // middle puck
            } else {
                int index = Random.Range(0, GameManager.Instance.pucksTeam2.Count - 1);
                selectedPuck = GameManager.Instance.pucksTeam2[index];
            }
            startPos = selectedPuck.GetPos();
            movePos = Vector2.zero;
        }
    }

    void ChoosePosToMove() {
        // Chooses 
        movePos = optimalShotZone;
        movePos.x = Random.Range(-optimalShotZone.x, optimalShotZone.x);
        Debug.Log(movePos.x);
        moveInterval = 0f;
    }

    void MovePuck() {
        // Moves selected puck towards movePos
        if (moveInterval >= moveIntervalMax) {
            ShootPuck();
        } else {
            Vector2 tmpMovePos = Vector2.Lerp(startPos, movePos, moveInterval);
            moveInterval += Time.fixedDeltaTime * moveSpeed;
            selectedPuck.ChangePos(tmpMovePos);
        }
    }

    void ShootPuck() {
        // Drags puck slightly to create tension on "rope"
        movePos.y -= shotSpeed;
        selectedPuck.ChangePos(movePos);
        ReleasePuck();
    }

    void ReleasePuck() {
        // Releases puck
        if (selectedPuck)
            selectedPuck.StopMove();
        canPickup = false;
        selectedPuck = null;
        StartCoroutine(WaitBetweenMoves());
    }

    IEnumerator WaitBetweenMoves() {
        yield return new WaitForSeconds(waitSec);
        canPickup = true;
    }
}
