using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFade : MonoBehaviour {
	public Renderer r;
    void Awake() {
        r = GetComponent<Renderer>();
    }
    void OnEnable()
    {
       // r.material.color;
        //sp. = new Color(sp.color.r, sp.color.g, sp.color.b, 1f);
    }

    void Update() {
        r.material.color = new Color(r.material.color.r, r.material.color.g, r.material.color.b, r.material.color.a - 0.1f);
        if(r.material.color.a <= 0) {
            Destroy(this.gameObject);
        }
    }
	
}
