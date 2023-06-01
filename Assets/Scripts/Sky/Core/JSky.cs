using UnityEngine;
using UnityEngine.Rendering;

namespace Pinwheel.Jupiter {
    [ExecuteInEditMode]
    public class JSky : MonoBehaviour {
        public static readonly Vector3 DefaultSunDirection = Vector3.forward;
        public static readonly Vector3 DefaultMoonDirection = Vector3.forward;

        private static Mesh sphereMesh;

        [SerializeField] private JSkyProfile profile;

        [SerializeField] private Light sunLightSource;

        [SerializeField] private Light moonLightSource;

        public JSkyProfile Profile {
            get => profile;
            set => profile = value;
        }

        public Light SunLightSource {
            get => sunLightSource;
            set {
                var src = value;
                if (src != null && src.type == LightType.Directional)
                    sunLightSource = src;
                else
                    sunLightSource = null;
            }
        }

        public Light MoonLightSource {
            get => moonLightSource;
            set {
                var src = value;
                if (src != null && src.type == LightType.Directional)
                    moonLightSource = src;
                else
                    moonLightSource = null;
            }
        }

        public JDayNightCycle DNC { get; set; }

        private static Mesh SphereMesh {
            get {
                if (sphereMesh == null) sphereMesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");
                return sphereMesh;
            }
        }

        private void Reset() {
            var lights = FindObjectsOfType<Light>();
            for (var i = 0; i < lights.Length; ++i)
                if (lights[i].type == LightType.Directional) {
                    SunLightSource = lights[i];
                    break;
                }
        }

        private void OnEnable() {
            Camera.onPreCull += OnCameraPreCull;
            RenderPipelineManager.beginCameraRendering += OnBeginCameraRenderingSRP;
        }

        private void OnDisable() {
            Camera.onPreCull -= OnCameraPreCull;
            RenderPipelineManager.beginCameraRendering -= OnBeginCameraRenderingSRP;
            RenderSettings.skybox = JJupiterSettings.Instance.DefaultSkybox;
        }

        private void OnDestroy() {
            RenderSettings.skybox = JJupiterSettings.Instance.DefaultSkybox;
        }

        private void OnCameraPreCull(Camera cam) {
            SetupSkyMaterial();
            SyncFog();
            RenderShadow(cam);
        }

        private void OnBeginCameraRenderingSRP(ScriptableRenderContext context, Camera cam) {
            OnCameraPreCull(cam);
        }

        private void SetupSkyMaterial() {
            if (Profile == null) {
                RenderSettings.skybox = JJupiterSettings.Instance.DefaultSkybox;
                return;
            }

            RenderSettings.skybox = Profile.Material;

            Profile.Material.SetTexture(JMat.NOISE_TEX, JJupiterSettings.Instance.NoiseTexture);
            Profile.Material.SetTexture(JMat.CLOUD_TEX,
                Profile.CustomCloudTexture ? Profile.CustomCloudTexture : JJupiterSettings.Instance.CloudTexture);

            if (Profile.EnableSun) {
                if (SunLightSource != null) {
                    var dncProfile = DNC ? DNC.Profile : null;
                    var isSunLightColorOverridden =
                        dncProfile != null && dncProfile.ContainProperty(nameof(Profile.SunLightColor));
                    if (!isSunLightColorOverridden) SunLightSource.color = Profile.SunLightColor;
                    var isSunLightIntensityOverridden = dncProfile != null &&
                                                        dncProfile.ContainProperty(nameof(Profile.SunLightIntensity));
                    if (!isSunLightIntensityOverridden) SunLightSource.intensity = Profile.SunLightIntensity;
                }

                var sunDirection = SunLightSource ? SunLightSource.transform.forward : DefaultSunDirection;
                if (Profile.UseBakedSun) {
                    var sunRotationMatrix = Matrix4x4.Rotate(
                        Quaternion.FromToRotation(sunDirection, DefaultSunDirection));
                    Profile.Material.SetMatrix(JMat.SUN_ROTATION_MATRIX, sunRotationMatrix);
                }
                else {
                    var positionToSunUV = Matrix4x4.TRS(
                        -sunDirection,
                        Quaternion.LookRotation(sunDirection),
                        Profile.SunSize * Vector3.one).inverse;
                    Profile.Material.SetVector(JMat.SUN_DIRECTION, sunDirection);
                    Profile.Material.SetMatrix(JMat.SUN_TRANSFORM_MATRIX, positionToSunUV);
                }
            }

            if (Profile.EnableMoon) {
                if (MoonLightSource != null) {
                    var dncProfile = DNC ? DNC.Profile : null;
                    var isMoonLightColorOverridden =
                        dncProfile != null && dncProfile.ContainProperty(nameof(Profile.MoonLightColor));
                    if (!isMoonLightColorOverridden) MoonLightSource.color = Profile.MoonLightColor;
                    var isMoonLightIntensityOverridden = dncProfile != null &&
                                                         dncProfile.ContainProperty(nameof(Profile.MoonLightIntensity));
                    if (!isMoonLightIntensityOverridden) MoonLightSource.intensity = Profile.MoonLightIntensity;
                }

                var moonDirection = MoonLightSource ? MoonLightSource.transform.forward : DefaultMoonDirection;
                if (Profile.UseBakedMoon) {
                    var moonRotationMatrix = Matrix4x4.Rotate(
                        Quaternion.FromToRotation(moonDirection, DefaultMoonDirection));
                    Profile.Material.SetMatrix(JMat.MOON_ROTATION_MATRIX, moonRotationMatrix);
                }
                else {
                    var positionToMoonUV = Matrix4x4.TRS(
                        -moonDirection,
                        Quaternion.LookRotation(moonDirection),
                        Profile.MoonSize * Vector3.one).inverse;
                    Profile.Material.SetVector(JMat.MOON_DIRECTION, moonDirection);
                    Profile.Material.SetMatrix(JMat.MOON_TRANSFORM_MATRIX, positionToMoonUV);
                }
            }
        }

        private void SyncFog() {
            if (Profile == null)
                return;
            if (Profile.FogSyncOption == JFogSyncOption.DontSync)
                return;
            if (Profile.FogSyncOption == JFogSyncOption.SkyColor)
                RenderSettings.fogColor = Profile.Material.GetColor(JMat.SKY_COLOR);
            else if (Profile.FogSyncOption == JFogSyncOption.HorizonColor)
                RenderSettings.fogColor = Profile.Material.GetColor(JMat.HORIZON_COLOR);
            else if (Profile.FogSyncOption == JFogSyncOption.GroundColor)
                RenderSettings.fogColor = Profile.Material.GetColor(JMat.GROUND_COLOR);
            else if (Profile.FogSyncOption == JFogSyncOption.CustomColor)
                RenderSettings.fogColor = Profile.Material.GetColor(JMat.FOG_COLOR);
        }

        private void RenderShadow(Camera cam) {
            if (Profile == null)
                return;
            if (Profile.EnableOverheadCloud && Profile.OverheadCloudCastShadow) {
                Profile.ShadowMaterial.SetTexture(JMat.NOISE_TEX, JJupiterSettings.Instance.NoiseTexture);
                Profile.ShadowMaterial.SetTexture(JMat.CLOUD_TEX,
                    Profile.CustomCloudTexture ? Profile.CustomCloudTexture : JJupiterSettings.Instance.CloudTexture);
                Graphics.DrawMesh(
                    SphereMesh,
                    Matrix4x4.TRS(Vector3.zero, Quaternion.identity, 2 * Vector3.one * Profile.OverheadCloudAltitude),
                    Profile.ShadowMaterial,
                    0,
                    cam,
                    0,
                    null,
                    ShadowCastingMode.ShadowsOnly,
                    false,
                    null,
                    LightProbeUsage.Off,
                    null);
            }
        }
    }
}