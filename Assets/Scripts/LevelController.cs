using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour {
    public LevelRenderer levelRenderer;

    AudioManager _audioManager;

    void Start() {
        _audioManager = GameObject.FindObjectOfType<AudioManager>();

        var l = new Level("levels/tutorial");
        var k = new Level("levels/kitchen");

        levelRenderer.RenderLevel(l);
        _audioManager.PlayMusic(4);
    }
}
