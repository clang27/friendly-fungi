/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Journal : MonoBehaviour {
	// Since some data is a mystery, it is paired with a value that determines if the data is known
	private struct Entry {
		public KeyValuePair<string, bool> Name;
		public KeyValuePair<Texture2D, bool> Headshot;
	}
	
	#region Serialized Fields
		[SerializeField] private RectTransform imagePanel;
	#endregion
	
	#region Attributes
		//public List<Entry> Entries { get; }
	#endregion
	
	#region Components
		private List<Entry> _entries = new(); 
		private RawImage[] _headshotImageSpots;
	#endregion
	
	#region Private Data
		// private float _dataOne, _dataTwo;
	#endregion
	
	#region Unity Methods
		private void Awake() {
			_headshotImageSpots = imagePanel.GetComponentsInChildren<RawImage>();
		}
	
	#endregion
	
	#region Other Methods
		public void Init() {
			_entries.Clear();
			
			foreach (var shroom in MushroomManager.AllActive) {
				var e = new Entry() {
					Name = new KeyValuePair<string, bool>(shroom.Data.Name, true),
					Headshot = new KeyValuePair<Texture2D, bool>(shroom.HeadshotCamera.HeadshotTexture, true)
				};
				_entries.Add(e);
			}
		}

		public void PopulateHeadshots() {
			var index = 0;
			foreach (var image in _headshotImageSpots) {
				image.gameObject.SetActive(index < _entries.Count);

				if (index < _entries.Count) {
					if (_entries[index].Headshot.Value)
						image.texture = _entries[index].Headshot.Key;
					image.GetComponentInChildren<TextMeshProUGUI>().text = 
						_entries[index].Name.Value ?
							_entries[index].Name.Key :
							"???";
				}
					
				
				index++;
			}
		}
	#endregion
}
