using UnityEngine;

public class PlayerSides : MonoBehaviour {
    public bool player1Side;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Puck"))
            other.GetComponent<Puck>().SetTeam(player1Side);
    }
}
