using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelRenderer : MonoBehaviour {
    public GameObject tilePrefab;

    public GameObject player;

    public void RenderLevel(Level level) {
        foreach (var tile in level.tiles) {
            var tileObject = Instantiate(tilePrefab, Vector3.zero, Quaternion.identity);
            tileObject.transform.parent = transform;

            tileObject.transform.localPosition = tile.position;
            var mr = tileObject.GetComponent<MeshRenderer>();
            mr.material = new Material(mr.material);
            mr.material.color = (tile.type == "wall") ? Color.black : Color.white;
        }

        player = Instantiate(tilePrefab, Vector3.zero, Quaternion.identity);
        player.transform.parent = transform;

        player.transform.localPosition = level.playerStartPosition;
        var pmr = player.GetComponent<MeshRenderer>();
        pmr.material = new Material(pmr.material);
        pmr.material.color = Color.red;
    }

    void Update() {
        if (Input.GetKeyDown("left")) {
            player.transform.localPosition += Vector3.left;
        }
        if (Input.GetKeyDown("right")) {
            player.transform.localPosition += Vector3.right;
        }
        if (Input.GetKeyDown("up")) {
            player.transform.localPosition += Vector3.down;
        }
        if (Input.GetKeyDown("down")) {
            player.transform.localPosition += Vector3.up;
        }
        Camera.main.transform.localPosition = player.transform.localPosition + new Vector3(0, 0, 7);
    }
}
