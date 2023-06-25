using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour {
    public LevelRenderer levelRenderer;

    void Start() {
        var l = new Level();
        l.name = "Test Level";
        l.playerStartPosition = new Vector3Int(0, 0, 0);
        l.tiles = new List<LevelTile>();

        for (var x = -5; x <= 5; ++x) {
            for (var y = -5; y <= 5; ++y) {
                l.tiles.Add(new LevelTile(new Vector3Int(x, y, -1), "floor"));
            }
        }

        levelRenderer.RenderLevel(l);
    }
}
