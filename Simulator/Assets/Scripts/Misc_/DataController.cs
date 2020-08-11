using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class DataController
{
	private static string dataFormat = ".json"; /*dataProjectFilePath = "/SavedData/",*/
	private static string projectName;
	private static string dataMapFilename = "_topology", dataGraphFilename = "_graph", 
	dataPeopleFilename = "_people", dataPathsFilename = "_paths", dataInfoFilename = "_info";

	private static MapData mapData;
	private static PeopleData peopleData;
	private static GraphData graphData;
	private static PathsData pathsData;
	private static SceneInfo infoData;
    private static SceneInfoForTest infoForTest;

	private static T LoadData<T>(string file_)
	{
		//string filePath = Application.dataPath + dataProjectFilePath + projectName + file_ + dataFormat;
		string filePath = System.IO.Path.Combine(Application.dataPath, "SavedData");
		
		//string[] dir = Directory.GetDirectories(filePath);
		//for(int i=0; i<dir.Length; i++) Debug.Log(dir[i]);
		// https://docs.unity3d.com/Manual/LoadingResourcesatRuntime.html

		filePath = System.IO.Path.Combine(filePath, projectName);
		string fname = projectName + file_ + dataFormat;
		filePath = System.IO.Path.Combine(filePath, fname);

		if(File.Exists(filePath))
		{
			string dataAsJson = File.ReadAllText(filePath);
			return JsonUtility.FromJson<T>(dataAsJson);
		}
		else{ Debug.Log("File does not exist"); return default(T);}
	}
	public static List<string> GetFolders()
	{
		string filePath = System.IO.Path.Combine(Application.dataPath, "SavedData");
	
		char[] charArray;
		string aux;
		int index;
		string[] dir = Directory.GetDirectories(filePath);
		var result = new List<string>();

		for(int i=0; i<dir.Length; i++) 
		{
			aux = dir[i];

			charArray = aux.ToCharArray();
			System.Array.Reverse(charArray);
			aux = new string(charArray);
			
			index = aux.IndexOf("\\");
			if(index > 0) 
			{
				aux = aux.Substring(0,index);
			}

			charArray = aux.ToCharArray();
			System.Array.Reverse(charArray);
			aux = new string(charArray);

			result.Add(aux);
		}

		return result;
	}

	public static void SaveData<T>(string file_, T data_)
	{
		string dataAsJson = JsonUtility.ToJson(data_);
		Debug.Log(dataAsJson);
		//string filePath = Application.dataPath + dataProjectFilePath + projectName + "/" + projectName + file_ + dataFormat;
		
		string filePath = System.IO.Path.Combine(Application.dataPath, "SavedData");
		filePath = System.IO.Path.Combine(filePath, projectName);
		if(!Directory.Exists(filePath)) Directory.CreateDirectory (filePath);
		string fname = projectName + file_ + dataFormat;
		filePath = System.IO.Path.Combine(filePath, fname);

		File.WriteAllText(filePath, dataAsJson);
	}

    public static void SaveImage(Texture2D img, string filename)
    {
        byte[] _bytes = img.EncodeToPNG();
        string filePath = System.IO.Path.Combine(Application.dataPath, "SavedData");
        filePath = System.IO.Path.Combine(filePath, projectName);
        if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
        string fname = projectName + "_"+filename + ".png";
        filePath = System.IO.Path.Combine(filePath, fname);
        File.WriteAllBytes(filePath, _bytes);
    }

    #region all save-load
    public static void LoadMap()
	{
		mapData = LoadData<MapData>(dataMapFilename);
	}

    public static void SaveMap()
    {
        SaveData<MapData>(dataMapFilename, mapData);
    }

    public static void LoadGraph()
	{
		graphData = LoadData<GraphData>(dataGraphFilename);
	}

	public static void SaveGraph()
	{
		SaveData<GraphData>(dataGraphFilename, graphData);
	}

	public static void LoadPeople()
	{
		peopleData = LoadData<PeopleData>(dataPeopleFilename);
	}

	public static void SavePeople()
	{
		SaveData<PeopleData>(dataPeopleFilename, peopleData);
	}

	public static void LoadPaths()
	{
		pathsData = LoadData<PathsData>(dataPathsFilename);
	}

	public static void SavePaths()
	{
		SaveData<PathsData>(dataPathsFilename, pathsData);
	}

	public static void LoadInfo()
	{
		infoData = LoadData<SceneInfo>(dataInfoFilename);
	}

    public static void LoadInfoForTest()
    {
        infoData = LoadData<SceneInfo>(dataInfoFilename);
    }

    public static void SaveInfo()
	{
		SaveData<SceneInfo>(dataInfoFilename + "_" + System.DateTime.Now.ToString("MM.dd.yyyy_HH.mm.ss"), infoData);
	}

    public static void SaveInfoForTest()
    {
        SaveData<SceneInfoForTest>(dataInfoFilename + "_" + System.DateTime.Now.ToString("MM.dd.yyyy_HH.mm.ss"), infoForTest);
    }
    #endregion

    public static MapData GetMapData(){return mapData;}
	public static GraphData GetGraphData() {return graphData;}
	public static PeopleData GetPeopleData(){return peopleData;}
	public static PathsData GetPathsData(){return pathsData;}
	public static SceneInfo GetInfoData(){return infoData;}
	public static string GetProjectName(){return projectName;}

	public static void SetMapData(MapData md_){ mapData = md_;}
	public static void SetGraphData(GraphData gd_){ graphData = gd_;}
	public static void SetPeopleData(PeopleData pd_){ peopleData = pd_;}
	public static void SetPathsData(PathsData pd_){ pathsData = pd_;}
	public static void SetInfoData(SceneInfo id_){ infoData = id_;}
    public static void SetInfoDataForTest(SceneInfoForTest idt_) { infoForTest = idt_; }

	public static void SetProjectName(string n_){ projectName = n_;}

}

[System.Serializable]
public class SceneInfoForTest
{
    public List<SceneInfoBitForTest> bits;
    public SceneInfoForTest(List<SceneInfoBitForTest> bits_) { bits = bits_; }
}

[System.Serializable]
public class SceneInfoBitForTest
{
    public int n;
    public float t;
    public float mt;
    public SceneInfoBitForTest(int n_, float t_, float mt_) { n = n_; t = t_; mt = mt_; }
}

[System.Serializable]
public class MapData
{
	public int width, height;
	public bool builtSections;
	public List<Vector3> walls;
	public List<SectionData> sections;
	public List<DoorData> doors;
	
	public MapData(int width_, int height_, List<Vector3> w_, List<SectionData> s_, List<DoorData> d_, bool bs_)
	{
		width = width_; height = height_; walls = w_; sections = s_; doors = d_; builtSections = bs_;
	}
}

[System.Serializable]
public class SectionData
{
	public int ID, maxCapacity, currentCapacity;
	public bool isCP;
	public Vector3 pos;
	public List<Vector3> tiles;
	public List<Vector3> corners;

	public SectionData(int ID_, int maxCapacity_, int currentCapacity_, Vector3 pos_, bool isCP_, List<Vector3> tiles_, List<Vector3> corners_)
	{
		ID = ID_;
		maxCapacity = maxCapacity_;
		currentCapacity = currentCapacity_;
		pos = pos_;
		isCP = isCP_;
		tiles = tiles_;
		corners = corners_;
	}
}

[System.Serializable]
public class DoorData
{
	public int ID, IDconnA, IDconnB;
	public int maxCapacity, currentCapacity;
	public List<Vector3> pos;

	public DoorData(int ID_, int IDconnA_, int IDconnB_, List<Vector3> pos_, int mc_, int cc_)
	{
		ID = ID_; IDconnA = IDconnA_; IDconnB = IDconnB_; pos = pos_; maxCapacity = mc_; currentCapacity = cc_;
	}
}

[System.Serializable]
public class PeopleData
{
	public List<PersonData> people;
	public PeopleData(List<PersonData> people_)
	{
		people = people_;
	}
}

[System.Serializable]
public class PersonData
{

	public int ID, age, priority, familyID, tutorID;
	public List<string> types;
	public float speed;
	public string firstName;

	public bool dependent, manualDependencies;

	public int initSectionID;
	public int initNodeID;
	public Vector3 initPos;

	public PersonData(int ID_, int p_, int a_, List<string> t_, float sp_, string n_, 
	int INID_, int ISID_, Vector3 ip_, int fID_, bool d_, bool md_, int tID_)
	{
		ID = ID_; priority = p_; age = a_; types = t_; speed = sp_; firstName = n_; familyID = fID_;
		initNodeID = INID_; initSectionID = ISID_; initPos = ip_;
		dependent = d_; manualDependencies = md_; tutorID = tID_;
	}
}

[System.Serializable]
public class GraphData
{
	public List<NodeData> nodes; 
	public List<EdgeData> edges;

	public GraphData(List<NodeData> nodes_, List<EdgeData> edges_)
	{
		nodes = nodes_;
		edges = edges_;
	}
}

[System.Serializable]
public class NodeData
{
	public int ID, sectionID, maxCapacity, currentCapacity;
	public bool isCP;
	public List<int> adjacentEdgesIDs;
	public Vector3 pos;

	public NodeData(int ID_, int sID_, int mc_, int cc_, List<int> aeIDs_, Vector3 pos_, bool isCP_)
	{
		ID = ID_; sectionID = sID_; maxCapacity = mc_; currentCapacity = cc_; adjacentEdgesIDs = aeIDs_; pos = pos_; isCP = isCP_;
	}
}

[System.Serializable]
public class EdgeData
{
	public int ID, doorID, maxCapacity, currentCapacity;
	public float distance;
	public int[] nodes;
	
	public EdgeData(int ID_, int dID_, float d_, int[] nodes_, int mc_, int cc_)
	{
		ID = ID_; doorID = dID_; distance = d_; nodes = nodes_; maxCapacity = mc_; currentCapacity = cc_;
	}
}

[System.Serializable]
public class PathsData
{
	public List<PathData> paths;

	public PathsData(List<PathData> p_)
	{
		paths = p_;
	}
}

[System.Serializable]
public class PathData
{
	public int personID;
	public List<int> nodes;

	public PathData(int pid_, List<int> n_)
	{
		personID = pid_;
		nodes = n_;
	}
}