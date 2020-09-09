using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge: MonoBehaviour
{
	[SerializeField] private int ID;
	[SerializeField] private Door data;
	[SerializeField] private Node[] nodes = new Node[2];
	[SerializeField] private float distance;
	private int maxCapacity, currentCapacity;
    private bool isStair;
    MeshRenderer renderer;

    public void Setup(int ID_, Door data_, Node nodeA_, Node nodeB_, bool isStair_)
	{
        renderer = GetComponent<MeshRenderer>();
        name = "Edge";
		ID = ID_;
		data = data_;
        isStair = isStair_;

        nodes[0] = nodeA_;
		nodes[1] = nodeB_;

		if(data!=null)maxCapacity = data.GetCurrentCapacity();
		else maxCapacity = 500;
		currentCapacity = maxCapacity;

        
	}
	
	public void CalculateDistance()
	{
		distance = Utils.CalculateDistance(nodes[0].GetPos(), nodes[1].GetPos());

	}
	
	public Door GetData() {return data;}
	public float GetDistance(){return distance;}
    public bool GetIsStair() { return isStair; }
    public void setIsStair()
    {
        isStair = true;
        renderer.material.color = Color.magenta;
    }
	public void SetData(Door data_) {data = data_;}
	
	
	public Node[] GetNodes()
	{
		return nodes;
	}
	
	public Node GetOtherNode(Node node_)
	{
		if(node_.Equals(nodes[0]))	return nodes[1];
		else if(node_.Equals(nodes[1])) return nodes[0];
		else return null;
	}

	public void PaintEdge()
	{
        if (isStair)
        {
            renderer.material.color = Color.blue;
            //renderer.material.SetColor("Magenta",Color.magenta);
        }
		Vector3 dir = (nodes[0].transform.position - nodes[1].transform.position).normalized;
		if(dir.x+dir.y+dir.z != 0f)
		{
			float dist = Vector3.Distance(nodes[0].transform.position, nodes[1].transform.position);
			Vector3 newScale = new Vector3 (transform.localScale.x, dist/2, transform.localScale.z);
			Vector3 newPosition = nodes[1].transform.position + dir * dist * 0.5f;
			newPosition.y -= .2f;
			transform.eulerAngles = new Vector3(90, 0, Utils.GetAngleFromVectorFloat(dir)+90);
			transform.localScale = newScale;
			transform.position = newPosition;
		}
	}

	public EdgeData TransformData()
	{
		int[] myNodes = new int[2];
		myNodes[0] = nodes[0].GetID();
		myNodes[1] = nodes[1].GetID();
		if(data == null){return new EdgeData(ID, -1, distance, myNodes, maxCapacity, currentCapacity);}
		else return new EdgeData(ID, data.GetID(), distance, myNodes, maxCapacity, currentCapacity);
	}

	public void ClearNodes()
	{
		nodes[0].RemoveAdjacentEdge(this);
		nodes[1].RemoveAdjacentEdge(this);
	}
	
	public void 	SetNodes(Node nodeA_, Node nodeB_) { nodes[0] = nodeA_; nodes[1] = nodeB_;}
	public int 		GetID(){ return ID;}
	public int 		GetMaxCapacity(){return maxCapacity;}
	public int 		GetCurrentCapacity(){return currentCapacity;}

	public void SetID(int id_){ID = id_;}
	public void SetDistance(float d_){distance = d_;}
	public void SetMaxCapacity(int c_){maxCapacity = c_;}
	public void SetCurrentCapacity(int c_){ currentCapacity = c_;}

	public override string ToString(){return "Edge"+ID+" with nodes "+nodes[0].GetID()+" and "+nodes[1].GetID();}


}
