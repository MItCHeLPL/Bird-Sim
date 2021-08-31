using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    private Camera cam;
	private CinemachineBrain brain;
	private CinemachineFreeLook activeCam;

	[HideInInspector] public float baseFOV;

    [SerializeField] private float timeToChangeFOV = 1.0f;

	//Coroutine
    private Coroutine fovChanger;
    private bool fovChangerIsRunning = false;

    private void Start()
    {
        cam = GetComponent<Camera>();
		brain = GetComponent<CinemachineBrain>();

        baseFOV = brain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineFreeLook>().m_Lens.FieldOfView; //save baseline fov
    }

    public void ChangeFOV(float newFOVValue)
	{
		activeCam = brain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineFreeLook>(); //save reference to active camera

		if (fovChanger != null && fovChangerIsRunning)
		{
            StopCoroutine(fovChanger); //stop changer if is running

            fovChangerIsRunning = false;
        }

        fovChanger = StartCoroutine(ChangeFOVCoroutine(newFOVValue)); //start coroutine to change fov to new value
	}

    private IEnumerator ChangeFOVCoroutine(float newFOVValue)
	{
		fovChangerIsRunning = true;

		float t = 0f;
		float startFOV = activeCam.m_Lens.FieldOfView;

		//Debug.Log("Started ChangeFOVCoroutine, fov: " + activeCam.m_Lens.FieldOfView);

		while (t < 1)
		{
			t += Time.deltaTime / timeToChangeFOV;
			activeCam.m_Lens.FieldOfView = Mathf.Lerp(startFOV, newFOVValue, t);

			//Debug.Log("fov: " + activeCam.m_Lens.FieldOfView);

			yield return null;
		}

		activeCam.m_Lens.FieldOfView = newFOVValue;
		fovChangerIsRunning = false;

		//Debug.Log("Finished ChangeFOVCoroutine, fov: " + activeCam.m_Lens.FieldOfView);
	}
}
