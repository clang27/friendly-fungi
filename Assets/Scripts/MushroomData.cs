/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MushroomData {
    private const int MaxHeadTextures = 9;
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

    private MushroomData(int headColorIndex, int bodyColorIndex, int glassesColorIndex, int headbandColorIndex, int hatColorIndex, string n) {
        HeadColorIndex = headColorIndex;
        BodyColorIndex = bodyColorIndex;
        
        GlassesColorIndex = glassesColorIndex;
        HeadbandColorIndex = headbandColorIndex;
        HatColorIndex = hatColorIndex;
        
        Name = n;
    }
    public static void Init() {
        AllData.Clear();
        
        var file = Resources.Load<TextAsset>("mushroom_names");
        var names = file.text.Split(",");
        
        if (!PlayerPrefs.HasKey("MushroomDataName0")) {
            for (var i = 0; i <= MaxHeadTextures; i++) {
                var headColorIndex = Random.Range(0,MaxHeadTextures+1);
                while (AllData.Any(m => m.HeadColorIndex == headColorIndex))
                    headColorIndex = Random.Range(0,10);

                var bodyColorIndex = Random.Range(0,3);
                var glassesColorIndex = (Random.Range(0,3) == 0) ? Random.Range(1,7) : 0;
                var hatColorIndex = (Random.Range(0,3) == 0) ? Random.Range(1,7) : 0;
                var headbandColorIndex = (Random.Range(0,4) == 0 && hatColorIndex != 0) ? Random.Range(1,7) : 0;
                // Ensures headband is not same color as head texture
                if (headbandColorIndex > 0 && headbandColorIndex == Mathf.FloorToInt(headColorIndex / 2f))
                    headbandColorIndex++;

                var n = names[Random.Range(0, names.Length)];
                while (AllData.Any(m => m.Name == n))
                    n = names[Random.Range(0, names.Length)];

                AllData.Add(new MushroomData(headColorIndex, bodyColorIndex, glassesColorIndex, headbandColorIndex, hatColorIndex, n));

                PlayerPrefs.SetInt("MushroomDataHeadColorIndex"+i, headColorIndex);
                PlayerPrefs.SetInt("MushroomDataBodyColorIndex"+i, bodyColorIndex);
                
                PlayerPrefs.SetInt("MushroomDataGlassesColorIndex"+i, glassesColorIndex);
                PlayerPrefs.SetInt("MushroomDataHeadbandColorIndex"+i, headbandColorIndex);
                PlayerPrefs.SetInt("MushroomDataHatColorIndex"+i, hatColorIndex);
                
                PlayerPrefs.SetString("MushroomDataName"+i, n);
                //PlayerPrefs.SetInt("MushroomDataType"+i, (int)mt);
            }
            
            PlayerPrefs.Save();
        } else {
            for (var i = 0; i < names.Length; i++) {
                var headColorIndex =  PlayerPrefs.GetInt("MushroomDataHeadColorIndex"+i);
                var bodyColorIndex =  PlayerPrefs.GetInt("MushroomDataBodyColorIndex"+i);
                
                var glassesColorIndex =  PlayerPrefs.GetInt("MushroomDataGlassesColorIndex"+i);
                var headbandColorIndex =  PlayerPrefs.GetInt("MushroomDataHeadbandColorIndex"+i);
                var hatColorIndex =  PlayerPrefs.GetInt("MushroomDataHatColorIndex"+i);

                var n = PlayerPrefs.GetString("MushroomDataName"+i);

                //var mt = (MushroomType)PlayerPrefs.GetInt("MushroomDataType"+i);
                AllData.Add(new MushroomData(headColorIndex, bodyColorIndex, glassesColorIndex, headbandColorIndex, hatColorIndex, n));
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
