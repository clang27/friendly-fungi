/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class QuestionQueue : MonoBehaviour {
	#region Serialized Fields
		[SerializeField] private int numberOfQuestions;

		[SerializeField] private Who[] whoQuestions;
		[SerializeField] private What[] whatQuestions;
		[SerializeField] private When[] whenQuestions;
		[SerializeField] private Where[] whereQuestions;
	#endregion
	
	#region Attributes
		public static List<Question> AllQuestions { get; private set; }
		public static int NumberOfQuestions { get; private set; }
	#endregion

	#region Other Methods
		private void Awake() {
			NumberOfQuestions = numberOfQuestions;
			
			AllQuestions = whoQuestions
				.Concat((IEnumerable<Question>) whatQuestions)
				.Concat(whenQuestions)
				.Concat(whereQuestions).ToList();
		}
		
	#endregion
}