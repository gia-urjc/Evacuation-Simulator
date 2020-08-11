using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using TMPro;

public class PersonBehavior : MonoBehaviour
{
	private NavMeshAgent agent;
	private People peopleController;
	[SerializeField] private Node initNode;
	[SerializeField] private Section initSection;
	private Vector3 initPos;
	[SerializeField] private Section destSection;
	
	// CHARACTERISTICS
	[SerializeField] private int ID;
	[SerializeField] private string firstName;
	[SerializeField] private List<string> types = new List<string>();
	[SerializeField] private int priority, age;
	private float speed, groupSpeed;
	[SerializeField] private int familyID = -1;
	[SerializeField] private bool dependent;
	[SerializeField] private bool manualDependencies;
	[SerializeField] private bool pathFinished;
	[SerializeField] private List<int> responsibleForIDs = new List<int>();
	private List<PersonBehavior> followers = new List<PersonBehavior>();
	private PersonBehavior nextDependent;
	private int tutorID = -1;
	
	// PATH
	[SerializeField] private bool reachedDestination = false;
	[SerializeField] private List<Node> nodesPath;
	//private List<Vector3> pointsPath;
	[SerializeField] private int pathIndex;
	[SerializeField] private bool startedMovement;
    private Color personPathColor;
	
	[SerializeField] private float timer;

	public TMP_Text myText;
    private Animator anim;
	private Quaternion initialRot;

	private Renderer myRenderer;
	
	//private string[] seriousNames = {"Abby", "Anne", "Alice", "Bronn", "Bram", "Charles", "Cat", "Cris", "Diana", "Dunning", "Ernesto", "Elena", "Earl", "Finn", "Gus", "Gia", "Harry", "Iago", "Javier", "Karen", "Laura", "María", "Nina", "Oscar", "Paco", "Quike", "Ron", "Sara", "Teo", "Uma", "Val"};
	
	//private string[] names = {"Lilly", "Miguelón", "Fran", "Sandra", "Harry", "Sara", "Albertini", "Nando", "Felipe", "Buch", "Gatito", "David", "Yols"};
	//private string[] surnames = {"Liliana", "Colorao", "Vadillo", "Valverde", "Potter", "Jackson", "Pitarque", "Pitotanque", "Sexto", "Nonda", "Caraculo", "Miau"};
	
	
	void Awake () 
	{
		agent = GetComponent<NavMeshAgent>();
		nodesPath = new List<Node>();
		//pointsPath = new List<Vector3>();
		initialRot = myText.transform.rotation;
		peopleController = transform.parent.GetComponent<People>();
        anim = GetComponentInChildren<Animator>();
        myRenderer = GetComponentInChildren<Renderer>();
        if (dependent) { reachedDestination = false; pathFinished = false; }
        
        agent.avoidancePriority = Random.Range(50, 55);
        personPathColor = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f, 1f, 1f);
	}

	public void Setup(int ID_, int age_, List<string> types_, float s_, int familyID_, bool d_, bool md_, List<int> resp_)
	{
		
		ID = ID_;
		age = age_;
		speed = s_;
		groupSpeed = s_;
		familyID = familyID_;
		if(familyID_!=-1) {AddToFamily(familyID_);}
		name = "Person "+ID;
		dependent = d_; manualDependencies = md_; 
		
		responsibleForIDs = resp_;
		foreach(int rid in responsibleForIDs)
		{
			peopleController.FindFamilyByID(familyID).FindMemberByID(rid).SetTutorID(ID);
		}

		//firstName = names[Random.Range(0, names.Length-1)]+" "+surnames[Random.Range(0, surnames.Length-1)];
		//firstName = seriousNames[Random.Range(0, seriousNames.Length-1)];

		if(types_ != null) types = types_;

        UpdateTypesMaterial();
        

	}

	public void SetupFromData(Node initNode_, int ID_, int priority_, int age_, List<string> types_, 
	float speed_, string firstName_, int familyID_, bool d_, bool md_, int tID_)
	{
		ID = ID_; SetInitNode(initNode_); priority = priority_; age = age_; types = types_; speed = speed_; 
		firstName = firstName_; dependent = d_; manualDependencies = md_; groupSpeed = speed_;
		name = "Person "+ID;

		familyID = -1;
		if(familyID_ != -1) AddToFamily(familyID_);
		tutorID = tID_;

        UpdateTypesMaterial();
    }

    private void UpdateTypesMaterial()
    {
        if (types.Count == 0)
        {
            myRenderer.material = peopleController.BMat;
        }
        else
        {
            foreach (string t in types)
            {
                switch (t)
                {
                    case "Group Leader": myRenderer.material = peopleController.GLMat; break;
                    case "Mobility Impaired": myRenderer.material = peopleController.MIMat; break;
                    case "Family Member": myRenderer.material = peopleController.FMMat; break;
                    default: break;
                }
            }
        }
    }

	public void SetDependenciesFromData(int tID_)
	{
		tutorID = -1;
		if(tID_ != -1 && familyID != -1)
		{
			Family family = peopleController.FindFamilyByID(familyID);
			if(family!=null)
			{
				PersonBehavior tutor = family.FindMemberByID(tID_);
			 	if(tutor!=null) tutor.AddToTutory(this);
			}
		} 
	}

    Coroutine ChangePriorityOverTimeCoroutine;

    private void Start()
    {
        ChangePriorityOverTimeCoroutine = StartCoroutine(ChangePriorityOverTime());
    }

    private IEnumerator ChangePriorityOverTime()
    {
        while (agent.avoidancePriority < 98)
        {
            agent.avoidancePriority = agent.avoidancePriority + Random.Range(-3, 4);
            yield return new WaitForSeconds(2f);
        }
    }

	void Update()
	{
		if(startedMovement)
		{
            

            if (reachedDestination && pathIndex<nodesPath.Count && pathIndex!=-1)
			{
                WhenIndependentReachedDestination();
			}
			else if (!agent.pathPending && ((!reachedDestination && !dependent) || dependent) && agent.remainingDistance <= 3f)
			{
                
				if(!dependent) 
				{
					int followcount = followers.Count;
					nodesPath[pathIndex].RemoveOcupancy(followcount+1);

					reachedDestination = true;
					pathIndex++;
					agent.speed = groupSpeed;

					if(pathIndex<nodesPath.Count) nodesPath[pathIndex].AddOcupancy(followcount+1);
				}
				if(pathIndex>=nodesPath.Count)
				{
					startedMovement = false;
					agent.isStopped = true;
					pathFinished = true;
					timer = peopleController.GetSceneController().GetTimer();
					destSection.PersonArrived(ID, timer);
					peopleController.PersonArrived(this, timer);
                    agent.avoidancePriority = 99;
                    if(ChangePriorityOverTimeCoroutine!=null) StopCoroutine(ChangePriorityOverTimeCoroutine);


                    foreach (PersonBehavior flw in followers)
					{
						flw.SetPathFinished(true);
					}
				}
				else if(dependent)
				{
					if(pathFinished)
                    {
                        startedMovement = false;
                        agent.isStopped = true;
                        agent.avoidancePriority = 99;
                        if (ChangePriorityOverTimeCoroutine != null) StopCoroutine(ChangePriorityOverTimeCoroutine);
                        timer = peopleController.GetSceneController().GetTimer();
						destSection.PersonArrived(ID, timer);
						peopleController.PersonArrived(this, timer);
					}
				}
			}
				
		}

        myText.text = "P"+ID;
		myText.transform.rotation = initialRot;

        if (!startedMovement || agent.isStopped) anim.SetFloat("Speed", 0f);
        else anim.SetFloat("Speed", agent.speed);

	}

    public void WhenIndependentReachedDestination()
    {
        reachedDestination = false;
        nextDependent = CheckDependentsInSection();
        if (nextDependent != null)
        {
            agent.SetDestination(nextDependent.GetPos());
            
            pathIndex--;
            responsibleForIDs.Remove(nextDependent.GetID());
            followers.Add(nextDependent);
            if (nextDependent.GetSpeed() < groupSpeed) groupSpeed = nextDependent.GetSpeed();
            nextDependent.GetAgent().speed = groupSpeed;
            nextDependent.SetDestSection(destSection);
            nextDependent.startedMovement = true;
            destSection.AddPerson(nextDependent);
        }
        else { agent.SetDestination(nodesPath[pathIndex].GetPos()); }
        UpdateFollowersMovement();
    }
	
	public void SetOnFloor(Vector3 pos_, Section s_, Node n_)
	{ 
		initPos = pos_;
		initSection = s_;
		SetInitNode(n_);
		agent.Warp(pos_);
	}
	
	public void ToggleMovement()
	{
        if (destSection != null)
        {
            startedMovement = !startedMovement;
            agent.isStopped = !agent.isStopped;
        }
	}

    public void SetStartingData(int idx_)
    {
        if (nodesPath.Count > 0)
        {
            pathIndex = idx_;
            destSection = nodesPath[nodesPath.Count - 1].GetData();
            destSection.AddPerson(this);
            reachedDestination = true;
            agent.isStopped = true;
            agent.speed = speed;

            if (reachedDestination && pathIndex < nodesPath.Count && pathIndex != -1)
            {
                WhenIndependentReachedDestination();
            }
        }
        else pathIndex = -1;
    }

	public void StartMovement(int idx_)
	{
		
		if(nodesPath.Count>0) 
		{
		//	pathIndex = idx_;
		//	destSection = nodesPath[nodesPath.Count-1].GetData();
  //          destSection.AddPerson(this);
			//reachedDestination = true;

   //         agent.speed = speed;
            startedMovement = true;
            agent.isStopped = false;
        } 
    //else pathIndex = -1;
		
	}

	public void ResetSimulation()
	{
		startedMovement = false;
		agent.isStopped = true;
		pathFinished = false;
		reachedDestination = false;
		timer = 0;
		agent.ResetPath();
		pathIndex = 0;
		agent.velocity = Vector3.zero;

        destSection?.ResetSimulation();


        if (!dependent)
		{
			int c = followers.Count;
			for(int i=0; i<c; i++)
			{
				responsibleForIDs.Add(followers[0].GetID());
				followers.RemoveAt(0);
			}
		}

		agent.Warp(initPos);
	}

	private void UpdateFollowersMovement()
	{
		//agent.speed = groupSpeed;
		foreach(PersonBehavior p in followers)		
		{
			p.GetAgent().SetDestination(agent.destination);
			p.GetAgent().speed = groupSpeed;
		}
	}


	public void StopMovement()
	{
		agent.isStopped = true;
		agent.ResetPath();
	}
	

	private PersonBehavior CheckDependentsInSection()
	{
		if(!dependent && responsibleForIDs.Count>0)
		{
			Family f = peopleController.FindFamilyByID(familyID);
			PersonBehavior found = null;
			foreach(int i in responsibleForIDs)
			{
				found = f.FindMemberByID(i);
				if(found.GetInitSection() == nodesPath[pathIndex-1].GetData()) return found;
			}
			return null;
		}
		return null;
	}

	public PersonData TransformData()
	{
		return new PersonData(ID, priority, age, types, speed, firstName, initNode.GetID(), initSection.GetID(), initPos, familyID, dependent, manualDependencies, tutorID);
	}
	
	public PathData TransformPathData()
	{
		List<int> nodesIDs = new List<int>();
		foreach(Node n in nodesPath)
		{
			nodesIDs.Add(n.GetID());
		}
		return new PathData(ID, nodesIDs);
	}

	// public int AddToNewFamily()
	// {
	// 	familyID = peopleController.AddFamily();
	// 	peopleController.FindFamilyByID(familyID).AddMember(this);
	// 	return familyID;
	// }

	public void AddToFamily(int id_)
	{
		// Remove this person from the family he/she was assigned
		if(familyID != -1) 
		{
			if(gameObject.activeSelf) RemoveFromFamily();
			else familyID = -1;
		}

		Family family = peopleController.FindFamilyByID(id_);

		// If the family exists
		if(family != null)
		{
			Debug.Log("I join to this family!");
			familyID = id_;
			if(gameObject.activeSelf)
			{
				family.AddMember(this);
			}
		}
		// If the family does not exist
		else
		{
			Debug.Log("Im loading data and i needed to create one family");
			int newID = peopleController.AddKnownFamily(id_);
            AddToFamily(newID);
		}
	}

	public void RemoveFromFamily()
	{
		if(familyID != -1)
		{
			RemoveAllTutories();
			tutorID = -1;
			peopleController.FindFamilyByID(familyID).RemoveMember(this);
			familyID = -1;
		}
	}

	public void AddToTutory(PersonBehavior p_)
	{
		responsibleForIDs.Add(p_.GetID());
		p_.SetTutorID(ID);
	}

	public void RemoveFromTutory(PersonBehavior p_)
	{
		responsibleForIDs.Remove(p_.GetID());
		p_.SetTutorID(-1);
	}

	public void RemoveAllTutories()
	{
		Family f = peopleController.FindFamilyByID(familyID);
		foreach(int i in responsibleForIDs)
		{
			f.FindMemberByID(i).SetTutorID(-1);
		}
		responsibleForIDs.Clear();
	}

	public bool IsPersonInDependents(int id_)
	{
		return responsibleForIDs.Contains(id_);
	}
	
	/*
	void OnMouseOver()
	{
		if(EventSystem.current.IsPointerOverGameObject()){
			return;
		}
		string text = "ID: "+ID+"\nName: "+firstName+"\nType: "+type+"\nSpeed: "+speedType+"\nPath: ";
		foreach(Section s in sectionsPath)
		{
			text += s.name+", ";
		}
		sc.ToggleText(text);
	}
	
	void OnMouseExit()
	{
		sc.ToggleText("");
	}
	*/
	
	#region GETTERS & SETTERS
		
	public NavMeshAgent 	GetAgent() {return agent;}
	public string 			GetPersonName(){return firstName;}
	public List<string>		GetPersonTypes(){ return types;}
	public Vector3 			GetInitPos() { return initPos;}
	public int 				GetID(){return ID;}
	public int 				GetAge(){ return age;}
	public bool 			GetStartedMovement(){ return startedMovement;}
	public Node 			GetInitNode(){return initNode;}
	public List<Node>		GetNodesPath(){return nodesPath;}
	public float 			GetTimer(){return timer;}
	public Section 			GetDestSection(){ return destSection;}
	public bool 			GetDependent(){ return dependent;}
	public bool 			GetManual(){ return manualDependencies;}
	public float 			GetSpeed(){	return speed;}
	public People			GetPeopleController(){ return peopleController;}
	public int 				GetFamilyID(){ return familyID;}
	public Family			GetFamily(){ return peopleController.FindFamilyByID(familyID);}
	public int 				GetTutorID(){ return tutorID;}
	public List<int>		GetResponsibleForIDs(){ return responsibleForIDs;}
	public Section			GetInitSection(){ return initSection;}
	public Vector3			GetPos(){ return transform.position;}
	public Color            GetPathColor() { return personPathColor; }
	
	public void 	SetAgentSpeed(){ agent.speed = GetSpeed();}

	public bool 	HasFinishedPath(){return pathFinished;}
	public void 	SetTimer(float t_){ timer = t_;}
	public void 	SetID(int id_){ID = id_; /*firstName = "person"+ID;*/}
	public void 	SetStartedMovement(bool s_){ startedMovement = s_;}
	public void 	SetInitSection(Section s_){initSection = s_;}
	public void 	SetName(string name_){ firstName = name_; /*textHolder.label.text = name_;*/}
	public void 	SetInitPos(Vector3 pos_){ initPos = pos_; transform.position = pos_;}
	public void 	SetInitNode(Node n_){initNode = n_; initNode.AddOcupancy(1);}
	public void 	SetNodesPath(List<Node> np_){nodesPath = np_;}
	//public void 	SetPointsPath(List<Vector3> pp_){pointsPath = pp_;}
	public void 	SetDestSection(Section s_){destSection = s_;}
	public void 	SetAge(int a_){ age = a_;}
	public void 	SetSpeed(float s_){speed = s_;}
	public void 	AddPersonType(string p_){ types.Add(p_); UpdateTypesMaterial(); }
	public void 	RemovePersonType(string p_)
	{ 
		if(p_=="Family Member")
		{
			RemoveFromFamily();
			SetDependent(false); 
		}  
		types.Remove(p_);

        UpdateTypesMaterial();
	}
	public void 	SetManual(bool m_){ manualDependencies = m_;}
	public void 	SetDependent(bool d_)
	{
		if(dependent && !d_ && familyID!=-1 && tutorID!=-1)
		{
			peopleController.FindFamilyByID(familyID).FindMemberByID(tutorID).RemoveFromTutory(this);
		}
		else if(!dependent && d_ && familyID!=-1)
		{
			RemoveAllTutories();
		}
		dependent = d_;
	}
	public void 	SetFamilyID(int id_){ familyID = id_;}
	public void 	SetTutorID(int id_){ tutorID = id_;}
	public void 	AddResponsibility(int id_)
	{
		PersonBehavior tutored = peopleController.FindFamilyByID(familyID).FindMemberByID(id_);
		AddToTutory(tutored);
	}
	public void 	RemoveResponsibility(int id_)
	{
		PersonBehavior tutored = peopleController.FindFamilyByID(familyID).FindMemberByID(id_);
		RemoveFromTutory(tutored);
	}
	public void 	ClearResponsibilities(){ responsibleForIDs.Clear();}
	public void 	SetPathFinished(bool pf_){ pathFinished = pf_;}

	public void 	SetText(TMP_Text t_){ myText = t_;}
	
	#endregion
	
	public void Print(string msg){Debug.Log(Random.Range(1000, 2000)+" PERSON:  "+msg);}
	
}