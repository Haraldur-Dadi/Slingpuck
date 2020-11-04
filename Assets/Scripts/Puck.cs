using UnityEngine;

public class Puck : MonoBehaviour {
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector2 posToMove;
    private bool team1;
    private bool moving = false;
    public PhysicsMaterial2D bounce;
    public ContactFilter2D contactFilter;
    
    public bool touchingRope;

    float hitTime = .75f;
    float hitTimer;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = ItemDb.Instance.GetEquippedPuck();
        touchingRope = false;
    }

    private void FixedUpdate() {
        if (moving)
            Move();
        hitTimer += Time.deltaTime;
    }
    private void Move() {
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
        } else {
            AudioManager.Instance.PlaySlingshotRelease();
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
    private void OnCollisionEnter2D(Collision2D other) {
        if (other.relativeVelocity.magnitude > 1.5f && hitTimer > hitTime) {
            if (other.gameObject.CompareTag("Puck") || other.gameObject.CompareTag("Border")) {
                AudioManager.Instance.PlayPuckCollission();
                hitTimer = 0f;
                GameManager.Instance.ReleasePuck(team1);
            }
        }
    }
}
