/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Knitwit Studios/Level", order = 1)]
public class Level : ScriptableObject {
	public string SceneName, LevelName;
	public float StartTime, EndTime;
	public AudioClip Song;
	public int NumberOfCorrectGuesses;
	public bool Tutorial, HasFrogs;
	public InfoEntry Entry;
	public Questions Questions;
	public bool Unlocked => Tutorial || PlayerPrefs.GetInt(LevelName + "Unlocked", 0) == 1;
	
	public void SaveLevelComplete() {
		PlayerPrefs.SetInt(LevelName + "Unlocked", 1);
		PlayerPrefs.Save();
	}
}
