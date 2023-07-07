using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Yarn.Unity;

using DG.Tweening;
using Yarn.Unity;

[System.Serializable]
public class PrefabInfo {
    public GameObject prefab;
    public List<string> tags;
}

public class LevelRenderer : MonoBehaviour {
    public GenericDictionary<string, PrefabInfo> prefabs;

    public Level currentLevel;

    public GameObject player;
    public Vector3Int playerPosition;
    public static bool isIsometric = false;
    public bool gotKey = false;

    public List<(GameObject, string)> npcs;
    public List<GameObject> billboardedSprites = new List<GameObject>();

    public DialogueRunner dialogueRunner;
    public UnityEngine.Events.UnityEvent continueCallback;

    public void ClearLevel() {
        while(transform.childCount > 0) {
            var obj = transform.GetChild(0).gameObject;
            GameObject.DestroyImmediate(obj);
        }

        billboardedSprites.Clear();
    }

    public void RenderLevel(Level level) {
        ClearLevel();

        npcs = new List<(GameObject, string)>();

        currentLevel = level;
        foreach (var tile in level.tiles) {
            var tileType = tile.type;
            if (!prefabs.ContainsKey(tile.type)) {
                Debug.LogError("Unknown tile type: " + tile.type);
                tileType = "floor";
                continue;
            }
            var tileObject = Instantiate(prefabs[tileType].prefab, Vector3.zero, Quaternion.identity);
            tileObject.transform.parent = transform;

            tileObject.transform.localPosition = tile.position;
            var mr = tileObject.GetComponent<MeshRenderer>();

            if (mr != null) {
                mr.material = new Material(mr.material);
            }
            tile.obj = tileObject;

            if (prefabs[tileType].tags.Contains("billboard")) {
                billboardedSprites.Add(tileObject);
                
            }

            if (prefabs[tileType].tags.Contains("npc")) {
                npcs.Add((tile.obj, tile.parameter));
            }


        }

        player = Instantiate(prefabs["snack"].prefab, Vector3.zero, Quaternion.identity);
        billboardedSprites.Add(player);
        player.transform.parent = transform;

        player.transform.localPosition = level.playerStartPosition;
        playerPosition = level.playerStartPosition;
        //var pmr = player.GetComponent<MeshRenderer>();
        //pmr.material = new Material(pmr.material);
        //pmr.material.color = Color.red;
                    
        var variableStorage = GameObject.FindObjectOfType<InMemoryVariableStorage>();
        variableStorage.SetValue("$gotKey", false);

        setupQuestionMark();

        CameraRefresh();
    }

    public GameObject qmarker;

    public void setupQuestionMark() {
        qmarker = Instantiate(Resources.Load("questionmark")) as GameObject;
        qmarker.transform.parent = transform;
        qmarker.SetActive(false);
    }

    public void CameraRefresh() {
        if (isIsometric) {
            Camera.main.transform.localPosition = player.transform.localPosition + Constants.ISOMETRIC_VIEW_CAMERA_OFFSET;
        } else {
            Camera.main.transform.localPosition = player.transform.localPosition + Constants.OVERHEAD_VIEW_CAMERA_OFFSET;
        }
        Camera.main.transform.LookAt(player.transform, Vector3.up);
    }

    [YarnCommand("loadLevel")]
    public static void LoadLevel(string levelName) {
        LevelController.currentLevel = "levels/" + levelName;
        UnityEngine.SceneManagement.SceneManager.LoadScene("LevelScene");
    }

    [YarnCommand("gotoCredits")]
    public static void gotoCredits() {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Chonk_Main_Menu");
        CreditsLoader.activate = true;
    }

    public bool TryMove(Vector3Int direction) {
        var newPosition = playerPosition + direction;
        bool ret = false;
        var anyTile = currentLevel.RayCastDownwards(
                new Vector2Int(newPosition.x, newPosition.z)
        );

        // check that theres something in the space
        if(anyTile == null) return false;

        if (isIsometric) {
            var newTile = currentLevel.TileAtPosition(newPosition);
            if ((newTile == null && currentLevel.TileAtPosition(newPosition - new Vector3Int(0, 1, 0)) != null) || (newTile != null && newTile.type == "key")) {
                handleMoveNextTile(newTile, newPosition);
                ret = true;
            } else {
                // check going up or down a block
                if(!prefabs[anyTile.type].tags.Contains("npc") && (anyTile.position.y <= newPosition.y) && (anyTile.position.y >= (newPosition.y - 2))) {
                    newPosition.y = anyTile.position.y + 1;
                    handleMoveNextTile(anyTile, newPosition);
                    ret = true;
                }
            }
        } else {
            var tile = currentLevel.TileAtPosition(newPosition);

            if (tile == null || (tile.type == "floor" || tile.type == "key")) {
                if(tile == null) tile = anyTile;
                handleMoveNextTile(tile, newPosition);
                ret = true;
            }
        }

        // make the question mark appear
        if(ret) {
            updateQuestionMark();
        }

        return ret;
    }

    public void handleMoveNextTile(LevelTile tile, Vector3Int newPosition) {
        playerPosition = newPosition;
        player.transform.localPosition = playerPosition;

        if (tile != null && tile.type == "key") {
            currentLevel.tiles.Remove(tile);
            Destroy(tile.obj);
            gotKey = true;
            var variableStorage = GameObject.FindObjectOfType<InMemoryVariableStorage>();
            variableStorage.SetValue("$gotKey", true);
        }
    }

    // stupid hack so sorry
    public bool IsFridge(GameObject o) {
        return o.name.Contains("fridge");
    } 

    public void updateQuestionMark() {
        var npc = GetNearbyNPC();
        if(npc != null && (!IsFridge(npc) || isIsometric)) {
            qmarker.transform.localPosition = npc.transform.localPosition;
            qmarker.transform.position += new Vector3(0,1, isIsometric ? 0 : 1);
            if(IsFridge(npc)) qmarker.transform.position += new Vector3(0,1,0);
            qmarker.SetActive(true);
        } else {
            qmarker.SetActive(false);
        }
    }

    public GameObject GetNearbyNPC() {
        // check surrounding tiles
        for(int x = 0; x < 3; x++) {
            for(int y = 0; y < 3; y++) {
                if(x == 0 && y == 0) continue;
                var newPos = playerPosition + new Vector3Int(x - 1, 0, y - 1);
                
                List<LevelTile> tiles = isIsometric ? new List<LevelTile>{ currentLevel.TileAtPosition(newPos) } : currentLevel.AllTileAtPosition(newPos);
                for(int i = 0; i < tiles.Count; i ++) {
                    LevelTile tile = tiles[i];
                    if(tile != null && prefabs[tile.type].tags.Contains("npc")) {
                        return tile.obj;
                    }
                }
            }
        }

        return null;
    }

    void Update() {
        if (!dialogueRunner.IsDialogueRunning) {
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
        }

        if (Input.GetButtonDown("Talk")) {
            if (!dialogueRunner.IsDialogueRunning) {
                foreach (var npc in npcs) {
                    var pos = npc.Item1.transform.position;
                    var d = Mathf.Abs(pos.x - playerPosition.x)
                        + Mathf.Abs(pos.z - playerPosition.z)
                        + (isIsometric ? Mathf.Abs(pos.y - playerPosition.y) : 0);

                    if (d == 1 && (!npc.Item1.name.Contains("fridge") || isIsometric)) {
                        dialogueRunner.StartDialogue(npc.Item2);
                        qmarker.SetActive(false);
                        return;
                    }
                }
            } else {
                continueCallback.Invoke();
            }
        }

        if (Input.GetButtonDown("SwitchPerspective")) {
            var currentCameraOffset = (isIsometric ? Constants.ISOMETRIC_VIEW_CAMERA_OFFSET : Constants.OVERHEAD_VIEW_CAMERA_OFFSET);
            var newCameraOffset = (isIsometric ? Constants.OVERHEAD_VIEW_CAMERA_OFFSET : Constants.ISOMETRIC_VIEW_CAMERA_OFFSET);

            var p = 0;
            var currentRotation = Camera.main.transform.rotation;
            var desiredRotation = Quaternion.Euler(isIsometric ? Constants.OVERHEAD_VIEW_CAMERA_ROTATION_EULER : Constants.ISOMETRIC_VIEW_CAMERA_ROTATION_EULER);

            qmarker.transform.position += isIsometric ? new Vector3(0,0,1) : new Vector3(0,0,-1);

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
            // move player if they fall
            if(isIsometric) {
                var anyTile = currentLevel.RayCastDownwards(
                    new Vector2Int(playerPosition.x, playerPosition.z)
                );
                if(anyTile != null) {
                    var newPosition = anyTile.position + new Vector3Int(0, 1, 0);

                    if(anyTile.type == "key") {
                        newPosition -= new Vector3Int(0, 1, 0);
                    }

                    if(newPosition.y != (playerPosition.y)) {
                        handleMoveNextTile(anyTile, newPosition);
                    }
                }
            }
            CameraRefresh();
            updateQuestionMark();
        }

        foreach (var sprite in billboardedSprites) {
            sprite.transform.rotation = Camera.main.transform.rotation;
        }
    }

    public string editorLevel = "levels/tutorial";
}
