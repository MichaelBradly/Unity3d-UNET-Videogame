using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Estulo_MyriapodaCameraController : MonoBehaviour {

    public float speedH = 0.02f;
    [SerializeField] private float minPathPos = 0f;
    [SerializeField] private float defaultPathPos = 2;
    [SerializeField] private float maxPathPos = 4f;

    public float speedScroll = 0.0125f;
    private float scrollwheelvalue;
    [SerializeField] Transform dollyTrackTransform;
    [SerializeField] float minDollyTrackHeight = -4f;

    CinemachineTrackedDolly dolly;

    // Use this for initialization
    void Start () {
        dolly = GetComponent<Cinemachine.CinemachineVirtualCamera>().GetCinemachineComponent<Cinemachine.CinemachineTrackedDolly>();
    }
	
	// Update is called once per frame
	void Update () {
        RotateCameraWithMouseInput();
        ZoomCameraWithMouseScroll();
    }

    void RotateCameraWithMouseInput()
    {
        float mouseMove = Input.GetAxis("Mouse X");

        defaultPathPos += speedH * mouseMove;
        //Debug.Log(yaw);

        //update transform's rotation within constraints
        if (defaultPathPos > maxPathPos)
        {

            //Do not orbit camera past the Myriapoda left side
            defaultPathPos = maxPathPos;


        }
        else if (defaultPathPos < minPathPos)
        {
            //Do not orbit camera past the Myriapoda right side
            defaultPathPos = minPathPos;
        }

        dolly.m_PathPosition = defaultPathPos;

    }

    void ZoomCameraWithMouseScroll()
    {
        scrollwheelvalue = Input.GetAxis("ScrollWheel");
        if (dollyTrackTransform.position.y <= minDollyTrackHeight)
        {
            print("less than");
            dollyTrackTransform.localPosition = new Vector3(0f, -4f, 0f);
        }
        dollyTrackTransform.Translate(0f, -scrollwheelvalue * speedScroll, 0f);
    }
}
