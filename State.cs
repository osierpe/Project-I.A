using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DeepCopyExtensions;
using System;

public class State : System.IComparable
{

    public float Score { get; set; }
    public Unit[,] board;
    public List<Unit> PlayersUnits;
    public List<Unit> AdversaryUnits;
    public Unit unitToPermormAction;
    public Unit unitAttacked;
    public State parentState;
    public bool isMove = false;
    public bool isAttack = false;
    public bool isRoot;
    public int depth = -1;



    public State(PlayerController player, PlayerController adversary)
    {
        // Generates an initial state before entering the adversarial search
        this.Score = 0.0f;
        this.board = GameManager.instance.board.DeepCopyByExpressionTree(); 
        this.PlayersUnits = player.PlayersUnits.DeepCopyByExpressionTree(); 
        this.AdversaryUnits = adversary.PlayersUnits.DeepCopyByExpressionTree();
        this.unitToPermormAction = null;
        this.unitAttacked = null;
        this.parentState = null;
        this.isRoot = true;
        this.depth = 0;
    }

    public State(State parentState, Unit currentUnit, bool isMove)
    {
        // Generates a new State based on the parent state
        this.Score = 0.0f;
        this.board = parentState.board.DeepCopyByExpressionTree(); 
        this.parentState = parentState.DeepCopyByExpressionTree(); 
        this.PlayersUnits = this.parentState.PlayersUnits;
        this.AdversaryUnits = this.parentState.AdversaryUnits;
        int indexToPermormAcion = this.PlayersUnits.IndexOf(currentUnit);
        this.unitToPermormAction = this.PlayersUnits[indexToPermormAcion];
        this.isMove = isMove;
        this.isAttack = !isMove;
        this.parentState = parentState.DeepCopyByExpressionTree();
        this.depth = parentState.depth + 1;
    }

    public State(State s)
    {
        // Performs Deep copy of a state. Changes Min Max Perspective
        this.Score = s.Score;
        this.board = s.board.DeepCopyByExpressionTree();
        this.parentState = s.parentState.DeepCopyByExpressionTree();
        if (s.isRoot)
        {
            this.PlayersUnits = s.PlayersUnits.DeepCopyByExpressionTree();
            this.AdversaryUnits = s.AdversaryUnits.DeepCopyByExpressionTree();
        }
        else
        {
            this.PlayersUnits = s.AdversaryUnits.DeepCopyByExpressionTree();
            this.AdversaryUnits = s.PlayersUnits.DeepCopyByExpressionTree();
        }
        this.unitToPermormAction = s.unitToPermormAction.DeepCopyByExpressionTree();
        this.unitAttacked = s.unitAttacked.DeepCopyByExpressionTree();
        this.isMove = s.isMove;
        this.isAttack = s.isAttack;
        this.depth = s.depth;
    }

    public bool IsMove()
    {
        return isMove;
    }

    public bool IsAttack()
    {
        return isAttack;
    }

    public int CompareTo(object obj)
    {
        State other = obj as State;
        if (other != null)
            return this.Score.CompareTo(other.Score);
        else
            throw new ArgumentException("Object is not a State");
    }
}