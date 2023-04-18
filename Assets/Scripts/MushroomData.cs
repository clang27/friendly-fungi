/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public enum MushroomType {
    Amanita, Porcini, Shiitake, Morel
}

public class MushroomData {
    private static string[] _names;
    private static List<string> _takenNames = new();
    public static List<string> UsedNames => _takenNames;
    public Color HeadColor { get; }
    public Color BodyColor { get; }
    public string Name { get; }
    public MushroomType Type { get; }

    private MushroomData(Color hc, Color bc, string n, MushroomType mt) {
        HeadColor = hc;
        BodyColor = bc;
        Name = n;
        Type = mt;
    }
    public static void Init() {
        var file = Resources.Load<TextAsset>("names");
        _names = file.text.Split(",");
    }
    
    public static MushroomData RandomMushroom() {
        var hc = Color.HSVToRGB(Random.Range(0f, 1f), 1f, 0.8f);
        var bc = Color.HSVToRGB(1f - hc.r, 0.2f, 1f);
        var n = _names[Random.Range(0, _names.Length)];
        while (_takenNames.Contains(n)) {
            n = _names[Random.Range(0, _names.Length)];
        }
        _takenNames.Add(n);
        var mt = (MushroomType)Random.Range(0, 4);

        return new MushroomData(hc, bc, n, mt);
    }
}
