using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class Tile : MonoBehaviour
{
    public GameObject wallPrefab, doorPrefab, slimWallPrefab, rotSlimWallPrefab;

    private GameObject wallGO, doorGO, lastWallPrefab;
    private Door door;

    private Vector3 pos;
    private bool hasWall = false, hasDoor = false;
    private Section section = null;

    private Renderer myRenderer;
    private Color originalColor;
    public Color myRedColor;

    private bool n, s, e, w;

    void Awake()
    {
        myRenderer = this.gameObject.GetComponent<Renderer>();
        originalColor = myRenderer.material.color;
        lastWallPrefab = wallPrefab;
        myRedColor = new Color32(255, 69, 69, 255);
    }

    public void AddWall()
    {
        // Create a wall on the tile if there's no wall already
        // also if there's a door, deactivate it

        if (!hasWall)
        {
            hasWall = true;
            if (wallGO) Destroy(wallGO);
            wallGO = GameObject.Instantiate(lastWallPrefab, this.transform.position + new Vector3(0f, 1.5f, 0f), this.transform.rotation, this.transform);
            wallGO.name = "Wall";

            if (hasDoor)
            {
                hasDoor = false;
                door.ReduceDoor(this);
                door = null;
                Destroy(doorGO);
            }
        }
    }

    //public void AddWall()
    //{
    //    // Create a wall on the tile if there's no wall already
    //    // also if there's a door, deactivate it

    //    if (!hasWall)
    //    {
    //        hasWall = true;
    //        if (wallGO == null)
    //        {
    //            wallGO = GameObject.Instantiate(wallPrefab, this.transform.position + new Vector3(0f, 1.5f, 0f), this.transform.rotation, this.transform);
    //            wallGO.name = "Wall";
    //        }
    //        else wallGO.SetActive(true);

    //        if (hasDoor)
    //        {
    //            hasDoor = false;
    //            door.ReduceDoor(this);
    //            door = null;
    //            doorGO.SetActive(false);
    //        }
    //    }
    //}


    public void DestroyWall()
    {
        if (hasWall)
        {
            hasWall = false;
            //wallGO.SetActive(false);
            Destroy(wallGO);
        }
    }

    //public void AddDoor(int ID_, Section s1_, Section s2_)
    public void AddDoor(Door d_)
    {
        if (!hasDoor)
        {

            DestroyWall();
            door = d_;
            hasDoor = true;
            if (doorGO == null)
            {
                doorGO = GameObject.Instantiate(doorPrefab, this.transform.position + new Vector3(0f, 1.5f, 0f), this.transform.rotation, this.transform);
                //doorGO.GetComponent<Door>().Setup(ID_,new List<Tile>(){this}, s1_, s2_);
            }
            else Destroy(doorGO);//doorGO.SetActive(true);

            //return doorGO.GetComponent<Door>();
        }
        //return null;
    }

    public void DestroyDoor()
    {
        if (hasDoor)
        {
            AddWall();
        }
    }

    public void UpdateWallPrefab()
    {
        if (!hasWall) return;
        if (wallGO) Destroy(wallGO);
        if ((n && s) && (!e && !w)) { lastWallPrefab = rotSlimWallPrefab; wallGO = Instantiate(rotSlimWallPrefab, transform.position + new Vector3(0f, 1.5f, 0f), rotSlimWallPrefab.transform.rotation, transform); }
        else if ((!n && !s) && (e && w)){ lastWallPrefab = slimWallPrefab; wallGO = Instantiate(slimWallPrefab, transform.position + new Vector3(0f, 1.5f, 0f), slimWallPrefab.transform.rotation, transform); }
        else { lastWallPrefab = wallPrefab; wallGO = Instantiate(wallPrefab, transform.position + new Vector3(0f, 1.5f, 0f), transform.rotation, transform);}

        
    }

    public void Highlight(int type)
	{
		if(type==0) myRenderer.material.color = myRedColor;
		else if(type==1)myRenderer.material.color = Color.white;
	}
	
	public void DeHighlight()
	{
		myRenderer.material.color = originalColor;
	}

	#region GETTERS & SETTERS
	
	public bool 		HasWall(){return hasWall;}
	public bool 		HasDoor(){return hasDoor;}
	public Vector3		GetPos(){return pos;}
	public Renderer 	GetRenderer(){return myRenderer;}
	public Section 		GetSection(){return section;}
	public Door			GetDoor(){ return door;}
	
	public void 		SetPos(Vector3 pos_){pos = pos_;}
	public void 		SetSection(Section newSection){section = newSection;}

    public void SetN(bool n_) { n = n_; }
    public void SetS(bool s_) { s = s_; }
    public void SetW(bool w_) { w = w_; }
    public void SetE(bool e_) { e = e_; }

    #endregion
}


























// public class Tile : MonoBehaviour
// {
//     public GameObject wallPrefab, doorPrefab;
// 	private GameObject wallGO, doorGO;
	
// 	private Color originalColor;
// 	private Renderer myRenderer;
	
// 	private Map map;
// 	private Section section = null;
	
// 	public Vector3 pos;
// 	private bool hasWall = false;
// 	private bool hasDoor = false;
	
//     void Start()
//     {
//         transform.position = pos - new Vector3(map.width/2, 0, map.height/2);
		
// 		myRenderer = this.gameObject.GetComponent<Renderer>();
// 		originalColor = myRenderer.material.color;
//     }
	

	
// 	void OnMouseOver()
// 	{
// 		if(EventSystem.current.IsPointerOverGameObject()){
// 			return;
// 		}
// 		if(section!=null) map.GetSceneController().ToggleText(section.name);
// 	}
	
// 	void OnMouseExit()
// 	{
// 		map.GetSceneController().ToggleText("");
// 	}
	
	
// 	#region GETTERS & SETTERS
	
// 	public bool 		HasWall(){return hasWall;}
// 	public bool 		HasDoor(){return hasDoor;}
// 	public Section 		GetSection(){return section;}
// 	public Vector3		GetPos(){return pos;}
// 	public Renderer 	GetRenderer(){return myRenderer;}
// 	public Map			GetMap(){return map;}
	
// 	public void 		SetPos(Vector3 pos_){pos = pos_;}
// 	public void 		SetMap(Map map_){map = map_;}
// 	public void 		SetSection(Section newSection){section = newSection;}
	
// 	#endregion
	
// 	public void Print(string msg){Debug.Log(Random.Range(1000, 2000)+" TILE:  "+msg);}
// }
