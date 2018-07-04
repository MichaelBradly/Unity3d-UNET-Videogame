using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyriapodaHeadController : Estulo_PlayerController
{

    public float acceleration = 75f;
    public float MaxSpeed;
    public float turnSpeed = 1.5f;
    public float deacceleration = 3f;

    public float stickToGroundHelperDistance = 0.5f; // stops the character
    public float radiusCheck = 0.5f;
    public float height = 0.5f;
    public float shellOffset = 0f;

    private Rigidbody headRigidbody;
    public Camera m_camera;

    // Use this for initialization
    void Start()
    {
        headRigidbody = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {


        //float moveHorizontal = Input.GetAxis("Horizontal");

        //headRigidbody.AddTorque(transform.up * turnSpeed * moveHorizontal, ForceMode.Impulse);
        //transform.eulerAngles += new Vector3(0.0f, moveHorizontal * turnSpeed, 0.0f);


        //Transform tempCameraTransform = m_camera.transform;  
        //tempCameraTransform.eulerAngles += new Vector3(0.0f, moveHorizontal * turnSpeed, 0.0f);
        //Vector3 newCameraForward = tempCameraTransform.forward;
        //m_camera.GetComponent<MyriapodaCameraController>().OrbitCameraAroundMyriapoda(newCameraForward);

    }

    void FixedUpdate()
    {
        //Get Player input
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        headRigidbody.AddTorque(transform.up * turnSpeed * moveHorizontal, ForceMode.Impulse);

        //convert input to 2D vector on the myriapoda's floor plane
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
            headRigidbody.AddForce(playerforwardOffsetVector * acceleration);
        }
        //Reverse Movement //apply a force to the myriapoda to bring it to rest
        if (moveVertical < 0 && headRigidbody.velocity.z > 0)
        {
            //Myriapoda can not go backwards
            //Apply force in opposite direction of current trajectory
            headRigidbody.AddForce(-headRigidbody.velocity * 20);
        }

        //[Debug] Visualize Force Vector 
        Vector3 forceVectorVisual = gameObject.transform.position + forceVector;
        Debug.DrawLine(headRigidbody.transform.position, forceVectorVisual, Color.red);

        //[Debug] Visualize Force Vector 
        Vector3 rigidbodyVelocityVectorVisual = gameObject.transform.position + headRigidbody.velocity;
        Debug.DrawLine(headRigidbody.transform.position, rigidbodyVelocityVectorVisual, Color.yellow);

        StickToGroundHelper();
    }

    public void SpeedUp()
    {
        acceleration *= 1.25f;
        turnSpeed += 1f;
    }

    private void StickToGroundHelper()
    {
        RaycastHit hitInfo;
        if (Physics.SphereCast(transform.position, radiusCheck * (1.0f - shellOffset), Vector3.down, out hitInfo,
                               ((height / 2f) - radiusCheck) +
                               stickToGroundHelperDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
        {
            if (Mathf.Abs(Vector3.Angle(hitInfo.normal, Vector3.up)) < 85f)
            {
                headRigidbody.velocity = Vector3.ProjectOnPlane(headRigidbody.velocity, hitInfo.normal);
            }
        }
    }

    void DebugDrawLine(GameObject gameObject1, Vector3 lineToDraw, Color color)
    {
        Vector3 vectorVisual = gameObject1.transform.position + lineToDraw;
        Debug.DrawLine(headRigidbody.transform.position, vectorVisual, color);
    }

}
