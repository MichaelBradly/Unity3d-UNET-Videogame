using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnLoadClearUI : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        GameObject.Find("Estulo_LobbyManager/LobbyPanel").SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
