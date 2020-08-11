using System.Collections.Generic;
using System.Collections;
using UnityEngine;

interface IGraph
{
	Node InsertNode(Section elem, Vector3 pos);
	Edge InsertEdge(Door elem, Node linkA, Node linkB);
	int NodesCount();
	int EdgesCount();
	Node ContainsNode(Section elem);
	Edge ContainsEdge(Node linkA, Node linkB);
	bool DeleteNode(Section elem);
	bool DeleteEdge(Edge e_);
	string ToString();
	List<Node> GetNodes();
	List<Edge> GetEdges();
	List<Node> GetAdjacentNodes(Node node_);
}
