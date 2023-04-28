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
		public Sprite Headshot;
		public KeyValuePair<string, bool> Name;
	}

	#region Attributes
		//public List<Entry> Entries { get; }
	#endregion
	
	#region Components
		private RectTransform _leftSide, _rightSide;
		private RectTransform[] _leftPages, _rightPages;
		
		//Page 1
		private Image[] _headshotImageSpots;
		
		//Page 2 
		private Image _headshotSpotlight;
		private TMP_InputField _nameInputField, miscInputField;
	#endregion
	
	#region Private Data
		private List<Entry> _entries = new();
	#endregion
	
	#region Unity Methods
		private void Awake() {
			_leftSide = transform.GetChild(0).GetComponent<RectTransform>();
			_leftPages = new RectTransform[_leftSide.childCount];
			for (var i = 0; i < _leftSide.childCount; i++) {
				_leftPages[i] = _leftSide.GetChild(i).GetComponent<RectTransform>();
			}

			_rightSide = transform.GetChild(1).GetComponent<RectTransform>();
			_rightPages = new RectTransform[_rightSide.childCount];
			for (var i = 0; i < _rightSide.childCount; i++) {
				_rightPages[i] = _rightSide.GetChild(i).GetComponent<RectTransform>();
			}
		}

		private void Start() {
			//Page 1
			_headshotImageSpots = _leftPages[0].GetComponentsInChildren<Image>()
				.Concat(_rightPages[0].GetComponentsInChildren<Image>())
				.ToArray();

			//Page 2
			_headshotSpotlight = _leftPages[1].GetComponentsInChildren<Image>()[0];
			_nameInputField = _leftPages[1].GetComponentsInChildren<TMP_InputField>()[0];
			miscInputField = _rightPages[1].GetComponentsInChildren<TMP_InputField>()[0];
		}
	
	#endregion
	
	#region Other Methods
		public void GoToHomePage() {
			GoToPage(0);
			
			var index = 0;
			foreach (var image in _headshotImageSpots) {
				image.gameObject.SetActive(index < _entries.Count);

				if (index < _entries.Count) {
					image.sprite = _entries[index].Headshot;
					var n = _entries[index].Name.Key;
					image.GetComponent<Button>().onClick.AddListener(() => GoToMushroomPage(n));
					image.GetComponentInChildren<TextMeshProUGUI>().text = _entries[index].Name.Value ? n : "???";
				}
				
				index++;
			}
		}
		public void GoToMushroomPage(string name) {
			GoToPage(1);
			
			_headshotSpotlight.sprite = _entries.First(e => e.Name.Key.Equals(name)).Headshot;
			_nameInputField.text = name;
		}
		private void GoToPage(int p) {
			foreach (var lp in _leftPages) {
				lp.gameObject.SetActive(false);
			}
			foreach (var rp in _rightPages) {
				rp.gameObject.SetActive(false);
			}
			
			_leftPages[p].gameObject.SetActive(true);
			_rightPages[p].gameObject.SetActive(true);
		}
		public void Init() {
			_entries.Clear();
			
			foreach (var shroom in MushroomManager.AllActive) {
				var e = new Entry() {
					Headshot = shroom.HeadshotCamera.HeadshotSprite,
					Name = new KeyValuePair<string, bool>(shroom.Data.Name, true),
				};
				_entries.Add(e);
			}
		}
	#endregion
}
