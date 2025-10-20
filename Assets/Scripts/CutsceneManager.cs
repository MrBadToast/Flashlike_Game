using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CutsceneManager : StaticSerializedMonoBehaviour<CutsceneManager>
{
    public class CutsceneElement
    {
        [TextArea] public string DialogueContent;
        [PreviewField(Alignment = ObjectFieldAlignment.Center,Height = 200f)] public Sprite cutsceneImage;
        public AudioClip soundEffect;

        public string CutsceneAnimation;
        public UnityEvent OnCutsceneStart;
        public UnityEvent OnCutsceneEnd;
    }

    public bool startFirstElement = false;

    public Animator cutsceneAnimator;
    public AudioSource audioSource;
    public Image cutsceneImageUI;
    public CanvasGroup dialogueAlpha;
    public List<CutsceneElement[]> CutsceneElements;
    public AudioClip soundWhemProgress;

    [SerializeField,ReadOnly]private int activeCutsceneIndex = -1;
    public int ActiveCutsceneIndex { get { return activeCutsceneIndex; } }


    private Coroutine currentRunningCutscene;

    public void Start()
    {
        if(startFirstElement)
        {
            StartCutsceneElements(0);
        }
    }

    public void StartCutsceneElements(int id)
    {
        if (currentRunningCutscene != null)
            return;

        activeCutsceneIndex = id;
        currentRunningCutscene = StartCoroutine(Cor_CutsceneSequence(CutsceneElements[id]));
    }

    public void LaodNewScene(string sceneName)
    {
        SceneLoader.Instance.LoadNewScene(sceneName);
    }

    IEnumerator Cor_CutsceneSequence(CutsceneElement[] cutsceneElements)
    {
        foreach (var item in cutsceneElements)
        {
            if(item.OnCutsceneStart != null)
                item.OnCutsceneStart.Invoke();
            if (item.soundEffect != null)
                audioSource.PlayOneShot(item.soundEffect);
            if (item.CutsceneAnimation != null && item.CutsceneAnimation != string.Empty)
                cutsceneAnimator.Play(item.CutsceneAnimation);
            if (item.cutsceneImage != null)
                cutsceneImageUI.sprite = item.cutsceneImage;

            yield return Dialogue.Instance.TypeSentence(item.DialogueContent);
            
            audioSource.PlayOneShot(soundWhemProgress);

            if (item.OnCutsceneEnd != null)
                item.OnCutsceneEnd.Invoke();
        }

        dialogueAlpha.alpha = 0f;
    }

    public void EnableBonusMode()
    {
        PlayerPrefs.SetInt("BonusModeUnlocked", 1);
        PlayerPrefs.Save();
    }

}
