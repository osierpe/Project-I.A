using UnityEngine;
using System.Collections;
using System;

public class UtilityFunction
{
    private float value;

    /*
     * The bool parameter indicates wether ValMax or ValMin called the function
     *  In case of Valmin, the PlayerUnits and the AdversaryUnits are switched in the respective state being evaluated  
     *  
     *  If the algorithm loses, it subtracts 200 and the Sum of the enemie's HP from the global variable "value";
     *  Else if the algotihm wins, it adds 200 + the sum of all it's players' HP to the global variable "value"
     *  In the end, it simply returns "value"
    */

    public float evaluate(State s, bool Max)
    {
        value = 0;
        if (Max)
        {
            if (s.PlayersUnits.Count == 0) // We (Max) LOST
            {
                foreach (Unit adv in s.AdversaryUnits)
                {
                    value -= adv.hp;
                }
                value -= 100000;
            }

            else
            { // We (Max) WON
                foreach (Unit u in s.PlayersUnits)
                {

                    value += u.hp;
                }
                value += 100000;
                value -= 100 * s.depth; // the less the depth, the greater the value
            }
        }
        else
        {
            if (s.PlayersUnits.Count == 0) // We (Max) WON
            {
                value += 100000;
                value -= 100*s.depth; // the less the depth, the greater the value
                foreach (Unit u in s.AdversaryUnits)
                {
                    value += u.hp;
                }
            }

            else // We (Max) LOST
            {
                value -= 100000;
                foreach (Unit u in s.PlayersUnits)
                {
                    value -= u.hp;
                }
            }
        }
        return value;
    }
}