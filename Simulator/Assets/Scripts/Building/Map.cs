using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Map : MonoBehaviour, ISceneElement
{
	public GameObject tilePrefab, sectionPrefab, doorPrefab;
    public GameEvent floorReady, sectionsReady;

	private SceneController sc;

	// Building's dimensions
	private int width, height; // Number of tiles
	private float size; // Size of each tile

	private List<List<Tile>> tiles;
	[SerializeField] private List<Section> sections, cpSections;
    
    private List<Door> doors, doorsStair;
	int currentSectionID, currentDoorID;

    private Vector3 initClickedPos = Vector3.positiveInfinity, actualClickedPos = Vector3.positiveInfinity, endClickedPos = Vector3.positiveInfinity;

	// For the walls building
	private List<Tile> lastSelectedOutTiles = new List<Tile>();
	private List<Tile> lastSelectedInTiles = new List<Tile>();

	[SerializeField] private bool sectionsBuilt;

    private SubMenuStates state;

	public void Setup(SceneController sc_, int width_, int height_, float size_)
	{
		sc = sc_;

		width = width_;
		height = height_;
		size = size_;

		tiles = new List<List<Tile>>();
		sections = new List<Section>();
 		cpSections = new List<Section>();
        doorsStair = new List<Door>();
		doors = new List<Door>();

		sectionsBuilt = false;

		StartCoroutine(CreateFloor());
	}

	public void MouseEvent(Utils.MouseInputEvents mouseEvent)
	{
		switch(state)
		{
			case SubMenuStates.create: CreateMouseHandler(mouseEvent); break;
			case SubMenuStates.edit: EditMouseHandler(mouseEvent); break;
			default: break;
		}
	}

	private void CreateMouseHandler(Utils.MouseInputEvents mouseEvent)
	{
		KeyValuePair<Vector3,GameObject> info = Utils.ScreenToWorld();
		switch(mouseEvent)
	 	{
			case Utils.MouseInputEvents.left_down: 
				initClickedPos = info.Key;
			break;

			case Utils.MouseInputEvents.left_held: 
				if(!sectionsBuilt && Utils.ValidMousePos(initClickedPos))
				{
					initClickedPos = ToMapPos(initClickedPos);
					HighlightSelectedTiles();
				}
			break;

			case Utils.MouseInputEvents.left_up: CreateWallsOrDoor(); break;

			case Utils.MouseInputEvents.right_up: RightUpMouseEvent(info); break;

			default: break;
		}
	}

	private void EditMouseHandler(Utils.MouseInputEvents mouseEvent)
	{
		KeyValuePair<Vector3,GameObject> info = Utils.ScreenToWorld();
		if(info.Value != null)
		{
			switch(mouseEvent)
			{
				case Utils.MouseInputEvents.left_up: 
					if(info.Value.tag == "tile" && sectionsBuilt)
					{
						sc.EnableEditMenu(info.Value);
					}
					else if(info.Value.tag == "door")
					{
						Vector3 mappos = ToMapPos(info.Key);
						sc.EnableEditMenu(tiles[(int)mappos.x][(int)mappos.z].GetDoor().gameObject);
					}
				break;

				default: break;
			}
		}
	}


	public void KeyboardEvent()
	{
	}

	// Local to world coordinates and vice-versa
	public Vector3 ToWorldPos(Vector3 mapPos_){	return new Vector3(mapPos_.x*size, 0, mapPos_.z*size);}
	public Vector3 ToMapPos(Vector3 worldPos_){ return new Vector3(Mathf.RoundToInt(worldPos_.x/size), 0, Mathf.RoundToInt(worldPos_.z/size));}
    
    private IEnumerator CreateFloor()
    {
        // Create the tiles of the floor

        Tile newTile = null;
        Vector3 mapPos;
        float index = 0;
        int percent = 0;
        int total = width * height;

        for (int i = 0; i < width; i++)
        {
            tiles.Add(new List<Tile>());
            for (int j = 0; j < height; j++)
            {
                mapPos = new Vector3(i, 0, j);

                newTile = GameObject.Instantiate(tilePrefab, ToWorldPos(mapPos), this.transform.rotation, this.transform).GetComponent<Tile>();
                newTile.gameObject.name = "Tile";
                newTile.SetPos(mapPos);

                tiles[i].Add(newTile);

                index++;
                
            }
            if (i % 2 != 0)
            {
                percent = (int)index * 100 / total;
                sc.SetFeedback("Creating floor... " + percent + "%");
                yield return null;
            }
        }
        
        floorReady.Raise();
    }

	private void HighlightSelectedTiles()
	{
		actualClickedPos = Utils.ScreenToWorld().Key;

		if(Utils.ValidMousePos(actualClickedPos))
		{
			foreach(Tile to in lastSelectedOutTiles)
			{
				to.DeHighlight();
			}
			foreach(Tile ti in lastSelectedInTiles)
			{
				ti.DeHighlight();
			}
			lastSelectedOutTiles.Clear();
			lastSelectedInTiles.Clear();
			
			endClickedPos = ToMapPos(actualClickedPos);
			
			int minX, minY, maxX, maxY;
			minX = Mathf.RoundToInt(Mathf.Min(initClickedPos.x, endClickedPos.x));
			maxX = Mathf.RoundToInt(Mathf.Max(initClickedPos.x, endClickedPos.x));
			minY = Mathf.RoundToInt(Mathf.Min(initClickedPos.z, endClickedPos.z));
			maxY = Mathf.RoundToInt(Mathf.Max(initClickedPos.z, endClickedPos.z));
			
			for(int i=minX; i<=maxX; i++)
			{
				for(int j=minY; j<=maxY; j++)
				{
					// Exterior
					if(i==minX || j==minY || i==maxX || j==maxY)
					{
						tiles[i][j].Highlight(0);
						lastSelectedOutTiles.Add(tiles[i][j]);
					}
					// Interior
					else
					{
						tiles[i][j].Highlight(1);
						lastSelectedInTiles.Add(tiles[i][j]);
					}
				}
			}
		}
	}

	private void CreateWallsOrDoor()
	{
        List<Tile> recentlyAddedWalls = new List<Tile>();
		GameObject go = Utils.ScreenToWorld().Value;
		if(go!=null && (lastSelectedOutTiles.Count>0 || sectionsBuilt))
		{
			
			if(sectionsBuilt && go.tag == "wall"/* && lastSelectedOutTiles.Count==1*/)
			{
				Tile t = go.transform.parent.GetComponent<Tile>();
				t.DeHighlight();
				CheckIfDoorCanBeAdded(t);
			}
			else if(!sectionsBuilt)
			{
				foreach(Tile to in lastSelectedOutTiles)
				{
					to.DeHighlight();
					if(to.HasDoor()) doors.Remove(to.GetDoor());
                    //aqui
					to.AddWall();
                    recentlyAddedWalls.Add(to);
				}
				foreach(Tile ti in lastSelectedInTiles)
				{
					ti.DeHighlight();
					if(ti.HasDoor()){doors.Remove(ti.GetDoor()); ti.DestroyDoor();}
					if(ti.HasWall())ti.DestroyWall();
				}
			}


            Tile nTile, sTile, eTile, wTile;
            int nx, nz, sx, sz, ex, ez, wx, wz;
            bool change = true;
            foreach (Tile t in recentlyAddedWalls)
            {
                change = true;
                nx = (int)t.GetPos().x; nz = (int)t.GetPos().z + 1; if (nz >= height) change = false;
                sx = (int)t.GetPos().x; sz = (int)t.GetPos().z - 1; if (sz < 0) change = false;
                wx = (int)t.GetPos().x - 1; wz = (int)t.GetPos().z; if (wx < 0) change = false;
                ex = (int)t.GetPos().x + 1; ez = (int)t.GetPos().z; if (ex >= width) change = false;

                if(change)
                {
                    nTile = tiles[nx][nz];
                    sTile = tiles[sx][sz];
                    wTile = tiles[wx][wz];
                    eTile = tiles[ex][ez];

                    t.SetN(nTile.HasWall() || nTile.HasDoor());
                    t.SetS(sTile.HasWall() || sTile.HasDoor());
                    t.SetE(eTile.HasWall() || eTile.HasDoor());
                    t.SetW(wTile.HasWall() || wTile.HasDoor());

                    t.UpdateWallPrefab();

                    nTile.SetS(true); sTile.SetN(true); eTile.SetW(true); wTile.SetE(true);
                    nTile.UpdateWallPrefab(); sTile.UpdateWallPrefab(); eTile.UpdateWallPrefab(); wTile.UpdateWallPrefab();
                }
               
            }
        }

        recentlyAddedWalls = new List<Tile>();
	}
    
    private void RightUpMouseEvent(KeyValuePair<Vector3, GameObject> info)
    {
        if (info.Value != null)
        {
            int nx, nz, sx, sz, ex, ez, wx, wz;
            Vector3 pos = ToMapPos(info.Key);
            if (!sectionsBuilt && info.Value.tag == "wall")
            {
                Tile t = tiles[(int)pos.x][(int)pos.z];
                t.DestroyWall();

                nx = (int)t.GetPos().x; nz = (int)t.GetPos().z + 1;
                if (nz < height) { tiles[nx][nz].SetS(false); tiles[nx][nz].UpdateWallPrefab(); }
                sx = (int)t.GetPos().x; sz = (int)t.GetPos().z - 1;
                if (sz >= 0) { tiles[sx][sz].SetN(false); tiles[sx][sz].UpdateWallPrefab(); }
                wx = (int)t.GetPos().x - 1; wz = (int)t.GetPos().z;
                if (wx >= 0) { tiles[wx][wz].SetE(false); tiles[wx][wz].UpdateWallPrefab(); }
                ex = (int)t.GetPos().x + 1; ez = (int)t.GetPos().z;
                if (ex < width) { tiles[ex][ez].SetW(false); tiles[ex][ez].UpdateWallPrefab(); }
            }
            else if (sectionsBuilt && info.Value.tag == "door")
            {
                Tile remT = tiles[(int)pos.x][(int)pos.z];
                Door remD = remT.GetDoor();
                if (remD.ReduceDoor(remT))
                {
                    doors.Remove(remD);
                    Destroy(remD.gameObject);
                }
                remT.DestroyDoor();
            }
        }
    }

	
	private bool CheckIfDoorCanBeAdded(Tile t_)
	{
		Vector3 tilePos = t_.GetPos();

		Vector3 eastPos = tilePos + Vector3.right;
		Vector3 northPos = tilePos + Vector3.forward;
		Vector3 westPos = tilePos + Vector3.left;
		Vector3 southPos = tilePos + Vector3.back;
		List<Vector3> poses = new List<Vector3>{eastPos, northPos, westPos, southPos};
		foreach(Vector3 p in poses)
		{
			if(p.x>=width || p.x<0 || p.z>=height || p.z<0) return false;
		}

		Tile eastTile = tiles[(int)eastPos.x][(int)eastPos.z];
		Tile northTile = tiles[(int)northPos.x][(int)northPos.z];
		Tile westTile = tiles[(int)westPos.x][(int)westPos.z];
		Tile southTile = tiles[(int)southPos.x][(int)southPos.z];
		
		Section s1, s2;

		if((eastTile.HasWall() || eastTile.HasDoor()) && (westTile.HasWall() || westTile.HasDoor()))
		{	
			s1 = northTile.GetSection();
			s2 = southTile.GetSection();
			if(s1 != null && s2 != null & s1 != s2)
			{
				if(eastTile.HasDoor())
				{
					eastTile.GetDoor().ExpandDoor(t_);
				}
				else if(westTile.HasDoor())
				{
					westTile.GetDoor().ExpandDoor(t_);
				}
				else
				{
					if(!CheckIfThereIsConexion(s1, s2))
					{
						Door d = GameObject.Instantiate(doorPrefab, this.transform).GetComponent<Door>();
						d.Setup(currentDoorID++, new List<Tile>(){t_}, s1, s2);
						if(d!=null) doors.Add(d);
					}
				}
				return true;
			}
		}
		else if((northTile.HasWall() || northTile.HasDoor()) && (southTile.HasWall() || southTile.HasDoor()))
		{
			s1 = eastTile.GetSection();
			s2 = westTile.GetSection();
			if(s1 != null && s2 != null & s1 != s2)
			{
				if(northTile.HasDoor())
				{
					northTile.GetDoor().ExpandDoor(t_);
				}
				else if(southTile.HasDoor())
				{
					southTile.GetDoor().ExpandDoor(t_);
				}
				else
				{
					if(!CheckIfThereIsConexion(s1, s2))
					{
						Door d = GameObject.Instantiate(doorPrefab, this.transform).GetComponent<Door>();
						d.Setup(currentDoorID++, new List<Tile>(){t_}, s1, s2);
						if(d!=null) doors.Add(d);
					}
				}
				return true;
			}
		}
		s1 = null; s2 = null;
		return false;
	}

	private bool CheckIfThereIsConexion(Section s1_, Section s2_)
	{
		foreach(Door d in s1_.GetDoors())
		{
			if(d.GetOtherConnection(s1_) == s2_)
			{
				return true;
			}
		}
		return false;
	}

	public void BuildNavMesh()
	{
		GetComponent<NavMeshSurface>().BuildNavMesh();
    }

	public IEnumerator BuildSections()
	{
		if(!sectionsBuilt)
		{

            float index = 0;
            int percent = 0;
            int total = width * height;

            Section currentSection = null;
			
			for(int i=0; i<width; i++)
			{
				for(int j=0; j<height; j++)
				{
					if(!tiles[i][j].HasWall() && !tiles[i][j].HasDoor() && tiles[i][j].GetSection()==null)
					{
						currentSection = GameObject.Instantiate(sectionPrefab, this.transform).GetComponent<Section>();
						currentSection.Setup(this, currentSectionID);
						currentSection.min = tiles[i][j].GetPos();
						currentSection.max = currentSection.min;
						
						sections.Add(currentSection);
						
						currentSectionID++;
						ExpandSectionSearch(i, j, currentSection);
						currentSection.Paint();
						currentSection.CalculatePosition();
						//currentSection.PrintCorners();

						//sc.GetSceneInfo().numberOfSections += 1;
                        
					}
                    index++;
                }
                if (i % 2 != 0)
                {
                    percent = (int)index * 100 / total;
                    sc.SetFeedback("Creating sections... " + percent + "%");
                    yield return null;
                }
            }
			sectionsBuilt = true;
            sectionsReady.Raise();
		}
	}
	
	//  queue
	public void ExpandSectionSearch(int i_, int j_, Section s_)
	{
		tiles[i_][j_].SetSection(s_);
		s_.AddTile(tiles[i_][j_]);
		
		Vector3 tilePos = tiles[i_][j_].GetPos();
		if(tilePos.x>s_.max.x || tilePos.z>s_.max.z) s_.max = tilePos;
		
		List<Tile> tileCorners = new List<Tile>();
		Vector3 cornerPos;
		int[] directions = {1,0, 0,1, -1,0, 0,-1};
		int idx, i, j;
		Tile t = null;
		
		for(idx = 0; idx<directions.Length; idx=idx+2)
		{
			i = i_+directions[idx];
			j = j_+directions[idx+1];
			
			if(i>=0 && j>=0 && i<width && j<height){
				t = tiles[i][j];
				tileCorners.Add(t);
				if(t.GetSection()==null && !t.HasWall() && !t.HasDoor())
				{
					ExpandSectionSearch(i, j, s_);
				}
			}
		}

		// Save section corners
		if(tileCorners.Count>3)
		{
			int next;
			for(int a=0; a<tileCorners.Count; a++)
			{
				next = (a+1)%tileCorners.Count;
				if(a == tileCorners.Count-1) next = 0;
				if(tileCorners[a].HasWall() == tileCorners[next].HasWall())
				{
					cornerPos = tilePos + (tileCorners[a].GetPos() - tilePos) + (tileCorners[next].GetPos() - tilePos);
					s_.AddCorner(cornerPos);
				}
			}
		}
	}

	public void ToggleSectionCP(Section s_)
	{
		//s_.SetIsCP(!s_.GetIsCP());
		if(s_.GetIsCP()){ if(!cpSections.Contains(s_)){ cpSections.Add(s_); /*sc.GetSceneInfo().numberOfCPSections += 1;*/ } }
		else {cpSections.Remove(s_); /*sc.GetSceneInfo().numberOfCPSections -= 1;*/}
	}

    public void ToggleDoorStair(Door d_)
    {
        if (d_.GetIsStair()) { if (!doorsStair.Contains(d_)) { doorsStair.Add(d_); /*sc.GetSceneInfo().numberOfCPSections += 1;*/ } }
        else { doorsStair.Remove(d_); /*sc.GetSceneInfo().numberOfCPSections -= 1;*/}
    }

    public Section GetRandomCP()
	{
		int total = cpSections.Count;
		if(total > 0)
		{
			int rnd = Mathf.RoundToInt(Random.Range(0,total));
			return cpSections[rnd];
		}
		else Debug.Log("ERROR. Not a single CP"); return null;
	}

	public Section IsPosSection(Vector3 pos_)
	{
		if(pos_.x != Mathf.Infinity) return tiles[(int)pos_.x][(int)pos_.z].GetSection();
		else return null;
	}

	public Section FindSectionByID(int id_)
	{
		return sections.Find( x => x.GetID()==id_);
	}

	public Door FindDoorByID(int id_)
	{
		return doors.Find( x => x.GetID()==id_);
	}

	public void ResetSimulation()
	{
		foreach(Section s_ in cpSections)
		{
			s_.ResetSimulation();
		}
	}

	#region ISceneElement
    public void ChangeState(SubMenuStates state_)
    {
        state = state_;
    }
	#endregion

	#region deleting stuff
	public void DeleteAll()
	{
		DeleteSections();
		DeleteDoors();
		DeleteWalls();

		for(int i=0; i<width; i++)
		{
			for(int j=0; j<height; j++)
			{
				Destroy(tiles[i][j].gameObject);
			}
		}
		RestartAllVars();
	}
	
	public void DeleteAllConstructions()
	{
		DeleteDoors();
		DeleteSections();
		DeleteWalls();
		sectionsBuilt = false;
	}

	private void DeleteSections()
	{
		for(int i=0; i<width; i++)
		{
			for(int j=0; j<height; j++)
			{
				tiles[i][j].SetSection(null);
				tiles[i][j].DeHighlight();
			}
		}
		foreach(Door d in doors)
		{
			d.ClearConnections();
		}
		
		cpSections.Clear();
        doorsStair.Clear();


        foreach (Section s in sections)
		{
			Destroy(s.gameObject);
		}
		sections.Clear();

		currentSectionID = 0;
	}

	public void DeleteWalls()
	{
		for(int i=0; i<width; i++)
		{
			for(int j=0; j<height; j++)
			{
				tiles[i][j].DestroyWall();
			}
		}
	}
	
	public void DeleteDoors()
	{
		foreach(Section s in sections)
		{
			s.GetDoors().Clear();
		}
		foreach(Door d in doors)
		{
			Destroy(d.gameObject);
		}
		doors.Clear();
		for(int i=0; i<width; i++)
		{
			for(int j=0; j<height; j++)
			{
				tiles[i][j].DestroyDoor();
			}
		}

		currentDoorID = 0;
	}

	private void RestartAllVars()
	{
		tiles.Clear();

		initClickedPos = Vector3.positiveInfinity; 
		actualClickedPos = Vector3.positiveInfinity;
		endClickedPos = Vector3.positiveInfinity;
		
		lastSelectedOutTiles.Clear();
		lastSelectedInTiles.Clear();

		sectionsBuilt = false;
	}

	#endregion

	#region loading stuff
	public void Load()
	{
		MapData md_ = DataController.GetMapData();

		// Dimens
		width = md_.width*2;
		height = md_.height*2;

        // Tiles
        //CreateFloor();

        // Walls
        List<Tile> allTilesWalls = new List<Tile>();
        Tile wallTile;
		foreach(Vector3 wall in md_.walls)
		{
            //aqui
            wallTile = tiles[(int)wall.x][(int)wall.z];
            allTilesWalls.Add(wallTile);
            wallTile.AddWall();
		}
        Tile nTile, sTile, eTile, wTile;
        int nx, nz, sx, sz, ex, ez, wx, wz;
        bool change = true;
        foreach (Tile t in allTilesWalls)
        {
            change = true;
            nx = (int)t.GetPos().x; nz = (int)t.GetPos().z + 1; if (nz >= height) change = false;
            sx = (int)t.GetPos().x; sz = (int)t.GetPos().z - 1; if (sz < 0) change = false;
            wx = (int)t.GetPos().x - 1; wz = (int)t.GetPos().z; if (wx < 0) change = false;
            ex = (int)t.GetPos().x + 1; ez = (int)t.GetPos().z; if (ex >= width) change = false;

            if (change)
            {
                nTile = tiles[nx][nz];
                sTile = tiles[sx][sz];
                wTile = tiles[wx][wz];
                eTile = tiles[ex][ez];

                t.SetN(nTile.HasWall() || nTile.HasDoor());
                t.SetS(sTile.HasWall() || sTile.HasDoor());
                t.SetE(eTile.HasWall() || eTile.HasDoor());
                t.SetW(wTile.HasWall() || wTile.HasDoor());

                t.UpdateWallPrefab();

                nTile.SetS(true); sTile.SetN(true); eTile.SetW(true); wTile.SetE(true);
                nTile.UpdateWallPrefab(); sTile.UpdateWallPrefab(); eTile.UpdateWallPrefab(); wTile.UpdateWallPrefab();
            }
        }

		// Sections
		Section newSection;
		List<Tile> sectionTiles = new List<Tile>();
		Tile sectionTile;
		int maxID = -1;

		//Vector3 diff;
		//int next;
		foreach(SectionData sd in md_.sections)
		{
			newSection = GameObject.Instantiate(sectionPrefab, this.transform).GetComponent<Section>();
			foreach(Vector3 t in sd.tiles)
			{
				sectionTile = tiles[(int)t.x][(int)t.z];
				sectionTile.SetSection(newSection);
				sectionTiles.Add(sectionTile);
			}
			if(sd.ID>maxID) maxID = sd.ID;
			newSection.Setup(this, sd.ID, sd.maxCapacity, sd.currentCapacity, sd.pos, sd.isCP, sectionTiles, sd.corners);
			sections.Add(newSection);
			if(newSection.GetIsCP()) cpSections.Add(newSection);
			newSection.Paint();

			sectionTiles = new List<Tile>();

			// Add section walls
			// for(int a=0; a<sd.corners.Count; a++)
			// {
			// 	next = (a+1)%sd.corners.Count;
			// 	diff = sd.corners[next] - sd.corners[a];
			// 	Debug.Log(diff);
			// 	BuildMultipleWalls(sd.corners[a], diff);
			// }
		}
		currentSectionID = maxID+1;
		maxID = -1;

		// Doors
		Door newDoor;
		List<Tile> doortiles = new List<Tile>();
		foreach(DoorData dd in md_.doors)
		{
			if(dd.ID>maxID) maxID = dd.ID;
			foreach(Vector3 doorpos in dd.pos)
			{
				doortiles.Add(tiles[(int)doorpos.x][(int)doorpos.z]);
			}
			newDoor = GameObject.Instantiate(doorPrefab, this.transform).GetComponent<Door>();
			newDoor.Setup(dd.ID, doortiles, FindSectionByID(dd.IDconnA), FindSectionByID(dd.IDconnB));
			doors.Add(newDoor);
		}
		currentDoorID = maxID+1;

		sectionsBuilt = md_.builtSections;

		//BuildNavMesh();
	}

	public void Save()
	{
        // Walls
        Texture2D texture = new Texture2D(width*sc.res, height*sc.res);
        Color color;
        List<Vector3> wallsPositions = new List<Vector3>();
        
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (tiles[i][j].HasWall()) { wallsPositions.Add(new Vector3(i, 0, j));}
            }
        }

        for (int i = 0; i < width; i++)
        {
            for (int a = 0; a < sc.res; a++)
            {
                for (int j = 0; j < height; j++)
                {
                    for (int b = 0; b < sc.res; b++)
                    {
                        if (tiles[i][j].HasWall()) { color = Color.black; }
                        else if (tiles[i][j].HasDoor()) { color = Color.gray; }
                        else color = Color.white;
                        texture.SetPixel(i * sc.res + a, j * sc.res + b, color);
                    }
                }
            }
        }

        texture.Apply();

		// Sections
		List<SectionData> sectionsData = new List<SectionData>();
		List<Vector3> sectionTiles = new List<Vector3>();
		foreach(Section s in sections)
		{
			foreach(Tile t in s.GetTiles())
			{
				sectionTiles.Add(t.GetPos());
			}
			sectionsData.Add(new SectionData(s.GetID(), s.GetMaxCapacity(), s.GetCurrentCapacity(), s.GetPos(), s.GetIsCP(), sectionTiles, s.GetCorners()));
			sectionTiles = new List<Vector3>();
		}

		// Doors
		List<DoorData> doorsData = new List<DoorData>();
		Section[] doorSections;
		List<Vector3> positions = new List<Vector3>();
		int connA, connB;
		foreach(Door d in doors)
		{
			doorSections = d.GetConnections();
			connA = doorSections[0].GetID();
			connB = doorSections[1].GetID();
			foreach(Tile doortile in d.GetTiles())
			{
				positions.Add(doortile.GetPos());
			}
			doorsData.Add(new DoorData(d.GetID(), connA, connB, positions, d.GetMaxCapacity(), d.GetCurrentCapacity()));
			positions = new List<Vector3>();
		}

		// Save all
		DataController.SetMapData(new MapData(width/2, height/2, wallsPositions, sectionsData, doorsData, sectionsBuilt));
        DataController.SaveImage(texture, "geometryMap");

    }
    #endregion
    

    public List<SectionInfo> GetSectionInfos()
    {
        List<SectionInfo> sectionInfos = new List<SectionInfo>();
        foreach (Section s in sections)
        {
            sectionInfos.Add(new SectionInfo(s.GetID(), s.GetIsCP(), s.GetTimer(), s.GetWorstLOS(), s.GetMediaLOS()));
        }
        return sectionInfos;
    }

    public List<Section> GetSections() { return sections;}
    public List<Section> GetCPSections() { return cpSections; }
    public List<Door> GetDoorsStair() { return doorsStair; }
    public List<Door> GetDoors(){ return doors;}
	public int GetWidth(){return width;}
	public int GetHeight(){return height;}
	public bool AreSectionsBuilt(){ return sectionsBuilt;}

	public void SetWidth(int w_){width = w_;}
	public void SetHeight(int h_){ height = h_;}
}