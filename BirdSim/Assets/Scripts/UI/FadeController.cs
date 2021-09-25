using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeController : MonoBehaviour
{
    private Image image;

    [SerializeField] private float startAlpha = 0f;
    [SerializeField] private float timeToFadeOnStart = 2f;

    [SerializeField] private bool FadeInOnStart = false;
    [SerializeField] private bool FadeOutOnStart = false;

    public bool isFading = false;

    private Color wantedColor;

    private void Start()
    {
        image = GetComponent<Image>();

        wantedColor = new Color(image.color.r, image.color.g, image.color.b, startAlpha);

        if(FadeInOnStart || FadeOutOnStart)
		{
            image.color = wantedColor;

            if (FadeInOnStart)
            {
                FadeIn(timeToFadeOnStart);
            }

            if (FadeOutOnStart)
            {
                FadeOut(timeToFadeOnStart);
            }
        }
    }

    private void Update()
	{
        if(isFading)
		{
            image.color = wantedColor;
        }
    }

    public void FadeIn(float time)
	{
        if (!isFading)
        {
            StartCoroutine(Fade(image.color.a, 1, time));
        }
    }

    public void FadeOut(float time)
    {
        if(!isFading)
		{
            StartCoroutine(Fade(image.color.a, 0, time));
        }
    }

    private IEnumerator Fade(float alphaFrom, float alphaTo, float time)
	{
        isFading = true;

        float t = 0f;
        float alpha = alphaFrom;

        while (t < 1)
        {
            t += Time.unscaledDeltaTime / time;
            
            alpha = Mathf.Lerp(alphaFrom, alphaTo, t);

            wantedColor = new Color(image.color.r, image.color.g, image.color.b, alpha);

            yield return null;
        }

        isFading = false;
    }
}
