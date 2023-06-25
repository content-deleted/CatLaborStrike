using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFade : MonoBehaviour {
	public SpriteRenderer sp;
    void Awake() {
        sp = GetComponent<SpriteRenderer>();
    }
    void OnEnable()
    {
        sp.color = new Color(sp.color.r, sp.color.g, sp.color.b, 1f);
    }

    void Update() {
        sp.color = new Color(sp.color.r, sp.color.g, sp.color.b, sp.color.a - 0.1f);
        if(sp.color.a <= 0) {
            gameObject.SetActive(false);
        }
    }
	
}
