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

public class VictoryParticles : MonoBehaviour {
    public AudioSource[] ExplosionAudio, ShotAudio;

    [Header("Explosion")]
    public AudioClip[] audioExplosion;
    public float explosionMinVolume = 0.3f, explosionMaxVolume = 0.7f, explosionPitchMin = 0.75f, explosionPitchMax = 1.25f;

    [Header("Shoot")]
    public AudioClip[] audioShot;
    public float shootMinVolume = 0.05f, shootMaxVolume = 0.1f, shootPitchMin = 0.75f, shootPitchMax = 1.25f;

    private Dictionary<PoolTag, Queue<AudioSource>> poolDictionary = new();
    private ParticleSystem _particleSystem;
    private bool Active { get; set; }
    private void Awake() {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    private void Start() {
        var soundPool1 = new Queue<AudioSource>();
        foreach (var a in ExplosionAudio) {
            a.clip = audioExplosion[Random.Range(0, audioExplosion.Length)];
            a.pitch = Random.Range(explosionPitchMin, explosionPitchMax);
            soundPool1.Enqueue(a);
            
        }
        
        var soundPool2 = new Queue<AudioSource>();
        foreach (var a in ShotAudio) {
            a.clip = audioShot[Random.Range(0, audioExplosion.Length)];
            a.pitch = Random.Range(shootPitchMin, shootPitchMax);
            soundPool2.Enqueue(a);
            
        }
        
        poolDictionary.Add(PoolTag.Explosion, soundPool1);
        poolDictionary.Add(PoolTag.Shot, soundPool2);
    }
    
    private void PlaySfxFromPool(PoolTag poolTag, Vector3 position) {
        if (!poolDictionary.ContainsKey(poolTag))
            return;

        var sfxAudioSource = poolDictionary[poolTag].Dequeue();

        var volume = poolTag switch {
            PoolTag.Explosion => Random.Range(explosionMinVolume, explosionMaxVolume),
            PoolTag.Shot => Random.Range(shootMinVolume, shootMaxVolume),
            _ => 1f
        };

        sfxAudioSource.volume = volume * Settings.MasterVolume * Settings.SfxVolume;
        sfxAudioSource.transform.position = position;
        sfxAudioSource.Play();
        
        poolDictionary[poolTag].Enqueue(sfxAudioSource);
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
        if (!Active || Settings.Quality == 0) return;
        
        var particles = new ParticleSystem.Particle[_particleSystem.particleCount];
        var length = _particleSystem.GetParticles(particles);
        
        var i = 0;
        while (i < length) {
            if (audioExplosion.Length > 0 && particles[i].remainingLifetime < Time.deltaTime)
                PlaySfxFromPool(PoolTag.Explosion, particles[i].position);
            if (audioShot.Length > 0 && particles[i].remainingLifetime >= particles[i].startLifetime - Time.deltaTime)
                PlaySfxFromPool(PoolTag.Shot, particles[i].position);

            i++;
        }
    }
}