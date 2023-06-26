using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bilboard : MonoBehaviour {
	public Camera camera;
    void Start () {
        if(camera == null) camera = Camera.main;
    }
	void Update () {
		transform.forward = camera.transform.forward;
	}
}
