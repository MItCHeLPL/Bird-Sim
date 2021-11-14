using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdPathfinder : MonoBehaviour
{
    private Transform targetTransform = null;
    private Vector3 direction;

    [SerializeField] private float speedRealtiveToPlayer = 1.0f;
    private float speed = 0.0f;

    [SerializeField] private float minSpeedMultiplier = 0.1f;

    [SerializeField] private float copyPlayerYInfluence = 0.5f;

    [SerializeField] private PlayerController playerController = null;
    [SerializeField] private LevelManager levelManager = null;

    void Start()
    {
        StartCoroutine(LateStart());
    }

    void Update()
    {
        if(targetTransform != null)
		{
            speed = playerController.speed + speedRealtiveToPlayer;

            //Scale speed depending of player angle to target on x and z axis
            Vector3 playerToTargetDir = targetTransform.position - playerController.transform.position;
            float playerToTargetAngle = Vector3.Angle(
                new Vector3(playerToTargetDir.x, 0, playerToTargetDir.z),
                new Vector3(playerController.transform.forward.x, 0, playerController.transform.forward.z)
                );

            speed *= Mathf.Clamp(ExtendedMathf.Map(playerToTargetAngle, 15.0f, 90.0f, 1.0f, minSpeedMultiplier), minSpeedMultiplier, 1.0f);
        }
    }

    public void GetCheckpoint(int checkpointIndex)
	{
        targetTransform = levelManager.checkpoints[0].GetComponent<Checkpoint>().birdTarget;

        direction = (targetTransform.position - transform.position).normalized;
    }

    private IEnumerator LateStart()
	{
        yield return new WaitUntil(() => levelManager.randomizedCheckpoints == true);

        GetCheckpoint(0);

        StartCoroutine(MoveToTarget());
    }

    private IEnumerator MoveToTarget()
	{
        while (Vector3.Distance(new Vector3(targetTransform.position.x, 0, targetTransform.position.z), new Vector3(transform.position.x, 0, transform.position.z)) > 0.1f)
		{
            //Move bird's target towards target with speed
            transform.position += direction * speed * Time.deltaTime;

            //Copy player Y position to offset birds, scale it with influence
            if(copyPlayerYInfluence != 0.0f)
			{
                float yOffset = Mathf.Abs(transform.position.y - playerController.transform.position.y);

                transform.position = new Vector3(
                    transform.position.x,
                    transform.position.y + ((playerController.transform.position.y > transform.position.y ? yOffset : -yOffset) * copyPlayerYInfluence), 
                    transform.position.z
                    );
            }
            
            yield return null;
		}
	}
}
