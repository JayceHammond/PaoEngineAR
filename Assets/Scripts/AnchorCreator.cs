using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

//
// This script allows us to create anchors with
// a prefab attached in order to visbly discern where the anchors are created.
// Anchors are a particular point in space that you are asking your device to track.
//

[RequireComponent(typeof(ARAnchorManager))]
[RequireComponent(typeof(ARRaycastManager))]
[RequireComponent(typeof(ARPlaneManager))]
public class AnchorCreator : MonoBehaviour
{
    // This is the prefab that will appear every time an anchor is created.
    [SerializeField]
    GameObject m_AnchorPrefab;
    public GameObject cam;

    public GameObject AnchorPrefab
    {
        get => m_AnchorPrefab;
        set => m_AnchorPrefab = value;
    }

    // Removes all the anchors that have been created.
    public void RemoveAllAnchors()
    {
        foreach (var anchor in m_AnchorPoints)
        {
            Destroy(anchor);
        }
        m_AnchorPoints.Clear();
    }

    // On Awake(), we obtains a reference to all the required components.
    // The ARRaycastManager allows us to perform raycasts so that we know where to place an anchor.
    // The ARPlaneManager detects surfaces we can place our objects on.
    // The ARAnchorManager handles the processing of all anchors and updates their position and rotation.
    void Awake()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();
        m_AnchorManager = GetComponent<ARAnchorManager>();
        m_PlaneManager = GetComponent<ARPlaneManager>();
        m_AnchorPoints = new List<ARAnchor>();
    }

void Update()
{
    if (Input.touchCount == 0)
        return;

    var touch = Input.GetTouch(0);
    if (touch.phase != TouchPhase.Began)
        return;

    if (m_RaycastManager.Raycast(touch.position, s_Hits, TrackableType.PlaneWithinPolygon))
    {
        var hitPose = s_Hits[0].pose;
        var hitTrackableId = s_Hits[0].trackableId;
        var hitPlane = m_PlaneManager.GetPlane(hitTrackableId);

        var anchor = m_AnchorManager.AttachAnchor(hitPlane, hitPose);
        GameObject die = Instantiate(m_AnchorPrefab, anchor.transform);
        die.GetComponent<Rigidbody>().velocity = new Vector3(die.GetComponent<Rigidbody>().velocity.x, die.GetComponent<Rigidbody>().velocity.y, 
        die.GetComponent<Rigidbody>().velocity.z + Random.Range(2, 5));
        die.GetComponent<Rigidbody>().AddTorque(Random.Range(0, 500), Random.Range(0, 500), Random.Range(0, 500));

        if (anchor == null)
        {
            Debug.Log("Error creating anchor.");
        }
        else
        {
            m_AnchorPoints.Add(anchor);
        }

        // Calculate the direction the camera is facing.
        Vector3 cameraForward = Camera.main.transform.forward;

        // Define a small initial velocity to shoot the die lightly forward.
        float initialVelocity = Random.Range(2, 5); // Adjust as needed.

        // Apply the initial velocity in the camera's forward direction.

    }
}

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    List<ARAnchor> m_AnchorPoints;

    ARRaycastManager m_RaycastManager;

    ARAnchorManager m_AnchorManager;

    ARPlaneManager m_PlaneManager;
}
