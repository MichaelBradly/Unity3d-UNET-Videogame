using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class CurrentForce : MonoBehaviour {


    private Text m_Text;
    public Rigidbody myriapodaRigidBody;

    // Use this for initialization
    void Start () {
        m_Text = GetComponent<Text>();

    }
	
	// Update is called once per frame
	void Update () {
        m_Text.text = myriapodaRigidBody.velocity.ToString();
	}
}
