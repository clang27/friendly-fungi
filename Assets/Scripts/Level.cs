/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Knitwit Studios/Level", order = 1)]
public class Level : ScriptableObject {
	public float StartTime, EndTime;
	public AudioClip Song;
}
