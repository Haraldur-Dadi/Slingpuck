using UnityEngine;

public class Puck : MonoBehaviour {
    Rigidbody2D rb;
    SpriteRenderer image;
    Vector2 posToMove;
    bool team1;
    bool moving;
    public PhysicsMaterial2D bounce;
    public ContactFilter2D contactFilter;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        image = GetComponent<SpriteRenderer>();
        moving = false;
    }

    void FixedUpdate() {
        if (moving)
            Move();
    }

    void Move() {
        rb.MovePosition(posToMove);
    }

    public void ChangePos(Vector2 pos) {
        rb.sharedMaterial = null;
        moving = true;
        posToMove = pos;

        if (team1) {
            posToMove.y = Mathf.Clamp(posToMove.y, -Mathf.Infinity, -transform.localScale.y/2);
        } else {
            posToMove.y = Mathf.Clamp(posToMove.y, transform.localScale.y/2, Mathf.Infinity);
        }
    }
    public void StopMove() {
        rb.sharedMaterial = bounce;
        moving = false;
        if (!rb.IsTouching(contactFilter)) {
            rb.velocity = Vector2.zero;
        }
    }
    public Vector2 GetPos() {
        return new Vector2(transform.position.x, transform.position.y);
    }
    public void SetTeam(bool team) {
        team1 = team;
        GameManager.Instance.PuckChangeTeam(this);
    }
    public bool GetTeam() {
        return team1;
    }
    public void ChangeColor(Color color) {
        if (team1) {
            image.color = color;
        } else {
            image.color = color;
        }
    }
}
