using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/New Dialogue Contaniner")]
public class DialogueText : ScriptableObject
{
    public string speakerName;

    public string[] paragraphs; // text를 담을 배열
}
