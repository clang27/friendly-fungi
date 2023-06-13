/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Journal : MonoBehaviour {
	[SerializeField] private Material bookImageMaterial;
	
	// Since some data is a mystery, it is paired with a value that the player guessed
	private class ShrooEntry {
		public Sprite Headshot;
		public Material Mat;
		public KeyValuePair<string, string> Name;
		public string Notes;
		public override string ToString() {
			return Name.Key;
		}
	}

	#region Attributes
		private bool NoEntry => _selectedEntryIndex == -1;
	#endregion
	
	#region Components
		private RectTransform _leftSide, _rightSide;
		private RectTransform[] _leftPages, _rightPages;
		private JournalTab[] _tabs;
		
		//Page 1
		private Button[] _infoButtons;
		
		//Page 2
		private TextMeshProUGUI _titleText, _infoText;
		private Button _prevPageButton, _nextPageButton;
		
		//Page 3
		private Image[] _headshotImageSpots;
		
		//Page 4 
		private Image _headshotSpotlight;
		private TMP_InputField _notesInputField;
		private TMP_Dropdown _nameDropdown;
	#endregion
	
	#region Private Data
		private static List<ShrooEntry> _entries = new();
		private int _selectedEntryIndex = -1;
		private int _infoPageNumber, _infoPageLevel;
		
		private static readonly int Highlighted = Shader.PropertyToID("_Highlighted");
		private static readonly int MainTex = Shader.PropertyToID("_MainTex");
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

			_tabs = GetComponentsInChildren<JournalTab>();
		}

		private void Start() {
			//Page 1
			_infoButtons = _leftPages[0].GetComponentsInChildren<Button>()
				.Concat(_rightPages[0].GetComponentsInChildren<Button>())
				.ToArray();
			
			//Page 2
			_titleText = _leftPages[1].GetComponentInChildren<TextMeshProUGUI>();
			_infoText =  _rightPages[1].GetComponentInChildren<TextMeshProUGUI>();
			_prevPageButton =  _rightPages[1].GetComponentsInChildren<Button>()[0];
			_nextPageButton =  _rightPages[1].GetComponentsInChildren<Button>()[1];

			//Page 3
			_headshotImageSpots = _leftPages[2].GetComponentsInChildren<Image>()
				.Concat(_rightPages[2].GetComponentsInChildren<Image>())
				.ToArray();

			//Page 4
			_headshotSpotlight = _leftPages[3].GetComponentsInChildren<Image>()[0];
			_nameDropdown = _leftPages[3].GetComponentsInChildren<TMP_Dropdown>()[0];
			_notesInputField = _rightPages[3].GetComponentsInChildren<TMP_InputField>()[0];
		}
	
	#endregion
	
	#region Other Methods
		public void GoToHomePage() {
			GoToPage(0);
			
			var index = 0;
			foreach (var button in _infoButtons) {
				var buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
				var active = index < LevelSelection.AllLevels.Length && LevelSelection.AllLevels[index].Unlocked;

				buttonText.DOFade(active ? 1f : 0.5f, 0f);
				buttonText.text = active ? LevelSelection.AllLevels[index].LevelName : "???";
				button.interactable = active;

				index++;
			}
		}

		public void GoToInfoPage(int levelNumber) {
			GoToPage(1);
			_infoPageLevel = levelNumber;
			GoToSubInfo(0);
		}

		private void GoToSubInfo(int i) {
			_infoPageNumber = i;
			
			var l = LevelSelection.AllLevels[_infoPageLevel];
			_titleText.text = l.Entry.Title;
			_infoText.text = l.Entry.Pages[_infoPageNumber].Replace("\\n", "\n");
			
			_prevPageButton.interactable = _infoPageNumber > 0;
			_nextPageButton.interactable = _infoPageNumber < l.Entry.Pages.Length - 1;
		}

		public void GoToTableOfContents() {
			GoToPage(2);
			
			var index = 0;
			foreach (var image in _headshotImageSpots) {
				image.gameObject.SetActive(index < _entries.Count);

				if (index < _entries.Count) {
					Debug.Log($"Setting TOC #{index}");
					//image.sprite = _entries[index].Headshot;
					image.material = _entries[index].Mat;

					var rt = image.GetComponent<RectTransform>();
					rt.DOKill();
					rt.localPosition = new Vector3(rt.localPosition.x, rt.localPosition.y, -0.1f);
					rt.DOLocalMoveZ(-1f, 0.1f);

					var n = _entries[index].Name.Key;
					image.GetComponent<Button>().onClick.RemoveAllListeners();
					image.GetComponent<Button>().onClick.AddListener(() => GoToMushroomPage(n));
					image.GetComponentInChildren<TextMeshProUGUI>().text = _entries[index].Name.Value;
				}
				
				index++;
			}
		}
		public void GoToMushroomPage(string mushroomName) {
			// Debug.Log("Seeing if " + name + " matches with the following:");
			// foreach (var e in _entries) {
			// 	Debug.Log(e);
			// }
			
			GoToPage(3);

			_selectedEntryIndex = _entries.IndexOf(_entries.First(e => e.Name.Key.Equals(mushroomName)));
			//_headshotSpotlight.sprite = _entries[_selectedEntryIndex].Headshot;
			_headshotSpotlight.material = _entries[_selectedEntryIndex].Mat;
			
			var rt = _headshotSpotlight.GetComponent<RectTransform>();
			rt.DOKill();
			rt.localPosition = new Vector3(rt.localPosition.x, rt.localPosition.y, -0.1f);
			rt.DOLocalMoveZ(-1f, 0.1f);
			
			for (var i = 0; i < _nameDropdown.options.Count; i++) {
				if (_nameDropdown.options[i].text.Equals(_entries[_selectedEntryIndex].Name.Value))
					_nameDropdown.SetValueWithoutNotify(i);
			}
			
			_notesInputField.SetTextWithoutNotify(_entries[_selectedEntryIndex].Notes);
		}
		public void AddNote(string s) {
			if (NoEntry) return;

			_entries[_selectedEntryIndex].Notes = s;
		}

		public void SetName(int i) {
			if (NoEntry) return;

			var s = _nameDropdown.options[i].text;
			_entries[_selectedEntryIndex].Name = new KeyValuePair<string, string>(_entries[_selectedEntryIndex].Name.Key, s);
		}
		
		private void GoToPage(int p) {
			if (TutorialManager.JournalTabsCanOperate) {
				_tabs[1].Enable(p != 0);
				_tabs[0].Enable(p != 2);
			}
			
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
			foreach (var shroom in Mushroom.All) {
				var mat = new Material(bookImageMaterial) {
					mainTexture = shroom.HeadshotCamera.HeadshotTexture,
					name = $"{shroom.Data.Name}sJournalMaterial"
				};

				Debug.Log($"Adding {shroom.Data.Name}sJournalMaterial");
				mat.SetTexture(MainTex, shroom.HeadshotCamera.HeadshotTexture);

				// Reapply new headshot on each shroom, but don't restart text data if already exists
				if (_entries.Any(e => shroom.Data.Name.Equals(e.Name.Key))) {
					var s = _entries.First(e => shroom.Data.Name.Equals(e.Name.Key));
					s.Headshot = shroom.HeadshotCamera.HeadshotSprite;
					s.Mat = mat;
					continue;
				}

				var e = new ShrooEntry() {
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

		public static void ClearData() {
			Debug.Log("Clearing entries from journal.");
			_entries.Clear();
		}

		public void HighlightImage(Image im) {
			im.material.SetInt(Highlighted, 1);
		}
		
		public void UnhighlightImage(Image im) {
			im.material.SetInt(Highlighted, 0);
		}

		public void SelectNextPage() {
			GoToSubInfo(_infoPageNumber+1);
		}
		
		public void SelectPrevPage() {
			GoToSubInfo(_infoPageNumber-1);
		}
	#endregion
}
