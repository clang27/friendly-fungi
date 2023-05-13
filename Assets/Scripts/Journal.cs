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
	[SerializeField] private Material bookImageMaterial;
	
	// Since some data is a mystery, it is paired with a value that the player guessed
	private class Entry {
		public Sprite Headshot;
		public Material Mat;
		public KeyValuePair<string, string> Name;
		public string Notes;
	}

	#region Attributes
		private bool NoEntry => _selectedEntryIndex == -1;
	#endregion
	
	#region Components
		private RectTransform _leftSide, _rightSide;
		private RectTransform[] _leftPages, _rightPages;
		
		//Page 1
		private Image[] _headshotImageSpots;
		
		//Page 2 
		private Image _headshotSpotlight;
		private TMP_InputField _miscInputField;
		private TMP_Dropdown _nameDropdown;
	#endregion
	
	#region Private Data
		private List<Entry> _entries = new();
		private int _selectedEntryIndex = -1;
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
			_headshotSpotlight = _leftPages[1].GetComponentsInChildren<Image>()[1];
			_nameDropdown = _leftPages[1].GetComponentsInChildren<TMP_Dropdown>()[0];
			_miscInputField = _rightPages[1].GetComponentsInChildren<TMP_InputField>()[0];
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
					image.material = _entries[index].Mat;
					var n = _entries[index].Name.Key;
					image.GetComponent<Button>().onClick.AddListener(() => GoToMushroomPage(n));
					image.GetComponentInChildren<TextMeshProUGUI>().text = _entries[index].Name.Value;
				}
				
				index++;
			}
		}
		public void GoToMushroomPage(string name) {
			GoToPage(1);

			_selectedEntryIndex = _entries.IndexOf(_entries.First(e => e.Name.Key.Equals(name)));
			_headshotSpotlight.sprite = _entries[_selectedEntryIndex].Headshot;
			
			for (var i = 0; i < _nameDropdown.options.Count; i++) {
				if (_nameDropdown.options[i].text.Equals(_entries[_selectedEntryIndex].Name.Value))
					_nameDropdown.SetValueWithoutNotify(i);
			}
			
			_miscInputField.SetTextWithoutNotify(_entries[_selectedEntryIndex].Notes);
		}
		public void AddNotes(string s) {
			if (NoEntry) return;

			_entries[_selectedEntryIndex].Notes = s;
		}

		public void SetName(int i) {
			if (NoEntry) return;

			var s = _nameDropdown.options[i].text;
			_entries[_selectedEntryIndex].Name = new KeyValuePair<string, string>(_entries[_selectedEntryIndex].Name.Key, s);
		}
		
		private void GoToPage(int p) {
			_selectedEntryIndex = -1;
			
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
				var mat = new Material(bookImageMaterial);
				mat.SetTexture("_sprite", shroom.HeadshotCamera.HeadshotTexture);
				
				var e = new Entry() {
					Headshot = shroom.HeadshotCamera.HeadshotSprite,
					Mat = mat,
					Name = new KeyValuePair<string, string>(shroom.Data.Name, "???"),
					Notes = ""
				};
				_entries.Add(e);
			}
			
			_nameDropdown.ClearOptions();
			_nameDropdown.AddOptions(_entries.Select(e => e.Name.Key).Prepend("???").ToList());
		}
	#endregion
}
