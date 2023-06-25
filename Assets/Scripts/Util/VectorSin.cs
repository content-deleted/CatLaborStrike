using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorSin : MonoBehaviour {

	public Vector3 axis;
    public float speed;
    public float t_offset; // Make it so that its not bobbing with the same rhythem as other shit
    
    private Vector3 st;
    void Start () { 
        t = t_offset;
        st = transform.position;
    }
    private float t;
	void FixedUpdate () {
        t+=Time.deltaTime;
        transform.position = Mathf.Sin(t*speed)*axis+st;
	}
    void OnDrawGizmosSelected () {DrawPath();}
	void DrawPath () {
        Gizmos.color = Color.blue;

        var p = !Application.isPlaying ? transform.position : st;

        Gizmos.DrawSphere(p + axis, 1);

        Gizmos.DrawLine(p + axis, p - axis);
        
        Gizmos.DrawSphere(p - axis, 1);
	}
}
