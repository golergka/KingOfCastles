using UnityEngine;
using System.Collections;

public class RTSCameraController : MonoBehaviour {

	public float cameraSpeed = 1f;

	public float cameraHeight = 5f;
	public float maxCameraHeight = 20f;
	public float minCameraHeight = 5f;

	public float zoomHeightSpeed = 1f;
	public float zoomRotationSpeed = 1f;
	public float zoomRailSpeed = 1f;
	public float zoomCameraAccelerationSpeed = 1f;

	// Use this for initialization
	void Start () {

		
	
	}
	
	// Update is called once per frame
	void Update () {

		// move in horizontal pane

		float x = -Input.GetAxis("Horizontal");
		float y = -Input.GetAxis("Vertical");

		Vector3 cameraMovement = new Vector3(x, 0f, y);
		cameraMovement.Normalize();
		cameraMovement *= cameraSpeed;

		transform.Translate(cameraMovement * Time.deltaTime, Space.World);

		// set height

		float cameraRotation = 0;
		float zoomChange = Input.GetAxis("Mouse ScrollWheel");
		cameraHeight += zoomChange * zoomHeightSpeed * Time.deltaTime;
		cameraRotation = zoomChange * zoomRotationSpeed * Time.deltaTime;
		cameraSpeed += zoomChange * zoomCameraAccelerationSpeed * Time.deltaTime;

		Vector3 cameraPosition = transform.position;
		cameraPosition.y = Terrain.activeTerrain.SampleHeight(transform.position) + cameraHeight;
		cameraPosition.z += zoomChange * zoomRailSpeed * Time.deltaTime;
		transform.Rotate(cameraRotation, 0, 0, Space.World);
		transform.position = cameraPosition;
	
	}
}
