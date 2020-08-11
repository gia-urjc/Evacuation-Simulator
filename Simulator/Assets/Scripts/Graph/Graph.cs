using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph: MonoBehaviour, IGraph, ISceneElement
{

	public GameObject nodePrefab, edgePrefab;

	private List<Node> nodes;
	private List<Edge> edges;
	private bool alreadyBaked = false;

	private int currentNodeID, currentEdgeID;

	private SceneController sc;
    
	private SubMenuStates state;
	private Node startNode;
	
	public void Setup(SceneController sc_)
	{
		sc = sc_;
		nodes = new List<Node>();
		edges = new List<Edge>();
	}
	
	#region IGraph

	public Node InsertNode(Section elem, Vector3 pos)
	{
		Node newNode = GameObject.Instantiate(nodePrefab, pos, Quaternion.Euler(0,0,0), this.transform).GetComponent<Node>();
		newNode.Setup(currentNodeID++,elem, pos, elem.GetIsCP());
		nodes.Add(newNode);
		
		//sc.GetSceneInfo().numberOfNodes += 1;
		
		return newNode;
	}
	
	public Edge InsertEdge(Door elem, Node linkA, Node linkB)
	{
		Vector3 pos = this.transform.position;
		if(elem!=null){ pos = new Vector3 (elem.GetPos().x, this.transform.position.y, elem.GetPos().z);}

		Edge newEdge = GameObject.Instantiate(edgePrefab, pos, Quaternion.Euler(90f,0,0),this.transform).GetComponent<Edge>();
		newEdge.Setup(currentEdgeID++, elem, linkA, linkB);
		edges.Add(newEdge);
		linkA.AddAdjacentEdge(newEdge);
		linkB.AddAdjacentEdge(newEdge);

		newEdge.CalculateDistance();

		// Paint the edge
		newEdge.PaintEdge();

		//sc.GetSceneInfo().numberOfEdges += 1;
		
		return newEdge;
	}
	
	public int NodesCount(){return nodes.Count;}
	public int EdgesCount(){return edges.Count;}
	public List<Node> GetNodes(){return nodes;}
	public List<Edge> GetEdges(){return edges;}

    // Sandra
    public Node GetNode(int ID_)
    {
        foreach (Node node in nodes)
        {
            if (node.GetID().Equals(ID_)) return node;
        }
        return null;
    }
	// 
	public Node ContainsNode(Section elem)
	{
		foreach(Node node in nodes)
		{
			if(node.GetData().Equals(elem)) return node;
		}
		return null;
	}

	public List<Node> NodesInSection(Section elem)
	{
		List<Node> sectionNodes = new List<Node>();
		foreach(Node node in nodes)
		{
			if(node.GetData().Equals(elem)) sectionNodes.Add(node);
		}
		return sectionNodes;
	}
	
	public Edge ContainsEdge(Node linkA, Node linkB)
	{
		return linkA.ConnectedTo(linkB);
	}
	
	public bool DeleteNode(Section elem){return false;}

	public bool DeleteEdgeBtwNodes(Node linkA, Node linkB){return false;}

	public bool DeleteEdge(Edge e_)
	{
		e_.GetNodes()[0].RemoveAdjacentEdge(e_);
		e_.GetNodes()[1].RemoveAdjacentEdge(e_);

		edges.Remove(e_);
		GameObject.Destroy(e_.gameObject);

		//sc.GetSceneInfo().numberOfEdges -= 1;

		return true;
	}
	
	
	public override string ToString()
	{
		string result = "";
		result += "There are "+nodes.Count+" nodes and "+edges.Count+" edges\nList of nodes:\n";
		
		foreach(Node node in nodes)
		{
			result+=node+", ";
		}
		
		result += "\n";
		
		foreach(Edge edge in edges)
		{
			result+=edge+"\n";
		}
		result += "\n";
		result += "\nShowing connections:\n";
		
		List<Node> adjacentNodes = null;
		
		foreach(Node node_ in nodes)
		{
			result+=node_+" is connected to ";
			adjacentNodes = GetAdjacentNodes(node_);
			foreach(Node n in adjacentNodes)
			{
				result += n+", ";
			}
			result+="\n";
		}
		
		return result;
	}
	
	
	public List<Node> GetAdjacentNodes(Node node_)
	{
		List<Node> adjacentNodes = new List<Node>();
		
		Node node = null;
		foreach(Edge edge in node_.GetAdjacentEdges())
		{
			node = edge.GetOtherNode(node_);
			if(node!=null) adjacentNodes.Add(node);
		}
		
		return adjacentNodes;
	}


	public Node GetClosestNode(Vector3 pos_, Section s_)
	{
		List<Node> sectionNodes = NodesInSection(s_);
		Node chosenNode = null;
		float maxDistance = Mathf.Infinity;
		foreach(Node n in sectionNodes)
		{
			float distance = (n.GetPos()-pos_).sqrMagnitude; 
			if(distance < maxDistance)
			{
				chosenNode = n;
				maxDistance = distance;
			}
		}

		return chosenNode;
	}
	
	public Node FindNodeByID(int id_)
	{
		return nodes.Find(x => x.GetID() == id_);
	}


	#endregion

	#region input
	public void MouseEvent(Utils.MouseInputEvents mouseEvent)
    {
        KeyValuePair<Vector3,GameObject> info = Utils.ScreenToWorld();

        if(mouseEvent == Utils.MouseInputEvents.left_down)
        {
            if(info.Value != null && info.Value.tag == "node")
            {
                startNode = info.Value.GetComponent<Node>();
            }
        }

        switch(state)
        {
            case SubMenuStates.move: MoveMouseHandler(mouseEvent); break;
            case SubMenuStates.create: CreateMouseHandler(mouseEvent); break;
            case SubMenuStates.edit: EditMouseHandler(mouseEvent); break;
            default: break;
        }
    }

    private void CreateMouseHandler(Utils.MouseInputEvents mouseEvent)
    {
        KeyValuePair<Vector3,GameObject> info = Utils.ScreenToWorld();

        if(mouseEvent == Utils.MouseInputEvents.left_up && info.Value != null)
        {
            if(info.Value.tag == "tile")
            {
                Tile t = info.Value.GetComponent<Tile>();
                Vector3 newNodePos = new Vector3(t.GetPos().x, this.transform.position.y, t.GetPos().z);
                InsertNode(t.GetSection(), newNodePos);
            }
            else if(info.Value.tag == "node" && startNode != null)
            {
                Node finishNode = info.Value.GetComponent<Node>();
				Door connectiondoor = startNode.GetData().GetConnection(finishNode.GetData());
                if(finishNode != startNode && ( startNode.GetData() == finishNode.GetData() || (startNode.GetData() != finishNode.GetData() && connectiondoor!=null) ))
                {
                    Edge edge = ContainsEdge(startNode, finishNode);
                    if(edge != null) DeleteEdge(edge);
                    else edge = InsertEdge(null, startNode, finishNode);
                }
            }
            startNode = null;
        }
		else if(mouseEvent == Utils.MouseInputEvents.right_up && info.Value != null)
		{
			if(info.Value.tag == "node")
			{
				Node removingnode = info.Value.GetComponent<Node>(); 
				int edgeslistlength = removingnode.GetAdjacentEdges().Count;
				Edge e;
				for(int i=0; i<edgeslistlength; i++)
				{
					e = removingnode.GetAdjacentEdges()[0];
					e.GetOtherNode(removingnode).RemoveAdjacentEdge(e);
					removingnode.RemoveAdjacentEdge(e);
					GameObject.Destroy(e.gameObject);
				}
				nodes.Remove(removingnode);
				GameObject.Destroy(info.Value);
			}
			else if(info.Value.tag == "edge")
			{
				info.Value.GetComponent<Edge>().ClearNodes();
				GameObject.Destroy(info.Value);
			}
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
					if(info.Value.tag == "node" || info.Value.tag == "edge")
					{
						sc.EnableEditMenu(info.Value);
					}
				break;
				default: break;
			}
		}
    }

	private void MoveMouseHandler(Utils.MouseInputEvents mouseEvent)
    {
        KeyValuePair<Vector3,GameObject> info = Utils.ScreenToWorld();

        if(mouseEvent == Utils.MouseInputEvents.left_held && startNode != null)
		{
			Vector3 destPos = Utils.ScreenToWorld().Key;
            if(destPos.x != Mathf.Infinity)
            {
                //Vector3 mapPos = sc.GetMap().ToMapPos(destPos);
                //startNode.transform.position = new Vector3(mapPos.x, this.transform.position.y, mapPos.z);
				startNode.transform.position = new Vector3(destPos.x, this.transform.position.y, destPos.z);
                foreach(Edge e in startNode.GetAdjacentEdges())
                {
                    e.PaintEdge();
                }
            }
		}
		else if(mouseEvent == Utils.MouseInputEvents.left_up && startNode != null) 
        {
            if(info.Key.x != Mathf.Infinity)
            {
                Vector3 mapPos = sc.GetMap().ToMapPos(info.Key);
                Section s = sc.GetMap().IsPosSection(mapPos);
                if(s != null)
                {
                    startNode.SetPos(new Vector3(mapPos.x, this.transform.position.y, mapPos.z));
                    startNode.SetData(s);
                }
                else startNode.transform.position = startNode.GetPos();
            }
            else
            {
                startNode.transform.position = startNode.GetPos();
            }

            foreach(Edge e in startNode.GetAdjacentEdges())
            {
                e.PaintEdge();
                e.CalculateDistance();
            }
            startNode = null;
        }
    }

	public void KeyboardEvent()
	{
	}

	#endregion

	public void Bake()
	{
		DeleteAll();
		CreateNodes(); 
		CreateEdges(); 
		alreadyBaked = true;
	}

	private void CreateNodes()
	{
		foreach(Section s in sc.GetMap().GetSections())
		{
			InsertNode(s, new Vector3 (s.GetPos().x, this.transform.position.y, s.GetPos().z));
		}
	}
	private void CreateEdges()
	{
		Section[] connections;
		foreach(Door d in sc.GetMap().GetDoors())
		{
			connections = d.GetConnections();
			Node nA = ContainsNode(connections[0]);
			Node nB = ContainsNode(connections[1]);
			Edge newEdge = InsertEdge(d, nA, nB);
		}
	}

	

	public void DeleteAll()
	{
		DeleteEdges();

		foreach(Node n in nodes)
		{
			GameObject.Destroy(n.gameObject);
		}
		nodes.Clear();
		currentNodeID = 0;

		alreadyBaked = false;
	}

	public void DeleteEdges()
	{
		foreach(Edge e in edges)
		{
			e.ClearNodes();
			GameObject.Destroy(e.gameObject);
		}
		edges.Clear();
		currentEdgeID = 0;
	}

	#region ISceneElement
    public void ChangeState(SubMenuStates state_)
    {
        state = state_;
    }

	public void Save()
	{
		List<NodeData> nodesDataList = new List<NodeData>();
		List<EdgeData> edgesDataList = new List<EdgeData>();

		foreach(Node n in nodes)
		{
			nodesDataList.Add(n.TransformData());
		}

		foreach(Edge e in edges)
		{
			edgesDataList.Add(e.TransformData());
		}

		DataController.SetGraphData(new GraphData(nodesDataList, edgesDataList));
	}

	public void Load()
	{
		GraphData gd = DataController.GetGraphData();
		Node newNode;
		Edge newEdge;
		int maxID=-1;

		// Nodes
		foreach(NodeData n in gd.nodes)
		{
			if(n.ID>maxID)maxID = n.ID;
			newNode = InsertNode(sc.GetMap().FindSectionByID(n.sectionID), n.pos);
			newNode.SetID(n.ID);
			newNode.SetMaxCapacity(n.maxCapacity);
			newNode.SetCurrentCapacity(n.currentCapacity);
		}
		currentNodeID = maxID+1;
		maxID=-1;

		// Edges
		foreach(EdgeData e in gd.edges)
		{
			if(e.ID>maxID)maxID = e.ID;
			newEdge = InsertEdge(sc.GetMap().FindDoorByID(e.doorID), FindNodeByID(e.nodes[0]), FindNodeByID(e.nodes[1]));
			newEdge.SetID(e.ID);
			newEdge.SetDistance(e.distance);
			newEdge.SetMaxCapacity(e.maxCapacity);
			newEdge.SetCurrentCapacity(e.currentCapacity);
		}
		currentEdgeID = maxID+1;
		
		alreadyBaked = true;
	}

	#endregion


	public SceneController GetSc(){ return sc;}
	public SubMenuStates GetState(){ return state;}

}
