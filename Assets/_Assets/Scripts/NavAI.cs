using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavAI : MonoBehaviour {

    [SerializeField] Transform me;
    NavMeshAgent nav;

    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {

    }

    // Use this for initialization
    void Start () {
		
	}



    // Update is called once per frame
    void Update () {
        
        nav.SetDestination(me.position);

    }

    public void FindPlayer()
    {
        me = GameObject.FindGameObjectWithTag("Player").transform;
    }
}
