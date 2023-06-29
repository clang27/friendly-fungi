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
    public int Index => AllData.IndexOf(this);
    public string Name { get; }
    private int LocationTypeRng { get; }
    
    private LocationData(string n) {
        Name = n;
        LocationTypeRng = Random.Range(0, 3);
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
    
    public string Suffix(LocationType lt) {
        switch (lt) {
            case LocationType.Cliff:
                string[] c = { "Cliff", "Heights", "Peak" };
                return c[LocationTypeRng];
            case LocationType.Pond:
                string[] p = { "Pond", "Waters", "Pond" };
                return p[LocationTypeRng];
            case LocationType.Forest:
                string[] f = { "Woods", "Forest", "Woods" };
                return f[LocationTypeRng];
            case LocationType.Building:
            default:
                string[] b = { "Shack", "House", "Abode" };
                return b[LocationTypeRng];
        }
    }
}
