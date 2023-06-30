/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Knitwit Studios/Info Entry", order = 1)]
public class InfoEntry : ScriptableObject {
    public Page[] Pages;
}

[Serializable]
public struct Page {
    public Sprite LeftImage;
    [Multiline(9)]
    public string RightText;
}