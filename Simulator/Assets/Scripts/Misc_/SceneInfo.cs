using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SectionInfo
{
    public int ID;
    public bool isCP;
    public float totalTime;
    public string worstLOS;
    public string mediaLOS;

    public SectionInfo(int iD, bool isCP, float totalTime, string worstLOS, string mediaLOS)
    {
        ID = iD;
        this.isCP = isCP;
        this.totalTime = totalTime;
        this.worstLOS = worstLOS;
        this.mediaLOS = mediaLOS;
    }
}

[System.Serializable]
public class PersonInfo
{
    public int ID;
    public int CPID;
    public float totalTime;

    public PersonInfo(int iD, int cPID, float totalTime)
    {
        ID = iD;
        CPID = cPID;
        this.totalTime = totalTime;
    }
}

[System.Serializable]
public class SceneInfo
{
    public bool simulationFinished;
    public float totalTime;
    public int numberOfSections;
    public int numberOfCPSections;
    public int numberOfNodes;
    public int numberOfEdges;
    public int numberOfPeople;
    public string pathsImage;
    public List<SectionInfo> sectionInfos;
    public List<PersonInfo> personInfos;

    public SceneInfo() { }

    public SceneInfo(bool simulationFinished, float totalTime, int numberOfSections, int numberOfCPSections, int numberOfNodes, int numberOfEdges, int numberOfPeople, string pathsImage, List<SectionInfo> si, List<PersonInfo> pi)
    {
        this.simulationFinished = simulationFinished;
        this.totalTime = totalTime;
        this.numberOfSections = numberOfSections;
        this.numberOfCPSections = numberOfCPSections;
        this.numberOfNodes = numberOfNodes;
        this.numberOfEdges = numberOfEdges;
        this.numberOfPeople = numberOfPeople;
        this.pathsImage = pathsImage;
        this.sectionInfos = si;
        this.personInfos = pi;
    }
}

