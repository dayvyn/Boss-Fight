using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace DavinB
{
    public class FellarinDartForward : FellarinState
    {
        public FellarinDartForward(FellarinTheGraceful fellarin, FellarinStateMachine fsm, Rigidbody fellarinRB, Animator fellarinAnim, CharacterMovement player, float moveSpeed, float attackSpeed, NavMeshAgent fellarinNav) : base(fellarin, fsm, fellarinRB, fellarinAnim, player, moveSpeed, attackSpeed, fellarinNav)
        {

        }

        public override void Enter()
        {
            fellarinNav.enabled = false;
            fellarin.StartCoroutine(enumerator());
        }

        public override void FixedDo()
        {
            var vector = player.transform.position - fellarinRB.transform.position;
            vector.y = 0;
            fellarinRB.velocity = vector.normalized * moveSpeed;
        }

        IEnumerator enumerator()
        {
            moveSpeed = 0;
            yield return new WaitForSeconds(1.5f);
            moveSpeed = 20;
        }
    }
}