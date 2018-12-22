using UnityEngine;

[Serializable]
public class GameData {
    enum State {
        Attack, Move, Stand, Block;
    }

    private Vector2 Position { get; set; }
    private State CurState { get; set; }

    public GameData(float x, float y) {
        Position = new Vector2(x, y);
        CurState = State.Stand;
    }
}