
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Camera arCam;
    [SerializeField] private ARRaycastManager _raycastManager;
    [SerializeField] private GameObject crosshair;

    private static List<ARRaycastHit> _hits = new List<ARRaycastHit>();

    private Pose pose;

    void Update()
    {
        CrosshairCalculation();

        if (Input.touchCount == 0)
            return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase != TouchPhase.Began)
            return;

        if (IsPointerOverUI(touch))
            return;

        // Raycast from the exact screen position tapped
        if (_raycastManager.Raycast(
            touch.position,
            _hits,
            TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = _hits[0].pose;

            GameObject furniture = DataHandler.Instance.GetFurniture();

            if (furniture != null)
            {
                Instantiate(
                    furniture,
                    hitPose.position,
                    hitPose.rotation
                );
            }
        }
    }

    bool IsPointerOverUI(Touch touch)
    {
        if (EventSystem.current == null)
            return false;

        PointerEventData eventData =
            new PointerEventData(EventSystem.current);

        eventData.position = touch.position;

        List<RaycastResult> results = new List<RaycastResult>();

        EventSystem.current.RaycastAll(eventData, results);

        return results.Count > 0;
    }

    void CrosshairCalculation()
    {
        if (arCam == null || _raycastManager == null)
            return;

        Vector3 origin = arCam.ViewportToScreenPoint(
            new Vector3(0.5f, 0.5f, 0)
        );

        Ray ray = arCam.ScreenPointToRay(origin);

        if (_raycastManager.Raycast(
            ray,
            _hits,
            TrackableType.PlaneWithinPolygon))
        {
            pose = _hits[0].pose;

            if (crosshair != null)
            {
                crosshair.transform.SetPositionAndRotation(
                    pose.position,
                    pose.rotation
                );
            }
        }
    }

    public void HomeSceneloader()
    {
        SceneManager.LoadScene(0);
    }

    public void Scene1loader()
    {
        SceneManager.LoadScene(1);
    }

    public void Scene2loader()
    {
        SceneManager.LoadScene(2);
    }

    public void Close_Instructions()
    {
        GameObject.Find("Instruction_Panel").SetActive(false);
    }
}
