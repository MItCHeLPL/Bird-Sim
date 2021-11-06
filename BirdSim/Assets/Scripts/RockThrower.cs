using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class RockThrower : MonoBehaviour
{
    private int throwCount = 1;

    [Header("Settings")]
    [SerializeField] private float timeForRockToReachTarget = 3.0f;
    [SerializeField] private float coolDownBetweenThrows = 15.0f;
    [SerializeField] private int throwInFrontOfPlayerEveryXThrows = 1;

    [SerializeField] private float targetY = 0;
    [SerializeField] private float curveOffset = -90.0f;

    [SerializeField] private float distanceFromPlayerOffset = 40.0f;

    [SerializeField] private float pathTimeBeforeThrow = 3.0f;

    [Header("Bounds")]
    [SerializeField] private Vector2 boundsCenter = Vector3.zero;
    [SerializeField] private Vector2 boundsSize = Vector3.zero;
    [SerializeField] private Vector2 boundsInnerMarginCenter = Vector3.zero;
    [SerializeField] private Vector2 boundsInnerMarginSize = Vector3.zero;

    private Vector2 targetImpactPosition = Vector3.zero;

    [Header("Game Objects")]
    [SerializeField] private GameObject player = null;
    private Rigidbody playerRB = null;
    private Vector3 playerVelocity = Vector3.zero;
    private Vector3 playerPosition = Vector3.zero;
    private bool playerInBounds;

    private GameObject rocksContainer = null;

    [SerializeField] private AudioController audioController = null;

    [SerializeField] private GameObject predictionPath = null;
    private VisualEffect predictionPathParticles = null;

    [SerializeField] private GameObject rockPrefab = null;

    [SerializeField] private List<Transform> curve = new(4);

    [SerializeField] private VisualEffect volcanoSparks = null;

    [SerializeField] private string explosionAudioName = "";

    [Header("Layers")]
    [SerializeField] private LayerMask collisionLayers;

    [Header("Debug")]
    [SerializeField] private bool showDebug = true;

    void Start()
    {
        playerRB = player.GetComponent<Rigidbody>();

        predictionPathParticles = predictionPath.GetComponent<VisualEffect>();

        //Create Rocks Container as a child
        rocksContainer = new GameObject("Rocks Container");
        rocksContainer.transform.parent = transform;

        //Start throwing rocks
        StartCoroutine(ThrowerCoroutine());
    }

    void OnDrawGizmosSelected()
    {
        //Bounds debug display
        if (showDebug)
		{
            //Cube Y size
            float sizeY = 300;

            //Bounds
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(new Vector3(boundsCenter.x, targetY + (sizeY / 2), boundsCenter.y), new Vector3(boundsSize.x, sizeY, boundsSize.y));

            //Inner margin
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(new Vector3(boundsInnerMarginCenter.x, targetY + (sizeY / 2), boundsInnerMarginCenter.y), new Vector3(boundsInnerMarginSize.x, sizeY, boundsInnerMarginSize.y));
        }
    }

    void Update()
    {
        //Get Player values
        playerVelocity = playerRB.velocity;
        playerPosition = player.transform.position;

        if (playerPosition.x > boundsCenter.x - (boundsSize.x / 2) && 
            playerPosition.x < boundsCenter.x + (boundsSize.x / 2) && 
            playerPosition.z > boundsCenter.y - (boundsSize.y / 2) && 
            playerPosition.z < boundsCenter.y + (boundsSize.y / 2)
            )
		{
            playerInBounds = true;
        }
        else
		{
            playerInBounds = false;
        }
    }

    private IEnumerator ThrowerCoroutine()
    {
        while (true)
        {
            float timeToReachPosition = 0;

            //Throw rock in front of the player
            if (throwCount % throwInFrontOfPlayerEveryXThrows == 0 && playerInBounds)
            {
                //player position + player velocity + distance offset * player angle to align impact timing
                targetImpactPosition = new Vector2(
                    playerPosition.x + playerVelocity.x + ((playerVelocity.x > 0 ? distanceFromPlayerOffset : -distanceFromPlayerOffset) * (1 - Mathf.Abs(player.transform.forward.y))), 
                    playerPosition.z + playerVelocity.z + ((playerVelocity.z > 0 ? distanceFromPlayerOffset : -distanceFromPlayerOffset) * (1 - Mathf.Abs(player.transform.forward.y)))
                    );

                timeToReachPosition = timeForRockToReachTarget;

                if (showDebug)
                {
                    Debug.Log("Throwing rock in front of a player, local position: " + transform.InverseTransformPoint(new Vector3(targetImpactPosition.x, targetY, targetImpactPosition.y)));
                }
            }
            else
            {
                targetImpactPosition = new Vector2(
                    Random.Range(-boundsSize.x / 2,  boundsSize.x / 2) + boundsCenter.x,
                    Random.Range(-boundsSize.y / 2, boundsSize.y / 2) + boundsCenter.y
                    );

                //Ensure that rock doesn't fall near spawn positon
                if(Mathf.Abs(targetImpactPosition.x) < (boundsInnerMarginSize.x/2) + boundsInnerMarginCenter.x)
				{
                    targetImpactPosition.x += targetImpactPosition.x > 0 ? boundsInnerMarginSize.x : -boundsInnerMarginSize.x;
                }
                if (Mathf.Abs(targetImpactPosition.y) < (boundsInnerMarginSize.y/2) + boundsInnerMarginCenter.y)
				{
                    targetImpactPosition.y += targetImpactPosition.y > 0 ? boundsInnerMarginSize.y : -boundsInnerMarginSize.y;
                }

                timeToReachPosition = timeForRockToReachTarget;

                if(showDebug)
                {
                    Debug.Log("Throwing rock at a random place, local position: " + transform.InverseTransformPoint(new Vector3(targetImpactPosition.x, targetY, targetImpactPosition.y)));
                }
            }

            SetUpPredictionPath(targetImpactPosition);

            predictionPathParticles.SendEvent("StartEmit");

            yield return new WaitForSeconds(pathTimeBeforeThrow);

            ThrowRock(targetImpactPosition, timeToReachPosition);

            if(showDebug)
			{
                Debug.Log("Thrown rock at world position: " + targetImpactPosition + ", thrown " + throwCount + " rocks");
            }

            throwCount++;

            yield return new WaitForSeconds(coolDownBetweenThrows);
        }
    }

    private void SetUpPredictionPath(Vector2 targetPos)
    {
        Vector3 targetPosition = new Vector3(targetPos.x, targetY, targetPos.y);

        Transform parent = curve[0].transform.parent; //Rotate parent that holds all curve points


        //Rotate curve around volcano to face target position
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0; //rotate only on Y axis

        parent.transform.rotation = Quaternion.LookRotation(direction);
        parent.transform.Rotate(new Vector3(0, curveOffset, 0)); //offset to make rock fly naturally


        curve[curve.Count - 1].transform.position = targetPosition; //set end point to target posiiton
    }

    private void ThrowRock(Vector2 targetPos, float timeToReachPosition)
	{
        Vector3 targetPosition = new Vector3(targetPos.x, targetY, targetPos.y);

        RockController rock = null;

        //Instantiate rock and assign values
        rock = Instantiate(rockPrefab, transform.position, Quaternion.identity, rocksContainer.transform).GetComponent<RockController>();

        rock.curve = curve;
        rock.targetPosition = targetPosition;
        rock.timeToReachPosition = timeToReachPosition;
        rock.predictionPathParticles = predictionPathParticles;
        rock.collisionLayers = collisionLayers;
        rock.showDebug = showDebug;
        rock.audioVolume = audioController.globalVolume;

        volcanoSparks.SendEvent("Emit");

        audioController.Play(explosionAudioName);

        rock.Throw();
    }
}
