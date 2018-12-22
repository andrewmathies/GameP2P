using UnityEngine;
using System;

[Serializable]
public class GameData {
    public enum State {
        Attack, Move, Stand, Block
    }

    public Vector2 Position { get; set; }
    public State CurState { get; set; }

    public GameData(float x, float y) {
        Position = new Vector2(x, y);
        CurState = State.Stand;
    }
}