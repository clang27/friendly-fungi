/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System;
using UnityEngine;

[Serializable]
public enum SettingsType {
	MasterVolume, MusicVolume, SfxVolume, AmbienceVolume, MouseRotateSensitivity, BoostMultiplier, InvertWorldRotation, InvertLookY, RotateSpeed, Quality
}
public static class Settings {
	
	#region Attributes
		// Gameplay
		public static float MouseRotateSensitivity { get; private set; }
		public static bool InvertLookY { get; private set; }
		public static bool InvertWorldRotation { get; private set; }
		public static float RotateSpeed { get; private set; }
		public static float BoostMultiplier { get; private set; }
		
		// Audio 
		public static float MasterVolume { get; private set; }
		public static float MusicVolume { get; private set; }
		public static float SfxVolume { get; private set; }
		public static float AmbienceVolume { get; private set; }
		
		// Graphics
		public static int Quality { get; private set; }
	#endregion

	#region Other Methods
		public static void ReadData() {
			InvertLookY = PlayerPrefs.GetInt("InvertLookY", 0) == 1;
			InvertWorldRotation = PlayerPrefs.GetInt("InvertWorldRotation", 0) == 1;
			MouseRotateSensitivity = PlayerPrefs.GetFloat("MouseRotateSensitivity", 0.1f);
			RotateSpeed = PlayerPrefs.GetFloat("RotateSpeed", 1f);
			BoostMultiplier = PlayerPrefs.GetFloat("BoostMultiplier", 3f);
			
			MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
			MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
			SfxVolume = PlayerPrefs.GetFloat("SfxVolume", 1f);
			AmbienceVolume = PlayerPrefs.GetFloat("AmbienceVolume", 1f);
			
			PlayerPrefs.DeleteKey("Quality");
			Quality = PlayerPrefs.GetInt("Quality", 1);
		}

		public static void SaveAllData() {
			PlayerPrefs.Save();
		}

		public static void SetData(SettingsType st, float f) {
			switch (st) {
				case SettingsType.MasterVolume:
					PlayerPrefs.SetFloat("MasterVolume", f); MasterVolume = f; break;
				case SettingsType.MusicVolume:
					PlayerPrefs.SetFloat("MusicVolume", f); MusicVolume = f; break;
				case SettingsType.SfxVolume:
					PlayerPrefs.SetFloat("SfxVolume", f); SfxVolume = f; break;
				case SettingsType.AmbienceVolume:
					PlayerPrefs.SetFloat("AmbienceVolume", f); AmbienceVolume = f; break;
				case SettingsType.MouseRotateSensitivity:
					PlayerPrefs.SetFloat("MouseRotateSensitivity", f); MouseRotateSensitivity = f; break;
				case SettingsType.RotateSpeed:
					PlayerPrefs.SetFloat("RotateSpeed", f); RotateSpeed = f; break;
				case SettingsType.BoostMultiplier:
					PlayerPrefs.SetFloat("BoostMultiplier", f); BoostMultiplier = f; break;
				default:
					throw new ArgumentOutOfRangeException(nameof(st), st, null);
			}
		}
		
		public static void SetData(SettingsType st, int i) {
			switch (st) {
				case SettingsType.InvertWorldRotation:
					PlayerPrefs.SetInt("InvertWorldRotation", i); InvertWorldRotation = i==1; break;
				case SettingsType.InvertLookY:
					PlayerPrefs.SetInt("InvertLookY", i); InvertLookY = i==1; break;
				case SettingsType.Quality:
					PlayerPrefs.SetInt("Quality", i); Quality = i; break;
				default:
					throw new ArgumentOutOfRangeException(nameof(st), st, null);
			}
		}

	#endregion
}
