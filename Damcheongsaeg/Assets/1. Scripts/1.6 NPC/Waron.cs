using System;
using System.Collections.Generic;
using UnityEngine;

public class Waron : NPC, ITalkable
{
    [SerializeField] private DialogueText DialogueText;
    [SerializeField] private DialogueController dialogueController;

    public override void OnInteractInput()
    {
         Talk(DialogueText);
    }

    public void Talk(DialogueText dialogueText)
    {
        // start conversation
        dialogueController.DisplayNextParagraph(dialogueText);
    }
}
