using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AI;

public static class Utils
{
    public enum MouseInputEvents{left_up, left_down, left_held, right_up, right_down}
    public enum KeyboardInputEvents{}
	//public enum Actions{bake, run}

    public static void Print(string msg_){ Debug.Log(Random.Range(1000, 2000) + ": "+ msg_);}
// openWarningMenu(string warning, delegate ok, delegate cancel
    public static KeyValuePair<Vector3,GameObject> ScreenToWorld()
    {
        // Get the mouse position in the world from the screen
        // and the object in that position
        // also avoid clicking over UI

        KeyValuePair<Vector3,GameObject> result = new KeyValuePair<Vector3,GameObject>(Vector3.positiveInfinity, null);
        
        if(EventSystem.current.IsPointerOverGameObject())
		{
			return result;
		}

        RaycastHit hitInfo;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hitInfo))
        {
            result = new KeyValuePair<Vector3,GameObject>(hitInfo.point, hitInfo.transform.gameObject);
        }
        return result;
    }

    public static bool ValidMousePos(Vector3 pos_){ return pos_.x != Mathf.Infinity;}



    public static float CalculateDistance(Vector3 pointA, Vector3 pointB)
	{
		NavMeshPath path = new NavMeshPath();
		bool validPath, validPointA, validPointB;
		NavMeshHit hitA, hitB;
		
		validPointA = NavMesh.SamplePosition(pointA, out hitA, 20.0f, NavMesh.AllAreas);
		validPointB = NavMesh.SamplePosition(pointB, out hitB, 20.0f, NavMesh.AllAreas);
		
		if(validPointA && validPointB)
		{
			validPath = NavMesh.CalculatePath(hitA.position, hitB.position, NavMesh.AllAreas, path);
			if(validPath)
			{
				return PathDistance(path);
			}
			else Debug.Log("Fallo en el path");
		}else Debug.Log("Fallo en situar los puntos en la navmesh");
		
		return Mathf.Infinity;
	}
	
	public static float PathDistance(NavMeshPath path_)
	{
		float distance = 0.0f;
		Vector3[] corners = path_.corners;
		for (int c = 0; c < corners.Length - 1; ++c) {
			distance += Mathf.Abs((corners[c] - corners[c + 1]).magnitude);
		}
		return distance;
	}

    public static float GetAngleFromVectorFloat(Vector3 dir)
    {
        float difference = (dir.z/dir.x);
        float angleRot = Mathf.Atan(difference)*180/Mathf.PI;
        return angleRot;
    }
}
