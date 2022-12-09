using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject horse;
    private Animator animator;
    public Animator horse_animator;
    private bool onHorse = false;
    public bool isMoving = false;
    public GameObject parent;
    public GameObject topBone;
    public GameObject view;
    private LayerMask mask;
    public bool canMove = false;
    private BoxCollider collider;
    public CapsuleCollider horse_collider;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        mask = LayerMask.GetMask("Obstacle");
        collider = transform.gameObject.GetComponent<BoxCollider>();
        print(mask.value);

    }

    // Update is called once per frame
    void Update()
    {
        if (!onHorse)
        {
            Move();
            Rotate();
            if (Input.GetKeyDown("space"))
            {
                Mount();
                onHorse = true;
                canMove = true;
            }
        }

        if (onHorse)
        {
            if (!(animator.GetCurrentAnimatorStateInfo(0).IsName("Mount_on_Horse") && animator.GetFloat("New Float") == 0f) && !(animator.GetCurrentAnimatorStateInfo(0).IsName("Rise")))
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(-topBone.transform.localPosition.x, -topBone.transform.localPosition.y,0), Time.deltaTime);
                transform.localRotation = Quaternion.Euler(0,-topBone.transform.localRotation.eulerAngles.y, 0);
                transform.GetChild(0).localPosition = Vector3.Lerp(transform.GetChild(0).localPosition, Vector3.zero, Time.deltaTime);
            }

            if (canMove)
            {
                HorseMove();
                HorseRotate();
                parent.transform.localRotation = Quaternion.Euler(0,5f,0);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                Dismount();
                onHorse = false;
                canMove = false;
            }

            if (Physics.Raycast(view.transform.position, Vector3.forward, 3, mask.value))
            {
                RiseUp();
            }
            else
            {
                horse_animator.SetBool("RiseUp", false);
                canMove = true;
                //horse_collider.enabled = true;
            }
        }
    }

    void Move()
    {
        float vMove = Input.GetAxis("Vertical");

        transform.Translate(Vector3.forward * vMove * 1.5f * Time.deltaTime);

        isMoving = vMove != 0;
        if (isMoving)
        {
            animator.SetBool("Walking", true);
        }
        else
        {
            animator.SetBool("Walking", false);
        }
    }

    public void RiseUp()
    {
        canMove = false;
        horse_animator.SetBool("IsWalking", false);
        animator.SetBool("Walking", false);
        Debug.Log("Cliff");
        horse_animator.SetBool("RiseUp", true);
        horse.transform.position = Vector3.Lerp(horse.transform.position, horse.transform.position + new Vector3(0, 0, -1f), Time.deltaTime);
        //horse_collider.enabled = false;
    }

    void HorseMove()
    {
        float vMove = Input.GetAxis("Vertical");

        horse.transform.Translate(Vector3.forward * vMove * 1.5f * Time.deltaTime);

        if (vMove != 0)
        {
            horse_animator.SetBool("IsWalking", true);
            animator.SetBool("Walking", true);
        }
        else
        {
            horse_animator.SetBool("IsWalking", false);
            animator.SetBool("Walking", false);
        }
    }

    void Rotate()
    {
        float hMove = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up, hMove * 180 * Time.deltaTime);
    }

    void HorseRotate()
    {
        float hMove = Input.GetAxis("Horizontal");
        horse.transform.Rotate(Vector3.up, hMove * 90 * Time.deltaTime);
        horse_animator.SetFloat("RunningDir", 0.5f + hMove);
    }

    void Mount()
    {
        transform.parent = parent.transform;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.localRotation = Quaternion.Euler(0, 0, 0);
        animator.SetBool("On_Horse", true);
        horse_animator.SetBool("onHorse", true);
        animator.SetFloat("New Float", 0f);
        horse_animator.SetFloat("New Float", 0f);
        collider.enabled = false;
    }

    void Dismount()
    {
        transform.parent = null;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        animator.SetBool("On_Horse", false);
        horse_animator.SetBool("onHorse", false);
        animator.SetFloat("New Float", 1f);
        horse_animator.SetFloat("New Float", 1f);
        collider.enabled = true;
    }
}
