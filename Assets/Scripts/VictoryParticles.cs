/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public enum PoolTag {
    Explosion, Shot
}

[Serializable]
public class Pool {
    public PoolTag tag;
    public GameObject prefab;
    public int size;
}

public class VictoryParticles : MonoBehaviour {
    public float poolReturnTimer = 1.5f;
    
    [Header("Explosion")]
    public AudioClip[] audioExplosion;
    public float explosionMinVolume = 0.3f;
    public float explosionMaxVolume = 0.7f;
    public float explosionPitchMin = 0.75f;
    public float explosionPitchMax = 1.25f;
    
    [Header("Shoot")]
    public AudioClip[] audioShot;
    public float shootMinVolume = 0.05f;
    public float shootMaxVolume = 0.1f;
    public float shootPitchMin = 0.75f;
    public float shootPitchMax = 1.25f;
    
    public List<Pool> pools;
    private Dictionary<PoolTag, Queue<GameObject>> poolDictionary = new();
    private ParticleSystem _particleSystem;
    
    private bool Active { get; set; }

    private void Awake() {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    private void Start() {
        foreach (var pool in pools) {
            var objectPool = new Queue<GameObject>();
            for (var i = 0; i < pool.size; i++) {
                var tmp = Instantiate(pool.prefab, gameObject.transform, true);
                var explosionComponent = tmp.GetComponent<AudioSource>();
                
                switch (pool.tag) {
                    case PoolTag.Explosion:
                        explosionComponent.clip = audioExplosion[Random.Range(0, audioExplosion.Length)];
                        explosionComponent.pitch = Random.Range(explosionPitchMin, explosionPitchMax);
                        break;
                    case PoolTag.Shot:
                    default:
                        explosionComponent.clip = audioShot[Random.Range(0, audioExplosion.Length)];
                        explosionComponent.pitch = Random.Range(shootPitchMin, shootPitchMax);
                        break;
                }

                tmp.SetActive(false);
                objectPool.Enqueue(tmp);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }
    
    private GameObject SpawnFromPool(PoolTag poolTag, Vector3 position) {
        if (!poolDictionary.ContainsKey(poolTag))
            return null;

        var objectToSpawn = poolDictionary[poolTag].Dequeue();
        objectToSpawn.SetActive(true);

        var volume = poolTag switch {
            PoolTag.Explosion => Random.Range(explosionMinVolume, explosionMaxVolume),
            PoolTag.Shot => Random.Range(shootMinVolume, shootMaxVolume),
            _ => 1f
        };

        objectToSpawn.GetComponent<AudioSource>().volume = volume * Settings.MasterVolume * Settings.SfxVolume;
            
        objectToSpawn.transform.position = position;
        poolDictionary[poolTag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }

    public void Activate(bool b) {
        if (Active == b) return;
        
        Active = b;
        
        if (b)
            _particleSystem.Play();
        else 
            _particleSystem.Stop();
    }

    private void LateUpdate() {
        if (!Active) return;
        
        var particles = new ParticleSystem.Particle[_particleSystem.particleCount];
        var length = _particleSystem.GetParticles(particles);
        var i = 0;
        while (i < length) {
            if (audioExplosion.Length > 0 && particles[i].remainingLifetime < Time.deltaTime) {
                var soundInstance = SpawnFromPool(PoolTag.Explosion, particles[i].position);
                if (soundInstance) 
                    StartCoroutine(LateCall(soundInstance));
            }

            if (audioShot.Length > 0 && particles[i].remainingLifetime >= particles[i].startLifetime - Time.deltaTime) {
                var soundInstance = SpawnFromPool(PoolTag.Shot, particles[i].position);
                if (soundInstance) 
                    StartCoroutine(LateCall(soundInstance));
            }

            i++;
        }
    }

    private IEnumerator LateCall(GameObject soundInstance) {
        yield return new WaitForSeconds(poolReturnTimer);
        soundInstance.SetActive(false);
    }
}