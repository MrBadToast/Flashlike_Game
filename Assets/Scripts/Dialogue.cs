using UnityEngine;
using TMPro;
using System.Collections;
using Unity.Loading;

public class Dialogue : StaticSerializedMonoBehaviour<Dialogue>
{
    public TextMeshProUGUI dialogueText; // Inspector에서 할당
    public GameObject dialogueContinue;
    public float typingSpeed = 0.05f;

    private KeyCode continueKey = KeyCode.Space; // 대사 스킵 키 설정
    private bool isTyping = false;
    private bool isShowing = false;

    private bool isKeyDown = false;
    private bool continueDialogueFlag = false;

    private void Start()
    {
        dialogueContinue.SetActive(false);
    }

    private void Update()
    {
        if (isTyping)
        {
            if (Input.GetKeyDown(continueKey) || continueDialogueFlag)
            {
                if (isKeyDown)
                    return;
                isKeyDown = true;
                continueDialogueFlag = false;
            }
        }
    }

    public void ContinueDialogue()
    {
        if (isShowing)
        {
            continueDialogueFlag = true;
        }
    }

    public IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";

        isTyping = true;
        isShowing = true;
        continueDialogueFlag = false;
        dialogueContinue.SetActive(false);

        for (int count = 0; count < sentence.Length; count++)
        {
            if (isKeyDown)
            {
                dialogueText.text = sentence;
                break;
            }

            dialogueText.text = sentence.Substring(0,count+1);
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;

        yield return new WaitForSeconds(0.5f);
        dialogueContinue.SetActive(true);
        isKeyDown = false;


        yield return new WaitUntil(() => Input.GetKeyDown(continueKey) || continueDialogueFlag);
        continueDialogueFlag = false;
        isShowing = false;
    }

}

