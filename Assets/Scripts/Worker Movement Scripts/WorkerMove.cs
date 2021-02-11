using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
    The WorkerMove.cs script will direct a Worker object to a target object set within the Unity editor. The Worker needs a Nav Mesh Agent attribute.
*/


public class WorkerMove : MonoBehaviour
{
    InteractionPoint point;

    protected Transform[] _interactionPoints; //list of interaction points on a target object

    [SerializeField]
    public GameObject _target; //target game object set in the inspector

    NavMeshAgent _navMeshAgent;
    int _currentInteractPoint;

    //Start() will get the interaction points, set the current interaction point and set the destination
    void Start()
    {
        _navMeshAgent = this.GetComponent<NavMeshAgent>(); //get navmeshcomponent
        //checks if nav mesh agent exists
        if(_navMeshAgent == null)
        {
            Debug.LogError("The nav mesh agent component is not attached to " + gameObject.name);
        }
        else
        {
            _interactionPoints = _target.GetComponentsInChildren<Transform>(); //gets list of interaction points tied to target object
            _currentInteractPoint = setCurrentInteractionPoint(); //sets current interaction point to first available point
            setDestination();
        }
    }

    //setDestionation() sets the targetVector of the worker to the location of the interaction point
    private void setDestination()
    {
        Debug.Log(_interactionPoints[_currentInteractPoint].transform.position); //debug

        //get transform position and set destination
        Vector3 targetVector = _interactionPoints[_currentInteractPoint].transform.position;
        _navMeshAgent.SetDestination(targetVector);
    }

    //loops through all interaction points to see if any are occupied and returns the first non-occupied point
    private int setCurrentInteractionPoint()
    {
        int selectedPoint = 0;
        int count = 1;
        foreach (Transform obj in _interactionPoints)
        {
            getScript(count); //get script of the current interaction point so we can see if it is being used or not
            if(point.inUse == false) //continue looping if the point is in use
            {
                selectedPoint = count;
                break;
            }
            count++;
        }
        return selectedPoint;
    }

    //not working, needs a rework
    /*private void getInteractionPoints(Transform[] children, GameObject parent)
    {
        //will likely become more complicated. Might have to iterate through the _interactionPoints
        //array to see which objects have the InteractionPoint tag
        children = parent.GetComponentsInChildren<Transform>();
    }*/

    //get script of the currently focused interaction point
    private void getScript(int num)
    {
        point = _interactionPoints[num].gameObject.GetComponent<InteractionPoint>();
    }
}
