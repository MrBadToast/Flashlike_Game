using Sirenix.OdinInspector;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class DodgePatternManager : StaticSerializedMonoBehaviour<DodgePatternManager>
{
    public TimelineAsset firstPattern;
    public TimelineAsset[] patterns1;
    public TimelineAsset[] patterns2;
    public TimelineAsset[] patterns3;
    public TimelineAsset AttackSuccessTimeline;
    public TimelineAsset GDownTimeline;
    public GameObject[] PatternObjects;
    public GameObject deathCamera;
    public AudioSource musicObject;
    private PlayableDirector playable;

    [Title("Optional")]
    public bool isBonusStage = false;
    public GameObject bonusGameEndUI;

    static public GameObject buttonSpawned;

    int lastPatternIndex = -1;


    private int bonusStageCurrentScore = 0;
    public int BonusStageCurrentScore
    {
        set
        {
            bonusStageCurrentScore = value;
            if (bonusScoreText != null)
                bonusScoreText.text = bonusStageCurrentScore.ToString();
        }
        get
        {
            return bonusStageCurrentScore;
        }
    }
    public int bonusAttackScore = 50;

    public TextMeshProUGUI bonusScoreText;

    [SerializeField,ReadOnly] int attackSuccessCount = 0;

    Coroutine patternCoroutine;

    public UnityEvent onPatternForceEnded;

    protected override void Awake()
    {
        base.Awake();
        playable = GetComponent<PlayableDirector>();
    }

    public void ResetPattern()
    {
        if (patternCoroutine != null)
        {
            playable.time = playable.duration;

            foreach (var obj in PatternObjects)
            {
                obj.SetActive(false);
            }
            onPatternForceEnded?.Invoke();
            playable.Stop();
            lastPatternIndex = 0;

            DestroyButton();

            StopCoroutine(patternCoroutine);
            patternCoroutine = null;
        }

        patternCoroutine = StartCoroutine(Cor_Pattern());
    }

    public void PatternEnd()
    {

    }

    public void AttackSuccesful()
    {
        attackSuccessCount++;

        playable.time = playable.duration;
        foreach (var obj in PatternObjects)
        {
            obj.SetActive(false);
        }
        onPatternForceEnded?.Invoke();
        playable.Stop();
        lastPatternIndex = 0;
        BonusStageCurrentScore += bonusAttackScore;

        patternCoroutine = null;
        StopAllCoroutines();

        patternCoroutine = StartCoroutine(Cor_Pattern());
    }

    private void Start()
    {
        lastPatternIndex = 0;
        patternCoroutine = StartCoroutine(Cor_Pattern());
    }

    float secondTimer = 0f;
    private void Update()
    {
        if (Player_Dodger.Instance.CurrentState is Player_Dodger.DodgeMoveState)
        {
            if (secondTimer >= 1f)
            {
                secondTimer = 0f;
                if (isBonusStage)
                {
                    BonusStageCurrentScore += 1;
                }
            }
            else
            {
                secondTimer += Time.deltaTime;
            }
        }
    }

    public void DestroyButton()
    {
        if (buttonSpawned)
        {
            Destroy(buttonSpawned);
            buttonSpawned = null;
        }
    }

    public void OnPlayerDeadInBonusStage()
    {
        if (!isBonusStage) return;

        playable.time = playable.duration;

        foreach (var obj in PatternObjects)
        {
            obj.SetActive(false);
        }
        onPatternForceEnded?.Invoke();
        playable.Stop();
        lastPatternIndex = 0;

        DestroyButton();

        StopCoroutine(patternCoroutine);
        patternCoroutine = null;


        musicObject.Stop();
        bonusGameEndUI.SetActive(true);
    }

    private IEnumerator Cor_Pattern()
    {
        if (attackSuccessCount == 0)
        {
            playable.Play(firstPattern);
            yield return new WaitUntil(() => playable.state != PlayState.Playing);
        }
        else if (attackSuccessCount < 3)
        {
            playable.Play(AttackSuccessTimeline);
            yield return new WaitUntil(() => playable.state != PlayState.Playing);

        }
        else if (attackSuccessCount == 3 && !isBonusStage)
        {
            playable.Play(GDownTimeline);

            deathCamera.SetActive(true);
            deathCamera.transform.position = new Vector3(0, 0, -10);

            yield return new WaitUntil(() => playable.state != PlayState.Playing);

            yield return new WaitForSeconds(0.7f);

            SceneLoader.Instance.LoadNewScene("Scene6");

            patternCoroutine = null;
            StopAllCoroutines();

            yield break;
        }
        else if (attackSuccessCount >= 3 && isBonusStage)
        {
            playable.Play(AttackSuccessTimeline);
            yield return new WaitUntil(() => playable.state != PlayState.Playing);
        }

        while (true)
        {
            if (attackSuccessCount == 0)
            {
                lastPatternIndex++;
                if (lastPatternIndex >= patterns1.Length)
                    lastPatternIndex = 0;

                playable.Play(patterns1[lastPatternIndex]);
                yield return new WaitUntil(() => playable.state != PlayState.Playing);
            }
            else if (attackSuccessCount == 1)
            {
                lastPatternIndex++;
                if (lastPatternIndex >= patterns2.Length)
                    lastPatternIndex = 0;
                playable.Play(patterns2[lastPatternIndex]);
                yield return new WaitUntil(() => playable.state != PlayState.Playing);
            }
            else // attackSuccessCount >= 2
            {
                lastPatternIndex++;
                if (lastPatternIndex >= patterns3.Length)
                    lastPatternIndex = 0;
                playable.Play(patterns3[lastPatternIndex]);
                yield return new WaitUntil(() => playable.state != PlayState.Playing);
            }
        }
    }

}
