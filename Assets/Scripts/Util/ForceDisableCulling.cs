using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceDisableCulling : MonoBehaviour
{
    Mesh mesh;
    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
    }
    void LateUpdate()
    {              
        // Create and set your new bounds
        Bounds newBounds = new Bounds(PlayerMovement.player.transform.position, new Vector3(50000000,500000000,50000000));
        mesh.bounds = newBounds;
    }
}
