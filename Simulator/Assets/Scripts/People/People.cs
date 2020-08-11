using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class People : MonoBehaviour, ISceneElement
{
    public GameObject personPrefab;
    public Material GLMat, MIMat, FMMat, BMat;
    public GameEvent peopleAutoReady, simulationReady;
    [SerializeField] private List<PersonBehavior> people;
    [SerializeField] private List<Family> families;

    private SceneController sc;
    private int currentPersonID, currentFamilyID;
    private PersonBehavior actualPerson, nextPerson, movingPerson;

    private bool alreadyBaked = false;

    private int peopleArrivedCounter;
    private float timer;
    private PersonBehavior lastArrivedPerson;

    private SubMenuStates state;

    private Dictionary<string,int> peopleTypes = new Dictionary<string,int>()
    {
        {"Group Leader", 0}, {"Mobility Impaired", 1}, {"Family Member", 2}
    };
    

    public void Setup(SceneController sc_)
    {
        sc = sc_;
        people = new List<PersonBehavior>();
        families = new List<Family>();

        if(nextPerson == null) nextPerson = GameObject.Instantiate(personPrefab, this.transform).GetComponent<PersonBehavior>();
        nextPerson.gameObject.SetActive(false);
        nextPerson.Setup(currentPersonID, 30, new List<string>(), 3f, -1, false, true, new List<int>());
    }

    public void MouseEvent(Utils.MouseInputEvents mouseEvent)
	{
        switch(state)
		{
			case SubMenuStates.create: CreateMouseHandler(mouseEvent); break;
            case SubMenuStates.move: MoveMouseHandler(mouseEvent); break;
			case SubMenuStates.edit: EditMouseHandler(mouseEvent); break;
			default: break;
		}
	}

    private void CreateMouseHandler(Utils.MouseInputEvents mouseEvent)
    {
        KeyValuePair<Vector3,GameObject> info = Utils.ScreenToWorld();

		switch(mouseEvent)
	 	{
			case Utils.MouseInputEvents.left_up: 
                if(info.Value != null && info.Value.tag == "tile")
                {
                    Tile t = info.Value.GetComponent<Tile>();
                    Section s = t.GetSection();
                    if(s != null)
                    {
                        PlacePerson(s, t.GetPos());
                    }
                }
			break;

            case Utils.MouseInputEvents.right_up:
                if(info.Value != null && info.Value.tag == "person")
                {
                    people.Remove(info.Value.GetComponent<PersonBehavior>());
                    //sc.GetSceneInfo().numberOfPeople -= 1;

                    GameObject.Destroy(info.Value);
                }
            break;

			default: break;
		}
    }

    private void MoveMouseHandler(Utils.MouseInputEvents mouseEvent)
    {
        KeyValuePair<Vector3,GameObject> info = Utils.ScreenToWorld();

        // Pick up person
        if(mouseEvent == Utils.MouseInputEvents.left_down)
        {
            if(info.Value != null && info.Value.tag == "person")
            {
                movingPerson = info.Value.GetComponent<PersonBehavior>();
            }
        }
        // Move person
        else if(mouseEvent == Utils.MouseInputEvents.left_held && movingPerson != null)
		{
			Vector3 destPos = Utils.ScreenToWorld().Key;
            if(destPos.x != Mathf.Infinity)
            {
                //Vector3 mapPos = sc.GetMap().ToMapPos(destPos);
                //movingPerson.transform.position = new Vector3(mapPos.x, movingPerson.transform.position.y, mapPos.z);
                movingPerson.transform.position = new Vector3(destPos.x, movingPerson.transform.position.y, destPos.z);
            }
		}
        // Place person
		else if(mouseEvent == Utils.MouseInputEvents.left_up && movingPerson != null) 
        {
            if(info.Key.x != Mathf.Infinity)
            {
                Vector3 mapPos = sc.GetMap().ToMapPos(info.Key);
                Section s = sc.GetMap().IsPosSection(mapPos);
                if(s != null)
                {
                    movingPerson.SetInitPos(new Vector3(mapPos.x, movingPerson.transform.position.y, mapPos.z));
                    movingPerson.SetInitSection(s);
                }
                else movingPerson.transform.position = movingPerson.GetInitPos();
            }
            else
            {
                movingPerson.transform.position = movingPerson.GetInitPos();
            }

            movingPerson = null;
        }
        
    }

    private void EditMouseHandler(Utils.MouseInputEvents mouseEvent)
    {
        KeyValuePair<Vector3,GameObject> info = Utils.ScreenToWorld();
		if(info.Value != null)
		{
			switch(mouseEvent)
			{
				case Utils.MouseInputEvents.left_up: 
					if(info.Value.tag == "person")
					{
						sc.EnableEditMenu(info.Value);
					}
				break;
				default: break;
			}
		}
    }

	public void KeyboardEvent()
	{
		
	}

    private PersonBehavior PlacePerson(Section s_, Vector3 pos_)
    {
        // Place the person in the NavMesh
        NavMeshHit hit;
        bool validPos = NavMesh.SamplePosition(pos_, out hit, 2.0f, NavMesh.AllAreas);

		if (validPos) {
            actualPerson = GameObject.Instantiate(personPrefab, this.transform).GetComponent<PersonBehavior>();
            actualPerson.Setup(nextPerson.GetID(), nextPerson.GetAge(), new List<string>(nextPerson.GetPersonTypes()), nextPerson.GetSpeed(),
                nextPerson.GetFamilyID(), nextPerson.GetDependent(), nextPerson.GetManual(), new List<int>(nextPerson.GetResponsibleForIDs()));
        
            actualPerson.SetOnFloor(hit.position, s_, sc.GetGraph().GetClosestNode(hit.position, s_));
            people.Add(actualPerson);
            
            nextPerson.SetID(currentPersonID+=1);
            nextPerson.ClearResponsibilities();
            nextPerson.SetTutorID(-1);

            // COMMUNICATE THAT THE MENU NEEDS TO CHANGE ITS EDITABLE ELEMENT
            sc.PersonAdded(actualPerson, nextPerson);

            return actualPerson;
		} 
        else return null;
        
    }

    public void Bake()
    {
        sc.RunAlgorithm();
        alreadyBaked = true;
    }

    public void StartMovement()
    {
        peopleArrivedCounter = 0;

        foreach(PersonBehavior p in people)
        {
            p.StartMovement(1);
        }
    }

    public void ToggleMovement()
    {
        foreach(PersonBehavior p in people)
        {
            if(p.GetDestSection() != null && !p.HasFinishedPath()) p.ToggleMovement();
        }
    }

    public void SetStartingData()
    {
        foreach (PersonBehavior p in people)
        {
            p.SetStartingData(1);
        }
    }

    public IEnumerator CheckAllPeopleAssingedPaths()
    {
        int max = people.Count;

        for (int i = 0; i < max; i++)
        {
            sc.SetFeedback("Starting simulation..." + i * 100 / max + "%");
            if (people[i].GetAgent().pathPending)
            {
                i -= 1;
                yield return new WaitForSeconds(max * .005f);
            }
            
        }

        simulationReady.Raise();
    }

    public void PersonArrived(PersonBehavior p_, float timer_)
    {
        peopleArrivedCounter++;
        if(peopleArrivedCounter>=people.Count) 
        {
            timer = timer_;
            Utils.Print("Everyone's safe in "+timer_+"s !!!!!!"); 
            lastArrivedPerson = p_;
            foreach(PersonBehavior p in people) p.StopMovement();
            sc.SimulationFinished(true);
        }
    }

    public int AddFamily()
    {
        int id = currentFamilyID;
        currentFamilyID+=1;
        families.Add(new Family(id));
        return id;
    }

    public int AddKnownFamily(int id_)
    {
        if(id_>currentFamilyID) currentFamilyID = id_+1;
        families.Add(new Family(id_));
        return id_;
    }

    #region ISceneElement
    public void ChangeState(SubMenuStates state_)
    {
        state = state_;
    }

    public void DeleteAll()
    {
        foreach(PersonBehavior p in people)
        {
            GameObject.Destroy(p.gameObject);
        }
        people.Clear();
        families.Clear();
        currentFamilyID = 0;
        currentPersonID = 0;
        alreadyBaked = false;

        nextPerson.Setup(currentPersonID, 30, new List<string>(), 3f, -1, false, true, new List<int>());
        sc.PeopleCreate();
    }
    
    public void Save()
    {
		List<PersonData> personDataList = new List<PersonData>();
        List<PathData> pathDataList = new List<PathData>();

        foreach(PersonBehavior p in people)
        {
            personDataList.Add(p.TransformData());
            pathDataList.Add(p.TransformPathData());
        }

		DataController.SetPeopleData(new PeopleData(personDataList));
        DataController.SetPathsData(new PathsData(pathDataList));
    }

    public void Load()
    {
        PeopleData pd_ = DataController.GetPeopleData();
        PersonBehavior newPerson;
        int maxID = -1, maxfamID = -1;
        foreach(PersonData p in pd_.people)
        {
            if(p.ID>maxID)maxID = p.ID;
            if(p.familyID>maxfamID) maxfamID = p.familyID;
            newPerson = PlacePerson(sc.GetMap().FindSectionByID(p.initSectionID), p.initPos);
            newPerson.SetupFromData(sc.GetGraph().FindNodeByID(p.initNodeID), p.ID, p.priority, p.age, p.types, p.speed, p.firstName, p.familyID, p.dependent, p.manualDependencies, p.tutorID);
        }
        currentPersonID = maxID+1;
        currentFamilyID = maxfamID+1;

        foreach(PersonBehavior p2 in people)
        {
            p2.SetDependenciesFromData(p2.GetTutorID());
        }
        

    }

    #endregion

    public IEnumerator AddAutoPeople(int density)
    {
        DeleteAll();

        int index = 0;
        int percent = 0;
        int total = sc.GetMap().GetSections().Count;
        PersonBehavior newPerson;
        int totalPeople = 0;

        foreach (Section s in sc.GetMap().GetSections())
        {
            if (!s.GetIsCP() && sc.GetClosestNode(s.GetPos(), s))
            {
                // Calculate number of people in that section according to density value
                int peopleInSection = Mathf.FloorToInt(s.GetMaxCapacity() * density / 100);
                if (peopleInSection == 0) yield return null;
                else
                {
                    // Calculate tiles distance between people
                    int tileDistance = s.GetMaxCapacity() / peopleInSection;
                    int randomTile;
                    for (int i = 0; i < peopleInSection; i++)
                    {
                        randomTile = Random.Range(0,s.GetTiles().Count);
                        //newPerson = PlacePerson(s, s.GetTiles()[i * tileDistance].GetPos());
                        newPerson = PlacePerson(s, s.GetTiles()[randomTile].GetPos());
                        totalPeople++;
                    }
                    index++;
                    percent = (int)index * 100 / total;
                    sc.SetFeedback("Generating people... " + percent + "%");
                    yield return null;
                }
                
            }
            
        }

        peopleAutoReady.Raise();
        sc.UnlockUI();
        sc.SetFeedback(totalPeople + " people added automatically.");
    }

    public IEnumerator CheckAllPeopleWithPath()
    {
        sc.LockUI();
        sc.SetFeedback("Starting simulation...");
        for (int i=0; i<5; i++)
        {
            if (!AllPathsSet()) yield return new WaitForSeconds(i + .5f);
        }

        sc.UnlockUI();
        sc.SetFeedback("Simulation started.");
    }

    public bool AllPathsSet()
    {
        foreach (PersonBehavior p in people)
        {
            if (!p.GetDependent() && p.GetAgent().pathPending)
            {
                return false;
            }
        }
        return true;
    }

    public void LoadPathsFromData(PathsData pd_)
    {
        List<Node> nodesPath = new List<Node>();
        PersonBehavior person = null;

        foreach(PathData p in pd_.paths)
        {
            person = FindPersonByID(p.personID);
            if(person != null)
            {
                foreach(int n in p.nodes)
                {
                    nodesPath.Add(sc.GetGraph().FindNodeByID(n));
                }
                person.SetNodesPath(nodesPath);
                nodesPath = new List<Node>();
            }
        }

        alreadyBaked = true;
    }

    public List<PersonInfo> GetPersonInfos()
    {
        List<PersonInfo> personInfos = new List<PersonInfo>();
        foreach(PersonBehavior p in people)
        {
            if (p.GetDestSection()) personInfos.Add(new PersonInfo(p.GetID(), p.GetDestSection().GetID(), p.GetTimer()));
            else personInfos.Add(new PersonInfo(p.GetID(), -1, p.GetTimer()));
        }
        return personInfos;
    }

    public PersonBehavior FindPersonByID(int id_)
    {
        return people.Find(x => x.GetID() == id_);
    }

    public Family FindFamilyByID(int id_)
    {
        return families.Find(x => x.GetID() == id_);
    }
    
    public void ResetSimulation()
    {
        peopleArrivedCounter = 0;
        timer = 0f;
        lastArrivedPerson = null;

        foreach(PersonBehavior p_ in people)
        {
            p_.ResetSimulation();
        }
    }

    public void SetSceneController(SceneController sc_){sc = sc_;}

    public float GetMediaTime()
    {
        float m = 0;
        foreach(PersonBehavior p in people)
        {
            m += p.GetTimer();
        }
        return (m / people.Count);
    }

    public List<PersonBehavior> GetPeople(){return people;}
    public SceneController GetSceneController() { return sc;}
    public PersonBehavior GetLastArrivedPerson(){return lastArrivedPerson;}
    public PersonBehavior GetNextPerson(){return nextPerson;}
    public List<Family> GetFamilies(){ return families;}
}
