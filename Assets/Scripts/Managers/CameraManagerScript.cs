using UnityEngine;
using System.Collections;
using UnityEngine.VR;

public class CameraManagerScript : SingletonComponent<CameraManagerScript>
{

    public Transform trackingCameraTransform;
    public Transform defaultCamera;
    public Transform vrCamera;

    private Transform trackingObject;
    private Vector3 trackingObjectOffset;

	// Use this for initialization
	void Start () {
        VRSettings.enabled = false;

        if (defaultCamera != null)
            defaultCamera.gameObject.SetActive(true);
        
        if (vrCamera != null)
            vrCamera.gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {

	    if (Input.GetKeyDown(KeyCode.T))
	    {
	        VRSettings.enabled = !VRSettings.enabled;

            if (defaultCamera != null)
                defaultCamera.gameObject.SetActive(!VRSettings.enabled);

            if (vrCamera != null)
                vrCamera.gameObject.SetActive(VRSettings.enabled);
        }

        if (Input.GetKeyDown(KeyCode.R))
            InputTracking.Recenter();

	    if (trackingObject != null)
	    {
            Vector3 targetPos = trackingObject.position + trackingObjectOffset;
            float dis = (trackingCameraTransform.position - targetPos).magnitude;
            trackingCameraTransform.position = Vector3.MoveTowards(trackingCameraTransform.position, targetPos, Time.deltaTime * dis * 2);
        }

        //if (vrCamera != null)
        //    vrCamera.localPosition = Vector3.MoveTowards(vrCamera.localPosition, Vector3.zero, 0.3f*Time.deltaTime);
        //if (defaultCamera != null)
        //    defaultCamera.localPosition = Vector3.MoveTowards(defaultCamera.localPosition, Vector3.zero, 0.3f * Time.deltaTime);

    }

    public static void SetTrackingObject(Transform obj)
    {
        CameraManagerScript.instance.SetTrackingObjectInternal(obj);
    }

    private void SetTrackingObjectInternal(Transform obj)
    {
        if (trackingObject == null)
            trackingObjectOffset = transform.position - obj.position;

        trackingObject = obj;
    }

    public static GameObject GetActiveCamera()
    {
        if (VRSettings.enabled)
            return instance.vrCamera.gameObject;
        else
            return instance.defaultCamera.gameObject;
    }
}
