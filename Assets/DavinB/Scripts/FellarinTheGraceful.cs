using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace DavinB
{
    public class FellarinTheGraceful : MonoBehaviour
    {
        private Rigidbody fellarinRB;
        private NavMeshAgent fellarinNav;
        private CharacterMovement player;
        private FellarinStateMachine fsm;
        private FellarinTheGraceful fellarin;
        private FellarinCircle fellarinCircle;
        private Animator fellarinAnim;
        

        [SerializeField] float moveSpeed;
        [SerializeField] float attackSpeed;
        [SerializeField] int radius;

        private void OnEnable()
        {
            fellarinRB = GetComponent<Rigidbody>();
            fellarinNav = GetComponent<NavMeshAgent>();
            player = FindAnyObjectByType<CharacterMovement>();
            fellarin = GetComponent<FellarinTheGraceful>();
            fsm = new FellarinStateMachine();
            fellarinCircle = new FellarinCircle(fellarin, fsm, fellarinRB, fellarinAnim, player, moveSpeed, attackSpeed, fellarinNav);
            fsm.Initialize(fellarinCircle);
        }

        private void FixedUpdate()
        {
            fsm.currentState.FixedDo();
        }

        private void Update()
        {
            fsm.currentState.Do();
        }
    }
}