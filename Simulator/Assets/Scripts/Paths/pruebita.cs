using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarthestAlgorithm : IAlgorithm
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
    private List<Path> foundPaths = new List<Path>();
    private List<Element> personPath;


    public List<Path> FindPaths(Graph graph_, List<PersonBehavior> people_)
    {
        CPNodes = graph_.GetNodes().FindAll(x => x.GetIsCP()); // Return Collection Points Nodes List
        Path path = null;
        foreach (PersonBehavior person in people_)
        {
            if (!person.GetDependent())
            {
                path = FindFarCP(person);
            }
            if (path != null) foundPaths.Add(path); else Utils.Print("PERSON W/O PATH");
        }
        return foundPaths;
    }

    private Path FindFarCP(PersonBehavior person)
    {
        float maxF = Mathf.NegativeInfinity;
        Path actualPath, assignedPath = null;

        foreach (Node cpn in CPNodes)
        {
            actualPath = FindPathToCP(person, cpn);
            if (actualPath.f > maxF) // if remaining length path is > actual max length path
            {
                assignedPath = actualPath;
                maxF = actualPath.f;
            }
        }

        return assignedPath;
    }

    // This func goes through graph and return a path when the collection point is found
    private Path FindPathToCP(PersonBehavior person, Node CPNode)
    {
        Path actualPath = null;
        float maxF = Mathf.NegativeInfinity;

        personPath = new List<Element>(); // Element list - Node Path
        personPath.Add(new Element(person.GetInitNode(), 0, Vector3.Distance(person.GetInitNode().GetPos(), CPNode.GetPos()), null));
        Element actual;
        int countdown = 3000;

        while (countdown > 0)
        {
            if (personPath.Count < 1 && actualPath != null)
            {
                return actualPath;
            }
            else if (personPath.Count < 1) // if personPath 
            {
                Utils.Print("ERROR. Could not find a path to destination " + CPNode.GetData() + " from " + person.GetInitNode().GetData());
                return null;
            }

            actual = personPath[0]; // First Element's List 
            personPath.RemoveAt(0); // Delete actual Element

            // Goal Reached
            if (actual.n == CPNode)
            {

                // Assign Path Variables
                List<Node> sol = new List<Node>();
                Element solNode = actual;
                float fsol = solNode.f;
                sol.Add(solNode.n);
                while (solNode.parent != null)
                {
                    sol.Add(solNode.parent.n);
                    solNode = solNode.parent;
                }
                sol.Reverse(); // Start in CPNode -->  Reverse --> Finish in CPNode --> Path
                if (fsol > maxF) // if remaining length path is > actual max length path
                {
                    actualPath = new Path(person, sol, fsol);
                    maxF = actualPath.f;
                    UnityEngine.Debug.LogWarning("Far ID N" + actual.n.GetID().ToString());
                }
            }

            // Expand node
            foreach (Edge e in actual.n.GetAdjacentEdges())
            {
                Node childNode = e.GetOtherNode(actual.n); // Associate node to actual node
                if (childNode.GetCurrentCapacity() == 0)
                {

                }
                else // If there is capacity
                {
                    if (!(actual.parent != null && childNode == actual.parent.n)) // Avoid simple loops
                    {
                        float walkedChild = actual.walked + e.GetDistance();
                        float remainingChild = Vector3.Distance(CPNode.GetPos(), childNode.GetPos());
                        Element child = new Element(childNode, walkedChild, remainingChild, actual);
                        personPath.Add(child);
                    }
                }
            }

            // Sort list by f value
            personPath.Sort((a, b) => a.f.CompareTo(b.f));

            countdown--;
        }
        Debug.Log("countdown expired");
        return actualPath;
    }
}
