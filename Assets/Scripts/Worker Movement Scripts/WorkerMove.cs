using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WorkerMove : MonoBehaviour
{
    Transform _destination;

    InteractionPoint point;

    //List<GameObject> _interactionPoints;
    protected Transform[] _interactionPoints;

    [SerializeField]
    public GameObject _target;

    NavMeshAgent _navMeshAgent;
    int _currentInteractPoint;
    // Start is called before the first frame update
    void Start()
    {
        _currentInteractPoint = 1; //put here for debugging
        _navMeshAgent = this.GetComponent<NavMeshAgent>();

        if(_navMeshAgent == null)
        {
            Debug.LogError("The nav mesh agent component is not attached to " + gameObject.name);
        }
        else
        {
            //gets list of interaction points tied to target object
            _interactionPoints = _target.GetComponentsInChildren<Transform>();
            //getInteractionPoints(_interactionPoints, _target);
            _currentInteractPoint = setCurrentInteractionPoint();
            getScript(_currentInteractPoint);
            setDestination();
        }
    }


    // Update is called once per frame
    void Update()
    {
        //if(_interactionPoints[_currentInteractPoint].)
        /*if(checkIfOccupied() == true))
        {
            //change current interaction point
            //setDestination()
        }*/
    }

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
            getScript(count);
            if(point.inUse == false)
            {
                selectedPoint = count;
                break;
            }
            count++;
        }
        return selectedPoint;
    }

    /*private bool checkIfOccupied()
    {

    }*/

    //not working
    private void getInteractionPoints(Transform[] children, GameObject parent)
    {
        //will likely become more complicated. Might have to iterate through the _interactionPoints
        //array to see which objects have the InteractionPoint tag
        children = parent.GetComponentsInChildren<Transform>();
    }

    //get script of the currently focused interaction point
    private void getScript(int num)
    {
        point = _interactionPoints[num].gameObject.GetComponent<InteractionPoint>();
    }
}
