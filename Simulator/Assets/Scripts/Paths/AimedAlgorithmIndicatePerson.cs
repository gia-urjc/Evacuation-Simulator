using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using UnityEngine;

// ********************************************************************** //
// In this class you can introduce the path of a INDICATE PERSON in a txt //
// ********************************************************************** //

public class AimedAlgorithmIndicatePerson : IAlgorithm
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


    public List<Path> FindPaths(Graph graph_, List<PersonBehavior> people_)
    {
        List<Path> foundPaths = new List<Path>();
        CPNodes = graph_.GetNodes().FindAll(x => x.GetIsCP()); // Return Collection Points Nodes List
        Path path = null;
        int count = 0;
        string line;
        List<PersonBehavior> peopleInFile = new List<PersonBehavior>();
        System.IO.StreamReader file = new System.IO.StreamReader(Directory.GetCurrentDirectory() + @"\Assets\SavedData\aimedIndicatePerson.txt");
        while (((line = file.ReadLine()) != null) && (count < people_.Count))
        {
            
            string[] nodesLines = line.Split(' ');

            int personID = Convert.ToInt32(nodesLines[0]);

            if (personID >= 0)
            {
                Debug.LogError("Persona ID1:" + personID);
                peopleInFile.Add(people_[personID]);
                if (people_[personID].GetDependent())
                {
                    count++;
                }
                else
                {
                    PersonBehavior person = people_[personID];
                    List<Node> personPath = new List<Node>();
                    int lastNodeID = person.GetInitNode().GetID();
                    personPath.Add(person.GetInitNode());
                    float fperson = 0;
                    path = new Path(person, personPath, fperson);
                    
                    for (int j = 1; j < nodesLines.Length; j++)
                    {
                        int currentlyNode = Convert.ToInt32(nodesLines[j]);
                        if (!(currentlyNode.Equals(person.GetInitNode().GetID()))) // We See if the second number is the first node
                        {
                            if (graph_.GetNodes().Contains(graph_.GetNode(currentlyNode))) // If the node exists 
                            {
                                personPath.Add(graph_.GetNode(currentlyNode));
                                fperson = fperson + graph_.GetNode(lastNodeID).ConnectedTo(graph_.GetNode(currentlyNode)).GetDistance();
                                lastNodeID = currentlyNode;
                                path = new Path(person, personPath, fperson);
                                if (path != null) foundPaths.Add(path); else Utils.Print("PERSON WITHOUT PATH");
                                
                            }
                            else
                            {
                                Debug.LogError("The Node " + currentlyNode + ", line " + count + ", in the txt doesn't exit");
                            }
                        }
                    }
                    count++;
                }
            }
            else
            {
                Debug.LogError("Only accept Person ID > 0");
            }
            
        }
        file.Close();

        for(int i=0; i<people_.Count; i++)
        {
            if (!peopleInFile.Contains(people_[i]))
            {
                PersonBehavior person = people_[i];
                List<Node> personPath = new List<Node>();
                personPath.Add(person.GetInitNode());
                float fperson = 0;
                path = new Path(person, personPath, fperson);
                if (path != null) foundPaths.Add(path); else Utils.Print("PERSON WITHOUT PATH");
            }
        }

        return foundPaths;

        
    }
}