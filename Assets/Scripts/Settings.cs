/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using UnityEngine;

public static class Settings {
	
	#region Attributes
		// Gameplay
		public static float MouseRotateSensitivity { get; private set; }
		public static bool InvertCameraY { get; private set; }
		public static bool InvertWorldRotateX { get; private set; }
		public static float RotateSpeed { get; private set; }
		public static float BoostMultiplier { get; private set; }
		
		// Audio 
		public static float MasterVolume { get; private set; }
		public static float MusicVolume { get; private set; }
		public static float SfxVolume { get; private set; }
		public static float AmbienceVolume { get; private set; }
	#endregion

	#region Other Methods
		public static void ReadSaveData() {
			InvertCameraY = PlayerPrefs.GetInt("InvertCameraY", 0) == 1;
			InvertWorldRotateX = PlayerPrefs.GetInt("InvertWorldRotateX", 1) == 1;
			MouseRotateSensitivity = PlayerPrefs.GetFloat("MouseRotateSensitivity", 0.1f);
			RotateSpeed = PlayerPrefs.GetFloat("RotateSpeed", 1f);
			BoostMultiplier = PlayerPrefs.GetFloat("BoostMultiplier", 4f);
			
			MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
			MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
			SfxVolume = PlayerPrefs.GetFloat("SfxVolume", 1f);
			AmbienceVolume = PlayerPrefs.GetFloat("AmbienceVolume", 1f);
		}
		
	#endregion
}
