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
    public int AccessoryIndex { get; }
    
    public string Name { get; }
    //public MushroomType Type { get; }

    private MushroomData(int hc, int bc, int a, string n) {
        HeadColorIndex = hc;
        BodyColorIndex = bc;
        AccessoryIndex = a;
        Name = n;
    }
    public static void Init() {
        AllData.Clear();
        
        var file = Resources.Load<TextAsset>("mushroom_names");
        var names = file.text.Split(",");
        
        if (!PlayerPrefs.HasKey("MushroomDataName0")) {
            var usedNames = new List<string>();
            
            for (var i = 0; i < names.Length; i++) {
                var hc = Random.Range(0,8);
                var bc = Random.Range(0,3);
                var a = Random.Range(0,4);
                var n = names[Random.Range(0, names.Length)];
                
                while (usedNames.Contains(n)) {
                    n = names[Random.Range(0, names.Length)];
                }
                usedNames.Add(n);

                AllData.Add(new MushroomData(hc, bc, a, n));
                
                PlayerPrefs.SetString("MushroomDataName"+i, n);
                PlayerPrefs.SetInt("MushroomDataHeadColorIndex"+i, hc);
                PlayerPrefs.SetInt("MushroomDataBodyColorIndex"+i, bc);
                PlayerPrefs.SetInt("MushroomDataAccessoryIndex"+i, a);
                //PlayerPrefs.SetInt("MushroomDataType"+i, (int)mt);
            }
            
            PlayerPrefs.Save();
        } else {
            for (var i = 0; i < names.Length; i++) {
                var hc =  PlayerPrefs.GetInt("MushroomDataHeadColorIndex"+i);
                var bc =  PlayerPrefs.GetInt("MushroomDataBodyColorIndex"+i);
                var a =  PlayerPrefs.GetInt("MushroomDataAccessoryIndex"+i);
                var n = PlayerPrefs.GetString("MushroomDataName"+i);
                //var mt = (MushroomType)PlayerPrefs.GetInt("MushroomDataType"+i);
                AllData.Add(new MushroomData(hc, bc, a, n));
            }
        }
    }
    public static void DeleteAllSaves() {
        var file = Resources.Load<TextAsset>("mushroom_names");
        var names = file.text.Split(",");
        
        if (PlayerPrefs.HasKey("MushroomDataName0")) {
            for (var i = 0; i < names.Length; i++) {
                PlayerPrefs.DeleteKey("MushroomDataHeadColorIndex"+i);
                PlayerPrefs.DeleteKey("MushroomDataBodyColorIndex"+i);
                PlayerPrefs.DeleteKey("MushroomDataAccessoryIndex"+i);
                PlayerPrefs.DeleteKey("MushroomDataName"+i);
                //PlayerPrefs.DeleteKey(("MushroomDataType"+i);
            }
        } 
    }
}
