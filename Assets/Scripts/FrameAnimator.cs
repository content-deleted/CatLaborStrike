using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

using Nova.Attributes;

public class FrameAnimator : MonoBehaviour {
    [SerializeField, ShowIf("!multipleFiles")]
    public Sprite sprite;

    [SerializeField, ShowIf("multipleFiles")]
    public Sprite[] _sprites;
    public bool multipleFiles;

    [SerializeField, HideInInspector]
    private SpriteRenderer _spriteRenderer;

    [SerializeField, HideInInspector]
    private Image _image;

    [SerializeField, HideInInspector]
    private bool usingImage = false;

    public bool simpleMode = true;


    [ShowIf("simpleMode")]
    public bool looping = true;
    [ShowIf("simpleMode")]
    public float frameRate = 3f;

    [HideInInspector]
    public bool playing = true;

    public System.Action<FrameAnimator> onCompleteCallback;


    private int _frameIndex = 0;
    private int _animationFrames = 0;
    private float _deltaTime = 0;
    private List<FrameAnimation> _animList =  new List<FrameAnimation>();
    public FrameAnimation currentAnimation { get; private set; }
    private string _previousAnimName = "";

    #if UNITY_EDITOR
    private void OnValidate() {
        if (Application.isPlaying) {
            return;
        }

        if (gameObject.GetComponent<SpriteRenderer>() != null) {
            _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        }
        else if (gameObject.GetComponentInChildren<SpriteRenderer>() != null) {
            _spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
        }
        else if (gameObject.GetComponent<Image>() != null) {
            _image = gameObject.GetComponent<Image>();
            usingImage = true;
        } else {
            _spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        if (sprite == null) {
            if (usingImage) {
                sprite = _image.sprite;
            } else {
                sprite = _spriteRenderer.sprite;
            }
        }

        if (!multipleFiles && sprite != null) {
            // If we're using one file, load all other sprites from the same asset path
            if (usingImage) {
                _image.sprite = sprite;
            } else {
                _spriteRenderer.sprite = sprite;
            }

            System.Object[] data = AssetDatabase.LoadAllAssetRepresentationsAtPath( AssetDatabase.GetAssetPath(sprite) );
            _sprites = new Sprite[data.Length];
            for (int i = 0; i < data.Length; i++) {
                _sprites[i] = (Sprite)data[i];
            }
        }
    }
    #endif

    void Awake() {
        if (!simpleMode) {
            return;
        }

        if (frameRate == 0) {
            return;
        }

        AddAnimation("_normal", Enumerable.Range(0, _sprites.Length).ToArray(), frameRate, true);
        PlayAnimation("_normal");
    }

    public void Update() {
        _deltaTime += Time.deltaTime;

        if (currentAnimation != null && playing) {
            //Reset the frame to 0 if the current animation is different
            if (_previousAnimName != currentAnimation.name) {
                _frameIndex = 0;
                _previousAnimName = currentAnimation.name;
            }

            while (_deltaTime >= currentAnimation.frameRate) {
                _deltaTime -= currentAnimation.frameRate;
                _frameIndex++;

                if (_frameIndex >= _animationFrames) {
                    if (currentAnimation.looped) {
                        _frameIndex = 0;
                    }
                    else {
                        _frameIndex = _animationFrames - 1;
                        playing = false;
                    }
                    if (onCompleteCallback != null) {
                        onCompleteCallback(this);
                    }
                }
            }

            if (!usingImage) {
                _spriteRenderer.sprite = _sprites[currentAnimation.frames[_frameIndex]];
            } else {
                _image.sprite = _sprites[currentAnimation.frames[_frameIndex]];
            }
        }
    }

    public FrameAnimation AddAnimation(string name, int[] frames, float frameRate = 3, bool looped = true) {
        FrameAnimation newAnimation = new FrameAnimation();
        newAnimation.name = name;
        newAnimation.frames = frames;
        newAnimation.frameRate = 1/frameRate;
        newAnimation.looped = looped;
        _animList.Add(newAnimation);
        return newAnimation;
    }

    public void GoToFrame(int frame) {
        currentAnimation = null;
        simpleMode = false;
        if (usingImage) {
            _image.sprite = _sprites[frame];
        } else {
            _spriteRenderer.sprite = _sprites[frame];
        }
    }

    public void PlayAnimation(string name) {
        currentAnimation = _animList.Find(item => item.name.Contains(name));

        if (currentAnimation != null) {
            _animationFrames = currentAnimation.frames.Length;
        } else {
            Debug.Log("No animation found with name " + name);
        }
    }

    [System.Serializable]
    public class FrameAnimation {
        public string name;
        public int[] frames;
        public float frameRate;
        public bool looped;
    }
}