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
    public GameFinishState FinishState = new();

    [SerializeField] public GameObject[] objsToHide;
    [SerializeField] public GameObject[] objsToShow;

    private Animator _playerAnimator;
    private int animIDLifeline;
    private int animIDFinal;
    private int animIDIncorrect;
    private int animIDCorrect1;
    private int animIDCorrect2;

    [SerializeField] private Animator _hostAnimator;

    [SerializeField]
    public AudioClip[] IdleAudioClip;
    [SerializeField]
    public AudioClip CorrectAudioClip;
    [SerializeField]
    public AudioClip IncorrectAudioClip;
    [SerializeField]
    public AudioClip FinishAudioClip;

    [SerializeField]
    public AudioClip[] CorrectEffectClip;
    [SerializeField]
    public AudioClip[] IncorrectEffectClip;
    [SerializeField]
    public AudioClip FinalEffectClip;

    public Animator PlayerAnimator { get { return _playerAnimator; } }
    public Animator HostAnimator { get { return _hostAnimator; } }
    public int AnimIDLifeline { get { return animIDLifeline; } }
    public int AnimIDFinal { get { return animIDFinal; } }
    public int AnimIDIncorrect { get { return animIDIncorrect; } }
    public int AnimIDCorrect1 { get { return animIDCorrect1; } }
    public int AnimIDCorrect2 { get { return animIDCorrect2; } }

    void Awake()
    {
        SetupVariables();
    }

    void Start()
    {
        currentState = IdleState;

        currentState.EnterState(this);
    }

    void Update()
    {
        currentState.UpdateState(this);
    }

    public void SwitchState(GameBaseState newState)
    {
        if (currentState != newState)
        {
            currentState.ExitState(this);

            currentState = newState;
            newState.EnterState(this);
        }
    }

    void SetupVariables()
    {
        _playerAnimator = GetComponent<Animator>();
        animIDLifeline = Animator.StringToHash("Lifeline");
        animIDFinal = Animator.StringToHash("Final");
        animIDIncorrect = Animator.StringToHash("Incorrect");
        animIDCorrect1 = Animator.StringToHash("Correct1");
        animIDCorrect2 = Animator.StringToHash("Correct2");
    }
}
