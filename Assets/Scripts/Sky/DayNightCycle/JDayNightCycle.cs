using UnityEngine;
using UnityEngine.Rendering;

namespace Pinwheel.Jupiter {
    [ExecuteInEditMode]
    public class JDayNightCycle : MonoBehaviour {
        [SerializeField] private JDayNightCycleProfile profile;
        [SerializeField] private JSky sky;
        [SerializeField] private bool useSunPivot;
        [SerializeField] private Transform sunOrbitPivot;
        [SerializeField] private bool useMoonPivot;
        [SerializeField] private Transform moonOrbitPivot;
        [SerializeField] private bool shouldUpdateEnvironmentReflection;
        [SerializeField] private int environmentReflectionResolution;
        [SerializeField] private ReflectionProbeTimeSlicingMode environmentReflectionTimeSlicingMode;
        [SerializeField] private ReflectionProbe environmentProbe;

        private Cubemap environmentReflection;
        private int probeRenderId = -1;

        public JDayNightCycleProfile Profile {
            get => profile;
            set => profile = value;
        }

        public JSky Sky {
            get => sky;
            set => sky = value;
        }

        public bool UseSunPivot {
            get => useSunPivot;
            set => useSunPivot = value;
        }

        public Transform SunOrbitPivot {
            get => sunOrbitPivot;
            set => sunOrbitPivot = value;
        }

        public bool UseMoonPivot {
            get => useMoonPivot;
            set => useMoonPivot = value;
        }

        public Transform MoonOrbitPivot {
            get => moonOrbitPivot;
            set => moonOrbitPivot = value;
        }

        public bool ShouldUpdateEnvironmentReflection {
            get => shouldUpdateEnvironmentReflection;
            set => shouldUpdateEnvironmentReflection = value;
        }

        public int EnvironmentReflectionResolution {
            get => environmentReflectionResolution;
            set {
                var oldValue = environmentReflectionResolution;
                var newValue = Mathf.Clamp(value, 16, 2048);
                environmentReflectionResolution = newValue;
                if (oldValue != newValue) {
                    if (environmentReflection != null) JUtilities.DestroyObject(environmentReflection);
                    if (environmentProbe != null) JUtilities.DestroyGameobject(environmentProbe.gameObject);
                }
            }
        }

        public ReflectionProbeTimeSlicingMode EnvironmentReflectionTimeSlicingMode {
            get => environmentReflectionTimeSlicingMode;
            set => environmentReflectionTimeSlicingMode = value;
        }

        private ReflectionProbe EnvironmentProbe {
            get {
                if (environmentProbe == null) {
                    var probeGO = new GameObject("~EnvironmentReflectionRenderer");
                    probeGO.transform.parent = transform;
                    probeGO.transform.position = new Vector3(0, -1000, 0);
                    probeGO.transform.rotation = Quaternion.identity;
                    probeGO.transform.localScale = Vector3.one;

                    environmentProbe = probeGO.AddComponent<ReflectionProbe>();
                    environmentProbe.resolution = EnvironmentReflectionResolution;
                    environmentProbe.size = new Vector3(1, 1, 1);
                    environmentProbe.cullingMask = 0;
                }

                environmentProbe.clearFlags = ReflectionProbeClearFlags.Skybox;
                environmentProbe.mode = ReflectionProbeMode.Realtime;
                environmentProbe.refreshMode = ReflectionProbeRefreshMode.ViaScripting;
                environmentProbe.timeSlicingMode = EnvironmentReflectionTimeSlicingMode;
                environmentProbe.hdr = false;
                return environmentProbe;
            }
        }

        private Cubemap EnvironmentReflection {
            get {
                if (environmentReflection == null)
                    environmentReflection = new Cubemap(EnvironmentProbe.resolution, TextureFormat.RGBA32, true);
                return environmentReflection;
            }
        }

        private float DeltaTime {
            get {
                if (Application.isPlaying)
                    return Time.deltaTime;
                return 1.0f / 60f;
            }
        }

        private void Reset() {
            Sky = GetComponent<JSky>();
        }

        private void Update() {
            AnimateSky();
            if (ShouldUpdateEnvironmentReflection)
                UpdateEnvironmentReflection();
            else
                RenderSettings.defaultReflectionMode = DefaultReflectionMode.Skybox;
        }

        private void OnEnable() {
            Camera.onPreCull += OnCameraPreCull;
            RenderPipelineManager.beginFrameRendering += OnBeginFrameRenderingSRP;
        }

        private void OnDisable() {
            Camera.onPreCull -= OnCameraPreCull;
            RenderPipelineManager.beginFrameRendering -= OnBeginFrameRenderingSRP;
            CleanUp();
        }

        private void OnCameraPreCull(Camera cam) {
            if (!Application.isPlaying)
                Update();
        }

        private void OnBeginFrameRenderingSRP(ScriptableRenderContext context, Camera[] cameras) {
            if (!Application.isPlaying)
                Update();
        }

        private void CleanUp() {
            if (environmentProbe != null) JUtilities.DestroyGameobject(environmentProbe.gameObject);
            if (environmentReflection != null) JUtilities.DestroyObject(environmentReflection);
            if (Sky != null) Sky.DNC = null;
        }

        private void AnimateSky() {
            if (Profile == null)
                return;
            if (Sky == null)
                return;
            if (Sky.Profile == null)
                return;
            Sky.DNC = this;

            var evalTime = Mathf.InverseLerp(0f, 24f, TimeManager.Hour);
            Profile.Animate(Sky, evalTime);

            if (Sky.Profile.EnableSun && Sky.SunLightSource != null) {
                var angle = evalTime * 360f;
                var localRotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(angle, 0, 0));
                var localDirection = localRotationMatrix.MultiplyVector(Vector3.up);

                var pivot = UseSunPivot && SunOrbitPivot != null ? SunOrbitPivot : transform;
                var localToWorld = pivot.localToWorldMatrix;
                var worldDirection = localToWorld.MultiplyVector(localDirection);
                Sky.SunLightSource.transform.forward = worldDirection;
                Sky.SunLightSource.color = Sky.Profile.Material.GetColor(JMat.SUN_LIGHT_COLOR);
                Sky.SunLightSource.intensity = Sky.Profile.Material.GetFloat(JMat.SUN_LIGHT_INTENSITY);
            }

            if (Sky.Profile.EnableMoon && Sky.MoonLightSource != null) {
                var angle = evalTime * 360f;
                var localRotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(angle, 0, 0));
                var localDirection = localRotationMatrix.MultiplyVector(Vector3.down);

                var pivot = UseMoonPivot && MoonOrbitPivot != null ? MoonOrbitPivot : transform;
                var localToWorld = pivot.localToWorldMatrix;
                var worldDirection = localToWorld.MultiplyVector(localDirection);
                Sky.MoonLightSource.transform.forward = worldDirection;
                Sky.MoonLightSource.color = Sky.Profile.Material.GetColor(JMat.MOON_LIGHT_COLOR);
                Sky.MoonLightSource.intensity = Sky.Profile.Material.GetFloat(JMat.MOON_LIGHT_INTENSITY);
            }
        }

        private void UpdateEnvironmentReflection() {
            if ((SystemInfo.copyTextureSupport & CopyTextureSupport.RTToTexture) != 0) {
                if (EnvironmentProbe.texture == null) {
                    probeRenderId = EnvironmentProbe.RenderProbe();
                }
                else if (EnvironmentProbe.texture != null || EnvironmentProbe.IsFinishedRendering(probeRenderId)) {
                    Graphics.CopyTexture(EnvironmentProbe.texture, EnvironmentReflection);
                    RenderSettings.customReflection = EnvironmentReflection;
                    RenderSettings.defaultReflectionMode = DefaultReflectionMode.Custom;
                    probeRenderId = EnvironmentProbe.RenderProbe();
                }
            }
        }
    }
}