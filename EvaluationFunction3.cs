using UnityEngine;
using System.Collections;
using System;

public class EvaluationFunction
{
    // Do the logic to evaluate the state of the game !
    public float evaluate(State s, bool max)
    {
        float val = 0;
        Unit current = s.unitToPermormAction;
        Tuple<float, float> bonus = current.GetBonus(s.board, s.PlayersUnits); //(currentHpBonus, currentAttackBonus)

        //Sum of our HP 
        for (int i = 0; i < s.PlayersUnits.Count; i++)
        {
            val  += s.PlayersUnits[i].hp;
        }
        //Subtraction of enemy's HP
        for (int k = 0; k < s.AdversaryUnits.Count; k++)
        {
            val -= s.AdversaryUnits[k].hp;
        }

        if (s.isAttack) // If Attack State
        {
            if (!max) //If is not a max State
            {
                Unit attacked = s.unitAttacked;
                val += 200;

                //Favour attacks where our HP is greater or equal to the unit attacked's HP
                if (current.hp >= attacked.hp)
                {
                    val += 200;
                }
                else
                {
                    val -= 100;
                }
                //Favour attacks where we have bonus(es)
                if (bonus.Item2 > 0 && bonus.Item1 > 0)
                {
                    val += 200;
                }
                else if (bonus.Item2 > 0)
                {
                    val += 75;
                }
                else if (bonus.Item1 > 0)
                {
                    val += 50;
                }

            }
            if (max) //If is a max State
            {
                Unit attacked = s.unitAttacked;
                val -= 200;

                //Favour attacks where them HP is less to the unit attacked's HP
                if (current.hp >= attacked.hp)
                {
                    val -= 200;
                }
                else
                {
                    val += 100;
                }
                //Favour attacks where they have not bonus(es)
                if (bonus.Item2 > 0 && bonus.Item1 > 0)
                {
                    val -= 200;
                }
                else if (bonus.Item2 > 0)
                {
                    val -= 100;
                }
                else if (bonus.Item1 > 0)
                {
                    val -= 100;
                }

            }
        }
        else //If Move State
        {
            //Favour moves where we keep/have bonuses 
            if (!max)
            {
                if (bonus.Item2 > 0 && bonus.Item1 > 0)
                {
                    val += 200;
                }
                else if (bonus.Item2 > 0)
                {
                    val += 75;
                }
                else if (bonus.Item1 > 0)
                {
                    val += 50;
                }
            }
            else //the opposite for their move
            {
                if (bonus.Item2 > 0 && bonus.Item1 > 0)
                {
                    val -= 200;
                }
                else if (bonus.Item2 > 0)
                {
                    val -= 75;
                }
                else if (bonus.Item1 > 0)
                {
                    val -= 50;
                }
            }
        }
        
        return val;
    }
}