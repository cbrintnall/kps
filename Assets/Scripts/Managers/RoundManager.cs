using System;
using System.Collections;
using System.Linq;
using Newtonsoft.Json;
using Sirenix.Utilities;
using UnityEngine;

[Singleton]
public class RoundManager : MonoBehaviour
{
    public event Action RoundEnded;

    public StatFloat RoundTimeSeconds = 60f;
    public float TimeLeft { get; private set; }
    public string TimerTag => TimeSpan.FromSeconds(TimeLeft).ToString(@"mm\:ss");
    public bool Active { get; private set; }
    public StatInt Round = 0;
    public int KPS => masters.Sum(m => m.KPS);

    private Coroutine roundCoroutine;
    private PlayerInputManager playerInputManager;
    private UpgradesManager upgradesManager;
    private bool active_;
    private EnemyMaster[] masters;

    [IngameDebugConsole.ConsoleMethod("endround", "Ends the current round")]
    public static void EndRound()
    {
        SingletonLoader.Get<RoundManager>().EndCurrentRound();
    }

    void Awake() { }

    void Start()
    {
        playerInputManager = SingletonLoader.Get<PlayerInputManager>();
        upgradesManager = SingletonLoader.Get<UpgradesManager>();
        TimeLeft = RoundTimeSeconds;
        masters = FindObjectsOfType<EnemyMaster>();
    }

    void Update()
    {
        if (!Active && playerInputManager.StartRound)
        {
            StartRound();
        }

        if (Active)
        {
            TimeLeft -= Time.deltaTime;
        }
    }

    public void StartRound()
    {
        TimeLeft = RoundTimeSeconds;
        roundCoroutine = StartCoroutine(HandleRound());
        Round.Incr(1, StatOperation.Value);
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
    }

    IEnumerator HandleRound()
    {
        Active = true;

        masters.ForEach(spawner => spawner.enabled = true);
        yield return new WaitForSeconds(TimeLeft);
        EndCurrentRound();
    }
}
