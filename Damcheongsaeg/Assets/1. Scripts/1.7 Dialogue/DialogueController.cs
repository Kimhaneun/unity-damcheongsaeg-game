using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI NPCNameText;
    [SerializeField] private TextMeshProUGUI NPCDialogueText;

    private Queue<string> paragraphs = new Queue<string>(); // FIFO 배열

    #region STATE PARAMETERS
    // float
    [Range(0f, 1f)][SerializeField] private float typeSpeed; 

    // string
    private string p;

    // bool
    private bool conversationEnded;
    private bool isTyping;
    #endregion

    #region STATE CONSTANT
    private const string HTML_ALPHA = "<color=#00000000>"; // 투명하게
    private const float MAX_TYPE_TIME = 0f;
    #endregion

    public void DisplayNextParagraph(DialogueText dialogueText)
    {
        // if there is nothing in the queue
        if (paragraphs.Count == 0)
        {
            if (!conversationEnded)
            {
                // start a conversation
                // 대화가 안 끝났으면 대화 시작
                StartConversation(dialogueText);
            }
            else if (conversationEnded && !isTyping)
            {
                // end the conversation
                EndConversation();
                return;
            }
        }
        // if there is somthing in the queue
        if (!isTyping)
        {
            p = paragraphs.Dequeue();

            StartCoroutine(nameof(TypeDialogueText), p);
        }
        
        //conversation IS being typed out
        else
        {
            FinishParagrahEarly();
        }

        // update conversation text
        // NPCDialogueText.text = p;

        // update conversationEnded bool
        if (paragraphs.Count == 0)
        {
            conversationEnded = true;
        }
    }

    private void StartConversation(DialogueText dialogueText)
    {
        // activate gameObject
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        // update the speaker name
        NPCNameText.text = dialogueText.speakerName;

        // add dialogue text to the queue
        for (int i = 0; i < dialogueText.paragraphs.Length; i++)
        {
            paragraphs.Enqueue(dialogueText.paragraphs[i]);
        }
    }

    private void EndConversation()
    {
        // clear the queue
        paragraphs.Clear();

        // return bool to false
        conversationEnded = false;

        // deactiveate gameObject
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }

    private IEnumerator TypeDialogueText(string p)
    {
        isTyping = true;

        NPCNameText.text = "";

        string originalText = p;
        string displayedText = "";
        int alphaIndex = 0;

        foreach (char item in p)
        {
            alphaIndex++;
            NPCDialogueText.text = originalText;

            displayedText = NPCDialogueText.text.Insert(alphaIndex, HTML_ALPHA);
            NPCDialogueText.text = displayedText;

            yield return new WaitForSeconds(MAX_TYPE_TIME / typeSpeed);
        }
        isTyping = false;
    }

    private void FinishParagrahEarly()
    {
        // stop the coroutine
        StopCoroutine(nameof(TypeDialogueText));

        // finish displaying text
        NPCDialogueText.text = p;

        // update isTyping  bool
        isTyping = false;
    }
    // 범위 벗어나면 로그 안 되고 이벤트 트리거 되기 
}

