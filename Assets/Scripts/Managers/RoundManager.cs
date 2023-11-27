using System;
using System.Collections;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public event Action RoundEnded;

    public int ElapsedSeconds => Mathf.FloorToInt(TimeLeft);
    public float RoundTimeSeconds;
    public float TimeLeft { get; private set; }
    public string TimerTag => TimeSpan.FromSeconds(TimeLeft).ToString(@"mm\:ss");
    public bool Active { get; private set; }
    public StatInt Round = 0;
    public int KPS => masters.Sum(m => m.KPS);

    private Coroutine roundCoroutine;
    private PlayerInputManager playerInputManager;
    private UpgradesManager upgradesManager;
    private EnemyMaster[] masters;
    private FlowManager flowManager;

    [IngameDebugConsole.ConsoleMethod("endround", "Ends the current round")]
    public static void EndRound()
    {
        FindObjectOfType<RoundManager>().EndCurrentRound();
    }

    void Awake() { }

    void Start()
    {
        playerInputManager = SingletonLoader.Get<PlayerInputManager>();
        upgradesManager = SingletonLoader.Get<UpgradesManager>();
        TimeLeft = RoundTimeSeconds;
        masters = FindObjectsOfType<EnemyMaster>();
        flowManager = SingletonLoader.Get<FlowManager>();
        RoundTimeSeconds = CalculateRoundTime(1);
    }

    int CalculateRoundTime(int round) =>
        Mathf.CeilToInt(
            Mathf.Min(Mathf.Pow(round, 2.0f) + 60.0f, flowManager.GameData.Rounds.MaxRoundTime)
        );

    void Update()
    {
        if (Active)
        {
            TimeLeft += Time.deltaTime;
        }
    }

    public void StartRound()
    {
        TimeLeft = 0;
        // roundCoroutine = StartCoroutine(HandleRound());
        Round.Incr(1, StatOperation.Value);
        Active = true;
        masters.ForEach(spawner => spawner.enabled = true);
    }

    public void EndCurrentRound()
    {
        Active = false;
        RoundEnded?.Invoke();
        masters.ForEach(spawner => spawner.enabled = false);
        if (roundCoroutine != null)
            StopCoroutine(roundCoroutine);
        roundCoroutine = null;

        // pull in and recalculate weights of upgrades
        upgradesManager.UpdateUpgradeList();

        foreach (var machine in FindObjectsOfType<VendingMachine>())
        {
            if (upgradesManager.TryGetViableUpgrade(out UpgradeData data))
            {
                machine.Upgrade = data;
            }
            else
            {
                machine.Upgrade = null;
                break;
            }
        }

        RoundTimeSeconds = CalculateRoundTime(Round);
    }

    IEnumerator HandleRound()
    {
        Active = true;

        masters.ForEach(spawner => spawner.enabled = true);
        yield return new WaitForSeconds(TimeLeft);
        EndCurrentRound();
    }
}
