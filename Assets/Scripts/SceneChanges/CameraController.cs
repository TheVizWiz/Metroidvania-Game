using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEditor.Android;
using UnityEngine;

public class CameraController : MonoBehaviour {
    
    public CinemachineVirtualCamera[] cameras;
    public CinemachineVirtualCamera[] followCameras;
    [SerializeField] private int startCam;

    private CinemachineVirtualCamera currentCam;


    private void Awake() {
        GameManager.cameraController = this;
    }

    void Start() {
        foreach (CinemachineVirtualCamera camera in followCameras) {
            camera.Follow = GameManager.player.transform;
        }

        if (startCam < cameras.Length) {
            cameras[startCam].Priority = 10;
            currentCam = cameras[startCam];
        }

        for (int i = 1; i < cameras.Length; i++) {
            cameras[i].Priority = 0;
        }

        GameManager.cameraController = this;


    }

    void Update() {
        
    }

    public void SetCurrentCam(CinemachineVirtualCamera camera) {
        currentCam.Priority = 0;
        camera.Priority = 10;
        currentCam = camera;
    }
}
