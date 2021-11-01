using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockThrower : MonoBehaviour
{
    private int throwCount = 1;

    [SerializeField] private float rockSpeed = 25.0f;
    [SerializeField] private float coolDownBetweenThrows = 5.0f;
    [SerializeField] private int throwInFrontOfPlayerEveryXThrows = 3;

    [SerializeField] private Vector2 boundsCenter = Vector3.zero;
    [SerializeField] private Vector2 boundsSize = Vector3.zero;
    [SerializeField] private Vector2 boundsInnerMargin = Vector3.zero;

    private Vector2 targetImpactPosition = Vector3.zero;

    [SerializeField] private float targetY = 0;

    [SerializeField] private GameObject player = null;
    private Rigidbody playerRB = null;
    private Vector3 playerVelocity = Vector3.zero;
    private Vector3 playerPosition = Vector3.zero;
    private bool playerInBounds;

    private GameObject rocksContainer = null;
    [SerializeField] private GameObject predictionPath = null;
    [SerializeField] private GameObject rockPrefab = null;

    [SerializeField] private bool showDebug = true;

    void Start()
    {
        playerRB = player.GetComponent<Rigidbody>();

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
            Gizmos.DrawWireCube(new Vector3(boundsCenter.x, targetY + (sizeY / 2), boundsCenter.y), new Vector3(boundsInnerMargin.x, sizeY, boundsInnerMargin.y));
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
                targetImpactPosition = new Vector2(playerPosition.x + playerVelocity.x, playerPosition.z + playerVelocity.z);

                timeToReachPosition = new Vector3(playerVelocity.x, playerPosition.y - targetY, playerVelocity.z).magnitude; //temp, Improve timing when bezier curve is implemented

                if(showDebug)
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
                if(Mathf.Abs(targetImpactPosition.x) < boundsInnerMargin.x)
				{
                    targetImpactPosition.x += targetImpactPosition.x > 0 ? boundsInnerMargin.x : -boundsInnerMargin.x;
                }
                if (Mathf.Abs(targetImpactPosition.y) < boundsInnerMargin.y)
				{
                    targetImpactPosition.y += targetImpactPosition.y > 0 ? boundsInnerMargin.y : -boundsInnerMargin.y;
                }

                timeToReachPosition = rockSpeed;

                if(showDebug)
                {
                    Debug.Log("Throwing rock at a random place, local position: " + transform.InverseTransformPoint(new Vector3(targetImpactPosition.x, targetY, targetImpactPosition.y)));
                }
            }

            SetUpPredictionPath(targetImpactPosition);

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

        Transform point = null;

        for(int i=1; i < predictionPath.transform.childCount; i++)
		{
            point = predictionPath.transform.GetChild(i);

            if(i == predictionPath.transform.childCount-1)
			{
                point.transform.position = targetPosition;
            }
            else
			{
                //temp, add bezier curve
			}
        }
    }

    private void ThrowRock(Vector2 targetPos, float timeToReachPosition)
	{
        Vector3 targetPosition = new Vector3(targetPos.x, targetY, targetPos.y);

        RockController rock = null;

        //rock = Instantiate(rockPrefab, transform.position, Quaternion.identity, rocksContainer.transform); //temp, remove comment after bezier curve is working
        rock = Instantiate(rockPrefab, targetPosition, Quaternion.identity, rocksContainer.transform).GetComponent<RockController>(); //temp, instantiating at final position

        rock.targetPosition = targetPosition;
        rock.timeToReachPosition = timeToReachPosition;
        //rock.curve = curve //todo, add bezier curve solution

        rock.Throw();

        //todo, add custom script to rock prefab, start coroutine on start, lerp over bezier curve, stop coroutine on colsiion with terrain, water or player, end level on collision with player, play sound particles, disable collision, etc. on collision with terrain or water. Try to instantiate with arguments of bezier curve and rockSpeed
    }
}
