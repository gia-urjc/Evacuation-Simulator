using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class Door : MonoBehaviour
{
    private int ID;
	private List<Tile> tiles;
	private Section connectionA, connectionB;
	private int maxCapacity;
	private int currentCapacity;
    [SerializeField] private bool isStair;

    public void Setup(int id_, List<Tile> t_, Section s1_, Section s2_)
	{
        isStair = false; 
		tiles = new List<Tile>();
		ID = id_;
		tiles = t_;
		maxCapacity = tiles.Count;
		currentCapacity = maxCapacity;				
		name = "Door";

		connectionA = s1_; connectionB = s2_;
		connectionA.AddDoor(this); connectionB.AddDoor(this);

		foreach(Tile t in tiles)
		{
			t.AddDoor(this);
		}

		transform.position = tiles[0].transform.position;
	}
	
	public void ClearConnections()
	{
		connectionA = null; connectionB = null;
	}

	public void ExpandDoor(Tile t_)
	{
		t_.AddDoor(this);
		tiles.Add(t_);
		maxCapacity++;
		currentCapacity = maxCapacity;
	}

	public bool ReduceDoor(Tile t_)
	{
		//Delete t door? or it does so by itself
		tiles.Remove(t_);
		maxCapacity--;
		currentCapacity = maxCapacity;
		
		if(tiles.Count<=0)
		{
			connectionA.RemoveDoor(this);
			connectionB.RemoveDoor(this);
			return true;
		}
		return false;
	}

	public Section GetOtherConnection(Section s_)
	{ 
		if(s_ == connectionA)
		{
			return connectionB;
		}
		else if(s_ == connectionB)
		{
			return connectionA;
		}
		else return null;
	}

    public void Paint()
    {
        // Re-color room if it's Stair
        Color randomColor = Random.ColorHSV(0.3f, 0.7f, 0.3f, 0.5f, 0.5f, 0.8f);
        foreach (Tile t in tiles)
        {
            if (isStair) { t.GetRenderer().material.color = Color.blue; } //new Color32(255, 69, 69, 255); } //RedColor 
            else t.GetRenderer().material.color = randomColor;
        }
    }



    public int 			GetID(){ return ID;}
	public int 			GetMaxCapacity(){ return maxCapacity;}
	public int 			GetCurrentCapacity(){ return currentCapacity;}
	public Vector3 		GetPos(){return tiles[0].GetPos();}
	public Section[] 	GetConnections(){Section[] aux = new Section[2]; aux[0] = connectionA; aux[1] = connectionB; return aux;}
	public List<Tile> 	GetTiles(){return tiles;}
    public bool GetIsStair() { return isStair; }

    public void SetID(int ID_){ID = ID_;}
	public void SetMaxCapacity(int c_){maxCapacity = c_;}
	public void SetCurrentCapacity(int c_){currentCapacity = c_;}
    public void SetIsStair(bool st_) {
        isStair = st_;
        Paint();
        connectionA.AddDoorIsStair(this);
        connectionB.AddDoorIsStair(this);
    }

}
