using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using Yarn.Unity;

public class LevelRenderer : MonoBehaviour {
    public GenericDictionary<string, GameObject> prefabs;

    public Level currentLevel;

    public GameObject player;
    public Vector3Int playerPosition;
    public bool isIsometric = false;

    public List<(Vector3Int, string)> npcs;
    public List<GameObject> billboardedSprites;

    public DialogueRunner dialogueRunner;

    public void RenderLevel(Level level) {
        npcs = new List<(Vector3Int, string)>();

        currentLevel = level;
        foreach (var tile in level.tiles) {
            var tileType = tile.type;
            if (!prefabs.ContainsKey(tile.type)) {
                Debug.LogError("Unknown tile type: " + tile.type);
                tileType = "floor";
                continue;
            }
            var tileObject = Instantiate(prefabs[tileType], Vector3.zero, Quaternion.identity);
            tileObject.transform.parent = transform;

            tileObject.transform.localPosition = tile.position;
            var mr = tileObject.GetComponent<MeshRenderer>();
            mr.material = new Material(mr.material);
            mr.material.color = (tileType == "wall") ? Color.black : Color.white;

            if (tileType == "snack" || tileType == "fridge") {
                npcs.Add((tile.position, tile.parameter));
            }
        }

        player = Instantiate(prefabs["player"], Vector3.zero, Quaternion.identity);
        player.transform.parent = transform;

        player.transform.localPosition = level.playerStartPosition;
        playerPosition = level.playerStartPosition;
        var pmr = player.GetComponent<MeshRenderer>();
        pmr.material = new Material(pmr.material);
        pmr.material.color = Color.red;

        CameraRefresh();
    }

    public void CameraRefresh() {
        if (isIsometric) {
            Camera.main.transform.localPosition = player.transform.localPosition + Constants.ISOMETRIC_VIEW_CAMERA_OFFSET;
        } else {
            Camera.main.transform.localPosition = player.transform.localPosition + Constants.OVERHEAD_VIEW_CAMERA_OFFSET;
        }
        Camera.main.transform.LookAt(player.transform, Vector3.forward);
    }

    public bool TryMove(Vector3Int direction) {
        var newPosition = playerPosition + direction;
        if (isIsometric) {
            if (currentLevel.TileAtPosition(newPosition) == null && currentLevel.TileAtPosition(newPosition - new Vector3Int(0, 1, 0)) != null) {
                playerPosition = newPosition;
                player.transform.localPosition = playerPosition;
                return true;
            }
            return false;
        } else {
            var tile = currentLevel.RayCastDownwards(
                new Vector2Int(newPosition.x, newPosition.z)
            );
            if (tile != null && tile.type == "floor") {
                playerPosition = newPosition;
                player.transform.localPosition = playerPosition;
                return true;
            }
            return false;
        }
    }

    void Update() {
        if (Input.GetKeyDown("left")) {
            TryMove(Vector3Int.left);
            CameraRefresh();
        }
        if (Input.GetKeyDown("right")) {
            TryMove(Vector3Int.right);
            CameraRefresh();
        }
        if (Input.GetKeyDown("up")) {
            TryMove(new Vector3Int(0, 0, 1));
            CameraRefresh();
        }
        if (Input.GetKeyDown("down")) {
            TryMove(new Vector3Int(0, 0, -1));
            CameraRefresh();
        }

        if (Input.GetButtonDown("Talk") && !dialogueRunner.IsDialogueRunning) {
            foreach (var npc in npcs) {
                var d = Mathf.Abs(npc.Item1.x - playerPosition.x)
                      + Mathf.Abs(npc.Item1.z - playerPosition.z)
                      + (isIsometric ? Mathf.Abs(npc.Item1.y - playerPosition.y) : 0);

                if (d == 1) {
                    dialogueRunner.StartDialogue(npc.Item2);
                    return;
                }
            }
        }

        if (Input.GetButtonDown("SwitchPerspective")) {
            var currentCameraOffset = (isIsometric ? Constants.ISOMETRIC_VIEW_CAMERA_OFFSET : Constants.OVERHEAD_VIEW_CAMERA_OFFSET);
            var newCameraOffset = (isIsometric ? Constants.OVERHEAD_VIEW_CAMERA_OFFSET : Constants.ISOMETRIC_VIEW_CAMERA_OFFSET);

            var p = 0;
            var currentRotation = Camera.main.transform.rotation;
            var desiredRotation = Quaternion.Euler(isIsometric ? Constants.OVERHEAD_VIEW_CAMERA_ROTATION_EULER : Constants.ISOMETRIC_VIEW_CAMERA_ROTATION_EULER);


            DOTween.To(
                () => p,
                (float p) => {
                    Camera.main.transform.localPosition =
                        player.transform.localPosition + Vector3.Slerp(currentCameraOffset, newCameraOffset, p);
                    Camera.main.transform.localRotation = Quaternion.Slerp(currentRotation, desiredRotation, p);
                },
                1f,
                0.5f
            );
            //Quaternion.Lerp

            isIsometric = !isIsometric;
            CameraRefresh();
        }

        foreach (var sprite in billboardedSprites) {
            sprite.transform.rotation = Camera.main.transform.rotation;
        }
    }
}
