using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DavinB
{
    public class FellarinStateMachine
    {
        public FellarinState currentState { get; set; }

        public void Initialize(FellarinState startingState)
        {
            currentState = startingState;
            currentState.Enter();
        }
        public void ChangeState(FellarinState changingState)
        {
            currentState.Exit();
            currentState = changingState;
            changingState.Enter();
        }
    }
}