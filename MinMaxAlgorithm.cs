﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DeepCopyExtensions;

public class MinMaxAlgorithm : MoveMaker
{
    State movimento;
    float valor;
    public EvaluationFunction evaluator;
    private UtilityFunction utilityfunc;
    public int depth = 0;
    private PlayerController MaxPlayer;
    private PlayerController MinPlayer;

    public MinMaxAlgorithm(PlayerController MaxPlayer, EvaluationFunction eval, UtilityFunction utilf, PlayerController MinPlayer)
    {
        this.MaxPlayer = MaxPlayer;
        this.MinPlayer = MinPlayer;
        this.evaluator = eval;
        this.utilityfunc = utilf;
    }

    public override State MakeMove()
    {
        // The move is decided by the selected state
        return GenerateNewState();
    }

    private State GenerateNewState()
    {
        // Creates initial state
        State newState = new State(this.MaxPlayer, this.MinPlayer);
        // Call the MinMax implementation
        State bestMove = MinMax(newState);
        // returning the actual state. You should modify this
        return bestMove;
    }

    public State MinMax(State e)
    {
        MaxPlayer.ExpandedNodes = 0;
        valor = Valmax(e); //Calls Valmax in initial state
        return movimento;  // Returns move associated with the greatest value
    }


    public float Valmax(State s)
    {
        if (s.depth != 0)
        {
            s = new State(s);
        }

        //Using Utility function in case of GAME OVER (in which case either of sides will have no units left)
        //GAME OVER -> Final State
        if (s.AdversaryUnits.Count == 0 || s.PlayersUnits.Count == 0)
        {
            s.Score = utilityfunc.evaluate(s, true);
        }

        // Base Case for Recursivity
        if (MaxPlayer.ExpandedNodes >= MaxPlayer.MaximumNodesToExpand) 
        {
            valor = evaluator.evaluate(s);
            return valor;
        }

        // Setting Value to -infinity
        valor = -9999999;
        // Generating children
        List<State> estadosfilhos1 = GeneratePossibleStates(s);

        float tmp;

        for (int i = 0; i < estadosfilhos1.Count; i++)
        {
            tmp = valor;
            valor = Math.Max(valor, Valmin(((estadosfilhos1[i]))));

            //If ValMax is at Root and the tmp is different from val, we set this child as movement since it has biggest value
            if ((s.isRoot) && (tmp != valor))
            {
                movimento = estadosfilhos1[i]; //Saves move associated with greatest value
            }
        }
        return valor;
    }

    public float Valmin(State s)
    {
        s = new State(s);

        //Using UtilityFunc in case of GAME OVER (in which case either of sides will have no units left)
        //GAME OVER -> Final State
        if(s.AdversaryUnits.Count == 0 || s.PlayersUnits.Count == 0)
        {
            s.Score = utilityfunc.evaluate(s, false);
        }

        // Base Case for Recursivity
        if (MaxPlayer.ExpandedNodes >= MaxPlayer.MaximumNodesToExpand) 
        {
            s = new State(s);
            valor = evaluator.evaluate(s);
            return valor;
        }

        //Setting Value to +infinity
        valor = 9999999;

        // Generating Children
        List<State> estadosfilhos1 = GeneratePossibleStates((s));

        // Iterating through generated children and retrieving Minimum Value
        for (int i = 0; i < estadosfilhos1.Count; i++)
        {
            valor = Math.Min(valor, Valmax((estadosfilhos1[i])));
        }
        return valor;
    }


    private List<State> GeneratePossibleStates(State state)
    {
        List<State> states = new List<State>();
        //Generate the possible states available to expand
        foreach (Unit currentUnit in state.PlayersUnits)
        {
            // Movement States
            List<Tile> neighbours = currentUnit.GetFreeNeighbours(state);
            Debug.Log(currentUnit.id + " posição " + currentUnit.x + " , " + currentUnit.y + " pode mover em " + neighbours.Count);

            foreach (Tile t in neighbours)
            {
                State newState = new State(state, currentUnit, true);
                newState = MoveUnit(newState, t);
                states.Add(newState);

            }
            // Attack states
            List<Unit> attackOptions = currentUnit.GetAttackable(state, state.AdversaryUnits);
            Debug.Log("unidade corrente " + currentUnit.id + " opçoes para ataque " + attackOptions.Count);
            foreach (Unit t in attackOptions)
            {
                State newState = new State(state, currentUnit, false);
                newState = AttackUnit(newState, t);
                states.Add(newState);

            }

        }

        // YOU SHOULD NOT REMOVE THIS
        // Counts the number of expanded nodes;
        this.MaxPlayer.ExpandedNodes += states.Count;
        //

        return states;
    }

    private State MoveUnit(State state, Tile destination)
    {
        Unit currentUnit = state.unitToPermormAction;
        //First: Update Board
        state.board[(int)destination.gridPosition.x, (int)destination.gridPosition.y] = currentUnit;
        state.board[currentUnit.x, currentUnit.y] = null;
        //Second: Update Players Unit Position
        currentUnit.x = (int)destination.gridPosition.x;
        currentUnit.y = (int)destination.gridPosition.y;
        state.isMove = true;
        state.isAttack = false;
        return state;
    }

    private State AttackUnit(State state, Unit toAttack)
    {
        Unit currentUnit = state.unitToPermormAction;
        Unit attacked = toAttack.DeepCopyByExpressionTree();

        Tuple<float, float> currentUnitBonus = currentUnit.GetBonus(state.board, state.PlayersUnits);
        Tuple<float, float> attackedUnitBonus = attacked.GetBonus(state.board, state.AdversaryUnits);


        attacked.hp += Math.Min(0, (attackedUnitBonus.Item1)) - (currentUnitBonus.Item2 + currentUnit.attack);
        state.unitAttacked = attacked;

        if (attacked.hp <= 0)
        {
            //Board update by killing the unit!
            state.board[attacked.x, attacked.y] = null;
            int index = state.AdversaryUnits.IndexOf(attacked);
            state.AdversaryUnits.RemoveAt(index);

        }
        state.isMove = false;
        state.isAttack = true;

        return state;

    }
}
