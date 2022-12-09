using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Horse_IK : MonoBehaviour
{
    public Transform t_root;
    public Transform t_L_thigh;
    public Transform t_L_clavicle;
    public Transform t_R_thigh;
    public Transform t_R_clavicle;
    public Transform spine;
    //public float angle;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateBones();
        //TurnBody();
    }

    void UpdateBones()
    {
        RaycastHit clavicle_hit;
        RaycastHit thigh_hit;

        if (Physics.Raycast(t_L_clavicle.position, Vector3.down, out clavicle_hit, 10) && Physics.Raycast(t_L_thigh.position, Vector3.down, out thigh_hit, 10))
        {
            float l = Vector3.Distance(thigh_hit.point, clavicle_hit.point);
            float h = thigh_hit.point.y - clavicle_hit.point.y;
            float angle = Mathf.Asin(h / l) * 180 / Mathf.PI;
            //angle = Mathf.Lerp(angle, Mathf.Asin(h / l) * 180 / Mathf.PI, Time.deltaTime);
            t_root.localEulerAngles = new Vector3(angle, 0, 0);
            t_root.localPosition = new Vector3(0, clavicle_hit.point.y, 0);

            //t_R_clavicle.localEulerAngles = t_R_clavicle.localEulerAngles + new Vector3(0,angle, 0);
            //t_L_clavicle.localEulerAngles = t_L_clavicle.localEulerAngles + new Vector3(0,angle, 0);

            //t_R_thigh.localEulerAngles = t_R_thigh.localEulerAngles + new Vector3(0, -angle, 0);
            //t_L_thigh.localEulerAngles = t_L_thigh.localEulerAngles + new Vector3(0, -angle, 0);

            Debug.DrawLine(t_L_clavicle.position, clavicle_hit.point);
        }
    }

    //void TurnBody()
    //{
    //    RaycastHit clavicle_hit;
    //    RaycastHit thigh_hit;
    //    if (Physics.Raycast(t_L_clavicle.position, Vector3.down, out clavicle_hit, 10) && Physics.Raycast(t_L_thigh.position, Vector3.down, out thigh_hit, 10))
    //    {
    //        spine.eulerAngles = new Vector3(0, 30, 0);
    //    }
    //}
}
