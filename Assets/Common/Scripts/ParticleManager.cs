using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ParticleManager : MonoBehaviourSingleton<ParticleManager> {
   
    public void PlayParticle(string str,Vector2 pos)
    {
        ParticleSystem particle = ParticlePool.Instance.getAvailabeParticle(str);
        particle.gameObject.transform.position = pos;
        particle.Play();
    }

    public void PlayParticle(string str, Vector2 pos, Sprite sprite)
    {
        ParticleSystem particle = ParticlePool.Instance.getAvailabeParticle(str);
        particle.gameObject.transform.position = pos;
        SpriteToMesh(sprite, particle);
        particle.Play();
        UtilityClass.Invoke(this, () => particle.gameObject.SetActive(false), particle.main.duration);
    }

    void SpriteToMesh(Sprite sprite, ParticleSystem particleSystem)
    {
        Mesh mesh = new Mesh();
        mesh.vertices = Array.ConvertAll(sprite.vertices, i => (Vector3)i);
        mesh.uv = sprite.uv;
        mesh.triangles = Array.ConvertAll(sprite.triangles, i => (int)i);

        var sh = particleSystem.shape;
        sh.mesh = mesh;
    }
}
