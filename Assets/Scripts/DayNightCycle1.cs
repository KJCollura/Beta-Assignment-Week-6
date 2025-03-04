using UnityEngine;

public class DayNightCycle1 : MonoBehaviour
{
    [Header("Time Settings")]
    public float dayDuration = 120f; // Full day in seconds
    private float timeOfDay = 0f; // 0 = Midnight, 0.5 = Noon, 1 = Midnight

    [Header("Sun Settings")]
    public Light sun; // Assign your Directional Light
    public Gradient lightColor; // Set in Inspector (Morning, Noon, Evening, Night)

    [Header("Skybox Settings (Blending)")]
    public Material skyboxMaterial; // A shared procedural skybox
    public Color dayTint;
    public Color sunsetTint;
    public Color nightTint;
    public Color sunriseTint;

    [Header("Lighting Settings")]
    public Color nightAmbientColor = Color.black;
    public Color dayAmbientColor = Color.white;

    [Header("Moon & Stars (Optional)")]
    public Transform moon; // Assign the moon object (optional)
    public ParticleSystem stars; // Assign the star particle system (optional)

    private ParticleSystem.MainModule starsMain;
    private float transitionSpeed = 0.5f; // Smooth transition speed

   void Start()
{
    if (skyboxMaterial != null)
    {
        RenderSettings.skybox = skyboxMaterial; // Ensure skybox is applied
    }

    if (stars != null)
    {
        Debug.Log("✅ Forcing Stars to Stay Active in Scene & Game Mode.");
        stars.gameObject.SetActive(true); // ✅ Ensure GameObject is always active
        stars.Play(); // ✅ Forces stars to be visible
        stars.Clear(); // ✅ Clears any lingering particles to refresh system
    }
}


    void Update()
    {
        // **Ensure timeOfDay updates properly**
        timeOfDay += (Time.deltaTime / dayDuration);
        
        if (timeOfDay >= 1f) 
        {
            timeOfDay = 0f; // Ensures smooth looping
        }

        Debug.Log($"⏳ Time of Day: {timeOfDay}");

        // **Determine if it's Nighttime**
        bool isNight = (timeOfDay < 0.2f || timeOfDay > 0.8f);
        Debug.Log($"🌙 Is it Night? {isNight}");

        if (stars != null)
        {
            if (!stars.gameObject.activeSelf)
            {
                Debug.Log("⚡ Re-enabling Stars GameObject.");
                stars.gameObject.SetActive(true); // ✅ Ensure GameObject is active
            }

            if (isNight)
            {
                if (!stars.isPlaying)
                {
                    Debug.Log("🌌 Enabling Stars - Nighttime Detected");
                    stars.Clear(); // ✅ Prevents glitches where particles don't appear
                    stars.Play();
                }
            }
            else
            {
                if (stars.isPlaying)
                {
                    Debug.Log("☀️ Disabling Stars - Daytime Detected");
                    stars.Stop();
                }
            }
        }

        // **Rotate Sun**
        float sunAngle = timeOfDay * 360f - 90f;
        sun.transform.rotation = Quaternion.Euler(sunAngle, 170f, 0);

        // **Change Sun Color using Gradient**
        sun.color = lightColor.Evaluate(timeOfDay);

        // **Adjust Ambient Lighting**
        RenderSettings.ambientLight = Color.Lerp(nightAmbientColor, dayAmbientColor, sun.intensity);

        // **Smooth Skybox Blending**
        if (skyboxMaterial != null)
        {
            Color targetTint = GetTargetSkyboxColor();
            Color currentTint = skyboxMaterial.GetColor("_SkyTint");
            Color blendedTint = Color.Lerp(currentTint, targetTint, transitionSpeed * Time.deltaTime);

            skyboxMaterial.SetColor("_SkyTint", blendedTint);
        }

        // **Optional: Rotate the Moon Opposite of the Sun**
        if (moon != null)
        {
            float moonAngle = sunAngle + 180f; // Opposite direction
            moon.rotation = Quaternion.Euler(moonAngle, 170f, 0);
        }
    }

    Color GetTargetSkyboxColor()
    {
        if (timeOfDay >= 0.75f || timeOfDay < 0.2f) // Night
            return nightTint;
        else if (timeOfDay >= 0.2f && timeOfDay < 0.35f) // Sunrise
            return sunriseTint;
        else if (timeOfDay >= 0.35f && timeOfDay < 0.65f) // Day
            return dayTint;
        else // Sunset
            return sunsetTint;
    }
}
