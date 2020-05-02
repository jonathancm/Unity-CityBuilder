using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour
{
    [System.Serializable]
    public struct CameraSettings
    {
        public float panSpeed;
        public float zoomSpeed;
        public float zoomNearest;
        public float zoomFarthest;
    }

    //
    // Configurable Parameters
    //
    public Camera _camera = null;
    public CameraSettings cameraSettings;

    public void Zoom(float deltaScroll)
    {
        float distanceFromRig = _camera.transform.localPosition.y - (deltaScroll * cameraSettings.zoomSpeed);
        distanceFromRig = Mathf.Clamp(distanceFromRig, cameraSettings.zoomNearest, cameraSettings.zoomFarthest);
        _camera.transform.localPosition = new Vector3(0, distanceFromRig, -distanceFromRig);
    }

    public void Rotate()
    {

    }

    public void Pan(Vector2 mouseDragStart, Vector2 currentMousePos)
    {
        var mouseDelta = new Vector3(currentMousePos.x - mouseDragStart.x, 0, currentMousePos.y - mouseDragStart.y);
        var cameraDelta = mouseDelta * cameraSettings.panSpeed * _camera.transform.localPosition.magnitude * Time.deltaTime;
        transform.Translate(cameraDelta, Space.Self);
    }
}
