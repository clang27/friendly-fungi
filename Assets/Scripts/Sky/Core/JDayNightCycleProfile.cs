using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Pinwheel.Jupiter {
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    [CreateAssetMenu(menuName = "Jupiter/Day Night Cycle Profile")]
    public class JDayNightCycleProfile : ScriptableObject {
        private static Dictionary<string, int> propertyRemap;

        [SerializeField] private List<JAnimatedProperty> animatedProperties;

        static JDayNightCycleProfile() {
            InitPropertyRemap();
        }

        private static Dictionary<string, int> PropertyRemap {
            get {
                if (propertyRemap == null) propertyRemap = new Dictionary<string, int>();
                return propertyRemap;
            }
            set => propertyRemap = value;
        }

        public List<JAnimatedProperty> AnimatedProperties {
            get {
                if (animatedProperties == null) animatedProperties = new List<JAnimatedProperty>();
                return animatedProperties;
            }
            set => animatedProperties = value;
        }

        private static void InitPropertyRemap() {
            PropertyRemap.Clear();
            var type = typeof(JSkyProfile);
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var p in props) {
                var animatable = p.GetCustomAttribute(typeof(JAnimatableAttribute), false) as JAnimatableAttribute;
                if (animatable == null)
                    continue;
                var name = p.Name;
                var id = Shader.PropertyToID("_" + name);
                PropertyRemap.Add(name, id);
            }
        }

        public void AddProperty(JAnimatedProperty p, bool setDefaultValue = true) {
            if (setDefaultValue) {
                var defaultProfile = JJupiterSettings.Instance.DefaultDayNightCycleProfile;
                if (defaultProfile != null) {
                    var defaultProp =
                        defaultProfile.AnimatedProperties.Find(p0 => p0.Name != null && p0.Name.Equals(p.Name));
                    if (defaultProp != null) {
                        p.Curve = defaultProp.Curve;
                        p.Gradient = defaultProp.Gradient;
                    }
                }
            }

            AnimatedProperties.Add(p);
        }

        public void Animate(JSky sky, float t) {
            CheckDefaultProfileAndThrow(sky.Profile);

            for (var i = 0; i < AnimatedProperties.Count; ++i) {
                var aProp = AnimatedProperties[i];
                var id = 0;
                if (!PropertyRemap.TryGetValue(aProp.Name, out id)) continue;

                if (aProp.CurveOrGradient == JCurveOrGradient.Curve)
                    sky.Profile.Material.SetFloat(id, aProp.EvaluateFloat(t));
                else
                    sky.Profile.Material.SetColor(id, aProp.EvaluateColor(t));
            }
        }

        private void CheckDefaultProfileAndThrow(JSkyProfile p) {
            if (p == null)
                return;
            if (p == JJupiterSettings.Instance.DefaultProfileSunnyDay ||
                p == JJupiterSettings.Instance.DefaultProfileStarryNight)
                throw new ArgumentException(
                    "Animating default sky profile is prohibited. You must create a new profile for your sky to animate it.");
        }

        public bool ContainProperty(string propertyName) {
            return AnimatedProperties.Exists(p => p.Name.Equals(propertyName));
        }
    }
}