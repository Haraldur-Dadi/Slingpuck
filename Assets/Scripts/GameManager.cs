using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public GameObject puck;
    public Transform[] puckSpawnPoints;

    public List<int> activeTouches;
    public List<Puck> selectedPucks;

    void Awake() {
        foreach (Transform spawnPoint in puckSpawnPoints) {
            Instantiate(puck, spawnPoint.position, Quaternion.identity);
        }

        activeTouches = new List<int>();
        selectedPucks = new List<Puck>();
    }

    private void Update() {
        InputHandler();
    }

    void InputHandler() {
        if (Input.touchCount > 0) {
            // Check if we should add another touch to activeTouches or remove
            foreach (Touch touch in Input.touches) {
                if (touch.phase == TouchPhase.Began) {
                    Vector2 rayPos = Camera.main.ScreenToWorldPoint(touch.position);
                    RaycastHit2D[] hits = Physics2D.RaycastAll(rayPos, Vector2.zero);
                    if (hits.Length > 0) {
                        foreach (RaycastHit2D hit in hits){
                            if (hit.collider.CompareTag("Puck")) {
                                activeTouches.Add(touch.fingerId);
                                selectedPucks.Add(hit.collider.GetComponent<Puck>());
                            }
                        }
                    }
                } else if (touch.phase == TouchPhase.Ended) {
                    if (activeTouches.Contains(touch.fingerId)) {
                        int index = activeTouches.IndexOf(touch.fingerId);
                        selectedPucks[index].StopMove();
                        activeTouches.RemoveAt(index);
                        selectedPucks.RemoveAt(index);
                    }
                } else if (touch.phase == TouchPhase.Moved) {
                    if (activeTouches.Contains(touch.fingerId)) {
                        int index = activeTouches.IndexOf(touch.fingerId);
                        Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
                        selectedPucks[index].ChangePos(touchPos);
                    }
                }
            }
        }
    }
}