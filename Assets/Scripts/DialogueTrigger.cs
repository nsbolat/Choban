using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueTrigger : MonoBehaviour
{
    public Message[] messages;
    public UnityEvent OnDialogueTriggered; // Yeni UnityEvent


    public void StartDialogue()
    {
        FindObjectOfType<DialogueManager>().OpenDialogue(messages);
        Debug.Log("Basıldı");
        OnDialogueTriggered?.Invoke(); // UnityEvent tetikleniyor

    }
    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().OpenDialogue(messages);
    }
}


[System.Serializable]
public class Message
{
    public int actorId;
    public string message;
}

[System.Serializable]
public class Actor
{
    public string name;
    public Sprite sprite;
}

