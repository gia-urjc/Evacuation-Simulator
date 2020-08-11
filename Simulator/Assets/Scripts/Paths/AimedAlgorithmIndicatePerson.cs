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
    //private List<Element> personPath;


    public List<Path> FindPaths(Graph graph_, List<PersonBehavior> people_)
    {
        List<Path> foundPaths = new List<Path>();
        CPNodes = graph_.GetNodes().FindAll(x => x.GetIsCP()); // Return Collection Points Nodes List
        Path path = null;
        int count = 0;
        string line;
        List<PersonBehavior> peopleInFile = new List<PersonBehavior>();
        System.IO.StreamReader file = new System.IO.StreamReader(Directory.GetCurrentDirectory() + @"\Assets\SavedData\aimedIndicatePerson.txt");
        while ((line = file.ReadLine()) != null)
        {

            string[] nodesLines = line.Split(' ');
            int personID = Convert.ToInt32(nodesLines[0]);
            if ((personID >= 0) && (personID < nodesLines.Length))
            {
                peopleInFile.Add(people_[personID]);
                if (people_[personID].GetDependent())
                {
                    count++;
                }
                else if (count < people_.Count)
                {
                    PersonBehavior person = people_[personID];
                    List<Node> personPath = new List<Node>();
                    int lastNodeID = 0;
                    float fperson = 0;
                    for (int j = 1; j < nodesLines.Length; j++)
                    //foreach (string nodeL in nodesLines)
                    {

                        if (nodesLines[j].Equals(person.GetInitNode().GetID())) // We See if the second number is the first node
                        {
                            personPath.Add(person.GetInitNode());
                            lastNodeID = person.GetInitNode().GetID();

                            path = new Path(person, personPath, fperson);
                            if (path != null) foundPaths.Add(path); else Utils.Print("PERSON W/O PATH");
                        }
                        else
                        {
                            if (!nodesLines[j].Equals(" ") && (!nodesLines[j].Equals("")))
                            {
                                int currentlyNode = Convert.ToInt32(nodesLines[j]);
                                if (graph_.GetNodes().Contains(graph_.GetNode(currentlyNode))) // If the node exists 
                                {
                                    //if (graph_.GetAdjacentNodes(graph_.GetNode(lastNodeID)).Contains(graph_.GetNode(currentlyNode)))
                                    //{
                                    personPath.Add(graph_.GetNode(currentlyNode));
                                    fperson = fperson + graph_.GetNode(lastNodeID).ConnectedTo(graph_.GetNode(currentlyNode)).GetDistance();
                                    lastNodeID = currentlyNode;
                                    path = new Path(person, personPath, fperson);
                                    if (path != null) foundPaths.Add(path); else Utils.Print("PERSON W/O PATH");
                                    //}
                                }
                                else
                                {
                                    Debug.LogError("The Node "+ currentlyNode+", line "+ count +", in the txt doesn't exit");
                                }
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
        }
        file.Close();

        int countAllPeople = 0; 
        while (count < people_.Count)
        {
            if (peopleInFile.Contains(people_[countAllPeople]))
            {
                countAllPeople++;
            }
            else
            {
                if (people_[countAllPeople].GetDependent())
                {
                    count++;
                }
                else
                {
                    PersonBehavior person = people_[countAllPeople];
                    List<Node> personPath = new List<Node>();
                    float fperson = 0;
                    personPath.Add(person.GetInitNode());
                    path = new Path(person, personPath, fperson);
                    if (path != null) foundPaths.Add(path); else Utils.Print("PERSON W/O PATH");
                    count++;
                }
                peopleInFile.Add(people_[countAllPeople]);
                countAllPeople++;
            }
        }

        return foundPaths;

        
    }
}