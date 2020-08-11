using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class Section: MonoBehaviour
{
	//private SceneController sc;
	private Map map;
	
	private int ID;
	private float area;
	public Vector3 pos;
	private List<Tile> tiles;
	[SerializeField] private bool isCP;
	[SerializeField] private int maxCapacity;
	[SerializeField] private int currentCapacity, currentOcupancy;
	private List<Vector3> corners;
    private Color myRedColor;

    public TMP_Text myText;

    private List<PersonBehavior> peopleAssigned = new List<PersonBehavior>();
	
	private List<Door> doors;
	
	public Vector3 min, max;

	[SerializeField] private float timer;
	private int peopleArrivedCounter;
	
	public void Setup(Map map_, int ID_)
	{
		isCP = false;
		maxCapacity = 0;
		tiles = new List<Tile>();
		doors = new List<Door>();
		//peopleAssigned = new List<PersonBehavior>();
		corners = new List<Vector3>();
		
		map = map_;
		//sc = sc_;
		ID = ID_;

        area = maxCapacity * .25f;
        name = "Section"+ID;
        myText.text = "S" + ID;
        myRedColor = new Color32(255, 69, 69, 255);
    }

	public void Setup(Map map_, int ID_, int maxCapacity_, int currentCapacity_, Vector3 pos_, bool isCP_, List<Tile> tiles_, List<Vector3> corners_)
	{
		doors = new List<Door>();
		corners = new List<Vector3>();

		map = map_;
		ID = ID_;
		maxCapacity = maxCapacity_;
		currentCapacity = currentCapacity_;
		pos = pos_;
		transform.position = pos;
		isCP = isCP_;
		tiles = tiles_;
		corners = corners_;

		area = maxCapacity * .25f;

		name = "Section"+ID;
        myText.text = "S" + ID;
        myRedColor = new Color32(255, 69, 69, 255);
    }

	public void PersonArrived(int ID_, float timer_)
	{
		//Utils.Print("P"+ID_+" is here!");
        peopleArrivedCounter++;
        if(peopleArrivedCounter>=peopleAssigned.Count)
		{ 
			Utils.Print("Everyone has arrived to section "+ID+" in "+timer_+" seconds!");
			timer = timer_;
		}
	}
	
	public void Paint()
	{
		// Re-color room if it's CP
		Color randomColor = Random.ColorHSV(0.3f, 0.7f, 0.3f, 0.5f, 0.5f, 0.8f);
		foreach(Tile t in tiles)
		{
			if(isCP){ t.GetRenderer().material.color = myRedColor; }
			else t.GetRenderer().material.color = randomColor;
		}
	}
	
	public void CalculatePosition()
	{
		// Center of rectangle
		//if(min!=max){pos = min + new Vector3(Mathf.Round((max.x-min.x)/2), min.y, Mathf.Round((max.z-min.z)/2));}
		//else{pos = max;}

		// Mid tile
		int c = (tiles.Count-1)/2;
		Tile midTile = tiles[c];
		pos = midTile.GetPos();
        transform.position = pos;

		area = maxCapacity * .25f;
	}
	
	public Door GetConnection(Section s_)
	{
		Section[] doorConnections;
		foreach(Door d in doors)
		{
			doorConnections = d.GetConnections();
			if(doorConnections[0]==s_ || doorConnections[1]==s_) return d;
		}
		return null;
	}

	public void ResetSimulation()
	{
        peopleAssigned = new List<PersonBehavior>();
		peopleArrivedCounter = 0;
		timer = 0f;


        ResetTick();
        totalDensity = 0; worstDensity = 0; mediaDensity = 0; tickCounter = 0; worstLOS = ""; mediaLOS = "";
	}


    private float densityInTick, totalDensity, worstDensity, mediaDensity;
    private int peopleInTick, tickCounter;
    private string worstLOS, mediaLOS;

    public void ResetTick() { densityInTick = 0; peopleInTick = 0;}
    public void AddPerson() { peopleInTick++; }
    public void CalculateDensity()
    {
        densityInTick = peopleInTick / area;
        tickCounter++;
        if (densityInTick > worstDensity) worstDensity = densityInTick;
        totalDensity += densityInTick;
    }
    public string CalculateLOS(float density)
    {
        string LOS = "";
        if (density >= 1.66f) LOS = "F";
        else if (density >= 0.69f) LOS = "E";
        else if (density >= 0.45f) LOS = "D";
        else if (density >= 0.27f) LOS = "C";
        else if (density >= 0.08f) LOS = "B";
        else if (density >= 0f) LOS = "A";

        return LOS;
    }
    public void GetFinalResults()
    {
        mediaDensity = totalDensity / tickCounter;
        worstLOS = CalculateLOS(worstDensity);
        mediaLOS = CalculateLOS(mediaDensity);
        //Debug.Log(worstLOS);
    }
    public string GetWorstLOS() { return worstLOS; }
    public string GetMediaLOS() { return mediaLOS; }


    public void AddTile(Tile t){ tiles.Add(t); maxCapacity++; currentCapacity++; area = maxCapacity * .25f; }
	public void RemoveTile(Tile t){ tiles.Remove(t); maxCapacity--; currentCapacity--; area = maxCapacity * .25f; }
	public void AddDoor(Door d){doors.Add(d);}
	public void RemoveDoor(Door d_){doors.Remove(d_);}
	public void AddCorner(Vector3 c_){corners.Add(c_);}
	public void PrintCorners()
	{
		string r = "Section"+ID+": ";
		foreach(Vector3 c in corners)
		{
			r+=c+" || ";
		}
		Debug.Log(r);
	}
	
	public bool AddPerson(PersonBehavior p){ peopleAssigned.Add(p); return peopleAssigned.Count>currentCapacity;}
	//public void RemovePerson(PersonBehavior p){ peopleAssigned.Remove(p);}
	//public bool IsFull(){return peopleAssigned.Count>=currentCapacity;}
	
	
	#region GETTERS & SETTERS
	
	public int 				GetID(){return ID;}
	public Vector3			GetPos(){return pos;}
	public bool 			GetIsCP(){return isCP;}
	public int 				GetMaxCapacity(){return maxCapacity;}
	public int 				GetCurrentCapacity(){return currentCapacity;}
	public List<Tile> 		GetTiles(){return tiles;}
	public List<Door> 		GetDoors(){return doors;}
	public List<Vector3> 	GetCorners(){return corners;}
	public float 			GetTimer(){ return timer;}
	
	public void 		SetIsCP(bool cp_){isCP = cp_; Paint();map?.ToggleSectionCP(this); }
	public void 		SetPos(Vector3 pos_){pos = pos_;}
	public void 		SetCurrentCapacity(int currentCapacity_){currentCapacity = currentCapacity_;}
	
	#endregion
	
	public override string ToString(){ return "Section"+ID;}
}
