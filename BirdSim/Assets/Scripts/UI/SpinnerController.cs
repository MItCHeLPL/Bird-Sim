using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpinnerController : MonoBehaviour
{
    private Image image;

    private float fill;

    [SerializeField] private float minFill = 0.65f;
    [SerializeField] private float maxFill = 0.85f;

    [SerializeField] private float fillSpeed = 7.5f;

    [SerializeField] private float rotationSpeed = 150.0f;

    [SerializeField] private bool ChangeSpinnerFill = true;

    void Awake()
    {
        image = GetComponent<Image>();

        fill = image.fillAmount;

        if(ChangeSpinnerFill)
		{
            StartCoroutine(FillChanger());
        }
    }

    void Update()
    {
        transform.Rotate(new Vector3(0, 0, -(rotationSpeed * Time.unscaledDeltaTime)));
    }

    private IEnumerator FillChanger()
	{
        bool achievedMax = false;

        while (true)
		{
            if(!achievedMax)
			{
                fill = Mathf.SmoothStep(fill, maxFill, fillSpeed * Time.unscaledDeltaTime);

                if (maxFill - fill < 0.01f)
				{
                    achievedMax = true;
                }
            }
            else if(achievedMax)
            {
                fill = Mathf.SmoothStep(fill, minFill, fillSpeed * Time.unscaledDeltaTime);

                if (fill - minFill < 0.01f)
                {
                    achievedMax = false;
                }
            }
            else
			{
                achievedMax = false;
                fill = Random.Range(minFill, maxFill);
                Debug.Log("In Random");
            }

            image.fillAmount = fill;

            yield return null;
		}
	}
}
