using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using UnityEngine.UI;


public class Estulo_PlayerSetup_Humanoid : NetworkBehaviour
{

    [SyncVar(hook = "OnNameChanged")] public string playerName;
    [SyncVar(hook = "OnColorChanged")] public Color playerColor;

    [SerializeField] ToggleEvent onToggleShare;
    [SerializeField] ToggleEvent onToggleLocal;
    [SerializeField] ToggleEvent onToggleRemote;
    [SerializeField] float respawnTime = 5f;

    [SerializeField] bool _isLocalPlayer;

    NetworkAnimator anim;

    private void Start()
    {
        //_isLocalPlayer = gameObject.transform.parent.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer;
        anim = GetComponent<NetworkAnimator>();
        EnabledPlayer();
    }


    private void Update()
    {
        if (isLocalPlayer)
            return;

        if (anim)
        {
            anim.animator.SetFloat("Speed", Input.GetAxis("Vertical"));
        }
        //anim.animator.SetFloat("Strafe", Input.GetAxis("Horizontal"));  

    }

    public void EnabledPlayer()
    {
        print("player enabled");
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
        if (isLocalPlayer)
        {
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
        //NetworkServer.ReplacePlayerForConnection(connectionToClient, my, id);
        // NetworkServer.ReplacePlayerForConnection(connectionToClient, my, GetComponent<NetworkIdentity>().playerControllerId);
    }

    public override void OnStartLocalPlayer() // this is our player
    {
        base.OnStartLocalPlayer();

        print("onstartlocalPlayer called");

        // add input handler component OR (see Update)
    }
}
