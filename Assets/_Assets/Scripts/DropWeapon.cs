using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DropWeapon : NetworkBehaviour
{
    [SerializeField] GameObject currWeapon;
    [SerializeField] Transform gunHolster;
    [SerializeField] Camera cam;

    bool canDrop;

    // Use this for initialization
    void Start () {

        if (isLocalPlayer)
        {
            canDrop = true;
            CmdRegisterWeapon();
        }
    }

    // Update is called once per frame
    void Update () {

        //if not local, skip
        if (!canDrop)
        {
            return;
        }        

        int layerMask = 1 << 2;
        layerMask = ~layerMask;

        RaycastHit hit;
        bool result = Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit, 4f, layerMask);

        //if my shot hit something
        if (result)
        {
            Debug.DrawRay(cam.transform.position, cam.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            if ((hit.transform.root.tag == "grabbableWeapon") && Input.GetButtonDown("GrabWeapon"))
            {
                //Drop current weapon          
                if (currWeapon != null)
                {
                    //print("check enter");
                    CmdDropWeapon(currWeapon);
                    currWeapon = null;
                }
                //switch currWeapon with Grabbable Weapon
                CmdGrabWeapon(hit.transform.root.gameObject);
                currWeapon = hit.transform.root.gameObject;

                //Change gun data for shooting and sfx
                print(GetComponent<PlayerShooting>());
                print(GetComponent<ShotEffectsManager>());
                currWeapon.GetComponentInChildren<WeaponData>().EquipData(GetComponent<PlayerShooting>(), GetComponent<ShotEffectsManager>());
            }
            else
            {
                Debug.DrawRay(cam.transform.position, cam.transform.TransformDirection(Vector3.forward) * hit.distance, Color.white);
            }
            
        }

        if (Input.GetButtonDown("DropWeapon") && currWeapon != null)
        {
            CmdDropWeapon(currWeapon);
            currWeapon = null;
        }
    }

    [Command]
    void CmdRegisterWeapon()
    {
        RpcRegisterWeapon();
    }

    [Command]
    void CmdGrabWeapon(GameObject newWeapon)
    {
        RpcGrabWeapon(newWeapon);
    }

    [Command]
    void CmdDropWeapon(GameObject equippedWeapon)
    {
        RpcDropWeapon(equippedWeapon);        
    }

    [ClientRpc]
    void RpcRegisterWeapon()
    {
        if (currWeapon.GetComponent<NetworkIdentity>() == null)
        {
            currWeapon.AddComponent<NetworkIdentity>();
        }
        ClientScene.RegisterPrefab(currWeapon, NetworkHash128.Parse(currWeapon.name));
        CmdSpawnFix(currWeapon);
    }

    [ClientRpc]
    void RpcGrabWeapon(GameObject newWeapon)
    {
        //organize in hierarachy 
        newWeapon.transform.parent = gunHolster;

        //Remove collider
        if (newWeapon.GetComponentInChildren<Collider>())
        {
            newWeapon.GetComponentInChildren<Collider>().enabled = false;
        }
        else
        {
            newWeapon.GetComponent<Collider>().enabled = false;
        }

        //remove physics
        if (newWeapon.GetComponent<Rigidbody>())
        {
            newWeapon.GetComponent<Rigidbody>().isKinematic = true;
        }

        //position gun
        if (newWeapon.GetComponentInChildren<GunPositioningDataScript>())
        {
            Transform data = newWeapon.GetComponentInChildren<GunPositioningDataScript>().transform;
            newWeapon.transform.localPosition = data.localPosition;
            newWeapon.transform.localRotation = data.localRotation;
        }
        
        //Remove invalid weapon hotfix (only used once after initial CmdSpawnFix)
        if (!currWeapon.GetComponent<NetworkIdentity>().isClient)
        {
            //Destroys weapon that has no netID
            Destroy(currWeapon);
        }

        //Stores Weapon
        currWeapon = newWeapon;
    }

    [ClientRpc]
    void RpcDropWeapon(GameObject equippedWeapon)
    {
        equippedWeapon.transform.SetParent(null);

        //enable collider
        if (equippedWeapon.GetComponentInChildren<Collider>())
        {
            equippedWeapon.GetComponentInChildren<Collider>().enabled = true;
        }
        else
        {
            equippedWeapon.GetComponent<Collider>().enabled = true;
        }

        //add physics
        if (equippedWeapon.GetComponent<Rigidbody>())
        {
            equippedWeapon.GetComponent<Rigidbody>().isKinematic = false;
        }

        //Do not set to null. Rpc Do not happen in order and will cause error during swap weapon.
        //currWeapon = null;
    }

    //Hotfix used for child-object gun attached to instantiated prefab
    [Command]
    void CmdSpawnFix(GameObject obj)
    {
        if (obj == null) {
            print("ERROR: invalid ServerSpawnFix called by player "+connectionToClient);
            return;
        }
        NetworkServer.Spawn(obj);
        CmdGrabWeapon(obj);


    }
}
