using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerShooting : NetworkBehaviour {

    public float shotCooldown = .3f;
    public Transform firePosition;
    public ShotEffectsManager shotEffects;

    [SyncVar (hook = "OnScoreChanged")] int score;

    float ellapsedTime;
    bool canShoot;

    // Use this for initialization
    void Start () {
        //shotEffects.Initialize();

        if (isLocalPlayer)
        {
            canShoot = true;
        }

            
	}

    [ServerCallback]
    private void OnEnable()
    {
        score = 0;
    }

    // Update is called once per frame
    void Update () {
        
        //if not local, skip
        if (!canShoot)
        {
            return;
        }
        if (!firePosition)
        {
            return;
        }

    
        //if local
        ellapsedTime += Time.deltaTime;


        Debug.DrawRay(firePosition.position, firePosition.forward * 3f, Color.red);
        if (Input.GetButtonDown("Fire1") && ellapsedTime > shotCooldown)
        {
            ellapsedTime = 0f;
            CmdFireShot(firePosition.position, firePosition.forward);
        }

	}

    //command attributes means its run on the server
    //client tells the server to do something
    [Command]
    void CmdFireShot(Vector3 origin, Vector3 direction)
    {
        RaycastHit hit;

        Ray ray = new Ray(origin, direction);
        Debug.DrawRay(ray.origin, ray.direction * 3f, Color.red, 1f);

        bool result = Physics.Raycast(ray, out hit, 50f);

        //if my shot hit something
        if (result)
        {
            PlayerHealth enemy = hit.transform.GetComponent<PlayerHealth>();

            if (enemy != null)
            {
                bool wasKillShot = enemy.TakeDamage();

                if (wasKillShot)
                {
                    score++;
                }

            }
        }

        RpcProcessShotEffects(result, hit.point);
    }

    //ClientRPC attribute the server tells all the clients to do something
    [ClientRpc]
    void RpcProcessShotEffects(bool playImpact, Vector3 point)
    {
        shotEffects.Initialize();
        shotEffects.PlayShotEffects();

        if (playImpact)
        {
            shotEffects.PlayImpactEffect(point);
        }
    }

    void OnScoreChanged(int value)
    {
        score = value;
        if (isLocalPlayer)
        {
            try
            {
                //add Server scoreboard 
                PlayerCanvasScript.canvas.SetKills(value);
            }
            catch { };
            
        }
    }
}
