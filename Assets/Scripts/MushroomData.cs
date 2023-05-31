/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MushroomData {
    public static List<MushroomData> AllData { get; } = new();

    public string Name { get; }
    public int HeadColorIndex { get; }
    public int BodyColorIndex { get; }
    public int GlassesColorIndex { get; }
    public int HeadbandColorIndex { get; }
    public int HatColorIndex { get; }

    //public MushroomType Type { get; }

    public override string ToString() {
        return $"Name: {Name}\n" +
               $"HeadColorIndex: {HeadColorIndex}\n" +
               $"BodyColorIndex: {BodyColorIndex}\n" +
               $"GlassesColorIndex: {GlassesColorIndex}\n" +
               $"HeadbandColorIndex: {HeadbandColorIndex}\n" +
               $"HatColorIndex: {HatColorIndex}\n";
    }

    private MushroomData(int hc, int bc, int gc, int hbci, int hci, string n) {
        HeadColorIndex = hc;
        BodyColorIndex = bc;
        
        GlassesColorIndex = gc;
        HeadbandColorIndex = hbci;
        HatColorIndex = hci;
        
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
                
                var gc = (Random.Range(0,3) == 0) ? Random.Range(1,7) : 0;
                var hci = (Random.Range(0,3) == 0) ? Random.Range(1,7) : 0;
                var hbci = (Random.Range(0,4) == 0 && hci != 0) ? Random.Range(1,7) : 0;
                
                var n = names[Random.Range(0, names.Length)];
                
                while (usedNames.Contains(n)) {
                    n = names[Random.Range(0, names.Length)];
                }
                usedNames.Add(n);

                AllData.Add(new MushroomData(hc, bc, gc, hbci, hci, n));

                PlayerPrefs.SetInt("MushroomDataHeadColorIndex"+i, hc);
                PlayerPrefs.SetInt("MushroomDataBodyColorIndex"+i, bc);
                
                PlayerPrefs.SetInt("MushroomDataGlassesColorIndex"+i, gc);
                PlayerPrefs.SetInt("MushroomDataHeadbandColorIndex"+i, hbci);
                PlayerPrefs.SetInt("MushroomDataHatColorIndex"+i, hci);
                
                PlayerPrefs.SetString("MushroomDataName"+i, n);
                //PlayerPrefs.SetInt("MushroomDataType"+i, (int)mt);
            }
            
            PlayerPrefs.Save();
        } else {
            for (var i = 0; i < names.Length; i++) {
                var hc =  PlayerPrefs.GetInt("MushroomDataHeadColorIndex"+i);
                var bc =  PlayerPrefs.GetInt("MushroomDataBodyColorIndex"+i);
                
                var gc =  PlayerPrefs.GetInt("MushroomDataGlassesColorIndex"+i);
                var hci =  PlayerPrefs.GetInt("MushroomDataHeadbandColorIndex"+i);
                var hbci =  PlayerPrefs.GetInt("MushroomDataHatColorIndex"+i);

                var n = PlayerPrefs.GetString("MushroomDataName"+i);

                //var mt = (MushroomType)PlayerPrefs.GetInt("MushroomDataType"+i);
                AllData.Add(new MushroomData(hc, bc, gc, hbci, hci, n));
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
                PlayerPrefs.DeleteKey("MushroomDataGlassesColorIndex"+i);
                PlayerPrefs.DeleteKey("MushroomDataHeadbandColorIndex"+i);
                PlayerPrefs.DeleteKey("MushroomDataHatColorIndex"+i);
                PlayerPrefs.DeleteKey("MushroomDataName"+i);
                //PlayerPrefs.DeleteKey(("MushroomDataType"+i);
            }
        } 
    }
}
