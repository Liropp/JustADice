using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateLevel : MonoBehaviour
{
    // UI
    [SerializeField] private List<Transform> btns;
    [SerializeField] private List<Transform> btnsPositions;

    // Cam
    private Camera _camera;
    private Vector3 startPos;
    private Quaternion startRot;

    private void Awake()
    {
        _camera = Camera.main;
        startPos = _camera.transform.position;
        startRot = _camera.transform.rotation;
    }

    /// <summary>
    /// rotate the camera (front or back)
    /// </summary>
    public void Rotation()
    {
        #region switch between 2 camera views
        if (startPos == _camera.transform.position && startRot == _camera.transform.rotation)
        {
            _camera.transform.position = new Vector3(-7, 4, 7);
            _camera.transform.rotation = Quaternion.Euler(30, -225, 0);

            btns[0].position = btnsPositions[1].position;
            btns[0].rotation = btnsPositions[1].rotation;
            //Debug.Log(btns[0].position+" "+ btnsPositions[1].position);
            btns[1].position = btnsPositions[0].position;
            btns[1].rotation = btnsPositions[0].rotation;
            btns[2].position = btnsPositions[3].position;
            btns[2].rotation = btnsPositions[3].rotation;
            btns[3].position = btnsPositions[2].position;
            btns[3].rotation = btnsPositions[2].rotation;
        }
        else
        {
            //Debug.Log("front");
            _camera.transform.position = startPos;
            _camera.transform.rotation = startRot;

            btns[0].position = btnsPositions[0].position;
            btns[0].rotation = btnsPositions[0].rotation;
            btns[1].position = btnsPositions[1].position;
            btns[1].rotation = btnsPositions[1].rotation;
            btns[2].position = btnsPositions[2].position;
            btns[2].rotation = btnsPositions[2].rotation;
            btns[3].position = btnsPositions[3].position;
            btns[3].rotation = btnsPositions[3].rotation;
        }
        #endregion
    }
}
