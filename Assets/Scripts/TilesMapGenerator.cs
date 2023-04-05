using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GeneratorParameter {
    None, VeryLow, Low, Medium, High, VeryHigh, Random
    
}
public enum EnableDisable {
    Enabled, Disabled
}
public class TilesMapGenerator : MonoBehaviour {
    [Header("[Tiles prefabs]")]
    public List<GameObject> tiles;

    [Header("[Map Parameters]")] 
    public GameObject map;
    [Range(10,100)] public int mapSize;
    public GeneratorParameter holesCount, holesSizes, heightsCount, heightsSizes, maxHeight;
    public EnableDisable heightSmoothing;

    [Header("[Map Filling]")]
    public GeneratorParameter additionalFilling;
    public List<GameObject> treesPrefabs, littleStonesPrefabs, bigStonesPrefabs, bushsPrefabs, grassPrefabs, branchsPrefabs, logsPrefabs;

    [Header("[Map points of interest (POI)]")]
    public EnableDisable contentOnMap;
    [Range(2,10)] public int poiCount;
    public GameObject startTile, endTile;
    public List<GameObject> interestPointTiles;

    [Header("[Map roads between POI's]")]
    public EnableDisable roads;
    [Range(0, 100)] public int roadsBetweenPOI;
    [Range(0, 100)] public int roadsFilling;
    [Range(0, 100)] public int roadsFenceChance;
    public GameObject roadStraight, roadRotate, roadCrossroad, roadTriple, roadEnd;
    public List<GameObject> roadBridges;

    [Header("[Ladders on map]")]
    public EnableDisable ladders;
    [Range(0, 100)] public int laddersChance;
    public List<GameObject> laddersTiles;
    
    private GameObject lastMap;
    private bool[,] lastRoadsMap, lastLaddersMap;
    private ScalerSystem _scalerSystem;

    private void Awake() {
        _scalerSystem = GetComponent<ScalerSystem>();
    }

    private void Start() {
        StartGenerator();
    }
    public void StartGenerator() {
        StartCoroutine(NewMap());
    }
    private IEnumerator NewMap() {
        if(_scalerSystem) {
            _scalerSystem.ReverseScaling();
            while(!_scalerSystem.Reversed) {
                yield return new WaitForSeconds(0.1f);
            }
        }

        if (lastMap)
            Destroy(lastMap);

        yield return new WaitForEndOfFrame();

        GenerateMap(map, mapSize, holesCount, holesSizes, heightsCount, heightsSizes, maxHeight,
                    heightSmoothing, contentOnMap, poiCount, roads, roadsFilling, roadsFenceChance, roadsBetweenPOI, ladders, laddersChance);

        if (_scalerSystem) {
            var meshs = lastMap.GetComponentsInChildren<MeshRenderer>();
            foreach (var mr in meshs) 
                mr.enabled = false;
        }

        yield return new WaitForEndOfFrame();

        AdditionalFilling(additionalFilling, mapSize);

        if(_scalerSystem) {
            _scalerSystem.StartScaling(lastMap.transform);
            
            foreach (var mr in lastMap.GetComponentsInChildren<MeshRenderer>()) 
                mr.enabled = true;
        }

        for(var i = 0; i < lastMap.transform.childCount; i++) {
            var thisPos = lastMap.transform.GetChild(i).localPosition;
            thisPos.x -= ((mapSize * 2f) / 2f)-1f;
            thisPos.z -= ((mapSize * 2f) / 2f)-1f;
            lastMap.transform.GetChild(i).localPosition = thisPos;
        }
    }
    public void GenerateMap(GameObject _map, int _mapSize, GeneratorParameter _holesCount,
                            GeneratorParameter _holesSizes, GeneratorParameter _heightsCount, GeneratorParameter _heightsSizes, GeneratorParameter _maxHeight, EnableDisable _heightSmoothing,
                            EnableDisable _POIs, int _POIsCount, EnableDisable _roads, int _roadsFilling, int _roadsFenceChance, int _roadsBetweenPOI, EnableDisable _ladders,
                            int _laddersChance) {
        var _mapPos = _map.transform.position;
        var _xSize = _mapSize;
        var _zSize = _mapSize;

        var roadsMap = new bool[_xSize, _zSize];
        var heightMap = new int[_xSize, _zSize];
        var holesMap = new bool[_xSize, _zSize];
        var laddersMap = new bool[_xSize, _zSize];

        //ROADS AND POI CREATING ----------------------------------------------
        var pointsOfInterest = new List<int[]>();

        var leftDownCorner = new Vector2(0, 0);
        var rightTopCorner = new Vector2(_xSize, _zSize);
        var maxMapDistance = Vector2.Distance(leftDownCorner, rightTopCorner);
        if (_POIs == EnableDisable.Enabled) {
            for (var i = 0; i < _POIsCount; i++) {
                var trys = 0;
                while (true) {
                    trys++;
                    var newPoi = new int[] { Random.Range(0, _xSize), Random.Range(0, _zSize) };

                    if (pointsOfInterest.Contains(newPoi)) continue;
                    
                    var minDistance = -1f;

                    foreach (var t in pointsOfInterest) {
                        var firstPoint = new Vector2(t[0], t[1]);
                        var secondPoint = new Vector2(newPoi[0], newPoi[1]);
                        var distance = Vector2.Distance(firstPoint, secondPoint);
                        if (distance < minDistance || minDistance < 0f) 
                            minDistance = distance;
                    }

                    if (minDistance > (maxMapDistance / 4f) || minDistance < 0f || trys > _xSize*_zSize) {
                        pointsOfInterest.Add(newPoi);
                        roadsMap[newPoi[0], newPoi[1]] = true;
                        trys = 0;
                        break;
                    }
                }
            }

            if (_roads == EnableDisable.Enabled) {
                for (var i = 0; i < pointsOfInterest.Count; i++) {
                    for (var a = i; a < pointsOfInterest.Count; a++) {
                        if (i == a) continue;
                        
                        var createThisConnection = true;

                        if (!(i == 0 && a == 1))
                            createThisConnection = Random.Range(0, 100) < _roadsBetweenPOI;

                        if (createThisConnection) {
                            var minX = pointsOfInterest[i][0] < pointsOfInterest[a][0] ? pointsOfInterest[i][0] : pointsOfInterest[a][0];
                            var maxX = pointsOfInterest[i][0] < pointsOfInterest[a][0] ? pointsOfInterest[a][0] : pointsOfInterest[i][0];
                            var minZ = pointsOfInterest[i][1] < pointsOfInterest[a][1] ? pointsOfInterest[i][1] : pointsOfInterest[a][1];
                            var maxZ = pointsOfInterest[i][1] < pointsOfInterest[a][1] ? pointsOfInterest[a][1] : pointsOfInterest[i][1];
                            var down = Random.Range(0, 100) < 50;
                            var left = true;
                            
                            if (pointsOfInterest[i][1] > pointsOfInterest[a][1]) {
                                if (pointsOfInterest[i][0] < pointsOfInterest[a][0])
                                    left = down;
                                else
                                    left = !down;
                            } else {
                                if (pointsOfInterest[a][0] < pointsOfInterest[i][0])
                                    left = down;
                                else
                                    left = !down;
                            }

                            for (var p = minX; p <= maxX; p++){
                                if (down)
                                    roadsMap[p, minZ] = true;
                                else
                                    roadsMap[p, maxZ] = true;
                            }
                            for (var p = minZ; p < maxZ; p++) {
                                if (left)
                                    roadsMap[minX, p] = true;
                                else
                                    roadsMap[maxX, p] = true;
                            }
                        }
                    }
                }
            }
        }
        //-------------------------------------------------------------

        //HOLES CREATING -----------------------------------------------
        if (_holesCount != GeneratorParameter.None) {
            if (_holesCount == GeneratorParameter.Random)
                _holesCount = (GeneratorParameter)Random.Range(1, 6);

            var holesMultiplier = _holesCount switch {
                GeneratorParameter.VeryLow => 0.1f,
                GeneratorParameter.Low => 0.2f,
                GeneratorParameter.Medium => 0.3f,
                GeneratorParameter.High => 0.4f,
                GeneratorParameter.VeryHigh => 0.5f,
                _ => 0f
            };
            
            if (_holesSizes is GeneratorParameter.Random or GeneratorParameter.None) 
                _holesSizes = (GeneratorParameter)Random.Range(1, 6);
            
            var holesSizesMultiplier = _holesSizes switch {
                GeneratorParameter.VeryLow => 0.1f,
                GeneratorParameter.Low => 0.25f,
                GeneratorParameter.Medium => 0.5f,
                GeneratorParameter.High => 0.85f,
                GeneratorParameter.VeryHigh => 1f,
                _ => 0f
            };
            

            var minSide = _zSize < _xSize ? _zSize : _xSize;
            var holesCountToCreate = Mathf.FloorToInt((float)minSide * holesMultiplier);
            var maxHoleSize = Mathf.FloorToInt(((float)minSide * 0.3f) * holesSizesMultiplier);

            for (var i = 0; i < holesCountToCreate; i++) {
                var hX = Random.Range(0, _xSize);
                var hZ = Random.Range(0, _zSize);
                holesMap = CreateHoles(holesMap, hX, hZ, maxHoleSize, 0);
            }
        }
        //-------------------------------------------------------------

        //HEIGHTS CREATING --------------------------------------------
        if (_heightsCount != GeneratorParameter.None) {
            if (_heightsCount == GeneratorParameter.Random) 
                _heightsCount = (GeneratorParameter)Random.Range(1, 6);

            var heightsMultiplier = _heightsCount switch {
                GeneratorParameter.VeryLow => 0.1f,
                GeneratorParameter.Low => 0.2f,
                GeneratorParameter.Medium => 0.3f,
                GeneratorParameter.High => 0.4f,
                GeneratorParameter.VeryHigh => 0.5f,
                _ => 0f
            };

            if (_heightsSizes is GeneratorParameter.Random or GeneratorParameter.None) 
                _heightsSizes = (GeneratorParameter)Random.Range(1, 6);

            var heightsSizesMultiplier = _heightsSizes switch {
                GeneratorParameter.VeryLow => 0.1f,
                GeneratorParameter.Low => 0.25f,
                GeneratorParameter.Medium => 0.5f,
                GeneratorParameter.High => 0.85f,
                GeneratorParameter.VeryHigh => 1f,
                _ => 0f
            };

            var minSide = _zSize < _xSize ? _zSize : _xSize;
            var heightsCountToCreate = Mathf.FloorToInt(minSide * heightsMultiplier);
            var maxHeightSize = Mathf.FloorToInt((minSide * 0.4f) * heightsSizesMultiplier);

            if (_maxHeight is GeneratorParameter.Random or GeneratorParameter.None) 
                _maxHeight = (GeneratorParameter)Random.Range(1, 6);

            var maxHeightInTiles = _maxHeight switch {
                GeneratorParameter.VeryLow => 1,
                GeneratorParameter.Low => 2,
                GeneratorParameter.Medium => 3,
                GeneratorParameter.High => 4,
                GeneratorParameter.VeryHigh => 5,
                _ => 0
            };

            for (var i = 0; i < heightsCountToCreate; i++) {
                var hX = Random.Range(0, _xSize);
                var hZ = Random.Range(0, _zSize);
                heightMap = RaiseHeight(heightMap, hX, hZ, maxHeightSize, maxHeightInTiles, holesMap, 0, _heightSmoothing);
            }
        }

        //-------------------------------------------------------------

        //HEIGHT SMOOTHING----------------------------------------------
        if (_heightSmoothing == EnableDisable.Enabled) {
            for (var i = 0; i < _xSize; i++) {
                for (var a = 0; a < _zSize; a++) {
                    SmoothHeights(heightMap, i, a);
                }
            }
        }
        //-------------------------------------------------------------

        //ROADS--------------------------------------------------------
        var roadsSumHeights = 0f;
        for(var i = 0; i<_xSize; i++) {
            for(var a = 0; a<_zSize; a++) {
                roadsSumHeights += heightMap[i, a];
            }
        }

        var roadsHeight = Mathf.CeilToInt(roadsSumHeights / (_xSize * _zSize));

        if (_POIs == EnableDisable.Enabled && roads == EnableDisable.Enabled) {
            foreach (var t in pointsOfInterest) {
                var xPos = t[1];
                var zPos = t[0];

                holesMap[xPos, zPos] = false;

                if (xPos + 1 < _xSize) {
                    heightMap[xPos + 1, zPos] = roadsHeight;
                    if (zPos + 1 < _zSize)
                        heightMap[xPos + 1, zPos + 1] = roadsHeight;
                    if (zPos - 1 > 0)
                        heightMap[xPos + 1, zPos - 1] = roadsHeight;
                }
                if (zPos + 1 < _zSize)
                    heightMap[xPos, zPos + 1] = roadsHeight;
                if (xPos - 1 > 0) {
                    heightMap[xPos - 1, zPos] = roadsHeight;
                    if (zPos + 1 < _zSize)
                        heightMap[xPos - 1, zPos + 1] = roadsHeight;
                    if (zPos - 1 > 0)
                        heightMap[xPos - 1, zPos - 1] = roadsHeight;
                }
                if (zPos - 1 > 0) {
                    heightMap[xPos, zPos - 1] = roadsHeight;
                }
            }

            for (var i = 0; i < _xSize; i++) {
                for (var a = 0; a < _zSize; a++) {
                    if (!roadsMap[i, a]) continue;
                    
                    heightMap[a, i] = roadsHeight;
                    SmoothHeightDown(heightMap, a, i);
                }
            }
        }
        //-------------------------------------------------------------

        //LADDERS------------------------------------------------------
        if (_ladders == EnableDisable.Enabled) {
            for (var i = 0; i < _xSize; i++) {
                for (var a = 0; a < _zSize; a++) {
                    if (holesMap[i, a]) continue;
                    var myHeight = heightMap[a, i];
                    
                    var right = (i + 1 < _xSize) && (!holesMap[i + 1, a] && !laddersMap[i + 1, a]) && (heightMap[a, i + 1] == (myHeight + 1));
                    var left = (i - 1 >= 0) && (!holesMap[i - 1, a] && !laddersMap[i - 1, a]) && (heightMap[a, i - 1] == (myHeight + 1));
                    var up = (a + 1 < _zSize) && (!holesMap[i, a + 1] && !laddersMap[i, a + 1]) && (heightMap[a + 1, i] == (myHeight + 1));
                    var down = (a - 1 >= 0) && (!holesMap[i, a - 1] && !laddersMap[i, a - 1]) && (heightMap[a - 1, i] == (myHeight + 1));
                   
                    var y = 0f;
                    var needSpawn = false;
                    
                    if (right && !left && !down && !up) //Ladder to right
                    {
                        if (i - 1 >= 0) {
                            if (heightMap[a, i - 1] == myHeight) {
                                if (!IsHole(i - 1, a, holesMap) && !IsHole(i + 1, a, holesMap) && !IsHole(i, a, holesMap)) {
                                    y = 0f;
                                    needSpawn = true;
                                }
                            }
                        }
                    }
                    if (!right && left && !down && !up) //Ladder to left
                    {
                        if (i + 1 < _xSize) {
                            if (heightMap[a, i + 1] == myHeight) {
                                if (!IsHole(i - 1, a, holesMap) && !IsHole(i + 1, a, holesMap) && !IsHole(i, a, holesMap)) {
                                    y = 180f;
                                    needSpawn = true;
                                }
                            }
                        }
                    }
                    if (!right && !left && down && !up) //Ladder to down
                    {
                        if (a + 1 < _zSize) {
                            if (heightMap[a + 1, i] == myHeight) {
                                if (!IsHole(i, a - 1, holesMap) && !IsHole(i, a + 1, holesMap) && !IsHole(i, a, holesMap)) {
                                    y = 90f;
                                    needSpawn = true;
                                }
                            }
                        }
                    }
                    if (!right && !left && !down && up) //Ladder to up
                    {
                        if (a - 1 >= 0) {
                            if (heightMap[a, a - 1] == myHeight) {
                                if (!IsHole(i, a - 1, holesMap) && !IsHole(i, a + 1, holesMap) && !IsHole(i, a, holesMap)) {
                                    y = -90f;
                                    needSpawn = true;
                                }
                            }
                        }
                    }

                    if (needSpawn && Random.Range(0, 100) < _laddersChance) {
                        laddersMap[i, a] = true;
                        var ladder = Instantiate(laddersTiles[Random.Range(0, laddersTiles.Count)]);
                        ladder.transform.position = new Vector3(_mapPos.x + i * 2f, _mapPos.y + (heightMap[a, i] * 2f), _mapPos.z + a * 2f);
                        ladder.transform.parent = _map.transform;
                        ladder.transform.localEulerAngles = new Vector3(0f, y, 0f);
                    }
                }
            }
        }
        //-------------------------------------------------------------

        //SPAWNING MAP-------------------------------------------------
        var x = 0f;
        var z = 0f;

        for (var i = 0; i < _xSize; i++) {
            for (var a = 0; a < _zSize; a++) {
                if (!holesMap[i, a]) {
                    SpawnTile(i, a, x, z, _mapPos, _map.transform, roadsMap[a, i], heightMap[i, a]);
                }

                x +=  2f;
            }
            z += 2f;
            x = 0f;
        }
        //-------------------------------------------------------------

        //POIS SPAWNING------------------------------------------------
        if (_POIs == EnableDisable.Enabled) {
            for (var i = 0; i < pointsOfInterest.Count; i++) {
                var xPos = pointsOfInterest[i][1];
                var zPos = pointsOfInterest[i][0];
                GameObject poiObj;
                if (i == 0)
                    poiObj = Instantiate(startTile);
                else if (i == 1) 
                    poiObj = Instantiate(endTile);
                else 
                    poiObj = Instantiate(interestPointTiles[Random.Range(0, interestPointTiles.Count)]);
                poiObj.transform.position = new Vector3(_mapPos.x + zPos * 2f, _mapPos.y + (heightMap[xPos, zPos] * 2f), _mapPos.z + xPos * 2f);
                poiObj.transform.parent = _map.transform;

                xPos = pointsOfInterest[i][0];
                zPos = pointsOfInterest[i][1];

                if (xPos + 1 < _xSize) {
                    if (roadsMap[xPos + 1, zPos])
                        poiObj.transform.localEulerAngles = new Vector3(0f, -90f, 0f);
                } if (zPos + 1 < _zSize) {
                    if (roadsMap[xPos, zPos + 1])
                        poiObj.transform.localEulerAngles = new Vector3(0f, -180f, 0f);
                } if (xPos - 1 > 0) {
                    if (roadsMap[xPos - 1, zPos])
                        poiObj.transform.localEulerAngles = new Vector3(0f, -270f, 0f);
                } if (zPos - 1 > 0) {
                    if (roadsMap[xPos, zPos - 1])
                        poiObj.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
                }
            }
        }
        //-------------------------------------------------------------

        //ROADS SPAWNING---------------------------------------------
        if (_POIs == EnableDisable.Enabled && _roads == EnableDisable.Enabled) {
            var bridgeNumber = -1;
            if (roadBridges.Count > 0) bridgeNumber = Random.Range(0, roadBridges.Count);
            for (var i = 0; i < _xSize; i++) {
                for (var a = 0; a < _zSize; a++) {
                    if (!roadsMap[i, a]) continue;
                    
                    var right = false;
                    var left = false;
                    var up = false;
                    var down = false;

                    if (i + 1 < _xSize) {
                        if (roadsMap[i + 1, a] && !holesMap[a, i + 1]) 
                            right = true;
                    } if (i - 1 >= 0) {
                        if (roadsMap[i - 1, a] && !holesMap[a, i - 1])
                            left = true;
                    } if (a + 1 < _zSize) {
                        if (roadsMap[i, a + 1] && !holesMap[a + 1, i])
                            up = true;
                    } if (a - 1 >= 0) {
                        if (roadsMap[i, a - 1] && !holesMap[a - 1, i])
                            down = true;
                    }

                    if (up && down && !IsHole(i, a, holesMap)) {
                        if (Random.Range(0, 100) < (100 - _roadsFilling))
                            roadsMap[i, a] = false;
                    } else if (left && right && !IsHole(i,a,holesMap)) {
                        if (Random.Range(0, 100) < (100 - _roadsFilling))
                            roadsMap[i, a] = false;
                    }
                }
            }

            for (var i = 0; i < _xSize; i++) {
                for (var a = 0; a < _zSize; a++) {
                    if (!roadsMap[i, a] || !RoadIsNotPOI(i, a, pointsOfInterest)) continue;
                    
                    var yEulers = 0f;
                    var spawned = true;

                    GameObject road = null;

                    var right = false;
                    var left = false;
                    var up = false;
                    var down = false;

                    if (i + 1 < _xSize) {
                        if (roadsMap[i + 1, a]) 
                            right = true;
                    } if (i - 1 >= 0) {
                        if (roadsMap[i - 1, a])
                            left = true;
                    } if (a + 1 < _zSize) {
                        if (roadsMap[i, a + 1])
                            up = true;
                    } if (a - 1 >= 0) {
                        if (roadsMap[i, a - 1])
                            down = true;
                    }

                    if (up && down && right && left) {
                        road = Instantiate(roadCrossroad);
                        yEulers = 90f * Random.Range(0, 4);
                    } else if (up && down && right) {
                        road = Instantiate(roadTriple);
                        yEulers = 0f;
                    } else if (up && left && right) {
                        road = Instantiate(roadTriple);
                        yEulers = -90f;
                    } else if (up && left && down) {
                        road = Instantiate(roadTriple);
                        yEulers = -180f;
                    } else if (right && left && down) {
                        road = Instantiate(roadTriple);
                        yEulers = -270f;
                    } else if (right && left) {
                        if (holesMap[a, i]) {
                            road = Instantiate(roadBridges[bridgeNumber]);
                            yEulers = 90f;
                        } else {
                            if (RoadIsNotPOI(i + 1, a, pointsOfInterest) && RoadIsNotPOI(i - 1, a, pointsOfInterest)) {
                                road = Instantiate(roadStraight);
                                if (Random.Range(0, 100) < _roadsFenceChance)
                                    road.transform.GetChild(1).gameObject.SetActive(true);
                                if (Random.Range(0, 100) < _roadsFenceChance)
                                    road.transform.GetChild(2).gameObject.SetActive(true);

                                yEulers = 90f;
                            }else {
                                if (holesMap[a, i])
                                    road = Instantiate(roadBridges[bridgeNumber]);
                                else
                                    road = Instantiate(roadEnd);

                                if (RoadIsNotPOI(i + 1, a, pointsOfInterest)) yEulers = -90f;
                                else yEulers = 90f;
                            }
                        }
                    } else if (up && down) {
                        if (holesMap[a, i]) {
                            road = Instantiate(roadBridges[bridgeNumber]);
                            yEulers = 0f;
                        } else {
                            if (RoadIsNotPOI(i + 1, a, pointsOfInterest) && RoadIsNotPOI(i - 1, a, pointsOfInterest)) {
                                road = Instantiate(roadStraight);
                                if (Random.Range(0, 100) < _roadsFenceChance)
                                    road.transform.GetChild(1).gameObject.SetActive(true);
                                if (Random.Range(0, 100) < _roadsFenceChance)
                                    road.transform.GetChild(2).gameObject.SetActive(true);

                                yEulers = 0f;
                            }else {
                                if (holesMap[a, i])
                                    road = Instantiate(roadBridges[bridgeNumber]);
                                else
                                    road = Instantiate(roadEnd);

                                if (RoadIsNotPOI(i + 1, a, pointsOfInterest)) yEulers = -90f;
                                else yEulers = 90f;
                            }
                        }
                    } else if (right && down) {
                        road = Instantiate(roadRotate);
                        yEulers = 0f;
                    } else if (right && up) {
                        road = Instantiate(roadRotate);
                        yEulers = -90f;
                    } else if (left && up) {
                        road = Instantiate(roadRotate);
                        yEulers = 180f;
                    } else if (left && down) {
                        road = Instantiate(roadRotate);
                        yEulers = 90f;
                    } else if (up) {
                        if (holesMap[a, i])
                            road = Instantiate(roadBridges[bridgeNumber]);
                        else
                            road = Instantiate(roadEnd);
                        yEulers = 180f;
                    } else if (down) {
                        if (holesMap[a, i])
                            road = Instantiate(roadBridges[bridgeNumber]);
                        else
                            road = Instantiate(roadEnd);
                        yEulers = 0f;
                    } else if (right) {
                        if (holesMap[a, i])
                            road = Instantiate(roadBridges[bridgeNumber]);
                        else
                            road = Instantiate(roadEnd);
                        yEulers = -90f;
                    } else if (left) {
                        if (holesMap[a, i])
                            road = Instantiate(roadBridges[bridgeNumber]);
                        else
                            road = Instantiate(roadEnd);
                        yEulers = 90f;
                    }
                    else
                        spawned = false;

                    //Additional Tiles
                    if ((left && down) || (right && down) || (right && up) || (up && left) || (up && down && right && left) ||
                        (up && down && right) || (up && left && right) || (up && left && down) || (right && left && down)) {
                        if (holesMap[a, i])
                            SpawnTile(i, a, (i * 2), (a * 2), _mapPos, _map.transform, true, heightMap[a, i]);
                    }

                    if (spawned) {
                        road.transform.position = new Vector3(_mapPos.x + i * 2f, _mapPos.y + (heightMap[a, i] * 2f), _mapPos.z + a * 2f);
                        road.transform.parent = _map.transform;
                        road.transform.localEulerAngles = new Vector3(0f, yEulers, 0f);
                    }
                }
            }
        }
        //-------------------------------------------------------------

        lastRoadsMap = roadsMap;
        lastLaddersMap = laddersMap;
        lastMap = _map;
    }
    private void AdditionalFilling(GeneratorParameter _additionalFilling, int sizeOfMap) {
        if (_additionalFilling == GeneratorParameter.Random) 
            _additionalFilling = (GeneratorParameter)Random.Range(1, 6);
        
        if (_additionalFilling != GeneratorParameter.None) {
            var countsCycle = (int)((sizeOfMap/5f)) * (int)_additionalFilling;
            var circlesRange = (sizeOfMap/6f) + ((sizeOfMap / 30f) * (int)_additionalFilling);
            var objectsCounts = sizeOfMap/2.5f + ((sizeOfMap/6) * (int)_additionalFilling);

            var treesPoints = new List<Vector3>();
            var bushsPoints = new List<Vector3>();
            var bigStonesPoints = new List<Vector3>();
            var grassPoint = new List<Vector3>();
            var branchsPoints = new List<Vector3>();
            var logsPoints = new List<Vector3>();

            for (var a = 0; a < countsCycle; a++) {
                var circleTreesPos = new Vector3(Random.Range(0f, sizeOfMap * 2f), 15f, Random.Range(0f, sizeOfMap * 2f));

                for (var i = 0; i < objectsCounts; i++) {
                    var rayPos = circleTreesPos;
                    rayPos.x += Random.Range(-circlesRange, circlesRange);
                    rayPos.z += Random.Range(-circlesRange, circlesRange);
                    if (Physics.Raycast(rayPos, Vector3.down, out var hit, Mathf.Infinity)) {
                        if (hit.transform.name.Contains("Tile") && IsPosAvailableByDistance(hit.point,treesPoints,1.5f) && isPosNotInPOI(hit.point,lastRoadsMap,lastLaddersMap)) {
                            var tree = Instantiate(treesPrefabs[Random.Range(0, treesPrefabs.Count)], hit.point, Quaternion.identity, lastMap.transform);
                            tree.transform.eulerAngles = new Vector3(Random.Range(-7.5f, 7.5f), Random.Range(0f, 360f), Random.Range(-7.5f, 7.5f));
                            treesPoints.Add(hit.point);
                        }
                    }
                }

                for (var i = 0; i < objectsCounts/3; i++) {
                    var rayPos = circleTreesPos;
                    rayPos.x += Random.Range(-circlesRange, circlesRange);
                    rayPos.z += Random.Range(-circlesRange, circlesRange);
                    if (Physics.Raycast(rayPos, Vector3.down, out var hit, Mathf.Infinity)) {
                        if (hit.transform.name.Contains("Tile") && IsPosAvailableByDistance(hit.point, bushsPoints, 2f) && isPosInRangeOf(hit.point,treesPoints,4f) && isPosNotInPOI(hit.point, lastRoadsMap, lastLaddersMap)) {
                            var tree = Instantiate(bushsPrefabs[Random.Range(0, bushsPrefabs.Count)], hit.point, Quaternion.identity, lastMap.transform);
                            tree.transform.eulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);
                            bushsPoints.Add(hit.point);
                        }
                    }
                }

                for (var i = 0; i < objectsCounts * 4; i++) {
                    var rayPos = circleTreesPos;
                    rayPos.x += Random.Range(-circlesRange, circlesRange);
                    rayPos.z += Random.Range(-circlesRange, circlesRange);
                    if (Physics.Raycast(rayPos, Vector3.down, out var hit, Mathf.Infinity)) {
                        if (hit.transform.name.Contains("Tile") && IsPosAvailableByDistance(hit.point,grassPoint, 0.25f) && isPosNotInPOI(hit.point, lastRoadsMap, lastLaddersMap)) {
                            var tree = Instantiate(grassPrefabs[Random.Range(0, grassPrefabs.Count)], hit.point, Quaternion.identity, lastMap.transform);
                            tree.transform.eulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);
                            grassPoint.Add(hit.point);
                        }
                    }
                }

                for (var i = 0; i < objectsCounts; i++) {
                    var rayPos = circleTreesPos;
                    rayPos.x += Random.Range(-circlesRange, circlesRange);
                    rayPos.z += Random.Range(-circlesRange, circlesRange);
                    if (Physics.Raycast(rayPos, Vector3.down, out var hit, Mathf.Infinity)) {
                        if (hit.transform.name.Contains("Tile") && isPosNotInPOI(hit.point, lastRoadsMap, lastLaddersMap)) {
                            var tree = Instantiate(littleStonesPrefabs[Random.Range(0, littleStonesPrefabs.Count)], hit.point, Quaternion.identity, lastMap.transform);
                            tree.transform.eulerAngles = new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
                        }
                    }
                }
            }


            for (var i = 0; i < ((objectsCounts * countsCycle)*(int)_additionalFilling); i++) {
                var rayPos = new Vector3(Random.Range(0f, sizeOfMap * 2f), 15f, Random.Range(0f, sizeOfMap * 2f));
                if (Physics.Raycast(rayPos, Vector3.down, out var hit, Mathf.Infinity)) {
                    if (hit.transform.name.Contains("Tile") && IsPosAvailableByDistance(hit.point, grassPoint, 0.25f) && isPosNotInPOI(hit.point, lastRoadsMap, lastLaddersMap)) {
                        var tree = Instantiate(grassPrefabs[Random.Range(0, grassPrefabs.Count)], hit.point, Quaternion.identity, lastMap.transform);
                        tree.transform.eulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);
                        grassPoint.Add(hit.point);
                    }
                }
            }

            for (var i = 0; i < objectsCounts / 2; i++) {
                var rayPos = new Vector3(Random.Range(0f, sizeOfMap * 2f), 15f, Random.Range(0f, sizeOfMap * 2f));
                if (Physics.Raycast(rayPos, Vector3.down, out var hit, Mathf.Infinity)) {
                    if (hit.transform.name.Contains("Tile") && IsPosAvailableByDistance(hit.point, bigStonesPoints, 10f) && isPosInRangeOf(hit.point, treesPoints, 8f) && isPosNotInPOI(hit.point, lastRoadsMap, lastLaddersMap)) {
                        var tree = Instantiate(bigStonesPrefabs[Random.Range(0, bigStonesPrefabs.Count)], hit.point, Quaternion.identity, lastMap.transform);
                        tree.transform.eulerAngles = new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
                        bigStonesPoints.Add(hit.point);
                    }
                }
            }

            for (var i = 0; i < objectsCounts / 2; i++) {
                var rayPos = new Vector3(Random.Range(0f, sizeOfMap * 2f), 15f, Random.Range(0f, sizeOfMap * 2f));
                if (Physics.Raycast(rayPos, Vector3.down, out var hit, Mathf.Infinity)) {
                    if (hit.transform.name.Contains("Tile") && IsPosAvailableByDistance(hit.point, bigStonesPoints, 10f) && isPosInRangeOf(hit.point, treesPoints, 8f) && isPosNotInPOI(hit.point, lastRoadsMap, lastLaddersMap)) {
                        var tree = Instantiate(branchsPrefabs[Random.Range(0, branchsPrefabs.Count)], hit.point, Quaternion.identity, lastMap.transform);
                        tree.transform.eulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);
                        branchsPoints.Add(hit.point);
                    }
                }
            }

            for (var i = 0; i < objectsCounts / 2; i++) {
                var rayPos = new Vector3(Random.Range(0f, sizeOfMap * 2f), 15f, Random.Range(0f, sizeOfMap * 2f));
                if (Physics.Raycast(rayPos, Vector3.down, out var hit, Mathf.Infinity)) {
                    if (hit.transform.name.Contains("Tile") && IsPosAvailableByDistance(hit.point, bigStonesPoints, 10f) && isPosInRangeOf(hit.point, treesPoints, 8f) && isPosNotInPOI(hit.point, lastRoadsMap, lastLaddersMap)) {
                        var tree = Instantiate(logsPrefabs[Random.Range(0, logsPrefabs.Count)], hit.point, Quaternion.identity, lastMap.transform);
                        tree.transform.eulerAngles = new Vector3(0f, Random.Range(0f, 360f),0f);
                        logsPoints.Add(hit.point);
                    }
                }
            }
        }
    }
    private bool isPosNotInPOI(Vector3 posToCheck, bool[,] roadsMap, bool[,] laddersMap) {
        var x = Mathf.CeilToInt(posToCheck.x / 2f);
        var z = Mathf.CeilToInt(posToCheck.z / 2f);

        if (x < roadsMap.GetLength(0) && z < roadsMap.GetLength(1))
            return !roadsMap[z, x] && !laddersMap[x, z];

        return false;
    }
    private bool IsPosAvailableByDistance(Vector3 posToCheck, List<Vector3> otherPoses, float minDistance) {
        var minDistanceWeHas = -1f;

        foreach (var t in otherPoses) {
            var distance = Vector3.Distance(posToCheck, t);
            if (distance < minDistanceWeHas || minDistanceWeHas < 0f) 
                minDistanceWeHas = distance;
        }

        return minDistanceWeHas > minDistance || minDistanceWeHas < 0f;
    }
    private bool isPosInRangeOf(Vector3 posToCheck, List<Vector3> otherPoses, float needDistance) {
        var minDistanceWeHas = -1f;

        foreach (var t in otherPoses) {
            var distance = Vector3.Distance(posToCheck, t);
            if (distance < minDistanceWeHas || minDistanceWeHas < 0f) 
                minDistanceWeHas = distance;
        }

        return minDistanceWeHas < needDistance;
    }
    private bool RoadIsNotPOI(int x, int z, List<int[]> poi) {
        var roadIsNotPoi = true;

        foreach (var t in poi) {
            if(t[0] == x && t[1] == z) {
                roadIsNotPoi = false;
                break;
            }
        }

        return roadIsNotPoi;
    }
    private void SpawnTile(int _i, int _a, float _x, float _z, Vector3 _mapPos, Transform _parent, bool _isRoad, int _height) {
        var newPart = Instantiate(tiles[Random.Range(0,tiles.Count)]);
        var posY = _mapPos.y;

        if (_height > 0)
            posY += (_height * 2f);
        newPart.transform.position = new Vector3(_mapPos.x + _x, posY, _mapPos.z + _z);

        if (_height > 0) {
            newPart.transform.GetChild(1).localScale = new Vector3(1f, 1f + (_height * 1.2f), 1f);
            newPart.transform.GetChild(1).localPosition = new Vector3(0f, -0.7f * _height, 0f);
        }

        var rotRandom = Random.Range(0, 4);
        newPart.transform.localEulerAngles = rotRandom switch {
            0 => new Vector3(0f, 90f, 0f),
            1 => new Vector3(0f, 180f, 0f),
            2 => new Vector3(0f, 270f, 0f),
            _ => new Vector3(0f, 0f, 0f)
        };
        newPart.transform.parent = _parent;
    }
    private int[,] SmoothHeights(int[,] currentMap, int x, int z) {
        var myHeight = currentMap[x, z];
        var heightDifference = 0;

        if (z - 1 > 0) {
            var difference = currentMap[x, z - 1] - myHeight;
            if (difference > heightDifference) heightDifference = difference;
        } if (z + 1 < currentMap.GetLength(1)) {
            var difference = currentMap[x, z + 1] - myHeight;
            if (difference > heightDifference) heightDifference = difference;
        } if (x - 1 > 0) {
            var difference = currentMap[x - 1, z] - myHeight;
            if (difference > heightDifference) heightDifference = difference;
        } if (x + 1 < currentMap.GetLength(0)) {
            var difference = currentMap[x + 1, z] - myHeight;
            if (difference > heightDifference) heightDifference = difference;
        }

        if (heightDifference > 1)
            currentMap[x, z] = myHeight + 1;

        return currentMap;
    }
    private int[,] SmoothHeightDown(int[,] currentMap, int x, int z) {
        var myHeight = currentMap[x, z];

        if (z - 1 > 0) {
            if (currentMap[x, z - 1] > myHeight) {
                currentMap[x, z - 1] = Random.Range(myHeight, myHeight + 2);
                currentMap = SmoothHeightDown(currentMap, x, z - 1);
            }
        } if (z + 1 < currentMap.GetLength(1)) {
            if (currentMap[x, z + 1] > myHeight) {
                currentMap[x, z + 1] = Random.Range(myHeight, myHeight + 2);
                currentMap = SmoothHeightDown(currentMap, x, z + 1);
            }
        }
        if (x - 1 > 0) {
            if (currentMap[x-1, z] > myHeight) {
                currentMap[x-1, z] = Random.Range(myHeight, myHeight + 2);
                currentMap = SmoothHeightDown(currentMap, x - 1, z);
            }
        }
        if (x + 1 < currentMap.GetLength(0)) {
            if (currentMap[x + 1, z] > myHeight) {
                currentMap[x + 1, z] = Random.Range(myHeight, myHeight + 2);
                currentMap = SmoothHeightDown(currentMap, x + 1, z);
            }
        }

        return currentMap;
    }
    private int[,] RaiseHeight(int[,] currentMap, int _x, int _z, int maxSize, int maxHeight, bool[,] holesMap, int iteration, EnableDisable hs) {
        if ((hs != EnableDisable.Enabled || IsNeighboringHole(_x, _z, holesMap)) && hs != EnableDisable.Disabled)
            return currentMap;
        
        if(currentMap[_x, _z] < maxHeight)
            currentMap[_x, _z] += 1;

        var chanceForAdditionalHeight = 100f;
        var anywayHeights = Mathf.FloorToInt(maxSize / 1.35f);

        if (iteration > anywayHeights)
            chanceForAdditionalHeight = 100f - ((100f / (maxSize - anywayHeights)) * (iteration - anywayHeights));

        iteration++;

        if (_z - 1 > 0 && Random.Range(0, 100) < (chanceForAdditionalHeight * Random.Range(0.75f,1f)))
            currentMap = RaiseHeight(currentMap, _x, _z - 1, maxSize, maxHeight, holesMap, iteration, hs);
        if (_z + 1 < currentMap.GetLength(1) && Random.Range(0, 100) < (chanceForAdditionalHeight * Random.Range(0.75f, 1f)))
            currentMap = RaiseHeight(currentMap, _x, _z + 1, maxSize, maxHeight, holesMap, iteration, hs);
        if (_x - 1 > 0 && Random.Range(0, 100) < (chanceForAdditionalHeight * Random.Range(0.75f, 1f)))
            currentMap = RaiseHeight(currentMap, _x - 1, _z, maxSize, maxHeight, holesMap, iteration, hs);
        if (_x + 1 < currentMap.GetLength(0) && Random.Range(0, 100) < (chanceForAdditionalHeight * Random.Range(0.75f, 1f)))
            currentMap = RaiseHeight(currentMap, _x + 1, _z, maxSize, maxHeight, holesMap, iteration, hs);
        return currentMap;
    }
    private bool[,] CreateHoles(bool[,] currentMap, int x, int z, int maxSize, int iteration) {
        if (currentMap[x, z]) return currentMap;
        
        currentMap[x, z] = true;

        var chanceForAdditionalHole = 100f;
        var anywayHoles = Mathf.FloorToInt(maxSize / 1.25f);

        if (iteration > anywayHoles)
            chanceForAdditionalHole = 100f - ((100f / (maxSize - anywayHoles)) * ((float)(iteration - anywayHoles)));

        iteration++;

        if (iteration < maxSize) {
            if (z - 1 > 0 && Random.Range(0, 100) < (chanceForAdditionalHole * Random.Range(0.75f, 1f)))
                currentMap = CreateHoles(currentMap, x, z - 1, maxSize, iteration);
            if (z + 1 < currentMap.GetLength(1) && Random.Range(0, 100) < (chanceForAdditionalHole * Random.Range(0.75f, 1f)))
                currentMap = CreateHoles(currentMap, x, z + 1, maxSize, iteration);
            if (x - 1 > 0 && Random.Range(0, 100) < (chanceForAdditionalHole * Random.Range(0.75f, 1f)))
                currentMap = CreateHoles(currentMap, x - 1, z, maxSize, iteration);
            if (x + 1 < currentMap.GetLength(0) && Random.Range(0, 100) < (chanceForAdditionalHole * Random.Range(0.75f, 1f)))
                currentMap = CreateHoles(currentMap, x + 1, z, maxSize, iteration);
        }

        return currentMap;
    }
    private bool IsNeighboringHigher(int[,] currentMap, int x, int z) {
        var isHigher = false;
        var myHeight = currentMap[x, z];
        if (z - 1 > 0) {
            if (currentMap[x, z - 1] > myHeight) 
                isHigher = true;
        } if (z + 1 < currentMap.GetLength(1)) {
            if (currentMap[x, z + 1] > myHeight) 
                isHigher = true;
        } if (x - 1 > 0) {
            if (currentMap[x - 1, z] > myHeight) 
                isHigher = true;
        } if (x + 1 < currentMap.GetLength(0)) {
            if (currentMap[x + 1, z] > myHeight) 
                isHigher = true;
        }
        return isHigher;
    }
    private bool IsNeighboringHole(int x, int z, bool[,] currentHoles) {
        var isHole = false;

        if (z - 1 > 0)
            if (currentHoles[x,z-1]) isHole = true;
        if (z + 1 < currentHoles.GetLength(1))
            if (currentHoles[x, z + 1]) isHole = true;
        if (x - 1 > 0)
            if (currentHoles[x - 1, z]) isHole = true;
        if (x + 1 < currentHoles.GetLength(0))
            if (currentHoles[x + 1, z]) isHole = true;

        return isHole;
    }
    private bool IsHole(int x, int z, bool[,] holes) {
        return z >= 0 && z < holes.GetLength(0) && x >= 0 && x < holes.GetLength(1);
    }
}
