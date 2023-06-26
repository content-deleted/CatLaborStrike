using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour {
    public LevelRenderer levelRenderer;

    public GenericDictionary<string, GameObject> prefabs;

    void Start() {
        var l = new Level("levels/tutorial");

        levelRenderer.RenderLevel(l);
    }
}
