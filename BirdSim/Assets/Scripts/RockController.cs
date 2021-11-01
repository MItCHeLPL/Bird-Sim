using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockController : MonoBehaviour
{
    public Vector3 targetPosition = Vector3.zero;
    public float timeToReachPosition = 5;

    public bool rockInAir = false;
    private Coroutine rockInAirCoroutine = null;

    public void Throw()
	{
        rockInAirCoroutine = StartCoroutine(RockInAir());
	}

	private void OnCollisionEnter(Collision collision)
	{
        if(collision.gameObject.CompareTag("Player"))
		{
            StopCoroutine(rockInAirCoroutine);

            PlayerImpact();
        }    
        else if(collision.gameObject.CompareTag("Water") || collision.gameObject.CompareTag("Terrain"))
		{
            StopCoroutine(rockInAirCoroutine);

            EnvironmentImpact();
        }
    }

    private IEnumerator RockInAir()
    {
        float t = 0;

        rockInAir = true;

        while (t < 1)
        {
            //todo, lerp over bezier curve

            yield return null;
        }
    }

    private void PlayerImpact()
	{
        //todo, End level
    }

    private void EnvironmentImpact()
    {
        //todo, Play collision particles, sound etc.
        rockInAir = false;
    }
}
