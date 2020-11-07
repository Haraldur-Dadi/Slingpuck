using UnityEngine;
using UnityEngine.UI;

public class CanvasScaling : MonoBehaviour {
    public CanvasScaler canvasScaler;
    void Awake () {
        float aspect = (float)Screen.height / (float)Screen.width; // Portrait
        if (aspect >= 1.87) {
            // 19.5:9
            canvasScaler.matchWidthOrHeight = 0.5f;
            Camera.main.orthographicSize = 5.85f;
        } else if (aspect >= 1.74) {
            // 16:9
            canvasScaler.matchWidthOrHeight = 0;
            Camera.main.orthographicSize = 5.75f;
        } else if (aspect >= 1.5) {
            // 3:2
            canvasScaler.matchWidthOrHeight = 1;
            Camera.main.orthographicSize = 5.35f;
        } else {
            // 4:3
            canvasScaler.matchWidthOrHeight = 1f;
            Camera.main.orthographicSize = 5.5f;
        }
    }
}
