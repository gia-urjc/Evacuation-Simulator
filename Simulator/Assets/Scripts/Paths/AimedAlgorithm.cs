using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using UnityEngine;

// ************************************************* //
// In this class you can introduce the path in a txt //
// ************************************************* //

public class AimedAlgorithm : IAlgorithm
{
    class Element
    {
        public Node n;
        public float remaining, walked, f;
        public Element parent;

        public Element(Node n_, float w_, float r_, Element p_)
        {
            n = n_;
            walked = w_;
            remaining = r_;
            f = r_ + w_;
            parent = p_;
        }
    }

    private List<Node> CPNodes;
    //private List<Element> personPath;


    public List<Path> FindPaths(Graph graph_, List<PersonBehavior> people_)
    {
        List<Path> foundPaths = new List<Path>();
        CPNodes = graph_.GetNodes().FindAll(x => x.GetIsCP()); // Return Collection Points Nodes List
        Path path = null;
        int count = 0;
        string line;
        System.IO.StreamReader file = new System.IO.StreamReader(Directory.GetCurrentDirectory() + @"\Assets\SavedData\aimed.txt");
        while ((line = file.ReadLine()) != null)
        {
            if (people_[count].GetDependent())
            {
                count++;
            }
            else if (count < people_.Count)
            {
                string[] nodesLines = line.Split(' ');
                PersonBehavior person = people_[count];
                List<Node> personPath = new List<Node>();
                int lastNodeID = 0;
                float fperson = 0;
                foreach (string nodeL in nodesLines)
                {
                    if (nodeL.Equals(person.GetInitNode().GetID()))
                    {
                        personPath.Add(person.GetInitNode());
                        lastNodeID = person.GetInitNode().GetID();

                        path = new Path(person, personPath, fperson);
                        if (path != null) foundPaths.Add(path); else Utils.Print("PERSON W/O PATH");
                    }
                    else
                    {
                        if (!nodeL.Equals(" ") && (!nodeL.Equals("")))
                        {
                            int currentlyNode = Convert.ToInt32(nodeL);
                            //if (graph_.GetAdjacentNodes(graph_.GetNode(lastNodeID)).Contains(graph_.GetNode(currentlyNode)))
                            //{
                            personPath.Add(graph_.GetNode(currentlyNode));
                            fperson = fperson + graph_.GetNode(lastNodeID).ConnectedTo(graph_.GetNode(currentlyNode)).GetDistance();
                            lastNodeID = currentlyNode;
                            path = new Path(person, personPath, fperson);
                            if (path != null) foundPaths.Add(path); else Utils.Print("PERSON W/O PATH");
                            //}
                        }
                    }   
                }
                count++;
            }
            else
            {
                Debug.LogError("More data that persons in txt");
            }
        }
        file.Close();

        while (count < people_.Count)
        {
            if (people_[count].GetDependent())
            {
                count++;
            }
            else
            {
                PersonBehavior person = people_[count];
                List<Node> personPath = new List<Node>();
                float fperson = 0;
                personPath.Add(person.GetInitNode());
                path = new Path(person, personPath, fperson);
                if (path != null) foundPaths.Add(path); else Utils.Print("PERSON W/O PATH");
                count++;
            }
        }

        return foundPaths;

    }
    
}