using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;


[System.Serializable]
public class ToggleEvent : UnityEvent<bool> { }

public class Player : NetworkBehaviour {

    [SyncVar(hook = "OnNameChanged")] public string playerName;
    [SyncVar(hook = "OnColorChanged")] public Color playerColor;

    [SyncVar] public NetworkInstanceId parentNetId;

    [SerializeField] ToggleEvent onToggleShare;
    [SerializeField] ToggleEvent onToggleLocal;
    [SerializeField] ToggleEvent onToggleRemote;
    [SerializeField] float respawnTime = 5f;

    NetworkAnimator anim;
    
    private void Start()
    {
        anim = GetComponent<NetworkAnimator>();
        EnabledPlayer();
    }


    private void Update()
    {
        if (!isLocalPlayer)
            return;

        if (anim)
        {
            anim.animator.SetFloat("Speed", Input.GetAxis("Vertical"));
        }
        //anim.animator.SetFloat("Strafe", Input.GetAxis("Horizontal"));  

        /*
        if (!parentNetId.IsEmpty())
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                CmdSelectPlayerObject(gameObject, "Estulo_Humanoid_RigidbodyFirstPerson");
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                CmdSelectPlayerObject(gameObject, "Estulo_Myriapoda");
            }
        }*/
    }

    public void EnabledPlayer()
    {
        //When player is enabled, turn on anything that should be shared
        onToggleShare.Invoke(true);

        if (isLocalPlayer)
        {
            try { PlayerCanvasScript.canvas.Initialize(); }
            catch
            { //do nothing
            }
            try { Estulo_PlayerCanvasScript.canvas.Initialize(); }
            catch
            { //do nothing
            }
            try { GameObject.Find("CM vcam1").SetActive(true); }
            catch
            { //do nothing
            }

            //if local player, turn on anything that the local player should need
            onToggleLocal.Invoke(true);
        }
        else
        {
            //if not local player, the server/other clients
            onToggleRemote.Invoke(true);
        }
    }

    //opposite of EnablePlayer()
    void DisablePlayer()
    {
        onToggleShare.Invoke(false);

        if (isLocalPlayer)
        {
            PlayerCanvasScript.canvas.HideReticule();
            onToggleLocal.Invoke(false);
        }
        else
        {
            onToggleRemote.Invoke(false);
        }
    }

    public void Die()
    {
        if (isLocalPlayer){
            //PlayerCanvas.canvas.WriteGameStatusText("You Died!");
            PlayerCanvasScript.canvas.PlayDeathAudio();

            //anim.SetTrigger("Died");
        }
        DisablePlayer();
        Invoke("Respawn", respawnTime);
    }

    void Respawn()
    {
        if (isLocalPlayer)
        {
            Transform spawn = NetworkManager.singleton.GetStartPosition();
            transform.position = spawn.position;
            transform.rotation = spawn.rotation;
            //anim.SetTrigger("Restart");
        }
        EnabledPlayer();
    }

    void OnNameChanged(string value)
    {
        playerName = value;
        gameObject.name = playerName;
        //GetComponentInChildren<Text>(true).text = playerName;
    }

    void OnColorChanged(Color value)
    {
        playerColor = value;
        //GetComponentInChildren<RendererToggler>().ChangeColor(playerColor);
    }

    public void buttonSpawn(string classPrefabInResources)
    {
        CmdSelectPlayerObject(gameObject, classPrefabInResources);
        onToggleLocal.Invoke(false);
    }

    [Command]
    public void CmdSelectPlayerObject(GameObject currentObj, string playerType)
    {
        //Store the NetworkConnection of local player
        NetworkConnection conn = currentObj.GetComponent<NetworkIdentity>().connectionToClient;
        //Instantiate prefab from Resource Folder 
        GameObject go = NewPlayerObject(playerType, currentObj.transform.position, currentObj.transform.rotation);

        if (parentNetId.IsEmpty())
        {
            //First time selecting a player object so we need to store the player netID
            go.GetComponent<Player>().parentNetId = currentObj.GetComponent<NetworkIdentity>().netId;
        }
        else
        {
            //Switching player object, so we need to pass the stored player NetID
            go.GetComponent<Player>().parentNetId = currentObj.GetComponent<Player>().parentNetId;
            NetworkServer.Destroy(currentObj);
        }

        //Assign local authority
        NetworkServer.ReplacePlayerForConnection(conn, go, 0);
    }

    public GameObject NewPlayerObject(string playerType, Vector3 pos, Quaternion rot)
    {
         GameObject go = GameObject.Instantiate((GameObject)Resources.Load(playerType), pos, rot);
        return go;
    }

    public override void OnStartClient()
    {
        ClientScene.RegisterPrefab((GameObject)Resources.Load("Estulo_Myriapoda"));
        ClientScene.RegisterPrefab((GameObject)Resources.Load("Estulo_Humanoid_RigidbodyFirstPerson"));

        //  sub scripts (no if needed)
        if (!parentNetId.IsEmpty())
        {
            transform.SetParent(ClientScene.FindLocalObject(parentNetId).transform);
        }

    }
}
