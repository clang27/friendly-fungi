using UnityEngine;

namespace Artngame.SKYMASTER {
    [ExecuteInEditMode]
    public class Circle_Around_ParticleSKYMASTER : MonoBehaviour {
        public float speedMult = 2.0f;

        public bool up_down_motion;
        public bool Shock_effect;

        public float up_down_speed = 1f;
        public float up_down_multiply = 1f;

        public float JITTER = 5f;

        public Transform sphereObject;
        private Transform _transform;

        private void Start() {
            _transform = transform;
        }

        private void FixedUpdate() {
            if (!sphereObject || Settings.Quality == 0) return;

            //random speed
            var RAND_SPEEDA = speedMult;
            if (Shock_effect) RAND_SPEEDA = Random.Range(speedMult - 1.1f, speedMult + JITTER);

            _transform.RotateAround(sphereObject.position, Vector3.up, RAND_SPEEDA * 20 * Time.deltaTime);

            if (up_down_motion) {
                //random speed
                var RAND_SPEED = up_down_speed;
                if (Shock_effect) RAND_SPEED = Random.Range(up_down_speed - 0.1f, up_down_speed + JITTER / 10);
                _transform.position = new Vector3(_transform.position.x,
                    sphereObject.transform.position.y + up_down_multiply * Mathf.Cos(Time.fixedTime + RAND_SPEED),
                    _transform.position.z);
            }
        }
    }
}