using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PatrolStateFlockUnit : FlockUnitStates
{
    public FlockUnitStates waitState;
    public FlockUnitStates attackState;
    public FlockUnitStates runAwayState;
    System.Random random = new System.Random();

    //public DeerDoeStates alertState;

    public override FlockUnitStates DoState(FlockUnitController deer)
    {
        deer.MoveUnit();

        //if (deer.DestinationReached())
        //{
        //    waitState.EnterState(deer);
        //    deer.SetState(waitState);
        //    return waitState;
        //}

        if (deer.TimeOut(random.Next(5, 11)))
        {
            waitState.EnterState(deer);
            deer.SetState(waitState);
            return waitState;
        }
        if (deer.EnemyFind() && deer.girl == false)
        {
            attackState.EnterState(deer);
            deer.SetState(attackState);
            return attackState;
        }
        if (deer.EnemyFind() && deer.girl == true)
        {
            runAwayState.EnterState(deer);
            deer.SetState(runAwayState);
            return runAwayState;
        }
        return this;
    }

    public override void EnterState(FlockUnitController deer)
    {
        deer.Timer = 0;
        //deer.Agent.isStopped = false;
        //deer.Animator.SetBool("IsRunning", false);
        deer.Animator.SetBool("IsWalking", true);
        deer.Animator.SetBool("IsGrazing", false);
        deer.Animator.SetBool("IsAttack", false);
    }
}