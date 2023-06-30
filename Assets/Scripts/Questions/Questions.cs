/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Knitwit Studios/Questions", order = 1)]
public class Questions : ScriptableObject {
	#region Serialized Fields
		[SerializeField] private Who[] whoQuestions;
		[SerializeField] private What[] whatQuestions;
		[SerializeField] private When[] whenQuestions;
		[SerializeField] private Where[] whereQuestions;
		[SerializeField] private Can[] canQuestions;
		[SerializeField] private HowMany[] howManyQuestions;
		[SerializeField] [TextArea(0,20)] private string notes; // Purely for documentation
	#endregion
	
	#region Attributes
		public List<Question> All { get; private set; }
	#endregion

	#region Other Methods
		public void Init() {
			All = whoQuestions
				.Concat((IEnumerable<Question>) whatQuestions)
				.Concat(canQuestions)
				.Concat(howManyQuestions)
				.Concat(whenQuestions)
				.Concat(whereQuestions).ToList();
		}
		
	#endregion
}
