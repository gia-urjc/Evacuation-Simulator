using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EditMenuPerson : EditMenu
{
    public TMP_Text IDText, ageText, speedText, typesText, responsibleText;
    public Slider ageSlider, speedSlider;
    public Toggle manualToggle, dependentToggle;
    public GameObject familyMenu, independentMenu;
    public TMP_Dropdown addTypeDD, removeTypeDD, addRespDD, removeRespDD, familyIdDD;
    public Button addFamilyBtn;

    private PersonBehavior p;
    private List<string> typesOptions = new List<string>(){"Add...","Group Leader","Mobility Impaired","Family Member","New..."};
    
    void Start()
    {
        //Add listeners and invoke a method when the values change.
        ageSlider.onValueChanged.AddListener(delegate {AgeValueChangeCheck();});
        speedSlider.onValueChanged.AddListener(delegate {SpeedValueChangeCheck();});
        addTypeDD.onValueChanged.AddListener(delegate{AddTypeCheck();});
        removeTypeDD.onValueChanged.AddListener(delegate{RemoveTypeCheck();});
        addFamilyBtn.onClick.AddListener(AddFamily);
        familyIdDD.onValueChanged.AddListener(delegate{SetToFamily();});
        manualToggle.onValueChanged.AddListener(delegate{ManualChangeCheck();});
        dependentToggle.onValueChanged.AddListener(delegate{DependentChangeCheck();});
        addRespDD.onValueChanged.AddListener(delegate{AddDependentCheck();});
        removeRespDD.onValueChanged.AddListener(delegate{RemoveDependentCheck();});

    }

    override public void SetEditableElement(GameObject element_)
    {
        p = element_.GetComponent<PersonBehavior>();
        if(p != null)
        {
            addTypeDD.onValueChanged.RemoveAllListeners();
            removeTypeDD.onValueChanged.RemoveAllListeners();
            familyIdDD.onValueChanged.RemoveAllListeners();
            addRespDD.onValueChanged.RemoveAllListeners();
            removeRespDD.onValueChanged.RemoveAllListeners();
            familyMenu.SetActive(false);
            independentMenu.SetActive(false);

            IDText.text = "ID: "+p.GetID();
            ageText.text = "Age: "+p.GetAge();
            ageSlider.value = p.GetAge();
            speedText.text = "Speed: "+p.GetSpeed()/2;
            speedSlider.value = p.GetSpeed()/2;
            UpdateTypes();
            manualToggle.isOn = p.GetManual();
            dependentToggle.isOn = p.GetDependent();
            UpdateFamilyMenu();

            addTypeDD.onValueChanged.AddListener(delegate{AddTypeCheck();});
            removeTypeDD.onValueChanged.AddListener(delegate{RemoveTypeCheck();});
            familyIdDD.onValueChanged.AddListener(delegate{SetToFamily();});
            addRespDD.onValueChanged.AddListener(delegate{AddDependentCheck();});
            removeRespDD.onValueChanged.AddListener(delegate{RemoveDependentCheck();});
           
        }   
    }

    private void UpdateFamilyMenu()
    {
        foreach(string t in p.GetPersonTypes())
        {
            if(t == "Family Member")
            {
                familyMenu.SetActive(true);
                UpdateFamilyIDs();
                UpdateIndependentMenu();
               
                return;
            }
        }
        familyMenu.SetActive(false);
    }

    private void UpdateFamilyIDs()
    {
        var idOptions = new List<string>();
        idOptions.Add("ID...");
        int i=1, v=0;
        familyIdDD.ClearOptions();
        foreach(Family f in p.GetPeopleController().GetFamilies())
        {
            idOptions.Add(f.GetID()+"");
            if(p.GetFamilyID() == f.GetID())
            {
                v = i;
            }
            i++;
        }
        familyIdDD.AddOptions(idOptions);
        familyIdDD.value = v;

        UpdateDependencies();
    }

    private void UpdateIndependentMenu()
    {
        if(!p.GetDependent() && p.GetManual())
        {
            independentMenu.SetActive(true);
            UpdateDependencies();
        } 
        else independentMenu.SetActive(false);
    }

    private void UpdateTypes()
    {
        if(p != null)
        {
            typesText.text = "Types: ";

            var addingOpts = new List<string>(typesOptions);
            var removingOpts = new List<string>();
            removingOpts.Add("Remove...");
            addTypeDD.ClearOptions();
            removeTypeDD.ClearOptions();

            if(p.GetPersonTypes() != null)
            {
                foreach(string t in p.GetPersonTypes())
                {
                    typesText.text += t+", ";
                    removingOpts.Add(t);
                    addingOpts.Remove(t);
                }
                typesText.text = typesText.text.Substring(0, typesText.text.Length - 2);
            }

            addTypeDD.AddOptions(addingOpts);
            removeTypeDD.AddOptions(removingOpts);

            addTypeDD.value = 0;
            removeTypeDD.value = 0;

            UpdateFamilyMenu();
        }
    }

    private void UpdateDependencies()
    {
        if(p!=null && p.GetFamilyID()!=-1 && independentMenu.activeSelf)
        {
            responsibleText.text = "Responsible For: ";

            addRespDD.ClearOptions();
            var addingOpts = new List<string>();
            addingOpts.Add("Add...");

            removeRespDD.ClearOptions();
            var removingOpts = new List<string>();
            removingOpts.Add("Remove...");

            var dependents = p.GetFamily().GetDependentMembers();
            foreach(PersonBehavior dependent in dependents)
            {
                if(p.IsPersonInDependents(dependent.GetID()))
                {
                    removingOpts.Add(dependent.name);
                    responsibleText.text += dependent.name+", ";
                }
                else
                {
                    if(dependent.GetTutorID()==-1) addingOpts.Add(dependent.name);
                }
            }
            
            if(p.GetResponsibleForIDs().Count>0) responsibleText.text = responsibleText.text.Substring(0, responsibleText.text.Length - 2);

            addRespDD.AddOptions(addingOpts);
            removeRespDD.AddOptions(removingOpts);

            addRespDD.value = 0;
            removeRespDD.value = 0;
        }
    }

    public void AgeValueChangeCheck()
    {
        if(p!=null)
        {
            p.SetAge(Mathf.RoundToInt(ageSlider.value));
            ageText.text = "Age: "+p.GetAge();
        }
    }

    public void SpeedValueChangeCheck()
    {
        if(p!=null)
        {
            p.SetSpeed(speedSlider.value*2);
            speedText.text = "Speed: "+p.GetSpeed()/2;
        }
    }

    public void AddTypeCheck()
    {
        if(p!=null)
        {
            string addingType = addTypeDD.captionText.text;
            if(addingType != "Add..." && addingType != "New...")
            {
                p.AddPersonType(addingType);
                UpdateTypes();
            }
        }
    }

    public void RemoveTypeCheck()
    {
        if(p!=null)
        {
            string removingType = removeTypeDD.captionText.text;
            if(removingType != "Remove...")
            {
                p.RemovePersonType(removingType);
                if(removingType == "Family Member") {dependentToggle.isOn = false;}
                UpdateTypes();
            }
        }
    }

    public void ManualChangeCheck()
    {
        if(p!=null)
        {
            p.SetManual(manualToggle.isOn);
            UpdateIndependentMenu();
        }
    }

    public void DependentChangeCheck()
    {
        if(p!=null)
        {
            p.SetDependent(dependentToggle.isOn);
            UpdateIndependentMenu();
        }
    }

    public void AddFamily()
    {
        if(p!=null)
        {
            int newID = p.GetPeopleController().AddFamily();
            p.AddToFamily(newID);
            UpdateFamilyIDs();
        } 
    }

    public void SetToFamily()
    {
        if(p!=null)
        {
            if(familyIdDD.captionText.text != "ID...")
            {
                int newID = System.Int32.Parse(familyIdDD.captionText.text);
                p.AddToFamily(newID);
            }
            UpdateFamilyIDs();
        }
    }


    public void AddDependentCheck()
    {
        if(p!=null)
        {
            string addingResp = addRespDD.captionText.text;
            if(addingResp != "Add...")
            {
                int addingNum = System.Int32.Parse(FindNumber(addingResp));
                if(addingNum!=-1)
                { 
                    p.AddResponsibility(addingNum);
                    UpdateDependencies();
                }
            }
        }

    }

    public void RemoveDependentCheck()
    {
        if(p!=null)
        {
            string addingResp = removeRespDD.captionText.text;
            if(addingResp != "Remove...")
            {
                int addingNum = System.Int32.Parse(FindNumber(addingResp));
                Debug.Log(addingNum);
                if(addingNum!=-1)
                { 
                    p.RemoveResponsibility(addingNum);
                    UpdateDependencies();
                }
            }
        }
    }

    private string FindNumber(string t_)
    {
        int index = t_.IndexOf(' ');
        if(index > 0) 
        {
            return t_.Substring(index+1);
        }
        return "-1";
    }
}
