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
        Transform fellarinTransform;
        Transform playerTransform;
        float _rMin = 5f;
        float _rMax = 50f;
        float circleSpeed = 3f;

        public FellarinCircle(FellarinTheGraceful fellarin, FellarinStateMachine fsm, Rigidbody fellarinRB, Animator fellarinAnim, CharacterMovement player, float moveSpeed, float attackSpeed, NavMeshAgent fellarinNav) : base(fellarin, fsm, fellarinRB, fellarinAnim, player, moveSpeed, attackSpeed, fellarinNav)
        {

        }

        public override void Exit()
        {
            fellarin.StopCoroutine(Circle(0));
        }

        public override void Enter()
        {
            Debug.Log("Entered Circle");
            arrayIndex = Random.Range(0, array.Length);
            posOrNeg = array[arrayIndex];
            int duration = Random.Range(4, 11);
            fellarin.StartCoroutine(Circle(duration));
            fellarinTransform = fellarin.transform;
            playerTransform = player.transform;
        }


        public override void Do()
        {
            float deltaTime = Time.deltaTime;
            Vector3 O = playerTransform.position;
            Vector3 P = fellarinTransform.position;
            Vector3 R = P - O;
            float r = Mathf.Clamp(
                Vector3.Magnitude(R) - (circleSpeed * deltaTime),
                _rMin, _rMax
            );
            float c = 2f * Mathf.PI * r;
            float angle = (circleSpeed * deltaTime / c) * 360f;
            Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up);

            fellarinTransform.position = O + (rot * Vector3.Normalize(R) * r);
            fellarinTransform.LookAt(playerTransform);
        }

        public override void FixedDo()
        {
            Collider[] hitColliders = Physics.OverlapSphere(fellarinTransform.position, 3);
            foreach (Collider collider in hitColliders)
            {
                if (collider.gameObject.layer == 6)
                {
                   
                    int nextState = Random.Range(0, 3);
                    Debug.Log(nextState);
                    switch (nextState)
                    {
                        case 0:
                            fsm.ChangeState(new FellarinJumpBack(fellarin, fsm, fellarinRB, fellarinAnim, player, moveSpeed, attackSpeed, fellarinNav));
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

        IEnumerator Circle(int waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            if (Vector3.Distance(fellarinTransform.position, playerTransform.position) > 20)
            {
                fsm.ChangeState(new FellarinDartForward(fellarin, fsm, fellarinRB, fellarinAnim, player, moveSpeed * 2, attackSpeed, fellarinNav));
            }
            else
            {
                int nextState = Random.Range(0, 3);
                switch (nextState)
                {
                    case 0:
                        fsm.ChangeState(new FellarinJumpBack(fellarin, fsm, fellarinRB, fellarinAnim, player, moveSpeed, attackSpeed, fellarinNav));
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
