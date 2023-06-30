using UnityEngine;

[ExecuteInEditMode]
public class VolumeParticleShadePDM : MonoBehaviour {
    private static readonly int SunColor = Shader.PropertyToID("_SunColor");
    private static readonly int SunLightIntensity = Shader.PropertyToID("_SunLightIntensity");
    private static readonly int ForwLight = Shader.PropertyToID("ForwLight");
    public bool Unity2020;
    public Light Sun;
    public Material Particle_Mat;

    // Update is called once per frame
    private void Update() {
        if (Settings.Quality == 0) return;
        if (!((Sun != null) & (Particle_Mat != null))) return;

        Particle_Mat.SetVector(SunColor, Sun.color);
        Particle_Mat.SetFloat(SunLightIntensity, Sun.intensity);
        if (Unity2020) Particle_Mat.SetVector(ForwLight, -Sun.transform.forward);
    }
}