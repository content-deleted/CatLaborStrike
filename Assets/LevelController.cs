using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour {
    public LevelRenderer levelRenderer;

    void Start() {
        var l = new Level("levels/level1");

        levelRenderer.RenderLevel(l);
    }
}
