using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CPuzzleMap : MonoBehaviour {

    #region Internal class

    [System.Serializable]
    public class UnityEventInt: UnityEvent<int, int> {}

    #endregion

    #region Fields

    public static string MAP_ASSET = string.Empty;
    [Header("Game")]
    [SerializeField]    protected bool m_IsGameComplete = false;
    [SerializeField]    protected bool m_IsGameHandling = false;
    [SerializeField]    protected int m_LimitRotate = 9999;
    public UnityEvent OnEventStartGame;
    public UnityEvent OnEventUpdateGame;
    public UnityEventInt OnEventUpdateRotate;
    public UnityEvent OnEventEndGame;
    public UnityEvent OnEventFailGame;

    [Header("Grid")]
    [SerializeField]    protected int m_Width = 10;
    [SerializeField]    protected int m_Height = 10;

    [Header("Tiles")]
    [SerializeField]    protected Transform m_MapRoot;
    [SerializeField]    protected string m_TileBorderFolder = "Tiles/Plain/Borders/";
    [SerializeField]    protected CCell[] m_BorderPrefabs;
    [SerializeField]    protected string m_TileFolder = "Tiles/Plain/Tiles/";
    [SerializeField]    protected CCell[] m_TilePrefabs;

    [Header("Markers")]
    [SerializeField]    protected Transform m_MarkerRoot;
    [SerializeField]    protected string m_MarkerFolder = "Markers/Plain/";
    [SerializeField]    protected GameObject[] m_MarkerPrefabs;

    [Header("Cloud")]
	[SerializeField]	protected LayerMask m_CloudLayerMask;
    [SerializeField]    protected Transform m_CloudRoot;
    [SerializeField]    protected string m_CloudFolder = "Clouds/Group/";
    [SerializeField]    protected CCloud[] m_CloudPrefabs;
    [SerializeField]    protected List<CCloud> m_CloudLists;
    [SerializeField]    protected List<CCloud> m_CloudSelecteds;

    [Header("Rain")]
    [SerializeField]    protected Transform m_RainRoot;
    [SerializeField]    protected string m_RainFolder = "Rains/Water/";
    [SerializeField]    protected CRain[] m_RainPrefabs;

    protected int[,] m_Grid;
    protected CCell[,] m_GridMap;
    protected List<CRain> m_RainPool;
    protected CMapParser.CMapFullInfo m_MapParsed;
    protected CMapParser.CMapFullInfo.CCloudInfo[] m_CloudGroups;
    protected CUIGameController m_GameController;
    protected CSaveLevel m_SaveLevel;

    #endregion

    #region Implementation Monobehaviour

    protected virtual void Awake() {
        this.m_SaveLevel = new CSaveLevel();
    }

    protected virtual void Start() {
        this.InitGame();
        this.GenerateMapCellObjects();
        this.GenerateCloudGroup();
        this.GenerateMarker();
        this.CheckCloud();
        this.OnStartGame();
    }

    // protected virtual void Update() {
    //     if (Input.GetKeyDown(KeyCode.A)) {
    //         this.RotationCloudX();
    //     }
    //     if (Input.GetKeyDown(KeyCode.S)) {
    //         this.RotationCloudY();
    //     }
    //     if (Input.GetKeyDown(KeyCode.D)) {
    //         this.RotationCloudZ();
    //     }
    // }

    #endregion

    #region GameState

    public virtual void OnStartGame() {
        this.m_IsGameComplete = false;
        if (this.OnEventStartGame != null) {
            this.OnEventStartGame.Invoke();
        }
        if (this.OnEventUpdateRotate != null) {
            this.OnEventUpdateRotate.Invoke(this.m_LimitRotate, this.m_MapParsed.limit);
        }
    }

    public virtual void OnGameUpdate() {
        this.m_LimitRotate -= 1;
        if (this.m_LimitRotate <= 0) {
            this.OnFailGame();
        }
        if (this.OnEventUpdateGame != null) {
            this.OnEventUpdateGame.Invoke();
        }
        if (this.OnEventUpdateRotate != null) {
            this.OnEventUpdateRotate.Invoke(this.m_LimitRotate, this.m_MapParsed.limit);
        }
    }

    public virtual void OnEndGame() {
        this.m_IsGameComplete = true;
        if (this.OnEventEndGame != null) {
            this.OnEventEndGame.Invoke();
        }
        this.m_SaveLevel.Save(this.m_MapParsed.name);
    }

    public virtual void OnFailGame() {
        this.m_IsGameHandling = true;
        if (this.OnEventFailGame != null) {
            this.OnEventFailGame.Invoke();
        }
    }

    #endregion

    #region Main methods

    public void InitGame() {
        // UI controller
        this.m_GameController = CUIGameController.GetInstance();
        // X
        this.m_GameController.OnRotateX -= this.RotationCloudX;
        this.m_GameController.OnRotateX += this.RotationCloudX; 
        // Y
        this.m_GameController.OnRotateY -= this.RotationCloudY;
        this.m_GameController.OnRotateY += this.RotationCloudY;
        // Z
        this.m_GameController.OnRotateZ -= this.RotationCloudZ;
        this.m_GameController.OnRotateZ += this.RotationCloudZ;
        // Current clouds
        this.m_CloudLists = new List<CCloud>();
        this.m_CloudSelecteds = new List<CCloud>();
        // Current pool
        this.m_RainPool = new List<CRain>();
        // MAP
        this.LoadMapData();
        // TILE
        this.LoadTiles();
        // MARKER
        this.LoadMarker();
        // CLOUD
        this.LoadCloud();
        // RAIN
        this.LoadRain();
    }

    public void LoadMapData() {
        if (string.IsNullOrEmpty(MAP_ASSET) == false) {
            this.m_MapParsed = CMapParser.parseMap (MAP_ASSET);
            // GAME
            this.m_LimitRotate = this.m_MapParsed.limit;
            // GRID
            this.m_Width = this.m_MapParsed.map.GetLength(0);
            this.m_Height = this.m_MapParsed.map.GetLength(1);
            this.m_Grid = new int[this.m_Width, this.m_Height];
            System.Buffer.BlockCopy(this.m_MapParsed.map, 0, this.m_Grid, 0, this.m_MapParsed.map.Length * sizeof(int));
            // CLOUD
            this.m_CloudGroups = new CMapParser.CMapFullInfo.CCloudInfo[this.m_MapParsed.clouds.Length];
            System.Array.Copy(this.m_MapParsed.clouds, this.m_CloudGroups, this.m_MapParsed.clouds.Length);
            // FOLDER
            Color cameraBackground;
            if (ColorUtility.TryParseHtmlString(this.m_MapParsed.backgroundColor, out cameraBackground)) {
                Camera.main.backgroundColor = cameraBackground;
            }
            this.m_TileBorderFolder = this.m_MapParsed.tileBorderFolder;
            this.m_TileFolder = this.m_MapParsed.tileFolder;
            this.m_MarkerFolder = this.m_MapParsed.markerFolder;
            this.m_CloudFolder = this.m_MapParsed.cloudFolder;
            this.m_RainFolder = this.m_MapParsed.rainFolder;
        } 
    }

    public void LoadTiles() {
        this.m_BorderPrefabs = Resources.LoadAll<CCell>(this.m_TileBorderFolder);
        this.m_TilePrefabs = Resources.LoadAll<CCell>(this.m_TileFolder);
    }

    public void LoadMarker() {
        this.m_MarkerPrefabs = Resources.LoadAll<GameObject>(this.m_MarkerFolder);
    }

    public void LoadCloud() {
        this.m_CloudPrefabs = Resources.LoadAll<CCloud>(this.m_CloudFolder);
    }

    public void LoadRain() {
        this.m_RainPrefabs = Resources.LoadAll<CRain>(this.m_RainFolder);
    }

    public void UpdateLimitRotate(int value) {
        this.m_LimitRotate = value;
        this.m_MapParsed.limit = value;
        this.m_IsGameHandling = !(value > 0);
        if (this.OnEventUpdateRotate != null) {
            this.OnEventUpdateRotate.Invoke(this.m_LimitRotate, this.m_MapParsed.limit);
        }
    }

    #endregion

    #region Camera

    #endregion

    #region Cloud

    public void RotationCloudX() {
        if (this.m_IsGameComplete)
            return;
        if (this.m_IsGameHandling)
            return;
        this.CombineXYZ(new Vector3(90f, 0f, 0f));
    }

    public void RotationCloudY() {
        if (this.m_IsGameComplete)
            return;
        if (this.m_IsGameHandling)
            return;
        this.CombineXYZ(new Vector3(0f, 90f, 0f));
    }

    public void RotationCloudZ() {
        if (this.m_IsGameComplete)
            return;
        if (this.m_IsGameHandling)
            return;
        this.CombineXYZ(new Vector3(0f, 0f, -90f));
    }

    private void CombineXYZ(Vector3 rotation) {
        if (this.m_CloudSelecteds.Count > 0) {
            this.m_IsGameHandling = true;
            // HANDLE CLOUD 1 -> LAST
            var savePosition = Vector3.zero;
            for (int i = 1; i < this.m_CloudSelecteds.Count; i++)
            {
                this.m_CloudSelecteds[i].Combine( 
                    this.m_CloudSelecteds[i].move,
                    rotation, 
                    null, null);
                savePosition = this.m_CloudSelecteds[i].spot;
                this.m_CloudSelecteds[i].spot = this.m_CloudSelecteds[i].move;
                this.m_CloudSelecteds[i].move = savePosition;
            }
            // HANDLE CLOUD 0
            this.m_CloudSelecteds[0].Combine(
                this.m_CloudSelecteds[0].move,
                rotation, 
            () => {
                this.ResetRain();
            }, () => {
                this.OnGameUpdate();
                this.CheckCloud();
            });
            savePosition = this.m_CloudSelecteds[0].spot;
            this.m_CloudSelecteds[0].spot = this.m_CloudSelecteds[0].move;
            this.m_CloudSelecteds[0].move = savePosition;
        }
    }

    public void CheckCloud () {
		RaycastHit hitInfo;
        int i, x, y;
        var isCorrect = this.m_CloudLists.Count > 0;
        for (y = 0; y < this.m_GridMap.GetLength(1); y++) {
            for (x = 0; x < this.m_GridMap.GetLength(0); x++) {
                var cell = this.m_GridMap[x, y];
                if (Physics.Raycast (cell.GetPosition(), Vector3.up, out hitInfo, 100f, this.m_CloudLayerMask)) {
                    var rain = this.GetRain();
                    rain.SetPosition (cell.GetPosition());
                    rain.SetCloudMesh (hitInfo.point);
                    rain.SetRainActive (false);
                    rain.SetActive (true);
                    rain.name = string.Format("Rain {0}:{1}:{2}", x, 0, y);    
                    isCorrect &= this.m_Grid[x, y] == 1;
                } else if (this.m_Grid[x, y] == 1) {
                    isCorrect = false;
                }
            }
        }
        if (isCorrect && this.m_LimitRotate > 0) {
            for (i = 0; i < this.m_RainPool.Count; i++)
            {
                var rain = this.m_RainPool[i];
                if (rain.active) {
                    rain.SetRainActive (true);
                }
            }
            this.OnEndGame();
        } 
        this.m_IsGameHandling = false;
    }

    public void GenerateCloudGroup() {
        for (int i = 0; i < this.m_CloudGroups.Length; i++)
        {
            var cloud = this.m_CloudGroups[i];
            if (cloud != null) {
                this.GenerateCloud (
                    cloud.index, 
                    CMapParser.parseV3 (cloud.spot1),
                    CMapParser.parseV3 (cloud.spot2),
                    CMapParser.parseV3 (cloud.rotate),
                    cloud.data
                );
            }
        }
        this.m_GameController.ShowCloudPanel (this.m_CloudLists, (index) => {
            this.AddCloudSelecteds(index);
        });
        // ADD SELECTED
        var minIndex = 9999;
        for (int i = 0; i < this.m_CloudLists.Count; i++)
        {
            if (minIndex >= this.m_CloudLists[i].index) {
                minIndex = this.m_CloudLists[i].index;
            }
        }
        this.AddCloudSelecteds (minIndex);
    }

    public void GenerateCloud(int index, Vector3 spot, Vector3 move, Vector3 rotate, int[,,] data) {
        var cloud = this.GenerateObject (spot.x, spot.y, spot.z, this.m_CloudRoot, this.m_CloudPrefabs[index]);
        cloud.name = string.Format("Cloud {0}:{1}:{2}", spot.x, spot.y, spot.z);
        cloud.spot = spot;
        cloud.move = move;
        cloud.index = index;
        cloud.GenerateCloud(data);
        cloud.SetRotation(rotate);
        this.m_CloudLists.Add (cloud);
    }

    public void AddCloudSelecteds(int index) {
        this.m_CloudSelecteds.Clear();
        this.m_CloudSelecteds.TrimExcess();
        for (int i = 0; i < this.m_CloudLists.Count; i++)
        {
            var cloud = this.m_CloudLists[i];
            if (cloud.index == index) {
                this.m_CloudSelecteds.Add (cloud);
            }
        }
    } 

    #endregion

    #region Rain

    public void ResetRain() {
        for (int i = 0; i < this.m_RainPool.Count; i++)
        {
            this.m_RainPool[i].SetActive (false);
            this.m_RainPool[i].SetRainActive (false);
        }
    }

    public CRain GenerateRain() {
        var random = Random.Range(0, this.m_RainPrefabs.Length);    
        var rain = this.GenerateObject (0f, 0f, 0f, this.m_RainRoot, this.m_RainPrefabs[random]);
        return rain;
    }

    public CRain GetRain() {
        for (int i = 0; i < this.m_RainPool.Count; i++)
        {
            var item = this.m_RainPool[i];
            if (item.GetActive() == false) {
                return item;
            } 
        }
        var rain = this.GenerateRain();
        this.m_RainPool.Add(rain);
        return rain;
    }

    #endregion

    #region Grid

    public void GenerateMapCellObjects() {
        this.m_GridMap = new CCell[this.m_Width, this.m_Height];
        for (int y = 0; y < this.m_Grid.GetLength(1); y++) {
            for (int x = 0; x < this.m_Grid.GetLength(0); x++) {
                if (   x == 0 || x == this.m_Grid.GetLength(0) - 1
                    || y == 0 || y == this.m_Grid.GetLength(1) - 1) {
                    this.m_GridMap[x, y] = this.GenerateTile(x, 0f, y, 
                        0, 
                        this.m_BorderPrefabs[x % this.m_BorderPrefabs.Length]);
                } else {
                    var value = this.m_Grid[x, y];
                    this.m_GridMap[x, y] = this.GenerateTile(x, 0f, y, 
                        value,
                        this.m_TilePrefabs[UnityEngine.Random.Range(0, this.m_TilePrefabs.Length)]);
                }
            }
        }
    }

    public CCell GenerateTile(float x, float y, float z, int value, CCell prefab) {
        var cell = Instantiate(prefab);
        cell.transform.SetParent(this.m_MapRoot);
        cell.transform.localPosition = new Vector3(x, y, z);
        cell.x = x;
        cell.y = y;
        cell.z = z;
        cell.value = value;
        cell.name = string.Format("Cell {0}:{1}:{2}", x, y, z);
        return cell;
    }

    #endregion

    #region Tree

    public void GenerateMarker() {
        for (int y = 0; y < this.m_Grid.GetLength(1); y++) {
            for (int x = 0; x < this.m_Grid.GetLength(0); x++) {
                var value = this.m_Grid[x, y];
                if (value == 1) {
                    var prefab = this.m_MarkerPrefabs[UnityEngine.Random.Range(0, this.m_MarkerPrefabs.Length)];
                    var tree = Instantiate(prefab);
                    tree.transform.SetParent(this.m_MarkerRoot);
                    tree.transform.localPosition = new Vector3(x, 0f, y);
                }
            }
        }
    }

    #endregion

    #region Utilities

    public T GenerateObject<T>(float x, float y, float z, Transform parent, T prefab) where T : MonoBehaviour {
        var cell = Instantiate(prefab);
        cell.transform.SetParent(parent);
        cell.transform.localPosition = new Vector3(x, y, z);
        return cell;
    }

    #endregion

}
