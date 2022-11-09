using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameBaseState
{
    public abstract void EnterState(GameStateManager state);
    public abstract void UpdateState(GameStateManager state);
    public abstract void ExitState(GameStateManager state);

}
