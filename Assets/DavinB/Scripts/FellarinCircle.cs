using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace DavinB
{
    public class FellarinCircle : FellarinState
    {
        int[] array = { -1, 1 };
        int arrayIndex;
        int posOrNeg;
        public FellarinCircle(FellarinTheGraceful fellarin, FellarinStateMachine fsm, Rigidbody fellarinRB, Animator fellarinAnim, CharacterMovement player, float moveSpeed, float attackSpeed, NavMeshAgent fellarinNav) : base(fellarin, fsm, fellarinRB, fellarinAnim, player, moveSpeed, attackSpeed, fellarinNav)
        {

        }

        public override void Exit()
        {
            int num = 0;
            fellarin.StopCoroutine(Circle(num));
        }

        public override void Enter()
        {
            arrayIndex = Random.Range(0, array.Length);
            posOrNeg = array[arrayIndex];
            int duration = Random.Range(4, 11);
            fellarin.StartCoroutine(Circle(duration));
        }


        public override void Do()
        {
            float dist = Vector3.Distance(player.transform.position, fellarin.transform.position);
            fellarinNav.speed = moveSpeed;
            fellarin.transform.RotateAround(player.transform.position, Vector3.up, posOrNeg *  moveSpeed * Time.deltaTime);
            fellarin.transform.LookAt(player.transform);
        }

        public override void FixedDo()
        {
            Collider[] hitColliders = Physics.OverlapSphere(fellarin.transform.position, 3);
            foreach (Collider collider in hitColliders)
            {
                if (collider.gameObject.layer == 6)
                {
                    arrayIndex = Random.Range(0, array.Length);
                    posOrNeg = array[arrayIndex];
                    if (posOrNeg == -1)
                    {
                        fsm.ChangeState(new FellarinJumpBack(fellarin, fsm, fellarinRB, fellarinAnim, player, moveSpeed, attackSpeed, fellarinNav));
                    }
                    else if (posOrNeg == 1)
                    {
                        fsm.ChangeState(new FellarinDartForward(fellarin, fsm, fellarinRB, fellarinAnim, player, moveSpeed, attackSpeed, fellarinNav));
                    }
                }
            }
        }

        IEnumerator Circle(int waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            if (Vector3.Distance(fellarin.transform.position, player.transform.position) > 20)
            {
                fsm.ChangeState(new FellarinDartForward(fellarin, fsm, fellarinRB, fellarinAnim, player, moveSpeed * 2, attackSpeed, fellarinNav));
            }
            else
            {
                int nextState = Random.Range(0, 2);
                if (nextState == 0)
                {
                    fsm.ChangeState(new FellarinJumpBack(fellarin, fsm, fellarinRB, fellarinAnim, player, moveSpeed, attackSpeed, fellarinNav));
                }
                else
                {
                    fsm.ChangeState(new FellarinDartForward(fellarin, fsm, fellarinRB, fellarinAnim, player, moveSpeed, attackSpeed, fellarinNav));
                }
            }
        }
    }
}
