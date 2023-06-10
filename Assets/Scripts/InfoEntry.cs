/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Knitwit Studios/Info Entry", order = 1)]
public class InfoEntry : ScriptableObject {
    public string Title;
    public string[] Pages;
}
