using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AiManager : MonoBehaviour {
    #region Instance
    public static AiManager Instance;
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(this);
        }
    }
    #endregion

    bool canPickup;

    float moveInterval;
    float moveIntervalMax = 1f;
    Vector2 startPos;
    Vector2 movePos;
    Puck selectedPuck;
    public LayerMask puckMask;

    // Base values
    Vector2 baseOptimalShotZone = new Vector2(0.65f, 4.5f);
    float baseMoveSpeed = 3.5f;
    float baseSecBetweenMoves = 3.5f;
    // Determined by skill rank
    Vector2 optimalShotZone;
    float moveSpeed;
    float secBetweenMoves;

    public void SelectDifficulty() {
        // Skill rank is earned by beating the ai, and can be at most 10
        float skillRank = PlayerPrefs.GetFloat("SkillRank", 0f);

        optimalShotZone = new Vector2(baseOptimalShotZone.x - ((skillRank/10f)*5f), baseOptimalShotZone.y + (skillRank/10f));
        moveSpeed = baseMoveSpeed + skillRank;
        secBetweenMoves = baseSecBetweenMoves - skillRank;
    }
    public void StartAIBeforeGame() {
        SelectDifficulty();
        ReleasePuck();
    }
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
}
