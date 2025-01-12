using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    public float typingSpeed = 0.05f; // Harflerin yazılma hızı
    public float messageDuration = 3f; // Mesaj tamamlandıktan sonra bekleme süresi
    public AudioSource typingSound; // Harf yazılırken çalacak ses

    private Message[] currentMessages;
    private int activeMessage = 0;

    public void OpenDialogue(Message[] messages)
    {
        currentMessages = messages;
        activeMessage = 0;
        StartCoroutine(PlayDialogue());
    }

    private IEnumerator PlayDialogue()
    {
        while (activeMessage < currentMessages.Length)
        {
            yield return StartCoroutine(TypeMessage(currentMessages[activeMessage].message)); // Mesajı harf harf yazdır
            yield return new WaitForSeconds(messageDuration); // Mesaj tamamlandıktan sonra bekle
            activeMessage++;
        }

        Debug.Log("Dialogue Finished!");
    }

    private IEnumerator TypeMessage(string message)
    {
        messageText.text = ""; // Mesajı temizle

        foreach (char letter in message.ToCharArray())
        {
            messageText.text += letter; // Harfi ekle
            typingSound?.Play(); // Ses çal
            yield return new WaitForSeconds(typingSpeed); // Harf yazılma hızı kadar bekle
        }
    }
}