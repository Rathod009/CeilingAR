using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlaceObject : MonoBehaviour
{
    public ARPlaneManager planeManager;
    public List<GameObject> gameObjects = new List<GameObject>();
    public List<GameObject> uiBtns = new List<GameObject>();
    int cur = 0;
    public GameObject placementIndicator;
    private ARSessionOrigin arOrigin;
    private Pose placementPose;
    private bool placementPoseIsValid = false;

    // Start is called before the first frame update
    void Start()
    {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
    }

    // Update is called once per frame
    void Update()
    {

        updatePose();
        updateIndicator();

        if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            placeObject();
        }
    }


    public void placeObject()
    {
        string buttonName = EventSystem.current.currentSelectedGameObject.name;

        for (int i = 0; i < uiBtns.Count; i++)
        {
            if (uiBtns[i].name == buttonName)
            {
                cur = i;
            }
        }

        foreach (var obj in gameObjects)
        {
            obj.SetActive(false);
        }

        gameObjects[cur].SetActive(true);
        gameObjects[cur].transform.position = placementPose.position;
        gameObjects[cur].transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
    }

    void updateIndicator()
    {
        if (placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }

        else
        {
            placementIndicator.SetActive(false);
        }
    }

    void updatePose()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        arOrigin.GetComponent<ARRaycastManager>().Raycast(screenCenter, hits, TrackableType.PlaneEstimated);
        if (hits.Count > 0)
            placementPoseIsValid = true;
        else
            placementPoseIsValid = false;

        if (placementPoseIsValid)
        {

            TrackableId planeHit_ID = hits[0].trackableId;
            ARPlane planeHit = planeManager.GetPlane(planeHit_ID);

            if (isUpperPlane(planeHit))
            {
                placementPose = hits[0].pose;
                var cameraForward = Camera.current.transform.forward;
                var cameraBearing = new Vector3(cameraForward.x * -1, 0, cameraForward.z * -1).normalized;
                placementPose.rotation = Quaternion.LookRotation(cameraBearing);
            }

            else
            {
                placementPoseIsValid = false;
            }
        }


    }


    bool isUpperPlane(ARPlane plane)
    {

        if (plane.alignment == PlaneAlignment.HorizontalDown)
        {
            return true;
        }

        return false;
    }

}



  

