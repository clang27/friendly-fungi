using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Pinwheel.Jupiter {
    [CustomEditor(typeof(JDayNightCycle))]
    public class JDayNightCycleInspector : Editor {
        private static List<JAnimatedProperty> allProperties;

        private static readonly int[] resolutionValues = {
            16, 32, 64, 128, 256, 512, 1024, 2048
        };

        private static readonly string[] resolutionLabels = {
            "16", "32", "64", "128", "256", "512", "1024", "2048"
        };

        private JDayNightCycle cycle;
        private bool isTimeFoldoutExpanded = true;
        private JDayNightCycleProfile profile;

        static JDayNightCycleInspector() {
            InitAllAnimatableProperties();
        }

        private static List<JAnimatedProperty> AllProperties {
            get {
                if (allProperties == null) allProperties = new List<JAnimatedProperty>();
                return allProperties;
            }
            set => allProperties = value;
        }

        private void OnEnable() {
            cycle = target as JDayNightCycle;
            profile = cycle.Profile;
        }

        private void OnSceneGUI() {
            if (cycle == null)
                return;

            var evalTime = Mathf.InverseLerp(0f, 24f, TimeManager.Hour);

            if (cycle.Sky.Profile.EnableSun && cycle.Sky.SunLightSource != null) {
                var c = cycle.Sky.Profile.SunColor;
                c.a = Mathf.Max(0.1f, c.a);

                var pivot = cycle.UseSunPivot && cycle.SunOrbitPivot != null ? cycle.SunOrbitPivot : cycle.transform;
                var normal = pivot.right;
                Handles.color = c;
                float radius = 10;
                Handles.DrawWireDisc(pivot.position, normal, radius);

                var angle = evalTime * 360f;
                var localRotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(angle, 0, 0));
                var localDirection = localRotationMatrix.MultiplyVector(Vector3.up);

                var localToWorld = pivot.localToWorldMatrix;
                var worldDirection = localToWorld.MultiplyVector(localDirection);

                var worldPos = pivot.transform.position - worldDirection * radius;
                Handles.color = c;
                Handles.DrawSolidDisc(worldPos, normal, 1);
            }

            if (cycle.Sky.Profile.EnableMoon && cycle.Sky.MoonLightSource != null) {
                var c = cycle.Sky.Profile.MoonColor;
                c.a = Mathf.Max(0.1f, c.a);

                var pivot = cycle.UseMoonPivot && cycle.MoonOrbitPivot != null ? cycle.MoonOrbitPivot : cycle.transform;
                var normal = pivot.right;
                Handles.color = c;
                float radius = 10;
                Handles.DrawWireDisc(pivot.position, normal, radius);

                var angle = evalTime * 360f;
                var localRotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(angle, 0, 0));
                var localDirection = localRotationMatrix.MultiplyVector(Vector3.down);

                var localToWorld = pivot.localToWorldMatrix;
                var worldDirection = localToWorld.MultiplyVector(localDirection);

                var worldPos = pivot.transform.position - worldDirection * radius;
                Handles.color = c;
                Handles.DrawSolidDisc(worldPos, normal, 1);
            }
        }

        private static void InitAllAnimatableProperties() {
            AllProperties.Clear();
            var type = typeof(JSkyProfile);
            var props = type.GetProperties();
            for (var i = 0; i < props.Length; ++i) {
                var att = props[i].GetCustomAttribute(typeof(JAnimatableAttribute));
                if (att != null) {
                    var animAtt = att as JAnimatableAttribute;
                    AllProperties.Add(JAnimatedProperty.Create(props[i].Name, animAtt.DisplayName,
                        animAtt.CurveOrGradient));
                }
            }
        }

        public override void OnInspectorGUI() {
            EditorGUI.BeginChangeCheck();
            cycle.Profile = JEditorCommon.ScriptableObjectField("Profile", cycle.Profile);
            profile = cycle.Profile;
            if (cycle.Profile == null)
                return;

            DrawSceneReferencesGUI();
            DrawSkyGUI();
            DrawStarsGUI();
            DrawSunGUI();
            DrawMoonGUI();
            DrawHorizonCloudGUI();
            DrawOverheadCloudGUI();
            DrawDetailOverlayGUI();
            DrawEnvironmentReflectionGUI();
            if (EditorGUI.EndChangeCheck()) {
                EditorUtility.SetDirty(cycle);
                EditorUtility.SetDirty(profile);
            }
        }

        private void DrawSceneReferencesGUI() {
            var label = "Scene References";
            var id = "scene-ref" + cycle.GetInstanceID();

            JEditorCommon.Foldout(label, false, id, () => {
                cycle.Sky = EditorGUILayout.ObjectField("Sky", cycle.Sky, typeof(JSky), true) as JSky;
                cycle.SunOrbitPivot =
                    EditorGUILayout.ObjectField("Orbit Pivot", cycle.SunOrbitPivot, typeof(Transform), true) as
                        Transform;
            });
        }

        private void DrawSkyGUI() {
            var label = "Sky";
            var id = "sky" + profile.GetInstanceID();

            JEditorCommon.Foldout(label, false, id, () => {
                DisplayAddedProperties("Sky");
                if (GUILayout.Button("Add")) DisplayAllPropertiesAsContext("Sky");
            });
        }

        private void DrawStarsGUI() {
            var label = "Stars";
            var id = "stars" + profile.GetInstanceID();

            JEditorCommon.Foldout(label, false, id, () => {
                DisplayAddedProperties("Stars");
                if (GUILayout.Button("Add")) DisplayAllPropertiesAsContext("Stars");
            });
        }

        private void DrawSunGUI() {
            var label = "Sun";
            var id = "sun" + profile.GetInstanceID();

            JEditorCommon.Foldout(label, false, id, () => {
                cycle.UseSunPivot = EditorGUILayout.Toggle("Custom Pivot", cycle.UseSunPivot);
                if (cycle.UseSunPivot)
                    cycle.SunOrbitPivot =
                        EditorGUILayout.ObjectField("Pivot", cycle.SunOrbitPivot, typeof(Transform), true) as Transform;
                JEditorCommon.Separator();

                DisplayAddedProperties("Sun");
                if (GUILayout.Button("Add")) DisplayAllPropertiesAsContext("Sun");
            });
        }

        private void DrawMoonGUI() {
            var label = "Moon";
            var id = "moon" + profile.GetInstanceID();

            JEditorCommon.Foldout(label, false, id, () => {
                cycle.UseMoonPivot = EditorGUILayout.Toggle("Custom Pivot", cycle.UseMoonPivot);
                if (cycle.UseMoonPivot)
                    cycle.MoonOrbitPivot =
                        EditorGUILayout.ObjectField("Pivot", cycle.MoonOrbitPivot, typeof(Transform),
                            true) as Transform;
                JEditorCommon.Separator();

                DisplayAddedProperties("Moon");
                if (GUILayout.Button("Add")) DisplayAllPropertiesAsContext("Moon");
            });
        }

        private void DrawHorizonCloudGUI() {
            var label = "Horizon Cloud";
            var id = "horizon-cloud" + profile.GetInstanceID();

            JEditorCommon.Foldout(label, false, id, () => {
                DisplayAddedProperties("Horizon Cloud");
                if (GUILayout.Button("Add")) DisplayAllPropertiesAsContext("Horizon Cloud");
            });
        }

        private void DrawOverheadCloudGUI() {
            var label = "Overhead Cloud";
            var id = "overhead-cloud" + profile.GetInstanceID();

            JEditorCommon.Foldout(label, false, id, () => {
                DisplayAddedProperties("Overhead Cloud");
                if (GUILayout.Button("Add")) DisplayAllPropertiesAsContext("Overhead Cloud");
            });
        }

        private void DrawDetailOverlayGUI() {
            var label = "Detail Overlay";
            var id = "detail-overlay" + profile.GetInstanceID();

            JEditorCommon.Foldout(label, false, id, () => {
                DisplayAddedProperties("Detail Overlay");
                if (GUILayout.Button("Add")) DisplayAllPropertiesAsContext("Detail Overlay");
            });
        }

        private void DrawEnvironmentReflectionGUI() {
            var label = "Environment Reflection";
            var id = "env-reflection";

            JEditorCommon.Foldout(label, false, id, () => {
                cycle.ShouldUpdateEnvironmentReflection =
                    EditorGUILayout.Toggle("Enable", cycle.ShouldUpdateEnvironmentReflection);
                if (cycle.ShouldUpdateEnvironmentReflection) {
                    cycle.EnvironmentReflectionResolution = EditorGUILayout.IntPopup("Resolution",
                        cycle.EnvironmentReflectionResolution, resolutionLabels, resolutionValues);
                    cycle.EnvironmentReflectionTimeSlicingMode =
                        (ReflectionProbeTimeSlicingMode)EditorGUILayout.EnumPopup("Time Slicing",
                            cycle.EnvironmentReflectionTimeSlicingMode);
                    EditorGUILayout.LabelField("Realtime Reflection Probe must be enabled in Quality Settings.",
                        JEditorCommon.WordWrapItalicLabel);
                }
            });
        }

        private void DisplayAddedProperties(string group) {
            EditorGUI.indentLevel -= 1;
            JAnimatedProperty toRemoveProp = null;
            var props = profile.AnimatedProperties.FindAll(p => p.DisplayName.StartsWith(group));
            for (var i = 0; i < props.Count; ++i) {
                EditorGUILayout.BeginHorizontal();
                var p = props[i];
                if (GUILayout.Button("▬", EditorStyles.miniLabel, GUILayout.Width(12))) toRemoveProp = p;

                var itemLabel = p.DisplayName.Substring(p.DisplayName.IndexOf("/") + 1);
                itemLabel = ObjectNames.NicifyVariableName(itemLabel);
                if (p.CurveOrGradient == JCurveOrGradient.Curve)
                    p.Curve = EditorGUILayout.CurveField(itemLabel, p.Curve);
                else
                    p.Gradient = EditorGUILayout.GradientField(new GUIContent(itemLabel), p.Gradient, true);

                EditorGUILayout.EndHorizontal();
            }

            if (props.Count > 0) JEditorCommon.Separator();

            if (toRemoveProp != null) profile.AnimatedProperties.Remove(toRemoveProp);
            EditorGUI.indentLevel += 1;
        }

        private void DisplayAllPropertiesAsContext(string group) {
            var menu = new GenericMenu();
            var props = AllProperties.FindAll(p => p.DisplayName.StartsWith(group));
            if (props.Count == 0) {
                menu.AddDisabledItem(new GUIContent("No item found"));
                menu.ShowAsContext();
                return;
            }

            for (var i = 0; i < props.Count; ++i) {
                var p = props[i];
                var itemLabel = p.DisplayName.Substring(p.DisplayName.IndexOf("/") + 1);
                var added = profile.AnimatedProperties.FindIndex(p0 => p0.Name.Equals(p.Name)) >= 0;

                if (added)
                    menu.AddDisabledItem(new GUIContent(itemLabel));
                else
                    menu.AddItem(
                        new GUIContent(itemLabel),
                        false,
                        () => { profile.AddProperty(p); });
            }

            menu.ShowAsContext();
        }

        public override bool RequiresConstantRepaint() {
            return isTimeFoldoutExpanded;
        }
    }
}