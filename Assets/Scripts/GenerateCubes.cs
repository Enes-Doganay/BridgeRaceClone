using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateCubes : MonoBehaviour
{
    public static GenerateCubes instance;
    public GameObject redCube, greenCube, blueCube;
    public Transform redCubeParent, greenCubeParent, blueCubeParent;
    public int minX, maxX, minZ, maxZ;
    public LayerMask layerMask;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    //0 red , 1 blue , 2 green
    public void GenerateCube(int number, AI aI = null)
    {
        if(number == 0)
        {
            Generate(redCube, redCubeParent, aI);
        }
        if (number == 1)
        {
            Generate(blueCube, blueCubeParent);
        }
        if (number == 2)
        {
            Generate(greenCube, greenCubeParent, aI);
        }
    }
    private void Generate(GameObject gameObj,Transform parent,AI aI = null)
    {
        GameObject obj = Instantiate(gameObj);
        obj.transform.parent = parent;

        Vector3 desPos = GiveRandomPos();
        obj.SetActive(false);

        Collider[] colliders = Physics.OverlapSphere(desPos, 1f, layerMask);
        while(colliders.Length != 0)
        {
            desPos = GiveRandomPos();
            colliders = Physics.OverlapSphere(desPos, 1f, layerMask);
        }
        obj.SetActive(true);
        obj.transform.position = desPos;

        /*if(aI != null)
        {
            aI.targets.Add(obj);
        }*/
    }

    private Vector3 GiveRandomPos()
    {
        return new Vector3(Random.Range(minX, maxX), blueCube.transform.position.y, Random.Range(minZ, maxZ));
    }
}
