using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace DavinB
{
    public class FellarinJumpBack : FellarinState
    {
        public FellarinJumpBack(FellarinTheGraceful fellarin, FellarinStateMachine fsm, Rigidbody fellarinRB, Animator fellarinAnim, CharacterMovement player, float moveSpeed, float attackSpeed, NavMeshAgent fellarinNav) : base(fellarin, fsm, fellarinRB, fellarinAnim, player, moveSpeed, attackSpeed, fellarinNav)
        {

        }

        public override void Do()
        {

        }

        public override void Enter()
        {
            fellarinNav.enabled = false;
            var direction = fellarin.transform.position - player.transform.position;
            fellarinRB.AddForce(direction.normalized * 15, ForceMode.Impulse);
            fellarinRB.AddForce(Vector3.up * 3, ForceMode.Impulse);
            fellarinRB.constraints = RigidbodyConstraints.FreezeRotation;
        }

        public override void Exit()
        {
            fellarinRB.useGravity = true;
            fellarinRB.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
        }

        public override void FixedDo()
        {
            if(fellarinRB.velocity.y < 0)
            {
                fellarinRB.useGravity = false;
                fellarinRB.AddForce(Vector3.down, ForceMode.Acceleration);
                if (Physics.Raycast(fellarin.transform.position, Vector3.down, 2, LayerMask.GetMask("Ground")))
                {
                    fellarinRB.velocity = new Vector3(Mathf.LerpUnclamped(fellarinRB.velocity.x, 0, .1f), fellarinRB.velocity.y, Mathf.LerpUnclamped(fellarinRB.velocity.z, 0, .1f));
                    if(fellarinRB.velocity == Vector3.zero)
                    {
                        if (Vector3.Distance(fellarin.transform.position, player.transform.position) > 30)
                        {
                            fsm.ChangeState(new FellarinDartForward(fellarin, fsm, fellarinRB, fellarinAnim, player, moveSpeed * 2, attackSpeed, fellarinNav));
                        }
                        else
                        {
                            int nextState = Random.Range(0, 2);
                            if (nextState == 0)
                            {
                                fsm.ChangeState(new FellarinCircle(fellarin, fsm, fellarinRB, fellarinAnim, player, moveSpeed, attackSpeed, fellarinNav));
                            }
                            else
                            {
                                fsm.ChangeState(new FellarinDartForward(fellarin, fsm, fellarinRB, fellarinAnim, player, moveSpeed, attackSpeed, fellarinNav));
                            }
                        }
                    }
                }
            }
        }

    }
}