using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IAlgorithm
{
    List<Path> FindPaths(Graph graph_, List<PersonBehavior> people_);    
}
