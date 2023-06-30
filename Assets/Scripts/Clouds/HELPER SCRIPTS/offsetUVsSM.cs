using UnityEngine;

//using UnityEngine.ma

public class offsetUVsSM : MonoBehaviour {
    public Material material;
    public float speed = 0.06f;
    private float offset;

    // Update is called once per frame
    private void Update() {
        if (material != null) {
            offset = 0.08f + Mathf.Abs(speed * Mathf.Cos(Time.fixedTime * 0.5f)); // offset + 0.0001f;
            material.SetFloat("_NormalScale", offset);
            //material.SetTextureOffset("_MainTex", new Vector2(offset, 0));
            //material.SetTextureOffset("_BumpMap", new Vector2(offset, 0));
        }
    }
}