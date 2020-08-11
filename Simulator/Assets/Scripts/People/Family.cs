using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Family
{
    [SerializeField] private int ID;
    [SerializeField] private List<PersonBehavior> members;

    public Family(int ID_){ ID = ID_; members = new List<PersonBehavior>();}

    public void AddMember(PersonBehavior p_){ members.Add(p_);}
    public List<PersonBehavior> GetMembers(){return members;}
    public int GetID(){return ID;}
    public void RemoveMember(PersonBehavior p_){ members.Remove(p_);}
    public List<PersonBehavior> GetDependentMembers()
    {
        var res = new List<PersonBehavior>();
        foreach(PersonBehavior p in members)
        {
            if(p.GetDependent())
            {
                res.Add(p);
            }
        }
        return res;

    }
    public PersonBehavior FindMemberByID(int id_)
    {
        return members.Find(x => x.GetID() == id_);
    }

}
