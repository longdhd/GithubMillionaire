using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    GameBaseState currentState;
    public GameIdleState IdleState = new();
    public GameLifelineState LifelineState = new();
    public GameFinalState FinalState = new();
    public GameCorrectState CorrectState = new();
    public GameIncorrectState IncorrectState = new();

    // Start is called before the first frame update
    void Start()
    {
        currentState = IdleState;

        currentState.EnterState(this);
    }

    // Update is called once per frame
    void Update()
    {
        currentState.UpdateState(this);
    }

    public void SwitchState(GameBaseState newState)
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, 90f, transform.eulerAngles.z);

        currentState.ExitState(this);

        currentState = newState;
        newState.EnterState(this);
    }
}
