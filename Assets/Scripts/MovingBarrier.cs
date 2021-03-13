using UnityEngine;

public class MovingBarrier : MonoBehaviour {
    private Vector3 target;
    public float speed = 0.5f;

    private void Start() {
        target = new Vector3(0.75f, 0f, 0f);
    }

    private void LateUpdate() {
        if (GameManager.Instance.playing) {
            // Move side to side
            transform.position = Vector3.MoveTowards(transform.position, target, speed*Time.deltaTime);

            if (Vector3.Distance(transform.position, target) < 0.001f) {
                // Swap the position of the cylinder.
                target.x *= -1f;
            }
        }
    }
}