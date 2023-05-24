/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System.Collections.Generic;
using UnityEngine;

public class MushroomData {
    public static List<MushroomData> AllData { get; } = new();

    public int HeadColorIndex { get; }
    public int BodyColorIndex { get; }
    public string Name { get; }
    //public MushroomType Type { get; }

    private MushroomData(int hc, int bc, string n) {
        HeadColorIndex = hc;
        BodyColorIndex = bc;
        Name = n;
    }
    public static void Init() {
        var file = Resources.Load<TextAsset>("mushroom_names");
        var names = file.text.Split(",");
        
        if (!PlayerPrefs.HasKey("MushroomDataName0")) {
            var usedNames = new List<string>();
            
            for (var i = 0; i < names.Length; i++) {
                var hc = Random.Range(0,6);
                var bc = Random.Range(0,6);
                var n = names[Random.Range(0, names.Length)];
                
                while (usedNames.Contains(n)) {
                    n = names[Random.Range(0, names.Length)];
                }
                usedNames.Add(n);

                AllData.Add(new MushroomData(hc, bc, n));
                
                PlayerPrefs.SetString("MushroomDataName"+i, n);
                PlayerPrefs.SetInt("MushroomDataHeadColorIndex"+i, hc);
                PlayerPrefs.SetInt("MushroomDataBodyColorIndex"+i, bc);
                //PlayerPrefs.SetInt("MushroomDataType"+i, (int)mt);
            }
            
            PlayerPrefs.Save();
        } else {
            for (var i = 0; i < names.Length; i++) {
                var hc =  PlayerPrefs.GetInt("MushroomDataHeadColorIndex"+i);
                var bc =  PlayerPrefs.GetInt("MushroomDataBodyColorIndex"+i);
                var n = PlayerPrefs.GetString("MushroomDataName"+i);
                //var mt = (MushroomType)PlayerPrefs.GetInt("MushroomDataType"+i);
                AllData.Add(new MushroomData(hc, bc, n));
            }
        }
    }
}
