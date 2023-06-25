using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTile {
    public Vector3Int position;
    public string type;

    public LevelTile(Vector3Int position, string type) {
        this.position = position;
        this.type = type;
    }
}

public class Level {
    public string name;

    public Vector3Int playerStartPosition;
    public List<LevelTile> tiles;
}