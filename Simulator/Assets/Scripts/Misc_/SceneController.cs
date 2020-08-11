using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;
using System;

public class SceneController : MonoBehaviour
{
	public GameObject mapPrefab, graphPrefab, peoplePrefab;
	[SerializeField] private string filename;
	
	// Building's dimensions
	[SerializeField] private int width = 40, height = 24; // Number of tiles
	private float size = 1f; // Size of each tile
	
	//private ISceneElement[] sceneElements;
	private List<ISceneElement> sceneElements;
	//private enum SceneElementEnum {mapElement, graphElement, peopleElement}
	private enum SceneElementEnum {map, graph, people}
	private SceneElementEnum currentSceneElement;
	private ISceneElement currentEC;
	//private int totalModes = 3;
	
	[SerializeField] private MenuStates currentMenuState;
	[SerializeField] private SubMenuStates currentSubMenuState;
	private Menu menu;

	private Map map;
	private Graph graph;
	private People people;
	private SceneInfo sceneInfo;
    private SceneInfoBitForTest sceneInfoBitForTest;
    private List<SceneInfoBitForTest> sceneInfoBitForTestList;
    private SceneInfoForTest sceneInfoForTest;


    private float timer, lastTime;
    Stopwatch generalStopWatch = new Stopwatch();
    float generalStartTime;
    private bool simulationStarted, simulationPaused, simulationFinished;

	private CameraMovement cam;
	private Vector3 originalCameraPosition, isoCameraPosition;
	private Quaternion originalCameraRotation, isoCameraRotation;
	private bool isoCam = false;

	private IAlgorithm algorithm;
    private bool loading;
    private int myupto_;

    public int res = 30;
    public int test, actReps;
    public int maxTests, reps, inc, minTests;
    public bool testing = false;
    //private TextInfoController textInfoController;

    void Awake()
	{
		cam = GameObject.Find("Main Camera").GetComponent<CameraMovement>();
		originalCameraPosition = cam.transform.position; 
		originalCameraRotation = cam.transform.rotation;
		menu = GameObject.Find("UI").GetComponent<Menu>();
        //textInfoController = GameObject.Find("TextInfoController").GetComponent<TextInfoController>();

        if (testing) test = minTests;
        else test = 0;
    }

	void Start()
	{
        //algorithm = new DummyAlgorithm();
        algorithm = new DummyOnlyIndependent();

        generalStopWatch.Start();
        SetFeedback("Creating new " + width * .5 + " x " + height * .5 + " scenario.");
        LockUI();
        generalStartTime = (float)generalStopWatch.Elapsed.TotalSeconds;
        NewScenario(width, height);
		menu.SetTimer(timer);
		simulationPaused = true;
        Time.timeScale = 1;
    }

    
	void Update()
    {
		// Capture input events
		ManageMouse();

        if (Input.GetKey(KeyCode.LeftControl)) {
            if (Input.GetKeyDown(KeyCode.T))
            {
                sceneInfoBitForTestList = new List<SceneInfoBitForTest>();
                BeginTest();
            }
        }

        //if(simulationStarted && !simulationPaused && !simulationFinished) {timer += (Time.time-lastTime); menu.SetTimer(timer);}
        //      lastTime = Time.time;

        if (simulationStarted && !simulationPaused && !simulationFinished) { timer = (float)stopWatch.Elapsed.TotalSeconds; menu.SetTimer(timer); }
        lastTime = Time.time;
    }
    
    private void BeginTest()
    {
        if(actReps<1)
        {
            actReps = reps;
            test += inc;
        }
        actReps--;
        AddPeopleAuto(test);
        PeopleFinish();
    }

    public void ManageMouse()
	{
		// Name mouse events and pass them
		if(currentMenuState != MenuStates.cam && currentMenuState != MenuStates.sim)
		{
			if(Input.GetMouseButtonDown(0)) currentEC.MouseEvent(Utils.MouseInputEvents.left_down);
			else if(Input.GetMouseButtonUp(0)) currentEC.MouseEvent(Utils.MouseInputEvents.left_up);
			else if(Input.GetMouseButton(0)) currentEC.MouseEvent(Utils.MouseInputEvents.left_held);
			else if(Input.GetMouseButtonUp(1)) currentEC.MouseEvent(Utils.MouseInputEvents.right_up);
		}
	}

    private string tempAlgTime = "";

	public void RunAlgorithm()
	{
        float max = people.GetPeople().Count;
        LockUI();
        SetFeedback("Generating paths...");
        StartCoroutine(MyCountdown(max * .01f, "Generated paths"));

        float beforeRun = (float)generalStopWatch.Elapsed.TotalSeconds;
        

		List<Path> paths = algorithm.FindPaths(graph, people.GetPeople());

		foreach(Path p in paths)
		{
			p.person.SetNodesPath(p.path);
			//p.path[p.path.Count-1].GetData().AddPerson(p.person);
		}
        
        tempAlgTime = ((float)generalStopWatch.Elapsed.TotalSeconds - beforeRun) + "s.";
    }

    private IEnumerator MyCountdown(float t, string msg)
    {
        yield return new WaitForSeconds(t);
        UnlockUI();
        SetFeedback(msg + " in "+tempAlgTime);
    }

	public void SimulationFinished(bool finished_)
	{
        stopWatch.Stop();
        timer = (float)stopWatch.Elapsed.TotalSeconds;
        UnityEngine.Debug.Log("RunTime " + timer);
        simulationFinished = finished_;
        if((test > 0 && simulationFinished) || test <= 0) GenerateInfo();
        
		menu.SetPlayText(simulationPaused);
        SetFeedback("Simulation finished.");


        if (test>0 && simulationFinished)
        {
            UnityEngine.Debug.Log("preparado para el siguiente test");
            StartCoroutine(ResetTest());
        }
        
    }
    private WaitForSeconds waitForSeconds;
    private IEnumerator ResetTest()
    {
        UnityEngine.Debug.Log("reseting test...");
        waitForSeconds = new WaitForSeconds(people.GetPeople().Count * 0.0075f);
        simulationStarted = false;
        simulationPaused = true;
        simulationFinished = false;

        yield return waitForSeconds;
        SimulationBack();
        yield return waitForSeconds;
        PathsBack();
        yield return waitForSeconds;
        PeopleBack();
        yield return waitForSeconds;
        if (test < maxTests || (test == maxTests && actReps>0) )
        {
            //if (test % 10 == 0)
            //{
            //    sceneInfoForTest = new SceneInfoForTest(sceneInfoBitForTestList);
            //    DataController.SetInfoDataForTest(sceneInfoForTest);
            //    DataController.SaveInfoForTest();
            //}
            BeginTest();
        }
        else
        {
            UnityEngine.Debug.Log("Fin del test");

            sceneInfoForTest = new SceneInfoForTest(sceneInfoBitForTestList);
            UnityEngine.Debug.Log("sceneInfoForTest obtained");
            DataController.SetInfoDataForTest(sceneInfoForTest);
            DataController.SaveInfoForTest();
        }

    }

    public void GenerateInfo()
    {
        if (test>0)
        {
            sceneInfoBitForTest = new SceneInfoBitForTest(people.GetPeople().Count, timer, people.GetMediaTime());
            sceneInfoBitForTestList.Add(sceneInfoBitForTest);
        }
        else
        {
            foreach (Section s in map.GetSections()) { s.GetFinalResults(); }
            sceneInfo = new SceneInfo(simulationFinished, timer, map.GetSections().Count, map.GetCPSections().Count, graph.GetNodes().Count, graph.GetEdges().Count, people.GetPeople().Count, "", map.GetSectionInfos(), people.GetPersonInfos());
            DataController.SetInfoData(sceneInfo);
            DataController.SaveInfo();
            if (pathsTexture != null) DataController.SaveImage(pathsTexture, "pathsVisual_" + System.DateTime.Now.ToString("MM.dd.yyyy_HH.mm.ss"));
        }
    }

	public void DeleteAll()
	{
		GameObject.Destroy(map.gameObject);
		GameObject.Destroy(graph.gameObject);
		GameObject.Destroy(people.gameObject);
		// map.DeleteAll();
		// people.DeleteAll();
		// graph.DeleteAll();
	}

    public void LockUI()
    {
        menu.LockUI();
    }
    public void UnlockUI()
    {
        menu.UnlockUI();
    }
    public void SetFeedback(string msg)
    {
        menu.SetFeedback(msg);
    }

    public void NewScenario(int w_, int h_)
	{

        simulationStarted = false;
        simulationPaused = true;
        simulationFinished = false;
        menu.SetPlayText(simulationPaused);


        width = w_; height = h_;
		if(map!=null) 
		{
			DeleteAll();
		}

		// Create the building/map passing the specified dimensions
		map = GameObject.Instantiate(mapPrefab, this.transform).GetComponent<Map>();
		map.Setup(this, width, height, size);

		graph = GameObject.Instantiate(graphPrefab, this.transform).GetComponent<Graph>();
		graph.Setup(this);

		people = GameObject.Instantiate(peoplePrefab, this.transform).GetComponent<People>();
		people.Setup(this);

        // scene info creation?
        

        sceneElements = new List<ISceneElement>();
		sceneElements.Add(map);
		sceneElements.Add(graph);
		sceneElements.Add(people);

		currentSceneElement = SceneElementEnum.map;
		currentEC = sceneElements[(int)currentSceneElement];

		currentMenuState = MenuStates.cam;
		currentSubMenuState = SubMenuStates.create;

		isoCameraPosition = new Vector3 (0,14,0);
		isoCameraRotation = Quaternion.Euler(new Vector3(45,45,0));

        //DataController.SetProjectName(filename);

        graph.gameObject.SetActive(true);

    }

    public void FinishedNewScenario()
    {
        if(loading)
        {
            map.Load();
            if(myupto_ >= 0)
            {
                ChangeToTopology(SubMenuStates.create);
            }
            if (myupto_ >= 1)
            {
                TopologyFinish();
                DataController.LoadGraph();
                graph.Load();
            }
            if (myupto_ >= 2)
            {
                GraphFinish();
                DataController.LoadPeople();
                people.Load();
            }
            if (myupto_ >= 3)
            {
                PeopleFinish();
                DataController.LoadPaths();
                people.LoadPathsFromData(DataController.GetPathsData());
            }
            if (myupto_ >= 4)
            {
                PathsFinish();
            }

            DataController.LoadInfo();
            sceneInfo = DataController.GetInfoData();
            if (sceneInfo == null) sceneInfo = new SceneInfo();

            loading = false;

            UnlockUI();
            SetFeedback("Scenario loaded in "+((float)generalStopWatch.Elapsed.TotalSeconds - generalStartTime) +"s.");
        }
        else
        {
            UnlockUI();
            SetFeedback("New " + width * .5 + " x " + height * .5 + " scenario created in " + ((float)generalStopWatch.Elapsed.TotalSeconds - generalStartTime) + "s.");
            ChangeToTopology(SubMenuStates.create);

            filename = "NoName";
            DataController.SetProjectName(filename);
        }

        timer = 0;
        menu.SetTimer(timer);
        menu.SetFilename(filename + " " + width*.5f + "x" + height*.5f);

    }

    public void FinishedCreatingSections()
    {
        UnlockUI();
        SetFeedback(map.GetSections().Count+" sections have been created in " + ((float)generalStopWatch.Elapsed.TotalSeconds - generalStartTime) + "s.");
    }

	public void Load(string name_, int upto_)
	{
        generalStartTime = (float)generalStopWatch.Elapsed.TotalSeconds;

        loading = true;
        myupto_ = upto_;

        //DeleteAll();
        filename = name_;
		DataController.SetProjectName(filename);

		DataController.LoadMap();
		MapData md_ = DataController.GetMapData();
		NewScenario(md_.width*2, md_.height*2);
        

    }

	public void Save()
	{
        generalStartTime = (float)generalStopWatch.Elapsed.TotalSeconds;

        DataController.SetProjectName(filename);

		// Topology
		map.Save();
		DataController.SaveMap();

		// Graph
		graph.Save();
		DataController.SaveGraph();

		// People and paths
		people.Save();
		DataController.SavePeople();
		DataController.SavePaths();

        // Info
        GenerateInfo();
        //sceneInfo.totalTime = timer;
        //if(people.GetLastArrivedPerson()) sceneInfo.maximumCPTime = people.GetLastArrivedPerson().GetTimer();
        //sceneInfo.timesPerPerson = people.GetTimesInfo();
        
        //DataController.SetInfoData(sceneInfo);
        //Debug.Log(sceneInfo.percentBasic);
        //DataController.SaveInfo();

        // Unlock UI interaction
        UnlockUI();
        menu.SetFeedback("Saved scenario in "+((float)generalStopWatch.Elapsed.TotalSeconds - generalStartTime) +"s.");
    }

	public void SaveAs(string name_, string description_)
	{
		filename = name_;
        menu.SetFilename(filename + " " + width * .5f + "x" + height * .5f);
        Save();
		// DateTime theTime = DateTime.Now;
		// string date = theTime.ToString("yyyy-MM-dd\\Z");
		// string time = theTime.ToString("HH:mm:ss\\Z");
		// string datetime = theTime.ToString("yyyy-MM-dd\\THH:mm:ss\\Z");
	}


    public void AddPeopleAuto(int density)
    {
        LockUI();
        StartCoroutine(people.AddAutoPeople(density));
    }

    public void OnPeopleAdded()
    {
        if (test<=0) return;
        PathsAuto();
        PathsFinish();
        PlaySimulation();
    }

    public Node GetClosestNode(Vector3 pos, Section s) { return graph.GetClosestNode(pos, s); }

	// public TMP_Text AddInfoText(string t_, Vector3 pos_)
	// {
	// 	return textInfoController.AddInfoText(t_, pos_);
	// }

	public Map GetMap(){return map;}
	public Graph GetGraph(){return graph;}
	//public SceneInfo GetSceneInfo(){return sceneInfo;}
	public float GetTimer(){return timer;}
    public int GetWidth() { return width; }
    public int GetHeight() { return height; }

	#region UI related methods
	public void PersonAdded(PersonBehavior actualPerson_, PersonBehavior nextPerson_)
	{
		// change stats
		//sceneInfo.PersonAdded(actualPerson_.GetPersonTypes());

		// change menu
		menu.EnableEditMenu(nextPerson_.gameObject);
	}

	public void EnableEditMenu(GameObject element)
	{
		menu.EnableEditMenu(element);
	}

	private void OpenNewScenarioMenu()
	{
		menu.OpenNewScenarioMenu();
	}

	private void OpenLoadScenarioMenu()
	{
		menu.OpenLoadScenarioMenu();
	}

	private void OpenSaveScenarioMenu()
	{
		menu.OpenSaveScenarioMenu();
	}

	#endregion

	#region State changing methods

	private void ChangeToTopology(SubMenuStates state_)
	{
		if(currentMenuState != MenuStates.map)
		{
			currentMenuState = MenuStates.map;
			currentEC = sceneElements[0];
			
			menu.EnableMenu("TopoButton");
			menu.DisableMenu("GraphButton");
			menu.DisableMenu("PeopleButton");
			menu.DisableMenu("PathsButton");
			menu.DisableSimulatorButtons();
			
			cam.DisableMovement();
		}

		menu.CloseEditMenus();
		currentSubMenuState = state_;
		currentEC.ChangeState(currentSubMenuState);
	}

	private void ChangeToGraph(SubMenuStates state_)
	{
		if(currentMenuState != MenuStates.graph)
		{
			currentMenuState = MenuStates.graph;
			currentEC = sceneElements[1];

			menu.EnableMenu("GraphButton");
			menu.DisableMenu("TopoButton");
			menu.DisableMenu("PeopleButton");
			menu.DisableMenu("PathsButton");
			menu.DisableSimulatorButtons();

			cam.DisableMovement();
		}
        graph.gameObject.SetActive(true);
		menu.CloseEditMenus();
		currentSubMenuState = state_;
		currentEC.ChangeState(currentSubMenuState);
	}

	private void ChangeToPeople(SubMenuStates state_)
	{
		if(currentMenuState != MenuStates.people)
		{
			currentMenuState = MenuStates.people;
			currentEC = sceneElements[2];

			menu.EnableMenu("PeopleButton");
			menu.DisableMenu("TopoButton");
			menu.DisableMenu("GraphButton");
			menu.DisableMenu("PathsButton");
			menu.DisableSimulatorButtons();

			cam.DisableMovement();
		}

		menu.CloseEditMenus();
		currentSubMenuState = state_;
		currentEC.ChangeState(currentSubMenuState);
		if(state_ == SubMenuStates.create) EnableEditMenu(people.GetNextPerson().gameObject);
	}

	private void ChangeToPaths(SubMenuStates state_)
	{
		if(currentMenuState != MenuStates.paths)
		{
			currentMenuState = MenuStates.paths;
			currentEC = sceneElements[2];

			menu.EnableMenu("PathsButton");
			menu.DisableMenu("TopoButton");
			menu.DisableMenu("GraphButton");
			menu.DisableMenu("PeopleButton");
			menu.DisableSimulatorButtons();

			cam.DisableMovement();
		}
		
		menu.CloseEditMenus();
		currentSubMenuState = state_;
		currentEC.ChangeState(currentSubMenuState);
        GameObject pgo = new GameObject("alg");
        if (state_ == SubMenuStates.edit) EnableEditMenu(pgo);
    }

	private void ChangeToSimulation()
	{
		if(currentMenuState != MenuStates.sim)
		{
			currentMenuState = MenuStates.sim;

			menu.CloseEditMenus();
			menu.EnableSimulatorButtons();
			menu.DisableMenu("TopoButton");
			menu.DisableMenu("GraphButton");
			menu.DisableMenu("PeopleButton");
			menu.DisableMenu("PathsButton");

			cam.DisableMovement();
		}
	}

	private void ChangeToCam()
	{
		if(currentMenuState != MenuStates.cam)
		{
			menu.CloseEditMenus();
			currentMenuState = MenuStates.cam;
		}
	}

	#endregion

	#region UIMETHODS

	// FILE
	public void ScenarioNew(){ UnityEngine.Debug.Log("New scenario"); OpenNewScenarioMenu();}
	public void ScenarioLoad(){ UnityEngine.Debug.Log("Load scenario"); OpenLoadScenarioMenu();}
	public void ScenarioSave(){ UnityEngine.Debug.Log("Save scenario"); if(filename=="NoName") ScenarioSaveAs(); else Save();}
	public void ScenarioSaveAs(){ UnityEngine.Debug.Log("Save scenario as"); OpenSaveScenarioMenu();}
	public void Exit(){ Application.Quit();}

	// TOPOLOGY
	public void TopologyCreate(){ UnityEngine.Debug.Log("Topology create mode"); ChangeToTopology(SubMenuStates.create);}
	public void TopologyEdit(){ UnityEngine.Debug.Log("Topology edit mode"); ChangeToTopology(SubMenuStates.edit);}
	public void TopologyClearAll(){ UnityEngine.Debug.Log("Clear all topology"); ChangeToTopology(SubMenuStates.create); map.DeleteAllConstructions();}
	public void TopologyClearDoors(){ UnityEngine.Debug.Log("Clear all doors"); ChangeToTopology(SubMenuStates.create); map.DeleteDoors();}
	public void TopologyCreateSections(){ UnityEngine.Debug.Log("Create sections"); if (!map.AreSectionsBuilt()) { ChangeToTopology(SubMenuStates.create); LockUI(); StartCoroutine(map.BuildSections()); generalStartTime = (float)generalStopWatch.Elapsed.TotalSeconds; } else SetFeedback("Sections are already built! Clear all to re-build."); }
	public void TopologyFinish()
    {
        if (map.AreSectionsBuilt() && map.GetRandomCP())
        {
            UnityEngine.Debug.Log("Finish topology");
            map.BuildNavMesh();
            ChangeToGraph(SubMenuStates.create);
        }
        else SetFeedback("Make sure that there is at least one section marked as Collection Point (CP)"); }

	// GRAPH
	public void GraphCreate(){ UnityEngine.Debug.Log("Graph create mode"); ChangeToGraph(SubMenuStates.create);}
	public void GraphEdit(){ UnityEngine.Debug.Log("Graph edit mode"); ChangeToGraph(SubMenuStates.edit);}
	public void GraphMove(){ UnityEngine.Debug.Log("Graph move mode"); ChangeToGraph(SubMenuStates.move);}
	public void GraphAuto(){ UnityEngine.Debug.Log("Create graph automatically"); ChangeToGraph(SubMenuStates.edit); graph.Bake(); }
	public void GraphClearAll(){ UnityEngine.Debug.Log("Clear all graph"); ChangeToGraph(SubMenuStates.create); graph.DeleteAll();}
	public void GraphClearEdges(){ UnityEngine.Debug.Log("Clear all edges"); ChangeToGraph(SubMenuStates.create); graph.DeleteEdges();}
	public void GraphFinish(){ UnityEngine.Debug.Log("Finish graph"); ChangeToPeople(SubMenuStates.create);}
	public void GraphBack(){ graph.DeleteAll(); ChangeToTopology(SubMenuStates.edit);}

	// PEOPLE
	public void PeopleCreate(){ UnityEngine.Debug.Log("People create mode"); ChangeToPeople(SubMenuStates.create);}
	public void PeopleEdit(){ UnityEngine.Debug.Log("People edit mode"); ChangeToPeople(SubMenuStates.edit);}
	public void PeopleMove(){ UnityEngine.Debug.Log("People move mode"); ChangeToPeople(SubMenuStates.move);}
    public void PeopleAuto() { UnityEngine.Debug.Log("Create auto people"); ChangeToPeople(SubMenuStates.edit); EnableEditMenu(null); }
    public void PeopleClearAll(){ UnityEngine.Debug.Log("Clear all people"); ChangeToPeople(SubMenuStates.create); people.DeleteAll();}
	public void PeopleFinish(){ UnityEngine.Debug.Log("Finish people"); ChangeToPaths(SubMenuStates.create);}
	public void PeopleBack(){people.DeleteAll(); ChangeToGraph(SubMenuStates.edit);}

	// PATHS
	public void PathsEdit(){ UnityEngine.Debug.Log("Paths edit mode"); ChangeToPaths(SubMenuStates.edit);}
	public void PathsAuto(){ UnityEngine.Debug.Log("Create paths automatically"); ChangeToPaths(SubMenuStates.create); people.Bake();}
    public void PathsSet() { ChangeToPaths(SubMenuStates.edit); }
	public void PathsFinish(){ UnityEngine.Debug.Log("Finish paths"); ChangeToSimulation();}
	public void PathsBack(){ ChangeToPeople(SubMenuStates.edit);}

	// SIMULATION
	public void PlaySimulation()
	{
        
        stopWatch.Reset();
		ChangeToSimulation(); 
		if(simulationStarted) people.ToggleMovement();
		if(!simulationStarted)
		{
            generalStartTime = (float)generalStopWatch.Elapsed.TotalSeconds;
            LockUI();
            SetFeedback("Starting simulation...");
            people.SetStartingData();
            StartCoroutine(people.CheckAllPeopleAssingedPaths());
		}
		simulationPaused = !simulationPaused;
		menu.SetPlayText(simulationPaused);
	}

    private WaitForSeconds LOSChecking = new WaitForSeconds(.5f);
    private WaitForSeconds PathsChecking = new WaitForSeconds(.1f);
    private IEnumerator CheckLOSCoroutine, CheckPathsCoroutine;
    Stopwatch stopWatch = new Stopwatch();
    public void PlaySimulationReady()
    {
        UnlockUI();
        SetFeedback("Simulation started in "+((float)generalStopWatch.Elapsed.TotalSeconds - generalStartTime) + "s.");
        people.StartMovement();
        simulationStarted = true;

        stopWatch.Start();

        CheckLOSCoroutine = CheckLOS();
        StartCoroutine(CheckLOSCoroutine);

        CheckPathsCoroutine = CheckPaths();
        StartCoroutine(CheckPathsCoroutine);
    }

    private IEnumerator CheckLOS()
    {
        Section s;
        while(simulationStarted)
        {
            foreach(PersonBehavior p in people.GetPeople())
            {
                s = map.IsPosSection(p.GetPos());
                s?.AddPerson();
            }
            foreach(Section section in map.GetSections())
            {
                section.CalculateDensity();
                section.ResetTick();
            }

            yield return LOSChecking;
        }
    }

    private Texture2D pathsTexture;
    public float allowedSpeedPerc;
    private int ticks = 0, lowTicks = 0;
    private IEnumerator CheckPaths()
    {
        PersonBehavior speedPerson;
        float minSpeed;
        ticks = 0; lowTicks = 0;
        
        pathsTexture = new Texture2D(width*res, height*res);
        //Color lastColor = Color.clear;
        //Color newColor;

        Color[] pix = pathsTexture.GetPixels(); // get pixel colors
        for (int i = 0; i < pix.Length; i++)
        {
            pix[i].a = pix[i].grayscale; // set the alpha of each pixel to the grayscale value
            pix[i] = Color.clear;
        }
        pathsTexture.SetPixels(pix);

        Color color = new Color(1, 0, 0, 1/people.GetPeople().Count);
        color = Color.red;
        while (simulationStarted)
        {
            if (simulationPaused) yield return PathsChecking;
            foreach (PersonBehavior p in people.GetPeople())
            {
                //newColor = (lastColor + color) / 2;
                pathsTexture.SetPixel(Mathf.RoundToInt(p.GetPos().x*res), Mathf.RoundToInt(p.GetPos().z*res), p.GetPathColor());
                //lastColor = newColor;
            }
            speedPerson = people.GetPeople()[UnityEngine.Random.Range(0,people.GetPeople().Count)];
            if (!speedPerson.HasFinishedPath())
            {
                ticks++;
                minSpeed = allowedSpeedPerc * speedPerson.GetAgent().desiredVelocity.magnitude;
                if (speedPerson.GetAgent().velocity.magnitude < minSpeed) lowTicks++;
            }
            yield return PathsChecking;
        }
        
        pathsTexture.Apply();
    }

    public void SetAlgorithm(string name)
    {
        switch(name)
        {
            case "Closest CP": algorithm = new DummyOnlyIndependent(); break;
            case "Random CP": algorithm = new AlgorithmB(); break;
            //case "Algorithm C": algorithm = new AlgorithmC(); break;
            default: break;
        }
    }


    public void PauseSimulation(){}
	public void StopSimulation()
	{
        SimulationFinished(false);

        ChangeToSimulation();
		map.ResetSimulation();
		people.ResetSimulation(); 
		simulationStarted = false;
		simulationPaused = true;
		simulationFinished = false;
		menu.SetPlayText(simulationPaused);
        
        timer = 0;
        menu.SetTimer(timer);
    }
	public void SimulationBack(){ StopSimulation(); ChangeToPaths(SubMenuStates.create); }

	// CAM
	public void CamNavigate(){ UnityEngine.Debug.Log("Navigate with cam"); ChangeToCam(); cam.EnableMovement();}
	public void CamChangeView(){ UnityEngine.Debug.Log("Change cam view");isoCam = !isoCam; if(isoCam){ cam.transform.position = isoCameraPosition; cam.transform.rotation = isoCameraRotation;} else { cam.transform.position = originalCameraPosition; cam.transform.rotation = originalCameraRotation;}}
	public void CamShowTopology(){ UnityEngine.Debug.Log("Toggle show topology");}
	public void CamShowGraph(){ UnityEngine.Debug.Log("Toggle show graph"); graph.gameObject.SetActive(!graph.gameObject.activeSelf); }
	public void CamShowPeople(){ UnityEngine.Debug.Log("Toggle show people");}
	public void CamShowLabels(){ UnityEngine.Debug.Log("Toggle show labels");}


	#endregion


}