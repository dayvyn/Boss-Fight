using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace DavinB
{
    public class FellarinJumpBack : FellarinState
    {
        bool dragDown = false;
        bool jumped = false;
        public FellarinJumpBack(FellarinTheGraceful fellarin, FellarinStateMachine fsm, Rigidbody fellarinRB, Animator fellarinAnim, CharacterMovement player, float moveSpeed, float attackSpeed, NavMeshAgent fellarinNav) : base(fellarin, fsm, fellarinRB, fellarinAnim, player, moveSpeed, attackSpeed, fellarinNav)
        {
            
        }
        public override void Enter()
        {
            fellarinNav.enabled = false;
            var direction = fellarin.transform.position - player.transform.position;
            direction.y = 0;
            fellarinRB.AddForce(direction.normalized * 15, ForceMode.Impulse);
            fellarinRB.AddForce(Vector3.up * 3, ForceMode.Impulse);
            fellarinRB.constraints = RigidbodyConstraints.FreezeRotation;
            jumped = true;
        }

        public override void Exit()
        {
            fellarinRB.useGravity = true;
            fellarinRB.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
            dragDown = false;
        }

        public override void FixedDo()
        {
            if(fellarinRB.velocity.y < 0f && jumped)
            {
                fellarinRB.useGravity = false;
                dragDown = true;
                if (Physics.Raycast(fellarin.transform.position, Vector3.down, 3f, LayerMask.GetMask("Ground")))
                {
                    fellarinRB.velocity = new Vector3(Mathf.LerpUnclamped(fellarinRB.velocity.x, 0, .1f), fellarinRB.velocity.y, Mathf.LerpUnclamped(fellarinRB.velocity.z, 0, .1f));
                    
                    if (Approximation(fellarinRB.velocity.y, 0, .00002f))
                    {
                        dragDown = false;
                        if (Vector3.Distance(fellarin.transform.position, player.transform.position) > 30)
                        {
                            fsm.ChangeState(new FellarinDartForward(fellarin, fsm, fellarinRB, fellarinAnim, player, moveSpeed * 2, attackSpeed, fellarinNav));
                        }
                        else
                        {
                            int nextState = Random.Range(0, 3);
                            switch (nextState)
                            {
                                case 0:
                                    fsm.ChangeState(new FellarinCircle(fellarin, fsm, fellarinRB, fellarinAnim, player, moveSpeed, attackSpeed, fellarinNav));
                                    break;
                                case 1:
                                    fsm.ChangeState(new FellarinDartForward(fellarin, fsm, fellarinRB, fellarinAnim, player, moveSpeed, attackSpeed, fellarinNav));
                                    break;
                                case 2:
                                    fsm.ChangeState(new FellarinSpawnPickups(fellarin, fsm, fellarinRB, fellarinAnim, player, moveSpeed, attackSpeed, fellarinNav));
                                    break;
                            }
                        }
                    }
                }
            }
            if (dragDown == true)
            {
                fellarinRB.AddForce(Vector3.down, ForceMode.Acceleration);
            }
        }
        private bool Approximation(float a, float b, float tolerance)
        {
            return (Mathf.Abs(a - b) < tolerance);
        }
    }
}