using UnityEngine;

public class Puck : MonoBehaviour {
    Rigidbody2D rb;
    SpriteRenderer image;
    Vector2 posToMove;
    bool shouldMove;
    bool team1;

    public Material team1Mat;
    public Material team2Mat;
    public PhysicsMaterial2D bounce;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        image = GetComponent<SpriteRenderer>();
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
        rb.sharedMaterial = null;
        posToMove = pos;

        if (team1) {
            posToMove.y = Mathf.Clamp(posToMove.y, -Mathf.Infinity, -transform.localScale.y/2);
        } else {
            posToMove.y = Mathf.Clamp(posToMove.y, transform.localScale.y/2, Mathf.Infinity);
        }
    }
    public void StopMove() {
        shouldMove = false;
        rb.sharedMaterial = bounce;
        rb.velocity = Vector2.zero;
    }
    public void SetTeam(bool team) {
        if (!shouldMove) {
            team1 = team;
            ChangeMaterial();
        }
    }
    public bool GetTeam() {
        return team1;
    }
    private void ChangeMaterial() {
        if (team1) {
            image.material = team1Mat;
        } else {
            image.material = team2Mat;
        }
    }
}
