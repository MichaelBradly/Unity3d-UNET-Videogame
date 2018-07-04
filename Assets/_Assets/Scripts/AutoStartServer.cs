using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class AutoStartServer : MonoBehaviour {

    GameObject joinGame;

    // Use this for initialization
    void Start () {

        GameObject startHost = GameObject.Find("PlayAndHostButton");
        startHost.GetComponent<Button>().onClick.Invoke();
        //Destroy(startHost);

    }
	
	// Update is called once per frame
	void Update () {

        if (joinGame == null)
        {
            joinGame = GameObject.Find("Estulo_LobbyManager/LobbyPanel/PlayerListSubPanel/PlayerList/PlayerInfo(Clone)/JoinButton");
        }
        else
        {
            joinGame.GetComponent<Button>().onClick.Invoke();
            DestroyImmediate(gameObject);
        }


    }
}
