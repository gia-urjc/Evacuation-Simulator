# Evacuation Simulator 

------------------------------
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

[] TODO

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

[] TODO

### General overview of the software architecture 

[] TODO

### Setup

[] TODO


### Build 
[] TODO

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
## Publications 
[] TODO

------------------------------
## Keywords
evacuation, simulation, visualization, game engine, Unity, multi-agents systems
