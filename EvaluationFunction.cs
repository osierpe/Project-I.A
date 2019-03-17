using UnityEngine;
using System.Collections;
using System;

public class EvaluationFunction
{
    // Do the logic to evaluate the state of the game !
    public float evaluate(State s)
    {
        float val = 0;
        if (s.isAttack)
            val = 150;
        
        return val;
    }
}
