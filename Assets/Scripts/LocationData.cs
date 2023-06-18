/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum LocationType {
    Cliff, Building, Pond, Forest
}
public class LocationData {
    public static List<LocationData> AllData { get; } = new();
    public string Name { get; }

    private LocationData(string n) {
        Name = n;
    }
    
    public static void Init() {
        AllData.Clear();
        
        var file = Resources.Load<TextAsset>("location_names");
        var names = file.text.Split(",");
        
        if (!PlayerPrefs.HasKey("LocationDataName0")) {
            for (var i = 0; i < names.Length; i++) {
                var n = names[Random.Range(0, names.Length)];
                
                while (AllData.Any(l => l.Name == n))
                    n = names[Random.Range(0, names.Length)];

                AllData.Add(new LocationData(n));
                
                PlayerPrefs.SetString("LocationDataName"+i, n);
            }
            
            PlayerPrefs.Save();
        } else {
            for (var i = 0; i < names.Length; i++) {
                var n = PlayerPrefs.GetString("LocationDataName"+i);
                AllData.Add(new LocationData(n));
            }
        }
    }
    
    public static void DeleteAllSaves() {
        var file = Resources.Load<TextAsset>("location_names");
        var names = file.text.Split(",");
        
        if (PlayerPrefs.HasKey("LocationDataName0")) {
            for (var i = 0; i < names.Length; i++)
                PlayerPrefs.DeleteKey("LocationDataName"+i);
        } 
    }
    
    public static string RandomSuffix(LocationType lt) {
        var rng = Random.Range(0, 3);
        switch (lt) {
            case LocationType.Cliff:
                string[] c = { "Cliff", "Heights", "Peak" };
                return c[rng];
            case LocationType.Pond:
                string[] p = { "Pond", "Waters", "Pond" };
                return p[rng];
            case LocationType.Forest:
                string[] f = { "Woods", "Forest", "Woods" };
                return f[rng];
            case LocationType.Building:
            default:
                string[] b = { "Shack", "House", "Abode" };
                return b[rng];
        }
    }
}
