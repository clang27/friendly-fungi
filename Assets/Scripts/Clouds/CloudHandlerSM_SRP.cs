﻿using UnityEngine;

namespace Artngame.SKYMASTER {
    [ExecuteInEditMode]
    public class CloudHandlerSM_SRP : MonoBehaviour {
        //v3.4 - Cloud properties
        //public float HeightSwitch = 250;
        //public bool AboveClouds = false;
        public enum CloudPreset {
            Custom,
            ClearDay,
            Storm
        } //, Storm, Mobile};

        private const float n = 1.0003f;
        private const float N = 2.545E25f;
        private const float pn = 0.035f;

        private static readonly int TimePass = Shader.PropertyToID("_TimePass");
        private static readonly int Velocity2 = Shader.PropertyToID("_Velocity2");
        private static readonly int Velocity1 = Shader.PropertyToID("_Velocity1");
        private static readonly int SunPosition = Shader.PropertyToID("sunPosition");
        private static readonly int SunColor = Shader.PropertyToID("_SunColor");
        private static readonly int ShadowColor = Shader.PropertyToID("_ShadowColor");
        private static readonly int Density = Shader.PropertyToID("_Density");
        private static readonly int Coverage1 = Shader.PropertyToID("_Coverage");
        private static readonly int Transparency = Shader.PropertyToID("_Transparency");
        private static readonly int HorizonFactor = Shader.PropertyToID("_HorizonFactor");
        private static readonly int Color1 = Shader.PropertyToID("_Color");
        private static readonly int FogColor = Shader.PropertyToID("_FogColor");
        private static readonly int FogUnity = Shader.PropertyToID("_FogUnity");
        private static readonly int CutHeight = Shader.PropertyToID("_CutHeight");
        private static readonly int MieCoefficient = Shader.PropertyToID("mieCoefficient");
        private static readonly int VortexControl = Shader.PropertyToID("vortexControl");
        private static readonly int VortexPosRadius = Shader.PropertyToID("vortexPosRadius");
        private static readonly int ColorDiff = Shader.PropertyToID("_ColorDiff");
        private static readonly int FogFactor = Shader.PropertyToID("_FogFactor");
        private static readonly int BetaR = Shader.PropertyToID("betaR");
        private static readonly int BetaM = Shader.PropertyToID("betaM");
        private static readonly int FogDepth = Shader.PropertyToID("fog_depth");
        private static readonly int MieDirectionalG = Shader.PropertyToID("mieDirectionalG");
        private static readonly int Bias = Shader.PropertyToID("ExposureBias");
        private static readonly int FogPower = Shader.PropertyToID("fogPower");
        private static readonly int FogPowerExp = Shader.PropertyToID("fogPowerExp");
        private static readonly int PointLightPos = Shader.PropertyToID("pointLightPos");
        private static readonly int PointLightPower = Shader.PropertyToID("pointLightPower");
        private static readonly int PointLightColor = Shader.PropertyToID("pointLightColor");

        //HDRP v0.1        
        [HideInInspector] public Transform Hero;
        public float WeatherSeverity = 1;
        public WindZone windZone;
        public float coverageSpeed = 1;
        public Transform Sun;
        public Transform Moon;
        [HideInInspector] public bool useGradients;

        [HideInInspector]
        public bool AutoSunPosition; //define position either by sun transform (true) or time of day defined (false)

        public float Shift_dawn;
        public float NightTimeMax = 23;
        public float fogPower;
        public float fogPowerExp = 1;

        //v3.4.3
        [HideInInspector] public bool followPlayer;
        [HideInInspector] public bool rotateWithCamera;

        [HideInInspector] public bool rotateMultiQuadC;

        //public Vector3 ShadowPlaneScale = new Vector3(500,600,900);
        public float IntensityDiffOffset;
        public float IntensitySunOffset;
        public float IntensityFogOffset;

        [HideInInspector] public bool EnableInsideClouds;

        [HideInInspector] public bool FogFromSkyGrad;

        [HideInInspector] public bool LerpRot;

        [HideInInspector] public AnimationCurve IntensityDiff = new(new Keyframe(0, 0.4f),
            new Keyframe(0.374f, 0.292f), new Keyframe(0.6f, 0.2766f), new Keyframe(0.757f, 0.278f),
            new Keyframe(0.798f, 0.271f),
            new Keyframe(0.849f, 0.275f), new Keyframe(0.887f, 0.248f), new Keyframe(0.944f, 0.280f),
            new Keyframe(1, 0.4f));

        [HideInInspector] public AnimationCurve IntensityFog = new(new Keyframe(0, 5f),
            new Keyframe(0.75f, 10f), new Keyframe(0.88f, 11f), new Keyframe(0.89f, 10.58f),
            new Keyframe(1, 5f));

        [HideInInspector] public AnimationCurve IntensitySun = new(new Keyframe(0, 0.078f),
            new Keyframe(0.1864f, 0.148f), new Keyframe(0.71f, 0.129f), new Keyframe(0.839f, 0.303f),
            new Keyframe(0.90f, 0.13f),
            new Keyframe(1, 0.078f));
        //public bool CurvesFromSkyMaster = false;//use Sky master inspector to grab the above curve values, these are global for all volume clouds and exported when SM is in gradient mode

        public bool DomeClouds = true;
        public Vector3 DomeScale;
        public Vector3 DomeHeights;

        public bool MultiQuadClouds;
        public Vector3 MultiQuadScale;
        public Vector3 MultiQuadHeights;

        public bool MultiQuadAClouds;
        public Vector3 MultiQuadAScale;
        public Vector3 MultiQuadAHeights;
        public bool MultiQuadBClouds;
        public Vector3 MultiQuadBScale;
        public Vector3 MultiQuadBHeights;
        public bool MultiQuadCClouds; //v3.4.3
        public Vector3 MultiQuadCScale;
        public Vector3 MultiQuadCHeights;

        public bool OneQuadClouds;

        public Vector3 OneQuadScale;

        //Heights of clouds - x=transform scale, y = shader height cutoff, z = 
        public Vector3 OneQuadHeights;

        //v3.4.2 - shadow planes not rotate
        public Transform FlatBedSPlane;
        public Transform SideSPlane;
        public Transform SideASPlane;
        public Transform SideBSPlane;
        public Transform SideCSPlane;
        public Transform RotCloudsSPlane;

        public bool WeatherDensity; //refine density based on weather

        public bool UseUnityFog;

        public float CloudEvolution = 0.07f;

        public float ClearDayCoverage = -0.12f;

        //public float ClearDayTransp = 0.51f;
        //public float ClearDayIntensity = 0.1f;
        public float ClearDayHorizon = 0.1f;

        public float StormCoverage;
        public float StormHorizon = 0.01f;
        public Color StormSunColor = new(0.5f, 0.5f, 0.5f, 0.5f);

        public float Coverage = -0.12f;
        public float Transp = 0.51f;
        public float Intensity = 0.1f;
        public float Horizon = 0.1f;

        public CloudPreset cloudType = CloudPreset.ClearDay;
        //public Vector3 cloudDomeScale = Vector3.one;

        //public SkyMasterManager SkyManager;

        [HideInInspector] public bool autoRainbow;
        [HideInInspector] public float rainbowApperSpeed = 1;

        [HideInInspector] public float rainbowTime = 60;

        //public float rainbowIntensity = 0;
        [HideInInspector] public float rainboxMaxIntensity = 0.7f;
        [HideInInspector] public float rainboxMinIntensity;
        public Material rainbowMat;

        public Material cloudFlatMat;
        public Material cloudSidesMat;
        public Material cloudSidesAMat;
        public Material cloudSidesBMat;
        public Material cloudSidesCMat; //v3.4.3
        public Material cloudRotMat;

        public Transform FlatBedClouds;
        public Transform SideClouds;
        public Transform SideAClouds;
        public Transform SideBClouds;
        public Transform SideCClouds; //v3.4.3
        public Transform RotClouds;

        public GameObject
            LightningPrefab; //Prefab to instantiate for lighting, use only 1-2 prefabs and move them around

        public bool EnableLightning;

        //Color DayCloudColor = new Color(91f/255f,139f/255f,129f/255f,209f/255f);
        public Color
            DayCloudColor =
                new(230f / 255f, 230f / 255f, 230f / 255f,
                    20f / 255f); //new Color(88f/255f,40f/255f,40f/255f,76f/255f);

        public Color
            DayCloudShadowCol =
                new(61f / 255f, 61f / 255f, 81f / 255f, 70f / 255f); // new Color(61f/255f,61f/255f,81f/255f,61f/255f);

        public Color
            DayCloudTintCol =
                new(11f / 255f, 11f / 255f, 11f / 255f,
                    255f / 255f); //new Color(156f/255f,142f/255f,142f/255f,255f/255f);

        public Color DayCloudFogCol = new(196f / 255f, 219f / 255f, 234f / 255f, 255f / 255f);
        public float DayFogFactor = 1;
        public float DayIntensity = 0.1f;

        public Color
            DawnCloudColor =
                new(231f / 255f, 190f / 255f, 240f / 255f,
                    110f / 255f); //new Color(31f/255f,29f/255f,29f/255f,37f/255f);

        public Color
            DawnCloudShadowCol =
                new(70f / 255f, 3f / 255f, 3f / 255f,
                    104f / 255f); // new Color(201f/255f,150f/255f,140f/255f,63f/255f);

        public Color
            DawnCloudTintCol =
                new(11f / 255f, 10f / 255f, 10f / 255f,
                    255f / 255f); //new Color(201f/255f,150f/255f,120f/255f,255f/255f);

        public Color DawnCloudFogCol = new(246f / 255f, 19f / 255f, 14f / 255f, 225f / 255f);
        public float DawnFogFactor = 1.94f;
        public float DawnIntensity = -3.79f;

        //	Color DuskCloudColor = new Color(191f/255f,99f/255f,119f/255f,209f/255f);
        public Color DuskCloudColor = new(31f / 255f, 29f / 255f, 29f / 255f, 37f / 255f);
        public Color DuskCloudShadowCol = new(161f / 255f, 82f / 255f, 39f / 255f, 63f / 255f);

        public Color
            DuskCloudTintCol =
                new(11f / 255f, 10f / 255f, 10f / 255f,
                    255f / 255f); //new Color(201f/255f,150f/255f,120f/255f,255f/255f);

        public Color DuskCloudFogCol = new(246f / 255f, 19f / 255f, 14f / 255f, 225f / 255f);
        public float DuskFogFactor = 1.94f;
        public float DuskIntensity = -1.25f;

        public Color NightCloudColor = new(220f / 255f, 225f / 255f, 210f / 255f, 7f / 255f); //v3.4.3

        [HideInInspector] public bool HasScatterShader;
        //[HideInInspector] public bool UpdateScatterShader = false;

        //scatter params
        public float fog_depth = 0.29f; // 1.5f;
        public float reileigh = 1.3f; //2.0f;
        public float mieCoefficient = 1; //0.1f;
        public float mieDirectionalG = 0.1f;
        public float ExposureBias = 0.11f; //0.15f; 
        public Vector3 lambda = new(680E-9f, 550E-9f, 450E-9f); //new Vector3(680E-9f, 550E-9f, 450E-9f);
        public Vector3 K = new(0.9f, 0.5f, 0.5f); //new Vector3(0.686f, 0.678f, 0.666f);

        public float WindStrength = 1;
        public float WindParallaxFactor = 1.2f;

        //public bool AutoDensity = true;
        public float CloudDensity = 0.0001f;

        //v1.7
        public float vortexVorticity = 30;
        public float vortexSpeed = 1;
        public float vortexThickness = 1;
        public float vortexCutoff = 1;
        public Vector3 vortexPosition = new(0, 0, 0);
        public bool grabVortexParamsOfShader = true; //initialize with current vortex shader values
        public Transform LightningBox;
        public float lightning_every = 15;
        public float max_lightning_time = 2;
        public float lightning_rate_offset = 5;

        public ChainLightning_SKYMASTER lightningScript1;
        public ChainLightning_SKYMASTER lightningScript2;

        private float last_lightning_time;
        private Transform LightningOne;
        private Transform LightningTwo;

        private int prev_preset;

        // bool was_rain = false;
        private float was_rain_end_time;
        public float Current_Time => TimeManager.Hour;

        private void Start() {
            if (Application.isPlaying) {
                if (rainbowMat != null) {
                    var RainbowC = rainbowMat.GetColor(Color1);
                    rainbowMat.SetColor(Color1, new Color(RainbowC.r, RainbowC.g, RainbowC.b, 0));
                }


                //v3.4 - if not defined, get material for water
                if (cloudFlatMat == null)
                    cloudFlatMat = FlatBedClouds.gameObject.GetComponentsInChildren<MeshRenderer>(true)[0].material;
                if (cloudSidesMat == null)
                    cloudSidesMat = SideClouds.gameObject.GetComponentsInChildren<MeshRenderer>(true)[0].material;
                if (cloudSidesAMat == null)
                    cloudSidesAMat = SideAClouds.gameObject.GetComponentsInChildren<MeshRenderer>(true)[0].material;
                if (cloudSidesBMat == null)
                    cloudSidesBMat = SideBClouds.gameObject.GetComponentsInChildren<MeshRenderer>(true)[0].material;
                if (cloudSidesCMat == null) //v3.4.3
                    cloudSidesCMat = SideCClouds.gameObject.GetComponentsInChildren<MeshRenderer>(true)[0].material;
                if (cloudRotMat == null)
                    cloudRotMat = RotClouds.gameObject.GetComponentsInChildren<MeshRenderer>(true)[0].material;

                //v3.0


                if (WeatherDensity) {
                    // && Application.isPlaying) { //v3.4.3

                    if (cloudType == CloudPreset.ClearDay) {
                        Coverage = ClearDayCoverage;
                        Horizon = ClearDayHorizon;
                        EnableLightning = false;
                    }

                    if (cloudType == CloudPreset.Storm) {
                        Coverage = StormCoverage;
                        Horizon = StormHorizon;
                        EnableLightning = true;
                    }
                }
            }

            if (FlatBedClouds != null && cloudFlatMat != null) DomeScale = FlatBedClouds.localScale;
            //DomeHeights = new Vector3 (FlatBedClouds.position.y,cloudFlatMat.GetFloat(CutHeight), 0);
            if (SideClouds != null && cloudSidesMat != null) MultiQuadScale = SideClouds.localScale;
            //MultiQuadHeights = new Vector3 (SideClouds.position.y,cloudSidesMat.GetFloat(CutHeight), 0);
            if (SideAClouds != null && cloudSidesAMat != null) MultiQuadAScale = SideAClouds.localScale;
            //MultiQuadAHeights = new Vector3 (SideAClouds.position.y,cloudSidesAMat.GetFloat(CutHeight), 0);
            if (SideBClouds != null && cloudSidesBMat != null) MultiQuadBScale = SideBClouds.localScale;
            //MultiQuadBHeights = new Vector3 (SideBClouds.position.y,cloudSidesBMat.GetFloat(CutHeight), 0);
            if (SideCClouds != null && cloudSidesCMat != null) //v3.4.3
                MultiQuadCScale = SideCClouds.localScale;
            //MultiQuadCHeights = new Vector3 (SideCClouds.position.y,cloudSidesCMat.GetFloat(CutHeight), 0);
            if (RotClouds != null && cloudRotMat != null) OneQuadScale = RotClouds.localScale;
            //OneQuadHeights = new Vector3 (RotClouds.position.y,cloudRotMat.GetFloat(CutHeight), 0);
        }


        // Update is called once per frame
        public void Update() {
            //v1.7

            //v3.4
            if (MultiQuadClouds) {
                if (!SideClouds.gameObject.activeInHierarchy) SideClouds.gameObject.SetActive(true);
                if (SideClouds != null && cloudSidesMat != null) {
                    SideClouds.localScale = MultiQuadScale;
                    SideClouds.position = new Vector3(SideClouds.position.x, MultiQuadHeights.x, SideClouds.position.z);
                    cloudSidesMat.SetFloat(CutHeight, MultiQuadHeights.y);
                }
            }
            else {
                if (SideClouds != null && SideClouds.gameObject.activeInHierarchy)
                    SideClouds.gameObject.SetActive(false);
            }

            if (MultiQuadAClouds) {
                if (!SideAClouds.gameObject.activeInHierarchy) SideAClouds.gameObject.SetActive(true);
                if (SideAClouds != null && cloudSidesAMat != null) {
                    SideAClouds.localScale = MultiQuadAScale;
                    SideAClouds.position =
                        new Vector3(SideAClouds.position.x, MultiQuadAHeights.x, SideAClouds.position.z);
                    cloudSidesAMat.SetFloat(CutHeight, MultiQuadAHeights.y);
                }
            }
            else {
                if (SideAClouds != null && SideAClouds.gameObject.activeInHierarchy)
                    SideAClouds.gameObject.SetActive(false);
            }

            if (MultiQuadBClouds) {
                if (!SideBClouds.gameObject.activeInHierarchy) SideBClouds.gameObject.SetActive(true);
                if (SideBClouds != null && cloudSidesBMat != null) {
                    SideBClouds.localScale = MultiQuadBScale;
                    SideBClouds.position =
                        new Vector3(SideBClouds.position.x, MultiQuadBHeights.x, SideBClouds.position.z);
                    cloudSidesBMat.SetFloat(CutHeight, MultiQuadBHeights.y);
                }
            }
            else {
                if (SideBClouds != null && SideBClouds.gameObject.activeInHierarchy)
                    SideBClouds.gameObject.SetActive(false);
            }

            //v3.4.3
            if (MultiQuadCClouds) {
                if (!SideCClouds.gameObject.activeInHierarchy) SideCClouds.gameObject.SetActive(true);
                if (SideCClouds != null && cloudSidesCMat != null) {
                    SideCClouds.localScale = MultiQuadCScale;
                    SideCClouds.position =
                        new Vector3(SideCClouds.position.x, MultiQuadCHeights.x, SideCClouds.position.z);
                    cloudSidesCMat.SetFloat(CutHeight, MultiQuadCHeights.y);
                }
            }
            else {
                if (SideCClouds != null && SideCClouds.gameObject.activeInHierarchy)
                    SideCClouds.gameObject.SetActive(false);
            }

            if (OneQuadClouds) {
                if (!RotClouds.gameObject.activeInHierarchy) RotClouds.gameObject.SetActive(true);
                if (RotClouds != null && cloudRotMat != null) {
                    RotClouds.localScale = OneQuadScale;
                    RotClouds.position = new Vector3(RotClouds.position.x, OneQuadHeights.x, RotClouds.position.z);
                    cloudRotMat.SetFloat(CutHeight, OneQuadHeights.y);
                }
            }
            else {
                if (RotClouds != null && RotClouds.gameObject.activeInHierarchy) RotClouds.gameObject.SetActive(false);
            }

            if (DomeClouds) {
                if (!FlatBedClouds.gameObject.activeInHierarchy) FlatBedClouds.gameObject.SetActive(true);
                if (FlatBedClouds != null && cloudFlatMat != null) {
                    FlatBedClouds.localScale = DomeScale;
                    FlatBedClouds.position =
                        new Vector3(FlatBedClouds.position.x, DomeHeights.x, FlatBedClouds.position.z);
                    cloudFlatMat.SetFloat(CutHeight, DomeHeights.y);
                }
            }
            else {
                if (FlatBedClouds != null && FlatBedClouds.gameObject.activeInHierarchy)
                    FlatBedClouds.gameObject.SetActive(false);
            }

            //v3.0
            if (Sun != null) {
                if (cloudFlatMat != null && DomeClouds) UpdateCloudParams(false, cloudFlatMat);
                if (cloudSidesMat != null && SideClouds) UpdateCloudParams(false, cloudSidesMat);
                if (cloudSidesAMat != null && SideAClouds) UpdateCloudParams(false, cloudSidesAMat);
                if (cloudSidesBMat != null && SideBClouds) UpdateCloudParams(false, cloudSidesBMat);
                if (cloudSidesCMat != null && SideCClouds)
                    //v3.4.3
                    UpdateCloudParams(false, cloudSidesCMat);
                if (cloudRotMat != null && OneQuadClouds) UpdateCloudParams(false, cloudRotMat);
            }
            //RAINBOW

            if (rainbowMat != null) {
                var RainbowC = rainbowMat.GetColor(Color1);
                if (!Application.isPlaying) {
                    //v3.4.5
                    var IntensityR = rainboxMaxIntensity;
                    if (autoRainbow) IntensityR = rainboxMinIntensity;
                    rainbowMat.SetColor(Color1, new Color(RainbowC.r, RainbowC.g, RainbowC.b, IntensityR));
                }
                else {
                    var IntensityR = rainboxMaxIntensity;

                    rainbowMat.SetColor(Color1,
                        Color.Lerp(RainbowC, new Color(RainbowC.r, RainbowC.g, RainbowC.b, IntensityR),
                            Time.deltaTime * 0.001f * rainbowApperSpeed + 0.002f));
                }
            }


            if (Application.isPlaying) {
                if (EnableLightning) {
                    if (LightningOne == null) {
                        LightningOne = Instantiate(LightningPrefab).transform;
                        LightningOne.gameObject.SetActive(false);
                        lightningScript1 =
                            LightningOne.gameObject.GetComponentInChildren<ChainLightning_SKYMASTER>(true); //HDRP
                    }

                    if (LightningTwo == null) {
                        LightningTwo = Instantiate(LightningPrefab).transform;
                        LightningTwo.gameObject.SetActive(false);
                        lightningScript2 =
                            LightningOne.gameObject.GetComponentInChildren<ChainLightning_SKYMASTER>(true); //HDRP
                    }


                    //move around
                    if (LightningBox != null) {
                        if (Time.fixedTime - last_lightning_time > lightning_every -
                            Random.Range(-lightning_rate_offset, lightning_rate_offset)) {
                            var MinMaXLRangeX = LightningBox.position.x * Vector2.one +
                                                LightningBox.localScale.x / 2 * new Vector2(-1, 1);
                            var MinMaXLRangeY = LightningBox.position.y * Vector2.one +
                                                LightningBox.localScale.y / 2 * new Vector2(-1, 1);
                            var MinMaXLRangeZ = LightningBox.position.z * Vector2.one +
                                                LightningBox.localScale.z / 2 * new Vector2(-1, 1);

                            LightningOne.position = new Vector3(Random.Range(MinMaXLRangeX.x, MinMaXLRangeX.y),
                                Random.Range(MinMaXLRangeY.x, MinMaXLRangeY.y),
                                Random.Range(MinMaXLRangeZ.x, MinMaXLRangeZ.y));
                            if (Random.Range(0, WeatherSeverity + 1) == 1) {
                                //do nothing
                            }
                            else {
                                LightningOne.gameObject.SetActive(true);
                            }

                            LightningTwo.position = new Vector3(Random.Range(MinMaXLRangeX.x, MinMaXLRangeX.y),
                                Random.Range(MinMaXLRangeY.x, MinMaXLRangeY.y),
                                Random.Range(MinMaXLRangeZ.x, MinMaXLRangeZ.y));
                            if (Random.Range(0, WeatherSeverity + 1) == 1) {
                                //do nothing
                            }
                            else {
                                LightningTwo.gameObject.SetActive(true);
                            }

                            last_lightning_time = Time.fixedTime;
                        }
                        else {
                            if (Time.fixedTime - last_lightning_time > max_lightning_time) {
                                if (LightningOne != null)
                                    if (LightningOne.gameObject.activeInHierarchy)
                                        LightningOne.gameObject.SetActive(false);
                                if (LightningTwo != null)
                                    if (LightningTwo.gameObject.activeInHierarchy)
                                        LightningTwo.gameObject.SetActive(false);
                            }
                        }
                    }
                }
                else {
                    if (LightningOne != null)
                        if (LightningOne.gameObject.activeInHierarchy)
                            LightningOne.gameObject.SetActive(false);
                    if (LightningTwo != null)
                        if (LightningTwo.gameObject.activeInHierarchy)
                            LightningTwo.gameObject.SetActive(false);
                }

                if (rotateWithCamera && Camera.main != null) {
                    //v3.4.3
                    if (RotClouds != null && RotClouds.gameObject.activeInHierarchy) {
                        if (!EnableInsideClouds) {
                            //v3.4.3
                            //Vector3 prevScale = Vector3.one;
                            if (RotCloudsSPlane != null && Application.isPlaying) {
                                RotCloudsSPlane.parent = null;
                                if (!RotCloudsSPlane.gameObject.activeInHierarchy)
                                    RotCloudsSPlane.gameObject.SetActive(true);
                                //prevScale = RotCloudsSPlane.localScale;
                            }

                            if (LerpRot)
                                RotClouds.eulerAngles = new Vector3(RotClouds.eulerAngles.x,
                                    Mathf.Lerp(RotClouds.eulerAngles.y, Camera.main.transform.eulerAngles.y,
                                        Time.deltaTime * 20), RotClouds.eulerAngles.z);
                            else
                                RotClouds.eulerAngles = new Vector3(RotClouds.eulerAngles.x,
                                    Camera.main.transform.eulerAngles.y, RotClouds.eulerAngles.z);
                        }
                        else {
                            float Desired_rot = 0;
                            var Cam_transf = Camera.main.transform;
                            var Ydiff = Cam_transf.position.y - 100;
                            if (Ydiff > 0) {
                                Desired_rot = Ydiff / 10 - Cam_transf.eulerAngles.x;
                                Debug.Log(Desired_rot);
                            }

                            RotClouds.eulerAngles = new Vector3(Desired_rot, Cam_transf.eulerAngles.y,
                                RotClouds.eulerAngles.z);
                        }
                    }
                    else {
                        if (RotCloudsSPlane != null && RotCloudsSPlane.gameObject.activeInHierarchy)
                            RotCloudsSPlane.gameObject.SetActive(false);
                    }

                    if (SideClouds != null && SideClouds.gameObject.activeInHierarchy) {
                        if (!EnableInsideClouds) {
                            //v3.4.3
                            //Vector3 prevScale = Vector3.one;
                            if (SideSPlane != null && Application.isPlaying) {
                                SideSPlane.parent = null;
                                if (!SideSPlane.gameObject.activeInHierarchy) SideSPlane.gameObject.SetActive(true);
                                //prevScale = SideSPlane.localScale;
                            }

                            if (LerpRot)
                                SideClouds.eulerAngles = new Vector3(SideClouds.eulerAngles.x,
                                    Mathf.Lerp(SideClouds.eulerAngles.y, Camera.main.transform.eulerAngles.y,
                                        Time.deltaTime * 20), SideClouds.eulerAngles.z);
                            else
                                SideClouds.eulerAngles = new Vector3(SideClouds.eulerAngles.x,
                                    Camera.main.transform.eulerAngles.y, SideClouds.eulerAngles.z);
                        }
                    }
                    else {
                        //v3.4.3
                        if (SideSPlane != null && SideSPlane.gameObject.activeInHierarchy)
                            SideSPlane.gameObject.SetActive(false);
                    }

                    //v3.4.3
                    if (rotateMultiQuadC && SideCClouds != null && SideCClouds.gameObject.activeInHierarchy) {
                        if (!EnableInsideClouds) {
                            //v3.4.3
                            //Vector3 prevScale = Vector3.one;
                            if (SideCSPlane != null && Application.isPlaying) {
                                SideCSPlane.parent = null;
                                if (!SideCSPlane.gameObject.activeInHierarchy) SideCSPlane.gameObject.SetActive(true);
                                //prevScale = SideSPlane.localScale;
                            }

                            if (LerpRot)
                                SideCClouds.eulerAngles = new Vector3(SideCClouds.eulerAngles.x,
                                    Mathf.Lerp(SideCClouds.eulerAngles.y, Camera.main.transform.eulerAngles.y,
                                        Time.deltaTime * 20), SideCClouds.eulerAngles.z);
                            else
                                SideCClouds.eulerAngles = new Vector3(SideCClouds.eulerAngles.x,
                                    Camera.main.transform.eulerAngles.y, SideCClouds.eulerAngles.z);
                        }
                    }
                    else {
                        //v3.4.3
                        if (SideCSPlane != null && SideCSPlane.gameObject.activeInHierarchy)
                            SideCSPlane.gameObject.SetActive(false);
                    }
                }


                if (followPlayer && Hero != null) {
                    //v3.4.3
                    SideClouds.position = new Vector3(Hero.position.x, SideClouds.position.y, Hero.position.z);
                    SideAClouds.position = new Vector3(Hero.position.x, SideAClouds.position.y, Hero.position.z);
                    SideBClouds.position = new Vector3(Hero.position.x, SideBClouds.position.y, Hero.position.z);
                    SideCClouds.position = new Vector3(Hero.position.x, SideCClouds.position.y, Hero.position.z);
                    FlatBedClouds.position = new Vector3(Hero.position.x, FlatBedClouds.position.y, Hero.position.z);
                    RotClouds.position = new Vector3(Hero.position.x, RotClouds.position.y, Hero.position.z);

                    //v3.4.3
                    if (SideSPlane != null)
                        SideSPlane.position = new Vector3(Hero.position.x, SideSPlane.position.y, Hero.position.z);
                    if (SideCSPlane != null)
                        SideCSPlane.position = new Vector3(Hero.position.x, SideCSPlane.position.y, Hero.position.z);
                    if (RotCloudsSPlane != null)
                        RotCloudsSPlane.position =
                            new Vector3(Hero.position.x, RotCloudsSPlane.position.y, Hero.position.z);
                }
            }
        }


        private void UpdateCloudParams(bool Init, Material oceanMat) {
            //v1.7
            if (oceanMat.HasProperty(VortexControl)) {
                if (grabVortexParamsOfShader) {
                    grabVortexParamsOfShader = false;
                    var paramsVortex = oceanMat.GetVector(VortexControl);
                    var posVortex = oceanMat.GetVector(VortexPosRadius);
                    vortexVorticity = paramsVortex.x;
                    vortexSpeed = paramsVortex.y;
                    vortexThickness = paramsVortex.z;
                    vortexCutoff = paramsVortex.w;
                    vortexPosition = new Vector3(posVortex.x, posVortex.y, posVortex.z);
                }

                oceanMat.SetVector(VortexControl,
                    new Vector4(vortexVorticity, vortexSpeed, vortexThickness, vortexCutoff));
                oceanMat.SetVector(VortexPosRadius,
                    new Vector4(vortexPosition.x, vortexPosition.y, vortexPosition.z, 1));
            }

            //IF UNDERWATER
            //float Sm_speed = SkyManager.SPEED;// + minShiftSpeed;
            var Time_delta = Time.deltaTime;

            if (Init) //Sm_speed=10000;
                Time_delta = 10000;

            if (Sun != null) oceanMat.SetVector(SunPosition, -Sun.forward.normalized);


            //APPLY WIND
            if (windZone) {
                var WindStr = windZone.windMain;
                var WindDir =
                    new Vector4(windZone.transform.forward.x, windZone.transform.forward.z, 0, -0.07f * 0.1f) *
                    (-WindStr * WindStrength);
                var WindParallaxDir = new Vector4(WindDir.x * WindParallaxFactor, WindDir.y * WindParallaxFactor, 0f,
                    CloudEvolution);

                oceanMat.SetVector(Velocity1, WindDir);
                oceanMat.SetVector(Velocity2, WindParallaxDir);
                oceanMat.SetFloat(TimePass, TimeManager.Hour);
            }

            if (HasScatterShader) {
                //v3.4.5
                if (!Application.isPlaying) //Speed_factor = 1000 * Speed_factor;
                    Time_delta = 10000;

                //GRAB PARAMETERS
                var _SunColor = oceanMat.GetColor(SunColor);
                var _ShadowColor = oceanMat.GetColor(ShadowColor);
                //			float _ColorDiff = oceanMat.GetFloat("_ColorDiff");

                //					_CloudMap ("_CloudMap", 2D) = "white" {}
                //					_CloudMap1 ("_CloudMap1", 2D) = "white" {}

                var _Density = oceanMat.GetFloat(Density);
                //if (AutoDensity) {

                //}
                oceanMat.SetFloat(Density, Mathf.Lerp(_Density, CloudDensity, 0.27f * Time_delta + 0.05f)); //v3.4.3

                var _Coverage = oceanMat.GetFloat(Coverage1);
                var _Transparency = oceanMat.GetFloat(Transparency);

                //	Vector4 _LightingControl = oceanMat.GetVector ("_LightingControl");   
                var _HorizonFactor = oceanMat.GetFloat(HorizonFactor);

                var _Color = oceanMat.GetColor(Color1);
                var _FogColor = oceanMat.GetColor(FogColor);

                if (UseUnityFog)
                    oceanMat.SetFloat(FogUnity, 1);
                else
                    oceanMat.SetFloat(FogUnity, 0);

                var Speed_factor = 0.3f * (coverageSpeed + 0.1f); //0.27

                //v3.4.5
                if (!Application.isPlaying) Speed_factor = 1000 * Speed_factor;
                //Time_delta=10000;

                float EvalSunIntensity = 0;
                float EvalLightDiff = 0;
                float EvalFog = 0;
                if (useGradients) {
                    EvalSunIntensity = IntensitySun.Evaluate(Current_Time) + IntensitySunOffset;
                    EvalLightDiff = IntensityDiff.Evaluate(Current_Time) + IntensityDiffOffset + 0.30f; //v3.4.3
                    EvalFog = IntensityFog.Evaluate(Current_Time) + IntensityFogOffset;
                }
                else {
                    EvalSunIntensity = IntensitySunOffset;
                    //oceanMat.SetFloat("_ColorDiff", IntensityDiffOffset - 0.50f);
                    EvalLightDiff = IntensityDiffOffset + 0.30f; //v3.4.3
                    EvalFog = IntensityFogOffset;
                }

                var Rot_Sun_X = Sun.transform.eulerAngles.x;

                var is_DayLight = (AutoSunPosition && Rot_Sun_X > 0) || (!AutoSunPosition &&
                                                                         Current_Time > 9.0f + Shift_dawn &&
                                                                         Current_Time <= NightTimeMax + Shift_dawn);

                var is_after_17 = (AutoSunPosition && Rot_Sun_X > 65) ||
                                  (!AutoSunPosition && Current_Time > 17.0f + Shift_dawn);

                var is_after_222 = (AutoSunPosition && Rot_Sun_X > 85) ||
                                   (!AutoSunPosition && Current_Time > 22.0f + Shift_dawn);

                var is_before_10 = (AutoSunPosition && Rot_Sun_X < 10) ||
                                   (!AutoSunPosition && Current_Time < 10.0f + Shift_dawn);

                //	bool is_after_22  = (SkyManager.AutoSunPosition && SkyManager.Rot_Sun_X < 5 ) | (!SkyManager.AutoSunPosition && SkyManager.Current_Time >  (21.0f + SkyManager.Shift_dawn));

                //	bool is_after_21  = (SkyManager.AutoSunPosition && SkyManager.Rot_Sun_X > 75) | (!SkyManager.AutoSunPosition && SkyManager.Current_Time >  (20.7f + SkyManager.Shift_dawn));

                //bool is_DayLight  = (SkyManager.AutoSunPosition && SkyManager.Rot_Sun_X > 0 ) | (!SkyManager.AutoSunPosition && SkyManager.Current_Time > ( 9.0f + SkyManager.Shift_dawn) & SkyManager.Current_Time <= (21.9f + SkyManager.Shift_dawn));
                //bool is_after_17  = (SkyManager.AutoSunPosition && SkyManager.Rot_Sun_X > 65) | (!SkyManager.AutoSunPosition && SkyManager.Current_Time >  (17.1f + SkyManager.Shift_dawn));

                if (is_DayLight) {
                    oceanMat.SetVector(SunPosition, -Sun.transform.forward.normalized);
                    //oceanMat.SetVector (SunColor, Color.Lerp (oceanMat.GetVector (SunColor), DayCloudColor, 0.5f * Time.deltaTime));


                    if (is_after_17) {
                        //is_after_21
                        //oceanMat.SetVector ("_TintColor", Color.Lerp (oceanMat.GetVector ("_TintColor"), Color.Lerp (DayCloudColor, DuskCloudColor, 0.5f), 0.5f * Time.deltaTime));
                        if (!is_after_222) {
                            if (cloudType == CloudPreset.Custom) { }
                            else if (cloudType == CloudPreset.ClearDay) {
                                oceanMat.SetColor(SunColor,
                                    Color.Lerp(_SunColor, DuskCloudColor + new Color(0, 0, 0, EvalSunIntensity),
                                        3.0f * Speed_factor * Time_delta)); //v3.4.3 - added dusk colors

                                oceanMat.SetColor(ShadowColor,
                                    Color.Lerp(_ShadowColor, DuskCloudShadowCol, Speed_factor * Time_delta));

                                //			oceanMat.SetFloat ("_ColorDiff", Mathf.Lerp (_ColorDiff, Intensity + DuskIntensity, Speed_factor * Time_delta));//hardcoded _ColorDiff
                                oceanMat.SetFloat(Coverage1,
                                    Mathf.Lerp(_Coverage, Coverage, Speed_factor * Time_delta));
                                oceanMat.SetFloat(Transparency,
                                    Mathf.Lerp(_Transparency, Transp, Speed_factor * Time_delta));
                                //			oceanMat.SetFloat (HorizonFactor, Mathf.Lerp (_HorizonFactor, Horizon, Speed_factor * Time_delta));

                                oceanMat.SetColor(Color1,
                                    Color.Lerp(_Color, DuskCloudTintCol, Speed_factor * Time_delta));
                                oceanMat.SetColor(FogColor,
                                    Color.Lerp(_FogColor, DuskCloudFogCol, Speed_factor * Time_delta));
                                //			oceanMat.SetFloat ("_FogFactor", Mathf.Lerp (_FogFactor, DuskFogFactor, Speed_factor * Time_delta));
                            }
                        }
                        else {
                            ////////////////////////////////////////// NIGHT CLOUDS
                            if (cloudType == CloudPreset.Custom) { }
                            else if (cloudType == CloudPreset.ClearDay) {
                                oceanMat.SetColor(SunColor,
                                    Color.Lerp(_SunColor, DayCloudColor * 0.3f + new Color(0, 0, 0, EvalSunIntensity),
                                        0.2f * Speed_factor * Time_delta));

                                oceanMat.SetColor(ShadowColor,
                                    Color.Lerp(_ShadowColor, 4 * DayCloudShadowCol, Speed_factor * Time_delta));

                                //			oceanMat.SetFloat ("_ColorDiff", Mathf.Lerp (_ColorDiff, Intensity + DayIntensity, 0.2f * Speed_factor * Time_delta));//hardcoded _ColorDiff
                                oceanMat.SetFloat(Coverage1,
                                    Mathf.Lerp(_Coverage, Coverage, Speed_factor * Time_delta));
                                oceanMat.SetFloat(Transparency,
                                    Mathf.Lerp(_Transparency, Transp, Speed_factor * Time_delta));
                                //			oceanMat.SetFloat (HorizonFactor, Mathf.Lerp (_HorizonFactor, Horizon, Speed_factor * Time_delta));

                                oceanMat.SetColor(Color1,
                                    Color.Lerp(_Color, DayCloudTintCol, Speed_factor * Time_delta));
                                oceanMat.SetColor(FogColor,
                                    Color.Lerp(_FogColor, DayCloudFogCol, 4 * Speed_factor * Time_delta));
                                //		oceanMat.SetFloat ("_FogFactor", Mathf.Lerp (_FogFactor, DayFogFactor * 2, 0.02f * Speed_factor * Time_delta));
                            }
                        }
                    }
                    else {
                        if (is_before_10) {
                            if (cloudType == CloudPreset.Custom) { }
                            else if (cloudType == CloudPreset.ClearDay) {
                                oceanMat.SetColor(SunColor,
                                    Color.Lerp(_SunColor, DawnCloudColor + new Color(0, 0, 0, EvalSunIntensity),
                                        1.0f * Speed_factor * Time_delta)); //

                                oceanMat.SetColor(ShadowColor,
                                    Color.Lerp(_ShadowColor, DawnCloudShadowCol, Speed_factor * Time_delta));

                                //			oceanMat.SetFloat ("_ColorDiff", Mathf.Lerp (_ColorDiff, Intensity + DawnIntensity, Speed_factor * Time_delta));//hardcoded _ColorDiff
                                oceanMat.SetFloat(Coverage1,
                                    Mathf.Lerp(_Coverage, Coverage, Speed_factor * Time_delta));
                                oceanMat.SetFloat(Transparency,
                                    Mathf.Lerp(_Transparency, Transp, Speed_factor * Time_delta));
                                //			oceanMat.SetFloat (HorizonFactor, Mathf.Lerp (_HorizonFactor, Horizon, Speed_factor * Time_delta));

                                oceanMat.SetColor(Color1,
                                    Color.Lerp(_Color, DawnCloudTintCol, Speed_factor * Time_delta));
                                oceanMat.SetColor(FogColor,
                                    Color.Lerp(_FogColor, DawnCloudFogCol, Speed_factor * Time_delta));
                                //		oceanMat.SetFloat ("_FogFactor", Mathf.Lerp (_FogFactor, DawnFogFactor, Speed_factor * Time_delta));
                            }
                            //oceanMat.SetVector ("_TintColor", Color.Lerp (oceanMat.GetVector ("_TintColor"), Color.Lerp (DayCloudColor, DawnCloudColor, 0.5f), 0.5f * Time.deltaTime * 0.2f * SkyManager.DawnAppearSpeed)); 
                        }
                        else {
                            if (cloudType == CloudPreset.Custom) { }
                            else if (cloudType == CloudPreset.ClearDay) {
                                oceanMat.SetColor(SunColor,
                                    Color.Lerp(_SunColor, DayCloudColor + new Color(0, 0, 0, EvalSunIntensity),
                                        0.3f * Speed_factor * Time_delta));

                                oceanMat.SetColor(ShadowColor,
                                    Color.Lerp(_ShadowColor, DayCloudShadowCol, Speed_factor * Time_delta));

                                //		oceanMat.SetFloat ("_ColorDiff", Mathf.Lerp (_ColorDiff, Intensity + DayIntensity, Speed_factor * Time_delta));//hardcoded _ColorDiff
                                oceanMat.SetFloat(Coverage1,
                                    Mathf.Lerp(_Coverage, Coverage, Speed_factor * Time_delta));
                                oceanMat.SetFloat(Transparency,
                                    Mathf.Lerp(_Transparency, Transp, Speed_factor * Time_delta));
                                //		oceanMat.SetFloat (HorizonFactor, Mathf.Lerp (_HorizonFactor, Horizon, Speed_factor * Time_delta));

                                oceanMat.SetColor(Color1,
                                    Color.Lerp(_Color, DayCloudTintCol, Speed_factor * Time_delta));
                                oceanMat.SetColor(FogColor,
                                    Color.Lerp(_FogColor, DayCloudFogCol, Speed_factor * Time_delta));
                                //		oceanMat.SetFloat ("_FogFactor", Mathf.Lerp (_FogFactor, DayFogFactor, Speed_factor * Time_delta));
                            }
                            //oceanMat.SetVector ("_TintColor", Color.Lerp (oceanMat.GetVector ("_TintColor"), Color.Lerp (DayCloudColor, DawnCloudColor, 0.5f), 0.5f * Time.deltaTime)); 
                        }
                    }
                }
                else {
                    if (Moon != null)
                        oceanMat.SetVector(SunPosition, -Moon.transform.forward.normalized);
                    else
                        oceanMat.SetVector(SunPosition, -Sun.transform.forward.normalized);
                    //oceanMat.SetVector (SunColor, Color.Lerp (oceanMat.GetVector (SunColor), NightCloudColor, 0.5f * Time.deltaTime));//		

                    ////////////////////////////////////////// NIGHT CLOUDS
                    if (cloudType == CloudPreset.Custom) { }
                    else if (cloudType == CloudPreset.ClearDay) {
                        //oceanMat.SetColor (SunColor, Color.Lerp (_SunColor, DayCloudColor * 0.3f, 0.8f * Speed_factor * Time_delta));
                        oceanMat.SetColor(SunColor,
                            Color.Lerp(_SunColor, NightCloudColor + new Color(0, 0, 0, EvalSunIntensity),
                                0.8f * Speed_factor * Time_delta)); //v3.4.3

                        oceanMat.SetColor(ShadowColor,
                            Color.Lerp(_ShadowColor, 1 * DayCloudShadowCol, Speed_factor * Time_delta));

                        //	oceanMat.SetFloat ("_ColorDiff", Mathf.Lerp (_ColorDiff, Intensity + DayIntensity, 0.2f * Speed_factor * Time_delta));//hardcoded _ColorDiff
                        oceanMat.SetFloat(Coverage1, Mathf.Lerp(_Coverage, Coverage, Speed_factor * Time_delta));
                        oceanMat.SetFloat(Transparency, Mathf.Lerp(_Transparency, Transp, Speed_factor * Time_delta));
                        //	oceanMat.SetFloat (HorizonFactor, Mathf.Lerp (_HorizonFactor, Horizon, Speed_factor * Time_delta));

                        oceanMat.SetColor(Color1, Color.Lerp(_Color, DayCloudTintCol, Speed_factor * Time_delta));
                        oceanMat.SetColor(FogColor,
                            Color.Lerp(_FogColor, DayCloudFogCol, 4 * Speed_factor * Time_delta));
                        //	oceanMat.SetFloat ("_FogFactor", Mathf.Lerp (_FogFactor, DayFogFactor * 2, 0.22f * Speed_factor * Time_delta));
                    }
                }

                //STORM
                if (WeatherDensity && cloudType == CloudPreset.Storm)
                    // StormSunColor
                    oceanMat.SetColor(SunColor,
                        Color.Lerp(_SunColor, StormSunColor, 2.8f * Speed_factor * Time_delta)); //v3.4.3


                oceanMat.SetFloat(ColorDiff, EvalLightDiff);
                oceanMat.SetFloat(HorizonFactor,
                    Mathf.Lerp(_HorizonFactor, Horizon, Speed_factor * Time_delta + 0.05f)); //v3.4.3
                oceanMat.SetFloat(FogFactor, EvalFog);


                oceanMat.SetVector(BetaR, totalRayleigh(lambda) * reileigh);
                oceanMat.SetVector(BetaM, totalMie(lambda, K, fog_depth) * mieCoefficient);
                oceanMat.SetFloat(FogDepth, fog_depth);
                oceanMat.SetFloat(MieCoefficient, mieCoefficient);
                oceanMat.SetFloat(MieDirectionalG, mieDirectionalG);
                oceanMat.SetFloat(Bias, ExposureBias);
                ///////////////////////////////////////////////////
                /// 

                oceanMat.SetFloat(FogPower, fogPower);
                oceanMat.SetFloat(FogPowerExp, fogPowerExp);


                var modifier = 0.002f; //v3.4.8
                if (!Application.isPlaying) modifier = 0.1f; //8 modifiers

                //HDRP
                if (lightningScript1 != null && LightningOne.gameObject.activeInHierarchy) {
                    //pass light to shader
                    oceanMat.SetVector(PointLightPos, lightningScript1.startLight.transform.position);
                    oceanMat.SetFloat(PointLightPower, lightningScript1.startLight.intensity);
                    oceanMat.SetVector(PointLightColor, lightningScript1.startLight.color);
                }

                if (lightningScript2 != null && LightningTwo.gameObject.activeInHierarchy) {
                    //pass light to shader
                    oceanMat.SetVector(PointLightPos, lightningScript2.startLight.transform.position);
                    oceanMat.SetFloat(PointLightPower, lightningScript2.startLight.intensity);
                    oceanMat.SetVector(PointLightColor, lightningScript2.startLight.color);
                }

                if (lightningScript1 != null && !LightningOne.gameObject.activeInHierarchy &&
                    lightningScript2 != null &&
                    !LightningTwo.gameObject.activeInHierarchy) oceanMat.SetFloat(PointLightPower, 0);

                if (WeatherDensity) {
                    // && Application.isPlaying) { //v3.4.3

                    if (cloudType == CloudPreset.ClearDay) {
                        Coverage = Mathf.Lerp(Coverage, ClearDayCoverage, 0.8f * Speed_factor * Time_delta + modifier);
                        Horizon = Mathf.Lerp(Horizon, ClearDayHorizon, Time_delta + 0.05f); //v3.4.3 - v3.4.5
                        EnableLightning = false;
                    }

                    if (cloudType == CloudPreset.Storm) {
                        Coverage = Mathf.Lerp(Coverage, StormCoverage, 0.8f * Speed_factor * Time_delta + modifier);
                        Horizon = Mathf.Lerp(Horizon, StormHorizon, Time_delta + 0.05f); //v3.4.3 - v3.4.5
                        EnableLightning = true;
                    }
                }
            }
        }

        //UPDATE SCATTER
        private static Vector3 totalRayleigh(Vector3 lambda) {
            var result = new Vector3(
                8.0f * Mathf.Pow(Mathf.PI, 3.0f) * Mathf.Pow(Mathf.Pow(n, 2.0f) - 1.0f, 2.0f) * (6.0f + 3.0f * pn) /
                (3.0f * N * Mathf.Pow(lambda.x, 4.0f) * (6.0f - 7.0f * pn)),
                8.0f * Mathf.Pow(Mathf.PI, 3.0f) * Mathf.Pow(Mathf.Pow(n, 2.0f) - 1.0f, 2.0f) * (6.0f + 3.0f * pn) /
                (3.0f * N * Mathf.Pow(lambda.y, 4.0f) * (6.0f - 7.0f * pn)),
                8.0f * Mathf.Pow(Mathf.PI, 3.0f) * Mathf.Pow(Mathf.Pow(n, 2.0f) - 1.0f, 2.0f) * (6.0f + 3.0f * pn) /
                (3.0f * N * Mathf.Pow(lambda.z, 4.0f) * (6.0f - 7.0f * pn)));
            return result;
        }

        private static Vector3 totalMie(Vector3 lambda, Vector3 K, float T) {
            var c = 0.2f * T * 10E-17f;
            var result = new Vector3(
                0.434f * c * Mathf.PI * Mathf.Pow(2.0f * Mathf.PI / lambda.x, 2.0f) * K.x,
                0.434f * c * Mathf.PI * Mathf.Pow(2.0f * Mathf.PI / lambda.y, 2.0f) * K.y,
                0.434f * c * Mathf.PI * Mathf.Pow(2.0f * Mathf.PI / lambda.z, 2.0f) * K.z
            );
            return result;
        }
    }
}