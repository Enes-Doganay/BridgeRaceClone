using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public enum CharacterEnum
{
    zero = 0,
    two = 2
}
public class AI : MonoBehaviour
{
    public GameObject targetsParent;
    public List<GameObject> targets = new List<GameObject>();
    public List<GameObject> cubes = new List<GameObject>();
    public Transform ropes;
    public float radius = 2f;
    public Transform bag;
    public GameObject prevObj;

    public CharacterEnum characterEnum;
    private Animator animator;
    private NavMeshAgent agent;
    private bool haveTarget = false;
    private Vector3 targetTransform;

    private Transform stopCol;
    private bool rope = false;
    private void Start()
    {
        for(int i = 0; i < targetsParent.transform.childCount; i++)
        {
            targets.Add(targetsParent.transform.GetChild(i).gameObject);
        }
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }
    private void Update()
    {

        if (!haveTarget && targets.Count > 0)
        {
            ChooseTarget();
        }
    }

    void ChooseTarget()
    {
        int randomNumber = Random.Range(0, 3);
        if (randomNumber == 0 && cubes.Count >= 5)
        {
            //int randomRope = Random.Range(0, ropes.Length);
            List<Transform> ropesNonActiveChild = new List<Transform>();
            foreach(Transform item in ropes)//[randomRope])
            {
                if (!item.GetComponent<MeshRenderer>().enabled || item.GetComponent<MeshRenderer>().enabled && item.gameObject.tag != "Align" + transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material.name.Substring(0, 1))
                {
                    ropesNonActiveChild.Add(item);
                }
            }
            targetTransform = cubes.Count > ropesNonActiveChild.Count ? ropesNonActiveChild[ropesNonActiveChild.Count - 1].position : ropesNonActiveChild[cubes.Count].position;
        }
        else
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
            List<Vector3> ourColors = new List<Vector3>();

            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i].tag.StartsWith(transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material.name.Substring(0, 1)))
                {
                    ourColors.Add(hitColliders[i].transform.position);
                }
            }
            if (ourColors.Count > 0)
            {
                targetTransform = ourColors[0];
            }
            else
            {
                int random = Random.Range(0, targets.Count);
                targetTransform = targets[random].transform.position;
            }
        }

        agent.SetDestination(targetTransform);
        if (!animator.GetBool("running"))
        {
            animator.SetBool("running", true);
        }
        haveTarget = true;
    }

    private void OnTriggerEnter(Collider target)
    {
        if(target.gameObject.tag.StartsWith(transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material.name.Substring(0, 1))){
            target.transform.SetParent(bag);
            Vector3 pos = prevObj.transform.localPosition;

            pos.y += 0.22f;
            pos.z = 0;
            pos.x = 0;

            target.transform.localRotation = new Quaternion(0, 0.7071068f, 0, 0.7071068f);
            target.transform.DOLocalMove(pos, 0.2f);
            prevObj = target.gameObject;
            cubes.Add(target.gameObject);

            targets.Remove(target.gameObject);
            target.tag = "Untagged";
            haveTarget = false;

            GenerateCubes.instance.GenerateCube((int)characterEnum, this);
        }
        if (cubes.Count > 0 && target.gameObject.tag == "AlignR" || cubes.Count > 0 && target.gameObject.tag != "Align" + transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material.name.Substring(0, 1) && target.gameObject.tag.StartsWith("Align"))
        {
            GameObject gObj = cubes[cubes.Count - 1];
            cubes.RemoveAt(cubes.Count - 1);
            Destroy(gObj);

            target.GetComponent<MeshRenderer>().material = transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material;
            target.GetComponent<MeshRenderer>().enabled = true;
            target.tag = "Align" + transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material.name.Substring(0, 1);

            if (cubes.Count == 0)
            {
                prevObj = bag.GetChild(0).gameObject;
                haveTarget = false;
            }
            else
            {
                prevObj = cubes[cubes.Count - 1];
            }

            if (rope && stopCol.localPosition.z != -3.48f)
            {
                stopCol.localPosition += new Vector3(0, 0,-0.45f);
            }

        }
        
        if (target.gameObject.tag == "rope")
        {
            rope = true;
            stopCol = target.gameObject.transform.GetChild(26).GetChild(0).transform;
        }
    }

    private void OnTriggerExit(Collider target)
    {
        if (target.gameObject.tag == "rope")
        {
            rope = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
