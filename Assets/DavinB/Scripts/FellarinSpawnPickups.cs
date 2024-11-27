using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace DavinB
{
    public class FellarinSpawnPickups : FellarinState
    {
        public FellarinSpawnPickups(FellarinTheGraceful fellarin, FellarinStateMachine fsm, Rigidbody fellarinRB, Animator fellarinAnim, CharacterMovement player, float moveSpeed, float attackSpeed, NavMeshAgent fellarinNav) : base(fellarin, fsm, fellarinRB, fellarinAnim, player, moveSpeed, attackSpeed, fellarinNav)
        {
            
        }

        public override void Enter()
        {
            fellarin.StartCoroutine(SpawnZones());
        }

        public override void Exit()
        {
            fellarin.StopCoroutine(SpawnZones());
        }


        IEnumerator SpawnZones()
        {
            for (int i = 0; i < 4; i++)
            {
                Vector3 spawnpoint = new Vector3(Random.Range(fellarin.radii[0].position.x, fellarin.radii[1].position.x), 5, Random.Range(fellarin.radii[0].position.z, fellarin.radii[1].position.z));
                Object.Instantiate(fellarin.aoePickup, spawnpoint, Quaternion.identity);
            }
            yield return new WaitForSeconds(6);
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
                    fsm.ChangeState(new FellarinCircle(fellarin, fsm, fellarinRB, fellarinAnim, player, moveSpeed, attackSpeed, fellarinNav));
                    break;
            }
        }
    }
}