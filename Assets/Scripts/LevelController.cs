using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour {
    public static string currentLevel = "levels/tutorial";

    public LevelRenderer levelRenderer;

    AudioManager _audioManager;

    void Start() {
        _audioManager = GameObject.FindObjectOfType<AudioManager>();

        var l = new Level(currentLevel);
        levelRenderer.RenderLevel(l);
        if(_audioManager) _audioManager.PlayMusic(4);
    }
}
