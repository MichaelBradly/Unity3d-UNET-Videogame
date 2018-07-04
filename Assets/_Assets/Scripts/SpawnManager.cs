using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpawnManager : NetworkBehaviour
{

    [SerializeField] public int m_ObjectPoolSize = 5;
    public GameObject m_Prefab;
    public GameObject[] m_Pool;
    public NetworkHash128 assetId { get; set; }

    // Handles requests to spawn GameObjects on the client
    public delegate GameObject SpawnDelegate(Vector3 position, NetworkHash128 assetId);
    // Handles requests to unspawn GameObjects on the client
    public delegate void UnSpawnDelegate(GameObject spawned);

    // Use this for initialization
    void Start () {
        assetId = m_Prefab.GetComponent<NetworkIdentity>().assetId;
        m_Pool = new GameObject[m_ObjectPoolSize];
        for (int i = 0; i < m_ObjectPoolSize; ++i)
        {
            m_Pool[i] = (GameObject)Instantiate(m_Prefab, Vector3.zero, Quaternion.identity);
            m_Pool[i].name = "PoolObject" + i;
            m_Pool[i].SetActive(false);
        }

        ClientScene.RegisterSpawnHandler(assetId, SpawnObject, UnSpawnObject);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public GameObject GetFromPool(Vector3 position, int mode = 1)
    {
        foreach (var obj in m_Pool)
        {
            if (!obj.activeInHierarchy)
            {
                //Debug.Log("Activating GameObject " + obj.name + " at " + position);
                obj.transform.position = position;
                obj.SetActive(true);
                obj.GetComponent<NavAI>().FindPlayer();
                return obj;
            }
        }
        Debug.LogError("Could not grab GameObject from pool, nothing available");
        return null;
    }

    public GameObject GetSingleFromPool(Vector3 position)
    {
        foreach (var obj in m_Pool)
        {
            if (!obj.activeInHierarchy)
            {
                //Debug.Log("Activating GameObject " + obj.name + " at " + position);
                obj.transform.position = position;
                obj.SetActive(true);
                obj.GetComponent<NavAI>().FindPlayer();
            }
        }
        Debug.LogError("Could not grab GameObject from pool, nothing available");
        return null;
    }

    public GameObject GetAllFromPool(Vector3 position)
    {
        foreach (var obj in m_Pool)
        {
            if (!obj.activeInHierarchy)
            {
                obj.transform.position = position;
                obj.SetActive(true);
                obj.GetComponent<NavAI>().FindPlayer();
            }
        }
        Debug.LogError("Could not grab GameObject from pool, nothing available");
        return null;
    }

    public GameObject SpawnObject(Vector3 position, NetworkHash128 assetId)
    {
        return GetFromPool(position);
    }

    public void UnSpawnObject(GameObject spawned)
    {
        //Debug.Log("Re-pooling GameObject " + spawned.name);
        spawned.SetActive(false);
    }

    [Command]
    public void CmdSpawnBot()
    {
        Vector3 randomPosition = new Vector3(Random.Range(-10.0F, 10.0F), transform.position.y + 1f, Random.Range(-30.0F, -10.0F));
        // Set up Bot on server
        GameObject bot = GetFromPool(randomPosition);

        // spawn coin on client, custom spawn handler is called
        NetworkServer.Spawn(bot, assetId);

        // when the coin is destroyed on the server, it is automatically destroyed on clients
        //StartCoroutine(Destroy(bot, 10.0f));

    }

    public IEnumerator Destroy(GameObject go, float timer)
    {
        yield return new WaitForSeconds(timer);
        UnSpawnObject(go);
        NetworkServer.UnSpawn(go);
    }
}
