using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ParticleController : MonoBehaviour {

    new ParticleSystem particleSystem;

    public void SpriteToMesh(Sprite sprite)
    {
        Mesh mesh = new Mesh();
        mesh.vertices = Array.ConvertAll(sprite.vertices, i => (Vector3)i);
        mesh.uv = sprite.uv;
        mesh.triangles = Array.ConvertAll(sprite.triangles, i => (int)i);

        var sh = particleSystem.shape;
        sh.mesh = mesh;
    }

    public void Active()
    {
        particleSystem.Play();
    }
}
