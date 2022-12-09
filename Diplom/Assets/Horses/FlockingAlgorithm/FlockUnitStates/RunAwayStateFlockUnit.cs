using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RunAwayStateFlockUnit : FlockUnitStates
{
    public FlockUnitStates patrolState;
    System.Random random = new System.Random();

    //public DeerDoeStates alertState;

    public override FlockUnitStates DoState(FlockUnitController deer)
    {
        //deer.MoveUnit();

        //if (deer.DestinationReached())
        //{
        //    waitState.EnterState(deer);
        //    deer.SetState(waitState);
        //    return waitState;
        //}

        //for(int i =0; i < deer.flockArr.allUnits.Length; i++)
        //{
        //    if(Vector3.Distance(deer.flockArr.allUnits[i].transform.position, deer.transform.position) < 5f)
        //    {
        //        Debug.Log("AAAAAAAAAAAa");
        //    }
        //}

        deer.RunAway();

        if (!deer.EnemyFind())
        {
            patrolState.EnterState(deer);
            deer.SetState(patrolState);
            deer.InitializeSpeed(5f);
            return patrolState;
        }
        return this;
    }

    public override void EnterState(FlockUnitController deer)
    {
        deer.Timer = 0;
        //deer.Agent.isStopped = true;
        //deer.Agent.speed = 0;
        deer.Animator.SetBool("IsAttack", false);
        deer.Animator.SetBool("IsWalking", true);
        deer.Animator.SetBool("IsGrazing", false);
    }
}