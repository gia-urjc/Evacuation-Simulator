using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// hill climbing
public class DummyAlgorithm: IAlgorithm
{
    class Element
    {
        public Node n;
        public float remaining;
        public float walked;
        public float f;
        public Element parent;

        public Element(Node n_, float w_, float r_, Element p_)
        {
            n = n_;
            walked = w_; 
            remaining = r_; 
            f = r_+w_; 
            parent = p_;
        }
    }

    private List<Path> foundPaths = new List<Path>();
    private List<Node> CPNodes;
    private List<Element> personPath;

    public List<Path> FindPaths(Graph graph_, List<PersonBehavior> people_)
    {
        CPNodes = graph_.GetNodes().FindAll( x => x.GetIsCP() );
        Path path;

        foreach(PersonBehavior person in people_)
        {
            path = FindClosestCP(person);
            if(path != null) foundPaths.Add(path); else Utils.Print("PERSON W/O PATH");
        }

        return foundPaths;   
    }

    private Path FindClosestCP(PersonBehavior person)
    {
        float minF = Mathf.Infinity;
        Path actualPath, assignedPath = null;

        foreach(Node cpn in CPNodes)
        {
            actualPath = FindPathToCP(person, cpn);
            if(actualPath.f < minF)
            {
                assignedPath = actualPath;
                minF = actualPath.f;
            }
        }

        return assignedPath;
    }

    private Path FindPathToCP(PersonBehavior person, Node CPNode)
    {
        personPath = new List<Element>();
        personPath.Add(new Element(person.GetInitNode(), 0, Vector3.Distance(person.GetInitNode().GetPos(), CPNode.GetPos()), null));
        Element actual;
        int countdown = 100;

        while(countdown > 0)
        {
            countdown--;
            if(personPath.Count<1) 
            {
                Utils.Print("ERROR. Could not find a path to destination "+CPNode.GetData()+" from "+person.GetInitNode().GetData());
                return null;
            }

            actual = personPath[0];

            // Goal reached
            if(actual.n == CPNode)
            {
                float fsol;
                List<Node> sol = new List<Node>();
                Element solNode = actual;
                sol.Add(solNode.n);
                fsol = solNode.f;
                while(solNode.parent != null)
                {
                    sol.Add(solNode.parent.n);
                    solNode = solNode.parent;
                }
                sol.Reverse();                
                return new Path(person, sol, fsol);
            }

            // Expand node
            personPath.RemoveAt(0);
            foreach(Edge e in actual.n.GetAdjacentEdges())
            {
                Node childNode = e.GetOtherNode(actual.n);
                
                if(!(actual.parent != null && childNode == actual.parent.n))
                {
                    float walkedChild = actual.walked + e.GetDistance();
                    float remainingChild = Vector3.Distance(CPNode.GetPos(),childNode.GetPos());
                    Element child = new Element(childNode, walkedChild, remainingChild, actual);
                    personPath.Add(child);
                }
            }

            // Sort list by f value
            personPath.Sort((a, b) => (a.f.CompareTo(b.f)));
        }

        return null;
    }
}
