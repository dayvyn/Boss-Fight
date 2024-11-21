using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace DavinB
{
    public class FellarinState
    {
        protected FellarinTheGraceful fellarin;
        protected FellarinStateMachine fsm;
        protected Rigidbody fellarinRB;
        protected Animator fellarinAnim;
        protected CharacterMovement player;
        protected float moveSpeed;
        protected float attackSpeed;
        protected NavMeshAgent fellarinNav;

        public virtual void Enter() { }

        public virtual void Do() { }

        public virtual void FixedDo() { }

        public virtual void Exit() { }

        //public virtual void GotHit() { fellarin.enemyStateMachine.ChangeState(fellarin.enemyHurt); }

        public FellarinState(FellarinTheGraceful fellarin, FellarinStateMachine fsm, Rigidbody fellarinRB, Animator fellarinAnim, CharacterMovement player, float moveSpeed, float attackSpeed, NavMeshAgent fellarinNav)
        {
            this.fellarin = fellarin;
            this.fsm = fsm;
            this.fellarinRB = fellarinRB;
            this.fellarinAnim = fellarinAnim;
            this.player = player;
            this.fellarinNav = fellarinNav;
            this.moveSpeed = moveSpeed;
            this.attackSpeed = attackSpeed;
        }
    }
}