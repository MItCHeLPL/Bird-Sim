using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class RockController : MonoBehaviour
{
    [SerializeField] private float yOffsetOnCollision = 10.0f;

    [SerializeField] private VisualEffect impactSparksParticles = null;
    [HideInInspector] public VisualEffect predictionPathParticles = null;

    [HideInInspector] public Vector3 targetPosition = Vector3.zero;

    [HideInInspector] public float timeToReachPosition = 5;

    [HideInInspector] public List<Transform> curve = new(4);

    [HideInInspector] public bool rockInAir = false;

    private Coroutine rockInAirCoroutine = null;

    [HideInInspector] public LayerMask collisionLayers;

    private AudioSource audioSource;
    [HideInInspector] public float audioVolume;

    [HideInInspector] public LevelManager levelManager = null;

    [HideInInspector] public bool showDebug = false;

    private void Start()
	{
        audioSource = GetComponent<AudioSource>();
    }

	public void Throw()
	{
        rockInAirCoroutine = StartCoroutine(RockInAir());
	}

	private void OnCollisionEnter(Collision collision)
	{
        if (showDebug)
        {
            Debug.Log("Rock collided with object: " + collision.gameObject.name);
        }

        if (collision.gameObject.CompareTag("Player") && rockInAir)
		{
            StopCoroutine(rockInAirCoroutine);

            PlayerImpact();
        }    
        else if ((collisionLayers.value & (1 << collision.transform.gameObject.layer)) > 0)
		{
            StopCoroutine(rockInAirCoroutine);

            EnvironmentImpact();
        }
    }

    private IEnumerator RockInAir()
    {
        float t = 0;

        rockInAir = true;

        //Convert transforms to vector3's
        List<Vector3> point = new(4);
        foreach(Transform transform in curve)
		{
            point.Add(transform.position);
		}

        while (t < 1)
        {
            t += Time.deltaTime / timeToReachPosition;

            //stick to bezier curve
            transform.position = Mathf.Pow(1 - t, 3) * point[0] +
                3 * Mathf.Pow(1 - t, 2) * t * point[1] +
                3 * (1 - t) * Mathf.Pow(t, 2) * point[2] +
                Mathf.Pow(t, 3) * point[3];

            yield return null;
        }
    }

    private void PlayerImpact()
	{
        levelManager.Die();

        if (showDebug)
		{
            Debug.Log("Rock collided with Player");
		}
    }

    private void EnvironmentImpact()
    {
        rockInAir = false;

        GetComponent<Rigidbody>().isKinematic = true;

        transform.position = new Vector3(transform.position.x, transform.position.y - yOffsetOnCollision, transform.position.z); //Sink rock into ground

        predictionPathParticles.SendEvent("StopEmit");

        impactSparksParticles.SendEvent("Emit");

        audioSource.volume *= audioVolume;
        audioSource.Play(0);
        
        if (showDebug)
        {
            Debug.Log("Rock collided with environment");
        }
    }
}
