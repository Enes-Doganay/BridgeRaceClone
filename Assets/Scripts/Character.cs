using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Character: MonoBehaviour
{
    private Camera cam;
    private Animator animator;
    public DynamicJoystick joystick;
    public float turnSpeed, speed,lerpValue;
    public LayerMask layer;

    public List<GameObject> cubes = new List<GameObject>();
    public Transform bag;
    public GameObject prevObj;

    private Transform stopCol;
    private int ropeChildCount;
    private bool rope = false;
    private GameObject activeRope;
    private void Start()
    {
        cam = Camera.main;
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            Movement();
        }
        else
        {
            if(animator.GetBool("running"))
                animator.SetBool("running", false);
        }
    }

    private void Movement()
    {
        float inputX = joystick.Horizontal;
        float inputY = joystick.Vertical;

        transform.position += new Vector3(inputX * speed * Time.deltaTime, 0, inputY * speed * Time.deltaTime);

        Vector3 direction = Vector3.forward * inputY  + Vector3.right * inputX;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), turnSpeed * Time.deltaTime);

        if (!animator.GetBool("running"))
        {
            animator.SetBool("running", true);
        }
    }
    private void OnTriggerEnter(Collider target)
    {
        if (target.gameObject.tag.StartsWith(transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material.name.Substring(0, 1)))
        {
            target.transform.SetParent(bag);
            Vector3 pos = prevObj.transform.localPosition;

            pos.y += 0.22f;
            pos.z = 0;
            pos.x = 0;

            target.transform.localRotation = new Quaternion(0, 0.7071068f, 0, 0.7071068f);

            target.transform.DOLocalMove(pos, 0.2f);
            prevObj = target.gameObject;
            cubes.Add(target.gameObject);

            target.tag = "Untagged";
            GenerateCubes.instance.GenerateCube(1);
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
            }
            else
            {
                prevObj = cubes[cubes.Count - 1];
            }

            if (rope)
            {
                stopCol.localPosition += new Vector3(0, 0, 0.45f);
            }
            if(stopCol.position.z > activeRope.transform.GetChild(ropeChildCount - 2).transform.position.z)
            {
                stopCol.gameObject.GetComponent<Collider>().isTrigger = true;
            }
        }

        if (target.gameObject.tag == "rope")
        {
            activeRope = target.gameObject;
            rope = true;
            ropeChildCount = target.transform.childCount;
            stopCol = target.gameObject.transform.GetChild(ropeChildCount - 1).GetChild(0).transform;
        }
        if (target.gameObject.tag == "Win")
        {
            animator.SetTrigger("Win");
            GetComponent<Character>().enabled = false;
        }
    }
    private void OnTriggerExit(Collider target)
    {
        if (target.gameObject.tag == "rope")
        {
            activeRope = null;
            rope = false;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 10f);
    }
}
