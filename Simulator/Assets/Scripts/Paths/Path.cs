using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path
{
    public PersonBehavior person;
    public List<Node> path;
    public float f;

    public Path(PersonBehavior p_, List<Node> path_, float f_)
    {
        person = p_; path = path_; f = f_;
    }
}
