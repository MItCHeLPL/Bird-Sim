using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    private Camera cam;
	private CinemachineBrain brain;
	private CinemachineFreeLook activeCam;

	[HideInInspector] public float baseFOV; //default fov

    [SerializeField] private float timeToChangeFOV = 1.0f; //How long does it tak to change fov

	//Coroutine
    private Coroutine fovChanger;
    private bool fovChangerIsRunning = false;

	//Player settings
	private bool invertCamY = false;


	private void Start()
    {
        cam = GetComponent<Camera>();
		brain = GetComponent<CinemachineBrain>();

        baseFOV = brain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineFreeLook>().m_Lens.FieldOfView; //save default fov
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
			activeCam.m_Lens.FieldOfView = Mathf.Lerp(startFOV, newFOVValue, t); //Change fov

			//Debug.Log("fov: " + activeCam.m_Lens.FieldOfView);

			yield return null;
		}

		activeCam.m_Lens.FieldOfView = newFOVValue; //Round fov value
		fovChangerIsRunning = false;

		//Debug.Log("Finished ChangeFOVCoroutine, fov: " + activeCam.m_Lens.FieldOfView);
	}

	public void GetSettingsFromPlayerPrefs(CinemachineFreeLook vcam)
	{
		if (PlayerPrefs.HasKey("Options_CamInvertY"))
		{
			invertCamY = PlayerPrefs.GetInt(("Options_CamInvertY")) != 1; //Get player Y axis inversion setting
		}

		vcam.m_YAxis.m_InvertInput = invertCamY; //Apply Y axis inversion setting
	}

	public void ChangeUpdateMehod(bool fixedUpdate)
	{
		//Cinemachine brain update method has to be set to fixed update to support Time.timeScale
		//During gameplay with rigidbodys cinemachine works better with Smart Update
		//Swap between update methods

		if(fixedUpdate)
		{
			brain.m_UpdateMethod = CinemachineBrain.UpdateMethod.FixedUpdate;
		}
		else
		{
			brain.m_UpdateMethod = CinemachineBrain.UpdateMethod.SmartUpdate;
		}
	}
}
