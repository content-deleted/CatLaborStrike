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

    public Level() { }

    public Level(string filePath) {
        name = filePath;

        tiles = new List<LevelTile>();

        var lines = Resources.Load<TextAsset>(filePath).text.Split('\n');
        foreach (var rawLine in lines) {
            var line = rawLine.Trim();
            if (line == "") {
                continue;
            }

            var parts = line.Split(' ');

            var xRange = ParseRange(parts[0]);
            var zRange = ParseRange(parts[1]);
            var yRange = ParseRange(parts[2]);
            var type = parts[3];

            if (type == "player") {
                playerStartPosition = new Vector3Int(
                    xRange.Item1,
                    yRange.Item1,
                    zRange.Item1
                );
                continue;
            }

            for (int x = xRange.Item1; x <= xRange.Item2; x++) {
                for (int z = zRange.Item1; z <= zRange.Item2; z++) {
                    for (int y = yRange.Item1; y <= yRange.Item2; y++) {
                        tiles.Add(new LevelTile(new Vector3Int(x, y, z), type));
                    }
                }
            }
        }
    }

    static (int, int) ParseRange(string rangeStr) {
        if (rangeStr.Contains('.')) {
            var firstDot = rangeStr.IndexOf('.');
            var lastDot = rangeStr.LastIndexOf('.');
            return (
                int.Parse(rangeStr.Substring(0, firstDot)),
                int.Parse(rangeStr.Substring(lastDot + 1))
            );
        }
        return (int.Parse(rangeStr), int.Parse(rangeStr));
    }

    public LevelTile TileAtPosition(Vector3 position) {
        foreach (var tile in tiles) {
            if (tile.position.x == position.x
                && tile.position.y == position.y
                && tile.position.z == position.z) {
                return tile;
            }
        }
        return null;
    }

    public LevelTile RayCastDownwards(Vector2Int xzPosition) {
        LevelTile r = null;
        int highestHeight = -999;
        foreach (var tile in tiles) {
            if (tile.position.x == xzPosition.x && tile.position.z == xzPosition.y) {
                if (tile.position.y > highestHeight) {
                    r = tile;
                    highestHeight = tile.position.y;
                }
            }
        }
        return r;
    }
}