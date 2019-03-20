using UnityEngine;
using System.Collections;
using System;

public class EvaluationFunction
{
    // Do the logic to evaluate the state of the game !
    public float evaluate(State s)
    {
        float val = 0;

        Unit current = s.unitToPermormAction;
        Tuple<float, float> bonus = current.GetBonus(s.board, s.PlayersUnits); //(currentHpBonus, currentAttackBonus)

        //Sum of our HP and adding "2 to the power of AttackableUnits" to val
        for (int i = 0; i < s.PlayersUnits.Count; i++)
        {
            val += s.PlayersUnits[i].hp;

            int count = s.PlayersUnits[i].GetAttackable().Count;
            if (count > 0)
            {
                val += (float)Math.Pow(2, count);
            }

        }

        //Sum of enemy's HP
        for (int k = 0; k < s.AdversaryUnits.Count; k++)
        {
            val -= s.AdversaryUnits[k].hp;
        }
        
        if (s.isAttack) // If Attack State
        {
            Unit attacked = s.unitAttacked;
            val += 200;

            //Favour attacks where our HP is greater or equal to the unit attacked's HP
            if(current.hp >= attacked.hp)
            {
                val += 200;
            }else
            {
                val -= 100;
            }

            //Favour attacks where we have bonus(es)
            if (bonus.Item2 > 0 && bonus.Item1 > 0)
            {
                val += 100;
            }else if (bonus.Item2 > 0)
            {
                val += 75;
            }else if (bonus.Item1 > 0)
            {
                val += 50;
            }
        }
        else //If Move State
        {
            //Favour moves where we keep/have bonuses 
            if (bonus.Item2 > 0 && bonus.Item1 > 0)
            {
                val += 100;
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
        return val;
    }
}

