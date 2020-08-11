# Evacuation Simulator 

## Introduction

The purpose of this simulator is simulates and allows visualization of the evacuation process of people located in buildings during emergencies. 

It helps to analyse the different situations that occur during an emergency evacuation in different scenarios, studying aspects such as selection of nearest exit point for an evacuee, selection of evacuation routes, group formation , improve or create new algorithms, etcetera. 

By using this simulator, security measures for emergency scenarios can be improved and, as a result, the maximum evacuation time can be reduced or you can save as many people as possible.

The simulator offers us the possibility to execute situations using different configurations: floor plans, graphs, occupants, paths (algorithms) and cameras. 

Here you'll find all the necessary documentation to use and develop new features in the Evacuation Simulator. 

------------------------------
## Author - v1.0.0
The version 1.0.0 is a BSc thesis of the "Design and develop of videogames" Degree - Rey Juan Carlos University, written by Sandra Valverde Tallón @svalverdet, and directed by Sascha Ossowski and Qasim Khalid. 

------------------------------
## For Users 

- [] TODO

------------------------------
## For Developers

If you are a developer or researcher and wants to create new things for the simulator, you should start here. 

### Prerequisites

1. Unity (better Unity Hub)

   Important: Unity Version: 2018.3.14f1

2. Visual Studio Code
   
3. Git

Please make sure that all the binaries are registered in your PATH.

### Getting Started for Development

- [] TODO

### General overview of the software architecture 

- [] TODO

### Setup

- [] TODO


### Build 
- [] TODO

### Introducing a new Algorithm 

  1) In Unity go to Project > Assets > Scripts > Paths → R mouse (click) → Create > C# Script 
      In Visual: Rename the Algorithm, e.g. ExampleAlgorithm.cs 
      
  2) In Visual > In the new C# Script: The Algorithm must implement the IAlgorithm interface. And type the Algorithm.  
  
  3) In Visual: Add the Algorithm in Project > Assets > Scripts > Misc_ > SceneController.cs > SetAlgorithm( ) method: 
      swith(name)
      {
          case "This is an example Algorithm": algorithm = new ExampleAlgorithm(); break;
          ....
      }
      
  4) In Unity: Go to Hierarchy Menu > UI > EditMenus > AlgorithmEdit > AddType → L mouse (click) > Go to Inspector > TMP_Dropwdown > Options > Add a new Option with the name "This is an example Algorithm".
      
Now, you can choose the algorithm in the simulator.  

------------------------------
## Simulator Guide

When you start the simulator, you can use "File", "Floor plan" and "Camera" drop down menus. 

### "Floor plan" drop down Menu
Create a new floor plan: 

   1) Click "Create" Option (Floor plan Menu) > In the greenish rectangle you can draw the floor plan. 
      Draw: Click L mouse button, move, Click L mouse button. 
      Remove: Click R mouse button in the black line. 
    
      You can remove all with "Clear All" Option (Floor plan Menu). 
      
   2) Click "Create Sections" Option (Floor plan Menu).
   
   3) Click R mouse button in the black line for create DOORS
   
   4) Click "Edit" Option (Floor plan Menu), choose a Section and select "Is Collection Point". In Addition, with "Edit" you can choose the capacity of the sections. 
   
   5) Click "Finish" Option (Floor plan Menu).
   

### "Graph" drop down Menu
Create a new graph: 

   a) Auto:
   
      1 - Click "Auto" Option (Graph Menu) for auto generate the graph
      
      2 - Click "Finish" Option (Graph Menu).
   
   b) Create: 
   
      1 - Click "Create" Option (Graph Menu): 
          Add node: Click L mouse button in a Section  
          Add edge: Press L mouse button in a node, move to other node, stop the press
          
      2 - Click "Finish" Option (Graph Menu).
          
You can use "Clear All" and "Clear Edges" Options (Graph Menu)

### "Occupants" drop down Menu
Create a new occupant, different types:  

   a) Normal Occupant: 
      
      1) Choose the age and speed. 
      
      2) Put the occupant in a floor plan: Click L mouse button in a Section. 
   
   b) Mobility Impaired: 
     
      1) Choose the age and speed.
      
      2) Types > Add... > Mobility Impaired
      
      3) Put the occupant in a floor plan: Click L mouse button in a Section. 
      
   c) Family Member: 
   
      1) 
         a) If is the first member: Click in "New" Button for create a new Family ID. 
         
         b) In another case: Select the Family ID. 
         
      2) 
         a) If is the "Leader": When you have created the all family members, add the "Dependent" members in the "Responsible For" Option. (For that, click "Edit" Option (Occupants Menu) and touch the leader)
         
         b) In another case: Mark "Dependent" Option.

You can create "Auto" (Option (Occupants Menu)) occupants or "Clear All" (Option (Occupants Menu)). 

Click "Finish" Option (Occupants Menu).

### "Paths" drop down Menu
Choose the Algorithm: 
   
   a) Auto: 
   
      1) Click "Auto" Option (Path Menu) for auto generate the path. Auto use the "Closest CP" Algorithm ( Assets/Scripts/Paths/DummyOnlyIndependent.cs).
      
      2) Click "Finish" Option (Path Menu).
         
   b) Choose: 
      
      1) Click "Set Algorithm" Option (Path Menu).
      
      2) Choose the Algorithm: 
      
         a) Closest CP
         
         b) Random CP
         
         c) Aimed Algorithm: This is for testing paths. For the test, go to Assets/SavedData/Aimed.txt 
            Each line is a person in order (the line 1 is the person with id 0, the line 2 is the person with id 1...). For example, in aimed.txt: 
                  *0 2 3*
                  *0 2*
                  *0 2 3 4*

            In this case, the line 1 is the person with id 0 and its path is the nodes: 0 -> 2 -> 3. 
            If you don't indicate the first node, por example 0, it is not a problem. This looks like this, in aimed.txt: 
                  *2 3*
                  *2*
                  *2 3 4*
            Is the same path: 0 -> 2 -> 3.
          
         c) Aimed Algorithm - Indicate Person: This is for testing paths, but you indicate the person ID in the txt. For the test, go to Assets/SavedData/AimedIndicatePerson.txt 
            For example, in AimedIndicatePerson.txt: 
                  *3 0 2 3*
                  *1 0 2 3 4*
                  *5 2 3*
            In this case, the line 1 is the person with id 3 and its path is the nodes: 0 -> 2 -> 3. The line 2 is the person with id 1 and its path is the nodes: 0 -> 2 -> 3 -> 4. The line 3 is the person with id 5 and its path is the nodes: 0 -> 2 -> 3. Notice than in the line 3 we don't indicate the first node, it is not a problem. 
            
       3) Click "Finish" Option (Path Menu).

### Start the simulation
Click "PLAY" Button

### "File" drop down Menu
In the File Menu you can: 

   a) "New" Option: Create new Simulation
   
   b) "Load" Option: Load a Save Simulation
   
   c) "Save" Option and "Save as.." Option: Save the current Simulation
   
   d) "Exit" Option: Close the Simulator

------------------------------
## Publications 
- [] TODO

------------------------------
## Keywords
evacuation, simulation, visualization, game engine, Unity, multi-agents systems
