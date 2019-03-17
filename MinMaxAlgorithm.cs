using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DeepCopyExtensions;

public class MinMaxAlgorithm: MoveMaker
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
        /////////////////////////////
        // You should implement this
        
        valor = Valmax(e); //chama a função do valor maximo
        return movimento;  // retorna movimento associado ao maior valor 
        
        /////////////////////////////
        
    }
    public float Valmax(State s)
    {
        if ((s.depth % 2)==0 & s.depth !=0)
        {
            valor = evaluator.evaluate(s);
           
            
            return valor;
        }
        valor = -9999999;
        List<State> estadosfilhos1 = GeneratePossibleStates(s); //generate children
        float tmp;
        
        Debug.Log("filhos da camada"+s.depth+" = "+ estadosfilhos1.Count);
        for (int i = 0; i < estadosfilhos1.Count; i++)
        {
            tmp = valor;
            
            State ss = new State(estadosfilhos1[i]);
            valor = Math.Max(valor, Valmin(ss));
            if (s.isRoot && (tmp < valor))
                
                movimento = estadosfilhos1[i];//guarda movimento associado ao maior valor
        }
        
        return valor;
    }
    public float Valmin(State st)
    {
        if ((st.depth % 2)== 0 & st.depth!=0)
        {
            
            valor = evaluator.evaluate(st);
            
            return valor;
        }
       
        valor = 9999999;
        List<State> estadosfilhos2 = GeneratePossibleStates(st); //generate children
        Debug.Log("filhos da camada" + st.depth + " = " + estadosfilhos2.Count);
        for (int i = 0; i < estadosfilhos2.Count; i++)
        {
            State sta = new State(estadosfilhos2[i]);
            valor = Math.Min(valor, Valmax(sta));

        }
        
        return valor;
    }
    private List<State> GeneratePossibleStates(State state)
    {
        List<State> states = new List<State>();
        //Generate the possible states available to expand
        foreach(Unit currentUnit in state.PlayersUnits)
        {
            // Movement States
            List<Tile> neighbours = currentUnit.GetFreeNeighbours(state);
            Debug.Log(currentUnit.id+" posição "+currentUnit.x+" , "+currentUnit.y+" pode mover em "+neighbours.Count);
            
            foreach (Tile t in neighbours)
            {
                State newState = new State(state, currentUnit, true);
                newState = MoveUnit(newState, t);
                states.Add(newState);
                
            }
            // Attack states
            List<Unit> attackOptions = currentUnit.GetAttackable(state, state.AdversaryUnits);
            Debug.Log("unidade corrente "+currentUnit.id+" opçoes para ataque "+ attackOptions.Count);
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

    private State MoveUnit(State state,  Tile destination)
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
