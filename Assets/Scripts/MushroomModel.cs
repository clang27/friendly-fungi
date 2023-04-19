/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Knitwit Studios/Mushroom", order = 1)]
public class MushroomModel : ScriptableObject {
	public MushroomType Type;
	public Mesh Mesh;
	public Texture2D[] HeadTextures, BodyTextures;
}
