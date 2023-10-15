using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopInText : MonoBehaviour
{
    public Image targetImage; // Assign the UI Image component here
    public Vector3 popScale = new Vector3(1.2f, 1.2f, 1.2f); // The scale to pop to
    public Vector3 endPopScale = new Vector3(1.5f, 1.5f, 1.5f); // The scale to pop to at the end
    public float popDuration = 0.5f; // Duration of the pop animation
    public float popEndDuration = 0.2f; // Duration of the end pop animation
    public AnimationCurve popCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // Animation curve for easing
    public float fadeDuration = 0.5f; // Duration of the fade animation

    private Vector3 originalScale;
    private Color originalColor;

    private void Start()
    {
        if (targetImage == null)
        {
            Debug.LogError("Please assign the target UI Image component!");
            return;
        }

        originalScale = targetImage.rectTransform.localScale;
        originalColor = targetImage.color;

        targetImage.rectTransform.localScale = Vector3.zero; // Start with a zero scale
        Color newColor = originalColor;
        newColor.a = 0; // Start with zero alpha (fully transparent)
        targetImage.color = newColor;

        // Start the pop, end pop, and fade animations
        StartCoroutine(PopAndFadeCoroutine());
    }

    private IEnumerator PopAndFadeCoroutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < popDuration)
        {
            float t = elapsedTime / popDuration;
            float scaleProgress = popCurve.Evaluate(t);
            Color newColor = targetImage.color;

            // Scale the UI Image from zero to the desired popScale
            targetImage.rectTransform.localScale = Vector3.Lerp(Vector3.zero, popScale, scaleProgress);

            // Fade the UI Image in during the pop animation
            newColor.a = Mathf.Lerp(0, originalColor.a, scaleProgress);
            targetImage.color = newColor;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the UI Image is at the final scale and opacity
        targetImage.rectTransform.localScale = popScale;
        targetImage.color = originalColor;

        // Wait for a moment before starting the end pop animation
        yield return new WaitForSeconds(1.0f);

        // Start the end pop animation
        float endPopElapsedTime = 0f;

        while (endPopElapsedTime < popEndDuration)
        {
            float t = endPopElapsedTime / popEndDuration;

            // Scale the UI Image from popScale to endPopScale
            targetImage.rectTransform.localScale = Vector3.Lerp(popScale, endPopScale, t);

            endPopElapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the UI Image is at the final endPopScale
        targetImage.rectTransform.localScale = endPopScale;

        // Wait for another moment before starting the fade out
        yield return new WaitForSeconds(0.5f);

        // Start the fade out animation
        float fadeElapsedTime = 0f;
        Color finalColor = originalColor;
        finalColor.a = 0; // Fully transparent

        while (fadeElapsedTime < fadeDuration)
        {
            float t = fadeElapsedTime / fadeDuration;
            Color newColor = Color.Lerp(originalColor, finalColor, t);

            // Fade out the UI Image
            targetImage.color = newColor;

            fadeElapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the UI Image is fully faded out
        targetImage.color = finalColor;
    }
}
