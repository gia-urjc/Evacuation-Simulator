using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlgorithmC : IAlgorithm
{
    public List<Path> FindPaths(Graph graph_, List<PersonBehavior> people_)
    {
        List<Path> foundPaths = new List<Path>();
        List<Node> nodes;

        foreach(PersonBehavior p in people_)
        {
            nodes = new List<Node>();
            switch (p.GetInitNode().GetID())
            {
                case 1: nodes.Add(graph_.FindNodeByID(1)); nodes.Add(graph_.FindNodeByID(0)); break;
                case 2:
                    nodes.Add(graph_.FindNodeByID(1));
                    nodes.Add(graph_.FindNodeByID(0)); break;
                case 3:
                    nodes.Add(graph_.FindNodeByID(4));
                    nodes.Add(graph_.FindNodeByID(5));
                    nodes.Add(graph_.FindNodeByID(6)); break;
                case 4:
                    nodes.Add(graph_.FindNodeByID(5));
                    nodes.Add(graph_.FindNodeByID(6)); break;
                case 5: nodes.Add(graph_.FindNodeByID(5)); nodes.Add(graph_.FindNodeByID(6)); break;
                case 7:
                    nodes.Add(graph_.FindNodeByID(1));
                    nodes.Add(graph_.FindNodeByID(0)); break;
                case 8:
                    nodes.Add(graph_.FindNodeByID(4));
                    nodes.Add(graph_.FindNodeByID(5));
                    nodes.Add(graph_.FindNodeByID(6)); break;
                default: break;
            }

            if(nodes.Count > 0) foundPaths.Add(new Path(p, nodes, 0));
            
        }
        return foundPaths;
    }
}
