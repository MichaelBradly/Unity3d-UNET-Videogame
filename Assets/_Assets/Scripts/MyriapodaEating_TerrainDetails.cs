using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MyriapodaEating_TerrainDetails : MonoBehaviour {

    //Change to terrain with grass
    private Terrain terrainWithGrass;
    private TerrainData terrainWithGrassData;
    private Vector3 terrainWithGrassPos;

    private Terrain terrainWithTrees;
    private TerrainData terrainWithTreesData;
    private TreeInstance[] treeInstances;
    private Vector3 terrainWithTreesPos;
    private Dictionary<string, int> splatmap_to_TreeIndex = new Dictionary<string, int>();

    [SerializeField] int stomach;
    [SerializeField] int foodRequirement = 20;
    [SerializeField] bool canEatTrees = false;

    //Position local to the player within range of being eaten
    Vector3[] LocalPosArray = new[] {Vector3.left,
                                        Vector3.left/2,
                                        Vector3.zero,
                                        Vector3.right,
                                        Vector3.right/2};

    //Coords of the terrain detail the player is in range of eating
    int[] terrainDetailCoords = new int[2];
    //var to store temp worldPos 
    Vector3 WorldPos;
    //data inserted into splay map to remove grass
    int[,] map = new int[1, 1] { { 0 } };

    private void Start()
    {
        terrainWithGrass = Terrain.activeTerrains[1];
        terrainWithGrassData = terrainWithGrass.terrainData;
        terrainWithGrassPos = terrainWithGrass.transform.position;

        terrainWithTrees = Terrain.activeTerrains[0];
        terrainWithTreesData = terrainWithTrees.terrainData;
        treeInstances = terrainWithTreesData.treeInstances;
        terrainWithTreesPos = terrainWithTrees.transform.position;
        ConvertTreeInstanceToHashMap();
    }

    private void Update()
    {
        EatGrass();
        if (stomach >= foodRequirement)
        {
            GetComponent<Rigidbody>().mass *= 1.3f;
            //GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            GetComponent<MyriapodaEating>().AddMidsection();
            GetComponent<MyriapodaEating>().IncreaseScale();
            GetComponent<MyriapodaHeadController>().SpeedUp();
            //StartCoroutine(WaitForPhysics());
            stomach = 0;
            foodRequirement += 20;

            if (GetComponent<Rigidbody>().mass >= 200)
            {
                canEatTrees = true;
            }
        }
        if (canEatTrees)
        {
            EatTree();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //print(collision.gameObject.name);
    }

    void EatGrass()
    {
        foreach (Vector3 localPos in LocalPosArray)
        {
            WorldPos = transform.TransformPoint(localPos);
            terrainDetailCoords = WorldPosToSplatPos(WorldPos, terrainWithGrassData, terrainWithGrassPos);

            //If splat Coords are beyond size of terrain
            if (terrainDetailCoords[0] < 0 || terrainDetailCoords[0] >= terrainWithGrassData.detailWidth
                || terrainDetailCoords[1] < 0 || terrainDetailCoords[1] >= terrainWithGrassData.detailHeight)
            {
                return;
            }

            var map2 = terrainWithGrassData.GetDetailLayer(terrainDetailCoords[0], terrainDetailCoords[1], terrainWithGrassData.detailWidth, terrainWithGrassData.detailHeight, 4);
            if (map2[0, 0] > 0)
            {
                stomach++;
            }
                
            //Eat grass on all layers
            terrainWithGrass.terrainData.SetDetailLayer(terrainDetailCoords[0], terrainDetailCoords[1], 0, map);
            terrainWithGrass.terrainData.SetDetailLayer(terrainDetailCoords[0], terrainDetailCoords[1], 1, map);
            terrainWithGrass.terrainData.SetDetailLayer(terrainDetailCoords[0], terrainDetailCoords[1], 2, map);
            terrainWithGrass.terrainData.SetDetailLayer(terrainDetailCoords[0], terrainDetailCoords[1], 3, map);
            terrainWithGrass.terrainData.SetDetailLayer(terrainDetailCoords[0], terrainDetailCoords[1], 4, map);

        }
    }

    void EatTree()
    {
        WorldPos = transform.position + (transform.forward * Vector3.Magnitude(transform.localScale) * 0.5f);
        Debug.DrawRay(transform.position, transform.forward * Vector3.Magnitude(transform.localScale) * 0.5f, Color.yellow);
        int[] splatPos = WorldPosToSplatPos(WorldPos, terrainWithTreesData, terrainWithTreesPos);
        string splatPosStr = "(" + splatPos[0] + "," + splatPos[1] + ")";

        if (splatmap_to_TreeIndex.ContainsKey(splatPosStr))
        {
            //Get index of tree
            int index = splatmap_to_TreeIndex[splatPosStr];
            splatmap_to_TreeIndex.Remove(splatPosStr);

            //get & modify treeInstance
            TreeInstance newTree = terrainWithTreesData.GetTreeInstance(index);
            newTree.heightScale = 0;
            newTree.widthScale = 0;

            //update treeInstances[] to remove tree
            terrainWithTreesData.SetTreeInstance(index, newTree);
            
            //update TerrainCollider (possible optimize: update heightmap seperately)
            DestroyImmediate(terrainWithTrees.GetComponent<TerrainCollider>());
            TerrainCollider terrainCollider = terrainWithTrees.gameObject.AddComponent<TerrainCollider>();
            terrainCollider.terrainData = terrainWithTreesData;
        }

        //temp debug 
        //print("x: "+splatPos[0]+ ", z: " + splatPos[1]);
    }

    //Splat cell on terrain map
    int[] WorldPosToSplatPos(Vector3 m_worldPos, TerrainData m_terrainData,Vector3 m_terrainOffsetPos)
    {
        // calculate which splat map cell the worldPos falls within (ignoring y)
        terrainDetailCoords[0] = (int)(((m_worldPos.x - m_terrainOffsetPos.x) / m_terrainData.size.x) * m_terrainData.detailWidth);
        terrainDetailCoords[1] = (int)(((m_worldPos.z - m_terrainOffsetPos.z) / m_terrainData.size.z) * m_terrainData.detailHeight);

        return terrainDetailCoords;
    }

    void ConvertTreeInstanceToHashMap()
    {
        int numTrees = terrainWithTreesData.treeInstanceCount;
        for (int i=0; i < numTrees; i++)
        {
            Vector3 treeInstanceToWorldPos = Vector3.Scale(treeInstances[i].position, terrainWithTreesData.size) + terrainWithTreesPos;
            int[] splatPos = WorldPosToSplatPos(treeInstanceToWorldPos, terrainWithTreesData, terrainWithTreesPos);
            string splatPosStr = "(" + splatPos[0] + "," + splatPos[1] + ")";
            if (!splatmap_to_TreeIndex.ContainsKey(splatPosStr))
            {
                splatmap_to_TreeIndex.Add(splatPosStr, i);
            }
        }
    }

    IEnumerator WaitForPhysics()
    {
        yield return new WaitForSeconds(1);
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }
}

