using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeLine : MonoBehaviour {
    // Draws a line to the targets
    private Transform[] target;
    private LineRenderer line;

    private void Start() {
        int children = transform.childCount;
        target = new Transform[children];
        for (int i = 0; i < children; ++i) {
            target[i] = transform.GetChild(i);
        }

        line = GetComponent<LineRenderer>();
        line.positionCount = children;
    }

    private void LateUpdate() {
        for (int i = 0; i < target.Length; ++i) {
            line.SetPosition(i, target[i].localPosition);
        }
    }
}
