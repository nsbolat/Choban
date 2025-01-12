using UnityEngine;

public class FootstepController : MonoBehaviour
{
    public Transform groundCheck; // Zemini kontrol eden obje
    public float checkDistance = 0.1f; // Zemine olan maksimum mesafe
    public LayerMask grassLayer; // Grass layer
    public LayerMask sandLayer; // Sand layer
    public AudioSource audioSource; // Ses kaynağı
    public Vector2 pitchRange = new Vector2(0.9f, 1.1f); // Pitch aralığı

    public AudioClip[] grassSounds; // Grass sesleri
    public AudioClip[] sandSounds; // Sand sesleri
    public AudioClip jumpSound; // Zıplama sesi

    private bool isGrounded;

    void Update()
    {
        // Zıplama sesini çalmak için zıplama durumu kontrolü
        if (Input.GetButtonDown("Jump") && isGrounded) // "Jump" tuşuna basıldığında ve yerden kalkmadığında
        {
            PlayJumpSound();
        }
    }

    public void PlayFootstepSound()
    {
        if (audioSource == null || groundCheck == null)
        {
            Debug.LogWarning("AudioSource veya GroundCheck atanmadı!");
            return;
        }

        // Raycast ile zemini kontrol et
        if (Physics.Raycast(groundCheck.position, Vector3.down, out RaycastHit hit, checkDistance))
        {
            if (((1 << hit.collider.gameObject.layer) & grassLayer) != 0)
            {
                // Grass layer: Grass sesleri çal
                PlayRandomSound(grassSounds);
            }
            else if (((1 << hit.collider.gameObject.layer) & sandLayer) != 0)
            {
                // Sand layer: Sand sesleri çal
                PlayRandomSound(sandSounds);
            }
        }
    }

    private void PlayRandomSound(AudioClip[] clips)
    {
        if (clips.Length > 0)
        {
            // Rastgele bir ses klibi seç
            AudioClip clip = clips[Random.Range(0, clips.Length)];

            // Rastgele bir pitch değeri ayarla
            audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);

            // Seçilen sesi çal
            audioSource.PlayOneShot(clip);
        }
    }

    private void PlayJumpSound()
    {
        if (jumpSound != null)
        {
            // Zıplama sesini çal
            audioSource.PlayOneShot(jumpSound);
        }
    }

}
