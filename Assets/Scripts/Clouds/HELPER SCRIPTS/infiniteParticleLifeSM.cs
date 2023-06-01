using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class infiniteParticleLifeSM : MonoBehaviour {
    private static ParticleSystem.Particle[] m_Particles;
    private static ParticleSystem m_System;

    private void Awake() {
        m_System = GetComponent<ParticleSystem>();
        m_Particles = new ParticleSystem.Particle[m_System.main.maxParticles];
    }

    private void Start() {
        UpdateQuality();
    }

    private void LateUpdate() {
        // GetParticles is allocation free because we reuse the m_Particles buffer between updates
        var numParticlesAlive = m_System.GetParticles(m_Particles);

        // Change only the particles that are alive
        for (var i = 0; i < m_Particles.Length; i++)
            if (m_Particles[i].remainingLifetime <= 0.1f)
                m_Particles[i].remainingLifetime = m_Particles[i].startLifetime;

        // Apply the particle changes to the particle system
        m_System.SetParticles(m_Particles, numParticlesAlive);
    }

    public static void UpdateQuality() {
        if (!m_System) return;
        
        var mainModule = m_System.main;
        mainModule.maxParticles = (Settings.Quality + 1) * 170;
    }
}