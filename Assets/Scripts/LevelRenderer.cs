using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class LevelRenderer : MonoBehaviour {
    public GameObject tilePrefab;

    public GameObject player;

    public bool isIsometric = false;

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

    void Update() {
        if (Input.GetKeyDown("left")) {
            player.transform.localPosition += Vector3.left;
            CameraRefresh();
        }
        if (Input.GetKeyDown("right")) {
            player.transform.localPosition += Vector3.right;
            CameraRefresh();
        }
        if (Input.GetKeyDown("up")) {
            player.transform.localPosition += Vector3.down;
            CameraRefresh();
        }
        if (Input.GetKeyDown("down")) {
            player.transform.localPosition += Vector3.up;
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
    }
}
