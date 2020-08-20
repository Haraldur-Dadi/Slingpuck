using UnityEngine;

public class Puck : MonoBehaviour {
    Rigidbody2D rb;
    Vector2 posToMove;
    bool shouldMove;
    bool team1;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        shouldMove = false;
    }

    void FixedUpdate() {
        if (shouldMove)
            Move();
    }

    void Move() {
        rb.MovePosition(posToMove);
    }

    public void ChangePos(Vector2 pos) {
        shouldMove = true;
        posToMove = pos;

        if (team1) {
            posToMove.y = Mathf.Clamp(posToMove.y, -Mathf.Infinity, -transform.localScale.y/2);
        } else {
            posToMove.y = Mathf.Clamp(posToMove.y, transform.localScale.y/2, Mathf.Infinity);
        }
    }
    public void StopMove() {
        shouldMove = false;
        rb.velocity = Vector2.zero;
    }
    public void SetTeam(bool team) {
        if (!shouldMove)
            team1 = team;
    }
}
