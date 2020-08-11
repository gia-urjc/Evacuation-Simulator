using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ************************************************************************** //
// In this class you can select the nodes of the paths. It's only for testing //
// ************************************************************************** //

public class AimedAlgorithmTest : IAlgorithm
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
    //private List<Element> personPath;


    public List<Path> FindPaths(Graph graph_, List<PersonBehavior> people_)
    {
        CPNodes = graph_.GetNodes().FindAll(x => x.GetIsCP()); // Return Collection Points Nodes List
        Path path = null;
        int count = 0;
        //for (int count = 0; count < people_.Count; count++)
        //{
        if (!people_[count].GetDependent())
        {
            if (people_[count].GetID() == 0)
            {
                //Path(PersonBehavior p_, List < Node > path_, float f_)
                PersonBehavior person = people_[count];
                List<Node> personPath = new List<Node>();
                personPath.Add(person.GetInitNode());
                float fperson = 0;

                personPath.Add(graph_.GetNode(2));
                fperson = fperson + person.GetInitNode().ConnectedTo(graph_.GetNode(2)).GetDistance();

                personPath.Add(graph_.GetNode(3));
                fperson = fperson + graph_.GetNode(2).ConnectedTo(graph_.GetNode(3)).GetDistance();

                //personPath.Add(graph_.GetNode(1));
                //fperson = fperson + graph_.GetNode(3).ConnectedTo(graph_.GetNode(1)).GetDistance();


                path = new Path(person, personPath, fperson);
                if (path != null) foundPaths.Add(path); else Utils.Print("PERSON W/O PATH");
            }

            count++;
            if (people_[count].GetID() == 1)
            {
                //Path(PersonBehavior p_, List < Node > path_, float f_)
                PersonBehavior person = people_[count];
                List<Node> personPath = new List<Node>();
                personPath.Add(person.GetInitNode());
                float fperson = 0;

                personPath.Add(graph_.GetNode(2));
                fperson = fperson + person.GetInitNode().ConnectedTo(graph_.GetNode(2)).GetDistance();

                personPath.Add(graph_.GetNode(3));
                fperson = fperson + graph_.GetNode(2).ConnectedTo(graph_.GetNode(3)).GetDistance();

                personPath.Add(graph_.GetNode(1));
                fperson = fperson + graph_.GetNode(3).ConnectedTo(graph_.GetNode(1)).GetDistance();


                path = new Path(person, personPath, fperson);
                if (path != null) foundPaths.Add(path); else Utils.Print("PERSON W/O PATH");
            }

        }


        //}
        return foundPaths;
    }

}