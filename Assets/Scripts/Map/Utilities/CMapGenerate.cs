using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CMapGenerate : MonoBehaviour {

	[Header("Generate")]
	private const string GENERATE_MARKER = "GENERATE_MARKER";
	private const string GENERATE_CLOUD = "GENERATE_CLOUD";
	[SerializeField]	protected string m_MapName = "Level";
	[SerializeField]	protected float m_MapHard = 1f;
	[SerializeField]	protected int m_MapLimitRotate = 9999;

	[SerializeField]	protected string m_GenerateState = GENERATE_MARKER;
	[SerializeField]	protected Text m_UIState;
	[SerializeField]	protected Vector3 m_StartSelectPoint;
	[SerializeField]	protected Vector3 m_EndSelectPoint;

	[Header("Grid")]
    [SerializeField]    protected TextAsset m_MapAsset;
    [SerializeField]    protected int m_Width = 10;
    [SerializeField]    protected int m_Height = 10;
    [SerializeField]    protected int m_LimitRotate = 9999;

    [Header("Tiles")]
    [SerializeField]    protected Transform m_MapRoot;
	[SerializeField]	protected LayerMask m_TileLayerMask;
    [SerializeField]    protected string m_TileBorderFolder = "Tiles/Plain/Borders";
    [SerializeField]    protected CCell[] m_BorderPrefabs;
    [SerializeField]    protected string m_TileFolder = "Tiles/Plain/Tiles";
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

    [Header("Rain")]
    [SerializeField]    protected Transform m_RainRoot;
    [SerializeField]    protected string m_RainFolder = "Rains/Water/";
    [SerializeField]    protected CRain[] m_RainPrefabs;

	protected int[,] m_Grid;
    protected CCell[,] m_GridMap;
	protected CMapParser.CMapFullInfo m_MapFullInfo;
    protected CMapParser.CMapFullInfo.CCloudInfo[] m_CloudGroups;

	protected virtual void Start() {
		this.InitMap();
		this.GenerateMapCellObjects();
    }

	protected virtual void Update() {
		if (this.m_GenerateState.Equals (GENERATE_MARKER)) {
			this.DetectMarker();
		} else if (this.m_GenerateState.Equals (GENERATE_CLOUD)) {
			this.DetectCloud();
		}
		// UPDATE UI
		this.m_UIState.text = this.m_GenerateState;
	}

	public void GenerateFullInfo() {
		this.m_MapFullInfo = new CMapParser.CMapFullInfo();
		this.m_MapFullInfo.name = this.m_MapName;
		this.m_MapFullInfo.hard = this.m_MapHard;
		this.m_MapFullInfo.limit = this.m_MapLimitRotate;
		this.m_MapFullInfo.backgroundColor = string.Format("#{0}", ColorUtility.ToHtmlStringRGB (Camera.main.backgroundColor));
		this.m_MapFullInfo.tileBorderFolder = this.m_TileBorderFolder;
		this.m_MapFullInfo.tileFolder = this.m_TileFolder;
		this.m_MapFullInfo.markerFolder = this.m_MarkerFolder;
		this.m_MapFullInfo.cloudFolder = this.m_CloudFolder;
		this.m_MapFullInfo.rainFolder = this.m_RainFolder;
		this.m_MapFullInfo.map = this.m_Grid;
		this.m_MapFullInfo.clouds = new CMapParser.CMapFullInfo.CCloudInfo[this.m_CloudLists.Count];
		for (int i = 0; i < this.m_CloudLists.Count; i++)
		{
			var cloud = this.m_CloudLists[i];
			if (cloud != null) {
				this.m_MapFullInfo.clouds[i] = new CMapParser.CMapFullInfo.CCloudInfo();
				this.m_MapFullInfo.clouds[i].index = cloud.index;
				this.m_MapFullInfo.clouds[i].spot1 = string.Format("{0},{1},{2}", cloud.spot.x, cloud.spot.y, cloud.spot.z);
				this.m_MapFullInfo.clouds[i].spot2 = string.Format("{0},{1},{2}", cloud.move.x, cloud.move.y, cloud.move.z);
				this.m_MapFullInfo.clouds[i].rotate = cloud.GetRotationStr();
				this.m_MapFullInfo.clouds[i].data = cloud.cloudGrid;
			}
		}
		Debug.Log (TinyJSON.JSON.Dump(this.m_MapFullInfo));
		GUIUtility.systemCopyBuffer = TinyJSON.JSON.Dump(this.m_MapFullInfo);
	}

	private void DetectMarker() {
		if (Input.GetMouseButtonDown(0)) {
			var raycast = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitInfo;
			if (Physics.Raycast(raycast, out hitInfo, 1000f, this.m_TileLayerMask)) {
				var cell = hitInfo.collider.GetComponent<CCell>();
				if (cell != null) {
					this.GenerateTree((int)cell.x, (int)cell.y, (int)cell.z);
				}
			}
		}
	}

	private void DetectCloud() {
		if (Input.GetMouseButtonDown(0)) {
			var raycast = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitInfo;
			if (Physics.Raycast(raycast, out hitInfo, 1000f, this.m_TileLayerMask)) {
				var cell = hitInfo.collider.GetComponent<CCell>();
				if (cell != null) {
					this.m_StartSelectPoint = new Vector3 (cell.x, cell.y, cell.z);
				}
			}
		}
		if (Input.GetMouseButton(0)) {
			// TODO
		}
		if (Input.GetMouseButtonUp(0)) {
			var raycast = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitInfo;
			if (Physics.Raycast(raycast, out hitInfo, 1000f, this.m_TileLayerMask)) {
				var cell = hitInfo.collider.GetComponent<CCell>();
				if (cell != null) {
					this.m_EndSelectPoint = new Vector3 (cell.x, cell.y, cell.z);
					this.GenerateCloudWith (this.m_StartSelectPoint, this.m_EndSelectPoint);
				}
			}
		}
	}

	public void InitMap() {
		this.m_MapName = string.Format("Map-{0}", System.DateTime.Now.ToString("dd-MM-yyyy hh-mm-ss"));
		this.m_CloudLists = new List<CCloud>();
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
		this.m_Grid = new int[this.m_Width, this.m_Height];
	}

	public void LoadMapDataWithAsset() {
		if (this.m_MapAsset == null)
			return;
		var mapAsset = this.m_MapAsset;
		this.m_MapFullInfo = CMapParser.parseMap (mapAsset.text);
		// GAME
		this.m_MapName = this.m_MapFullInfo.name;
		this.m_MapHard = this.m_MapFullInfo.hard;
		this.m_LimitRotate = this.m_MapFullInfo.limit;
		Color cameraBackground;
		if (ColorUtility.TryParseHtmlString(this.m_MapFullInfo.backgroundColor, out cameraBackground)) {
			Camera.main.backgroundColor = cameraBackground;
		}
		this.m_TileBorderFolder = this.m_MapFullInfo.tileBorderFolder;
		this.m_TileFolder = this.m_MapFullInfo.tileFolder;
		this.m_MarkerFolder = this.m_MapFullInfo.markerFolder;
		this.m_CloudFolder = this.m_MapFullInfo.cloudFolder;
		this.m_RainFolder = this.m_MapFullInfo.rainFolder;
		// GRID
		this.m_Width = this.m_MapFullInfo.map.GetLength(0);
		this.m_Height = this.m_MapFullInfo.map.GetLength(1);
		this.m_Grid = new int[this.m_Width, this.m_Height];
		System.Buffer.BlockCopy(this.m_MapFullInfo.map, 0, this.m_Grid, 0, this.m_MapFullInfo.map.Length * sizeof(int));
		// CLOUD
		this.m_CloudGroups = new CMapParser.CMapFullInfo.CCloudInfo[this.m_MapFullInfo.clouds.Length];
		System.Array.Copy(this.m_MapFullInfo.clouds, this.m_CloudGroups, this.m_MapFullInfo.clouds.Length);
		// GENARATE
        this.GenerateCloudGroup();
        this.GenerateMarker();
    }

	 public void GenerateCloudGroup() {
		int i;
		for (i = 0; i < this.m_CloudRoot.childCount; i++)
		{
			DestroyImmediate (this.m_CloudRoot.GetChild(i).gameObject);
			i--;
		}
        for (i = 0; i < this.m_CloudGroups.Length; i++)
        {
            var cloud = this.m_CloudGroups[i];
            this.GenerateCloud (
                cloud.index, 
                CMapParser.parseV3 (cloud.spot1),
                CMapParser.parseV3 (cloud.spot2),
                CMapParser.parseV3 (cloud.rotate),
                cloud.data
            );
        }
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

	public void GenerateMarker() {
		int i;
		for (i = 0; i < this.m_MarkerRoot.childCount; i++)
		{
			DestroyImmediate (this.m_MarkerRoot.GetChild(i).gameObject);
			i--;
		}
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

	public void GenerateMapCellObjects() {
		int x, y;
        this.m_GridMap = new CCell[this.m_Width, this.m_Height];
        for (y = 0; y < this.m_Grid.GetLength(1); y++) {
            for (x = 0; x < this.m_Grid.GetLength(0); x++) {
                if (   x == 0 || x == this.m_Grid.GetLength(0) - 1
                    || y == 0 || y == this.m_Grid.GetLength(1) - 1) {
                    this.m_GridMap[x, y] = this.GenerateCell(x, 0f, y, 
                        0, 
                        this.m_BorderPrefabs[x % this.m_BorderPrefabs.Length]);
                } else {
                    var value = this.m_Grid[x, y];
                    this.m_GridMap[x, y] = this.GenerateCell(x, 0f, y, 
                        value,
                        this.m_TilePrefabs[UnityEngine.Random.Range(0, this.m_TilePrefabs.Length)]);
                }
            }
        }
    }

	public CCell GenerateCell(float x, float y, float z, int value, CCell prefab) {
        var cell = Instantiate(prefab);
        cell.transform.SetParent(this.m_MapRoot);
        cell.transform.localPosition = new Vector3(x, y, z);
        cell.x = x;
        cell.y = y;
        cell.z = z;
        cell.value = value;
        cell.name = string.Format("Cell {0}:{1}:{2}", x, y, z);
		cell.gameObject.AddComponent<BoxCollider>();
        return cell;
    }

	public void GenerateTree(int x, int y, int z) {
		if (this.m_Grid[x, z] == 0) {
			var prefab = this.m_MarkerPrefabs[UnityEngine.Random.Range(0, this.m_MarkerPrefabs.Length)];
			var marker = Instantiate(prefab);
			marker.transform.SetParent(this.m_MarkerRoot);
			marker.transform.localPosition = new Vector3(x, y, z);
			marker.name = string.Format("Marker {0}:{1}:{2}", x, y, z);
			this.m_Grid[x, z] = 1;
		} else {
			var name = string.Format("Marker {0}:{1}:{2}", x, y, z);
			var marker = this.m_MarkerRoot.Find(name);
			if (marker != null) {
				DestroyImmediate(marker.gameObject);
				this.m_Grid[x, z] = 0;
			}
		}
    }

	public void GenerateCloudWith(Vector3 start, Vector3 end) {
		var minX = (int) Mathf.Min(start.x, end.x);
		var maxX = (int) Mathf.Max(start.x, end.x);
		var minZ = (int) Mathf.Min(start.z, end.z);
		var maxZ = (int) Mathf.Max(start.z, end.z);
		// GRID CLOUD
		var newWidth 	= maxX - minX + 1;
		var newDeep 	= maxZ - minZ + 1;
		var newHeight 	= Random.Range (newWidth, (int) ((newWidth + newDeep) / 2));
		var newGrid = new int[newWidth, newHeight, newDeep];
		var centerX = minX + Mathf.FloorToInt ((maxX - minX) / 2f);
		var centerZ = minZ + Mathf.FloorToInt ((maxZ - minZ)  / 2f);
		var spot = new Vector3(centerX, 0f, centerZ);
		// CLOUD
		int value, randomY, x, y, z;
		for (z = minZ; z <= maxZ; z++)
		{
			for (y = 0; y < 2; y++)
			{
				for (x = minX; x <= maxX; x++)
				{
					randomY = Random.Range(0, newHeight);
					value = this.m_Grid[maxX - (x - minX), maxZ - (z - minZ)];
					newGrid [
						x - minX, 
						randomY, 
						z - minZ
					] = value;
				}
			}
		}
		this.DeleteCloud(spot);
		this.GenerateCloud(0, spot, spot, newGrid);
	}

	public void GenerateCloud(int index, Vector3 spot, Vector3 move, int[,,] data) {
        var cloud = this.GenerateObject (spot.x, spot.y, spot.z, this.m_CloudRoot, this.m_CloudPrefabs[index]);
        cloud.name = string.Format("Cloud {0}:{1}:{2}", spot.x, spot.y, spot.z);
        cloud.spot = spot;
        cloud.move = move;
        cloud.index = index;
        cloud.GenerateCloud(data);
		this.m_CloudLists.Add (cloud);
    }

	public void DeleteCloud(Vector3 spot) {
		var name = string.Format("Cloud {0}:{1}:{2}", spot.x, spot.y, spot.z);
		var cloud = this.m_CloudRoot.Find(name);
		if (cloud != null) {
			var cloudCtr = cloud.GetComponent<CCloud>();
			if (cloudCtr != null) {
				this.m_CloudLists.Remove (cloudCtr);
			}
			DestroyImmediate (cloud.gameObject);
		}
	}

	public virtual void SwitchMode() {
		this.m_GenerateState = this.m_GenerateState == GENERATE_MARKER ? GENERATE_CLOUD : GENERATE_MARKER;
	}

	public T GenerateObject<T>(float x, float y, float z, Transform parent, T prefab) where T : MonoBehaviour {
        var cell = Instantiate(prefab);
        cell.transform.SetParent(parent);
        cell.transform.localPosition = new Vector3(x, y, z);
        return cell;
    }
	
}
