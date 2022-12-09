using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class WaitStateFlockUnit : FlockUnitStates
{
    public FlockUnitStates patrolState;
    public FlockUnitStates attackState;
    public FlockUnitStates runAwayState;
    System.Random random = new System.Random();

    public override FlockUnitStates DoState(FlockUnitController deer)
    {
        if (deer.TimeOut(random.Next(5, 11)))
        {
            patrolState.EnterState(deer);
            deer.SetState(patrolState);
            return patrolState;
        }
        if (deer.EnemyFind() && deer.girl == false)
        {
            attackState.EnterState(deer);
            deer.SetState(attackState);
            return attackState;
        }
        if(deer.EnemyFind() && deer.girl == true)
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
        //deer.Agent.speed = 0;
        //deer.Agent.isStopped = true;
        //deer.Animator.SetBool("IsRunning", false);
        deer.Animator.SetBool("IsWalking", false);
        deer.Animator.SetBool("IsGrazing", true);
        deer.Animator.SetBool("IsAttack", false);
    }
}
