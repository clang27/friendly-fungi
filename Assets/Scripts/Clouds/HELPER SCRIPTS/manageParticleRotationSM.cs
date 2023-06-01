using UnityEngine;

namespace Artngame.SKYMASTER {
    public class manageParticleRotationSM : MonoBehaviour {
        public bool StableRollMethod2;
        public bool StableRollAllAxis;

        private Transform Cam_transf;

        private ParticleSystem.Particle[] particles;
        private ParticleSystemRenderer PrenderS;
        private Vector3 Prev_cam_rot;

        private int psize;

        //v3.4.6
        private ParticleSystem ShurikenParticle;

        private void Start() {
            Cam_transf = Camera.main.transform;
            Prev_cam_rot = Cam_transf.eulerAngles;

            var MainMod = GetComponent<ParticleSystem>().main; //v3.4.9
            psize = MainMod.maxParticles; //v3.4.9

            var SizeP = MainMod.maxParticles;

            //int SizeP = ((int)(psize / divider)) * (divider);
            //MainMod.maxParticles = SizeP; //+ (divider)*1;
            //MainMod.maxParticles = SizeP; //+ (divider)*1;
            //}
            //		GetComponent<ParticleSystem> ().ClearParticles ();
            //GetComponent<ParticleSystem>().Emit(SizeP);
            var particles = new ParticleSystem.Particle[SizeP];
            GetComponent<ParticleSystem>()
                .GetParticles(particles); //		Particle[] particles = GetComponent<ParticleEmitter> ().particles;

            //v1.7 - init clouds on start
            //if (GetComponent<ParticleSystem>() != null)
            //{

            //v3.5
            if (GetComponent<ParticleSystem>() != null) {
                //ScatterMat = this.GetComponent<ParticleSystemRenderer>().material;
                PrenderS = gameObject.GetComponent<ParticleSystemRenderer>();
                ShurikenParticle = GetComponent<ParticleSystem>();
            }

            if (PrenderS != null)
                PrenderS.sortMode = ParticleSystemSortMode.Distance; // = ParticleRenderMode.SortedBillboard;
            //PrenderS.renderMode = ParticleSystemRenderMode.Billboard;
        }

        // Update is called once per frame
        private void LateUpdate() {
            //v3.1
            var currentRot = Cam_transf.eulerAngles.z;
            var diffRot = currentRot - Prev_cam_rot.z;

            var currentRotY = Cam_transf.eulerAngles.y;
            var diffRotY = currentRotY - Prev_cam_rot.y;

            if (Mathf.Abs(diffRotY) > 180) diffRotY = 360 - Mathf.Abs(diffRotY);
            float Angle_check = 0;
            float Dot_check = 0;
            //v3.1
            if (StableRollMethod2) {
                Angle_check =
                    Mathf.Abs(Vector3.Angle(Cam_transf.forward, Mathf.Sign(Cam_transf.forward.y) * Vector3.up));
                Dot_check = -Mathf.Sign(Cam_transf.forward.y) *
                            Vector3.Dot(Cam_transf.forward, Mathf.Sign(Cam_transf.forward.y) * Vector3.up);
                //Debug.Log("diffRotY="+diffRotY + "Dot_check="+Dot_check + " SUM="+(diffRotY*Dot_check).ToString() + " Angle="+Angle_check);
            }

            if (particles == null) particles = new ParticleSystem.Particle[ShurikenParticle.main.maxParticles]; //v3.4.9
            ShurikenParticle.GetParticles(particles);
            for (var i = 0; i < particles.Length; i = i + 1) {
                ////////////////
                //v3.0.2
                if (StableRollMethod2) {
                    if (!StableRollAllAxis) {
                        var currentRot1 = Cam_transf.eulerAngles.z;
                        var diffRot1 = currentRot1 - Prev_cam_rot.z;
                        var currentRotY1 = Cam_transf.eulerAngles.y;
                        if (diffRot1 == 0 && currentRotY1 != Prev_cam_rot.y &&
                            Mathf.Abs(Vector3.Angle(Cam_transf.forward, Vector3.up)) < 65) {
                            //Mathf.Abs(Vector3.Angle(Cam_transf.forward, Vector3.up))< 65){
                            var diffRotY1 = currentRotY1 - Prev_cam_rot.y;
                            particles[i].rotation = particles[i].rotation + diffRotY1;
                            //Debug.Log("aa");
                        }
                        else {
                            if (Mathf.Abs(Vector3.Angle(Cam_transf.forward, Vector3.up)) < 45) {
                                //particles[i].rotation = Mathf.Lerp(particles[i].rotation,particles[i].rotation+diffRot, Time.deltaTime*2);
                            }
                            else {
                                particles[i].rotation = particles[i].rotation + diffRot1;
                            }
                        }
                    }
                    else {
                        //v3.4.9 - disabled old code below
                        //											if(diffRot == 0 && currentRotY!=Prev_cam_rot.y && Angle_check < 65){//Mathf.Abs(Vector3.Angle(Cam_transf.forward, Vector3.up))< 65){									
                        //
                        //												//particles[i].rotation = particles[i].rotation-diffRotY*Dot_check;
                        //												particles[i].rotation = particles[i].rotation+diffRotY*Dot_check * 1; //v3.5
                        //												//Debug.Log("aa");
                        //											}else{
                        //												if(Angle_check < 45){
                        //													//particles[i].rotation = Mathf.Lerp(particles[i].rotation,particles[i].rotation+diffRot, Time.deltaTime*2);
                        //													//particles[i].rotation = particles[i].rotation+diffRot-diffRotY*Dot_check;
                        //												}else{
                        //													//particles[i].rotation = particles[i].rotation+diffRot-diffRotY*Dot_check;
                        //												}
                        //												particles[i].rotation = particles[i].rotation-diffRot+diffRotY*Dot_check  * 1; //v3.5 
                        //											}

                        //v3.4.9
                        //float currentRot1 = Cam_transf.eulerAngles.z;
                        //float diffRot1 = currentRot1 - Prev_cam_rot.z;

                        //float currentRotY1 = Cam_transf.eulerAngles.y;
                        if (diffRot == 0 && currentRotY != Prev_cam_rot.y && Angle_check < 65) {
                            //Mathf.Abs(Vector3.Angle(Cam_transf.forward, Vector3.up))< 65){

                            //float diffRotY1 = currentRotY1 - Prev_cam_rot.y;
                            particles[i].rotation = particles[i].rotation - diffRotY * Dot_check;
                            //Debug.Log("aa");
                        }
                        else {
                            if (Mathf.Abs(Vector3.Angle(Cam_transf.forward, Vector3.up)) < 45) {
                                //particles[i].rotation = Mathf.Lerp(particles[i].rotation,particles[i].rotation+diffRot, Time.deltaTime*2);
                            }

                            particles[i].rotation = particles[i].rotation + diffRot - diffRotY * Dot_check;
                        } //end v3.4.9
                    }
                }

                if (Prev_cam_rot == Cam_transf.eulerAngles) {
                    //particles[i].startColor = Color.Lerp(SunColor, MainColor, Diff);
                }
                else {
                    Prev_cam_rot = Cam_transf.eulerAngles;
                }
                ////////////////
            } //END PARTILE LOOP

            if (Prev_cam_rot != Cam_transf.eulerAngles) Prev_cam_rot = Cam_transf.eulerAngles;
            ShurikenParticle.SetParticles(particles, particles.Length); //v3.5
        }
    }
}