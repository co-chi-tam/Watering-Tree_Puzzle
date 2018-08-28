using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinyJSON;
public class CMapParser {

	public static CMapDisplay parseMapDisplay(string value) {
		var display = JSON.Load(value).Make<CMapDisplay>();
		return display;
	}

	public static CMapFullInfo parseMap(string value) {
		var map = JSON.Load(value).Make<CMapFullInfo>();
		return map;
	}

	public static Vector3 parseV3(string value) {
		var split = value.Split(',');
		return new Vector3 (
			float.Parse(split[0].ToString()),
			float.Parse(split[1].ToString()),
			float.Parse(split[2].ToString())
		);
	}

	public class LevelComparer : IComparer<TextAsset>
    {
        public int Compare(TextAsset a, TextAsset b)
        {
            string arr1 = a.name.Replace("Level", string.Empty);
            string arr2 = b.name.Replace("Level", string.Empty);
            int int1 = int.Parse(arr1);
            int int2 = int.Parse(arr2);
            return int1.CompareTo(int2);
        }
    }

	public class CMapDisplay {
		public string name;
		public float hard;
		public int limit;
		public string backgroundColor = "#5396FF";
		public string tileBorderFolder = "Tiles/Plain/Borders/";
		public string tileFolder = "Tiles/Plain/Tiles/";
		public string markerFolder = "Markers/Plain/";
		public string cloudFolder = "Clouds/Group/";
		public string rainFolder = "Rains/Water/";
	}

	public class CMapFullInfo: CMapDisplay {
		public int[,] map;
		public CCloudInfo[] clouds;
		public class CCloudInfo {
			public int index;
			public string spot1;
			public string spot2;
			public string rotate;
			public int[,,] data;
		}
	}
	
}
