using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class LevelRenderer : MonoBehaviour {
    public GameObject tilePrefab;
    public Level currentLevel;

    public GameObject player;
    public Vector3Int playerPosition;

    public bool isIsometric = false;

    public void RenderLevel(Level level) {
        currentLevel = level;
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
            if (currentLevel.TileAtPosition(newPosition) == null) {
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

        if (Input.GetButtonDown("Fire1")) {
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
        Camera.main.transform.LookAt(player.transform, Vector3.up);
    }
}
