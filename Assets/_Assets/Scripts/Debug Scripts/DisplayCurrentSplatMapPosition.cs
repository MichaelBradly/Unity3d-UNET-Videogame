using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayCurrentSplatMapPosition : MonoBehaviour {

    int[] terrainDetailCoords = new int[2];
    Vector3 WorldPos;
    GameObject splat2world;
    private Terrain terrainWithTrees;
    private TerrainData terrainWithTreesData;
    private TreeInstance[] treeInstances;
    private Vector3 terrainWithTreesPos;

    // Use this for initialization
    void Start () {
        splat2world = GameObject.CreatePrimitive(PrimitiveType.Cube);
        terrainWithTrees = Terrain.activeTerrains[0];
        terrainWithTreesData = terrainWithTrees.terrainData;
        treeInstances = terrainWithTreesData.treeInstances;
        terrainWithTreesPos = terrainWithTrees.transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        WorldPos = transform.position + (transform.forward * Vector3.Magnitude(transform.localScale) * 0.5f);
        Debug.DrawRay(transform.position, transform.forward * Vector3.Magnitude(transform.localScale) * 0.5f, Color.yellow);
        int[] splatPos = WorldPosToSplatPos(WorldPos, terrainWithTreesData, terrainWithTreesPos);

        float intx = (((float)splatPos[0] / (float)terrainWithTreesData.detailWidth) * terrainWithTreesData.size.x) + terrainWithTreesPos.x;
        float intz = (((float)splatPos[1] / (float)terrainWithTreesData.detailHeight) * terrainWithTreesData.size.z) + terrainWithTreesPos.z;

        splat2world.transform.position = new Vector3(intx, 3f, intz);
    }

    //Redudant code combine into namespace
    //Splat cell on terrain map
    int[] WorldPosToSplatPos(Vector3 m_worldPos, TerrainData m_terrainData, Vector3 m_terrainOffsetPos)
    {
        
        // calculate which splat map cell the worldPos falls within (ignoring y)
        terrainDetailCoords[0] = (int)(((m_worldPos.x - m_terrainOffsetPos.x) / m_terrainData.size.x) * m_terrainData.detailWidth);
        terrainDetailCoords[1] = (int)(((m_worldPos.z - m_terrainOffsetPos.z) / m_terrainData.size.z) * m_terrainData.detailHeight);

        return terrainDetailCoords;
    }
}
