using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenariosManager : MonoBehaviour
{
    public GameObject scenarioPrefab;
    private SceneController[] myScenes = new SceneController[4];
    private int actualScene = 0;

    // Start is called before the first frame update
    void Start()
    {
        myScenes[0] = GameObject.Instantiate(scenarioPrefab, this.transform).GetComponent<SceneController>();
    }




    #region UIMETHODS

	public void ScenarioNew(){ myScenes[actualScene].ScenarioNew();}
	public void ScenarioLoad(){ myScenes[actualScene].ScenarioLoad();}
	public void ScenarioSave(){ myScenes[actualScene].ScenarioSave();}
	public void ScenarioSaveAs(){ myScenes[actualScene].ScenarioSaveAs();}
	public void Exit(){ myScenes[actualScene].Exit();}

	public void TopologyCreate(){ myScenes[actualScene].TopologyCreate();}
	public void TopologyEdit(){ myScenes[actualScene].TopologyEdit();}
	public void TopologyClearAll(){ myScenes[actualScene].TopologyClearAll();}
	public void TopologyClearDoors(){ myScenes[actualScene].TopologyClearDoors();}
	public void TopologyCreateSections(){ myScenes[actualScene].TopologyCreateSections();}
	public void TopologyFinish(){ myScenes[actualScene].TopologyFinish();}

	public void GraphCreate(){ myScenes[actualScene].GraphCreate();}
	public void GraphEdit(){ myScenes[actualScene].GraphEdit();}
	public void GraphMove(){ myScenes[actualScene].GraphMove();}
	public void GraphAuto(){ myScenes[actualScene].GraphAuto();}
	public void GraphClearAll(){ myScenes[actualScene].GraphClearAll();}
	public void GraphClearEdges(){ myScenes[actualScene].GraphClearEdges();}
	public void GraphFinish(){ myScenes[actualScene].GraphFinish();}
	public void GraphBack(){ myScenes[actualScene].GraphBack();}

	public void PeopleCreate(){ myScenes[actualScene].PeopleCreate();}
	public void PeopleEdit(){ myScenes[actualScene].PeopleEdit();}
	public void PeopleMove(){ myScenes[actualScene].PeopleMove(); }
    public void PeopleAuto() { myScenes[actualScene].PeopleAuto(); }
    public void PeopleClearAll(){ myScenes[actualScene].PeopleClearAll();}
	public void PeopleFinish(){ myScenes[actualScene].PeopleFinish();}
	public void PeopleBack(){ myScenes[actualScene].PeopleBack();}

	public void PathsEdit(){ myScenes[actualScene].PathsEdit();}
	public void PathsAuto(){ myScenes[actualScene].PathsAuto();}
	public void PathsSet(){ myScenes[actualScene].PathsSet(); }
    public void PathsFinish(){ myScenes[actualScene].PathsFinish();}
	public void PathsBack(){ myScenes[actualScene].PathsBack();}

	public void PlaySimulation(){ myScenes[actualScene].PlaySimulation();}
	public void PauseSimulation(){ myScenes[actualScene].PauseSimulation();}
	public void StopSimulation(){ myScenes[actualScene].StopSimulation();}
	public void SimulationBack(){ myScenes[actualScene].SimulationBack();}

	public void CamNavigate(){ myScenes[actualScene].CamNavigate();}
	public void CamChangeView(){ myScenes[actualScene].CamChangeView();}
	public void CamShowTopology(){ myScenes[actualScene].CamShowTopology();}
	public void CamShowGraph(){ myScenes[actualScene].CamShowGraph();}
	public void CamShowPeople(){ myScenes[actualScene].CamShowPeople();}
	public void CamShowLabels(){ myScenes[actualScene].CamShowLabels();}


	#endregion

}
