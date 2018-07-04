using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyriapodaController : Estulo_PlayerController
{

    public float acceleration = 150f;
    public float MaxSpeed;
    public float turnSpeed = 1.5f;
    public float deacceleration = 3f;
    private Rigidbody MyriapodaParentRigidbody;
    public Camera m_camera;

    // Use this for initialization
    void Start () {
        MyriapodaParentRigidbody = GetComponent<Rigidbody>();
        
    }
	
	// Update is called once per frame
	void Update () {


        float moveHorizontal = Input.GetAxis("Horizontal");
        transform.eulerAngles += new Vector3(0.0f, moveHorizontal* turnSpeed, 0.0f);

    }

    void FixedUpdate()
    {
        //Debug.Log(MyriapodaParentRigidbody.velocity.magnitude);
        //if (MyriapodaParentRigidbody.velocity.magnitude > MaxSpeed)
        //{
        //    Debug.Log(MyriapodaParentRigidbody.velocity.magnitude);
        //    return;
        //}

        //Get Player input
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        
        //convert input to 2D vector on the myriapoda's plane
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        //strength the input by a speed scalar
        Vector3 forceVector = movement * acceleration;

        
        //Forward Movement //apply a force to the myriapoda in the direction of the camera
        if (moveVertical > 0)
        { 
            //get camera forward vector and take x-z plane direction
            Vector3 forwardOffsetVector = new Vector3(m_camera.transform.forward.x, 0f, m_camera.transform.forward.z);
            //get player forward vector and take x-z directions
            Vector3 playerforwardOffsetVector = new Vector3(transform.forward.x, 0f, transform.forward.z);
            //add force in that direction if 'w' is pressed
            MyriapodaParentRigidbody.AddForce(playerforwardOffsetVector * acceleration);
        }
        //Reverse Movement //apply a force to the myriapoda to bring it to rest
        if (moveVertical < 0 && MyriapodaParentRigidbody.velocity.z > 0)
        {
            //Myriapoda can not go backwards
            //Apply force in opposite direction of current trajectory
            MyriapodaParentRigidbody.AddForce(-MyriapodaParentRigidbody.velocity* 20);
        }
        
        //[Debug] Visualize Force Vector 
        Vector3 forceVectorVisual = gameObject.transform.position + forceVector;
        Debug.DrawLine(MyriapodaParentRigidbody.transform.position, forceVectorVisual, Color.red);

        //[Debug] Visualize Force Vector 
        Vector3 rigidbodyVelocityVectorVisual = gameObject.transform.position + MyriapodaParentRigidbody.velocity;
        Debug.DrawLine(MyriapodaParentRigidbody.transform.position, rigidbodyVelocityVectorVisual, Color.yellow);

     
    }

    void DebugDrawLine(GameObject gameObject1, Vector3 lineToDraw, Color color)
    {
        Vector3 vectorVisual = gameObject1.transform.position + lineToDraw;
        Debug.DrawLine(MyriapodaParentRigidbody.transform.position, vectorVisual, color);
    }

}
