using UnityEngine;

public class DynamicInteractiveObject : MonoBehaviour
{
    public Color newColor = Color.red; // Color to change on interaction
    public float scaleFactor = 1.5f;   // Scale multiplier
    public Vector3 moveOffset = new Vector3(2f, 0, 0); // How far it moves
    public AudioClip interactSound;    // Sound effect (optional)
    public ParticleSystem effect;      // Particle effect (optional)

    private Color originalColor;
    private Vector3 originalScale;
    private Vector3 originalPosition;
    private Renderer objRenderer;
    private AudioSource audioSource;

    void Start()
    {
        objRenderer = GetComponent<Renderer>();
        audioSource = GetComponent<AudioSource>();

        if (objRenderer != null)
            originalColor = objRenderer.material.color;

        originalScale = transform.localScale;
        originalPosition = transform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ReactToPlayer();
        }
    }

    void ReactToPlayer()
    {
        // Change color
        if (objRenderer != null)
            objRenderer.material.color = newColor;

        // Scale up
        transform.localScale = originalScale * scaleFactor;

        // Move object
        transform.position += moveOffset;

        // Play sound
        if (interactSound != null && audioSource != null)
            audioSource.PlayOneShot(interactSound);

        // Trigger particle effect
        if (effect != null)
            effect.Play();

        // Reset after a few seconds
        Invoke(nameof(ResetObject), 3f);
    }

    void ResetObject()
    {
        // Reset color, scale, and position
        if (objRenderer != null)
            objRenderer.material.color = originalColor;

        transform.localScale = originalScale;
        transform.position = originalPosition;
    }
}
