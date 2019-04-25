﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
//<alexc> This class rotates and moves the Player 2 (right side camera) on a given input.
public class CameraTwoRotator : MonoBehaviour {
    [Header("Programmers - GameObjects/Scripts -----")]
    [SerializeField] private GameObject tower;
    [SerializeField] private GameObject playerTwoCam;
    [SerializeField] private Image gridUI;
    [SerializeField] private Toggle gridToggle;
    [SerializeField] private GameObject cameraTarget;
    [SerializeField] private GameManager gm;

    [Header("Designers - Speeds & Offsets -----")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float zoomOutAmount;
    [SerializeField] private float moveUpSlowMultiplier;
    [SerializeField] private int offsetFromAbove;

    //Change these static variables iff tower is scaled
    private static int camPosHorizontal = 75;
    private static int camPosVertical = 20;
    private static int camRotationX = 0;
    private static int numFloors;
    

    private Vector3[] basePositions = new [] { new Vector3(0,                   camPosVertical, -camPosHorizontal),
                                               new Vector3(camPosHorizontal,    camPosVertical, 0),
                                               new Vector3(0,                   camPosVertical, camPosHorizontal),
                                               new Vector3(-camPosHorizontal,   camPosVertical, 0)};

    private Quaternion[] baseRotations = new[] { Quaternion.Euler(camRotationX,  0,   0),
                                                 Quaternion.Euler(camRotationX, -90,  0),
                                                 Quaternion.Euler(camRotationX, -180, 0),
                                                 Quaternion.Euler(camRotationX, -270, 0)};

    private IEnumerator camTween;
    private IEnumerator targetTween;

    private int currentPos, floor;

    private bool moveEnabled = true;
    private PauseMenu pause;
    private InputManager inputManager;

    private CinemachineVirtualCamera cinemachineCam;

    private void Awake()
    {
        inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();
        cinemachineCam = playerTwoCam.GetComponent<CinemachineVirtualCamera>();
    }
    private void Start()
    {
        numFloors = tower.GetComponent<NumberOfFloors>().NumFloors;
        pause = gm.GetComponent<PauseMenu>();
        Vector3 startPos = basePositions[0] + new Vector3(0, 20 - offsetFromAbove, 0);
        playerTwoCam.transform.position = startPos;
        playerTwoCam.transform.rotation = baseRotations[0];

        currentPos = 1;
        floor = 2;

        moveEnabled = true;
    }

    //Rotate camera around tower when arrow keys are pressed
    private void Update()
    {
        //Allow toggling grid on/off for playtesting
        if(gridToggle.isOn && !gridUI.gameObject.activeInHierarchy)
        {
            gridUI.gameObject.SetActive(true);
        }
        else if(!gridToggle.isOn && gridUI.gameObject.activeInHierarchy)
        {
            gridUI.gameObject.SetActive(false);
        }


        if (moveEnabled)
        {
            if (inputManager.GetButtonDown(InputCommand.TopPlayerRotate) && !pause.GameIsPaused)
            {
                moveEnabled = false;

                if (currentPos == basePositions.Length)
                {
                    if (floor < numFloors)
                    {
                        moveEnabled = false;
                        floor++;
                        //cameraTarget.transform.position = new Vector3(cameraTarget.transform.position.x, cameraTarget.transform.position.y + 20, cameraTarget.transform.position.z);

                        StartMove(basePositions[0], baseRotations[0], 1, moveSpeed * moveUpSlowMultiplier);
                        StartCoroutine(ChangeFOV(moveSpeed * moveUpSlowMultiplier));
                    }
                }
                else
                {
                    StartMove(basePositions[currentPos], baseRotations[currentPos], currentPos + 1, moveSpeed);
                }
            }
        }
    }

    //Initialize camera movement variables and start movement coroutine
    private void StartMove(Vector3 goalPos, Quaternion goalRot, int goal, float time)
    {
        currentPos = goal;

        if (camTween != null)
        {
            StopCoroutine(camTween);
        }
        //Tween the vcam rotation
        camTween = TweenToPosition(goalPos, goalRot, time);
        StartCoroutine(camTween);

        //Tween the targets (at edges of face) rotation - vcam will follow at this speed
        targetTween = TargetTween(goalPos, goalRot, time);
        StartCoroutine(targetTween);



        MoveGrid();

    }

    //Camera movement coroutine
    private IEnumerator TweenToPosition(Vector3 targetPos, Quaternion targetRot, float time)
    {
        Vector3 currentPos = playerTwoCam.transform.position;
        Quaternion currentRot = playerTwoCam.transform.rotation;

        targetPos.y *= floor;
        targetPos.y -= offsetFromAbove;
        for (float t = 0; t < time; t += Time.deltaTime)
        {
            //playerTwoCam.transform.position = Vector3.Lerp(currentPos, targetPos, t/time);
            playerTwoCam.transform.rotation = Quaternion.Slerp(currentRot, targetRot, t/time);

            yield return null;
        }

        playerTwoCam.transform.position = targetPos;
        playerTwoCam.transform.rotation = targetRot;

        moveEnabled = true;
        targetTween = null;
    }

    private IEnumerator TargetTween(Vector3 targetPos, Quaternion targetRot, float time)
    {
        Vector3 currentPos = cameraTarget.transform.position;
        Quaternion currentRot = playerTwoCam.transform.rotation;

        targetPos.x = targetPos.z = 0;
        targetPos.y = floor * 20 - 40;
        for (float t = 0; t < time; t += Time.deltaTime)
        {
            cameraTarget.transform.position = Vector3.Lerp(currentPos, targetPos, t / time);
            cameraTarget.transform.rotation = Quaternion.Slerp(currentRot, targetRot, t / time);

            yield return null;
        }
        cameraTarget.transform.rotation = targetRot;

        moveEnabled = true;
        camTween = null;
    }

    private IEnumerator ChangeFOV(float time)
    {
        float normalFOV = cinemachineCam.m_Lens.FieldOfView;
        float zoomedFOV = cinemachineCam.m_Lens.FieldOfView - zoomOutAmount;

        cinemachineCam.m_Lens.FieldOfView = zoomedFOV;

        for (float t = 0; t < time / 2; t += Time.deltaTime)
        {
            cinemachineCam.m_Lens.FieldOfView = Mathf.Lerp(normalFOV, zoomedFOV, t / time);
            yield return null;
        }

        for (float t = time / 2; t < time; t += Time.deltaTime)
        {
            cinemachineCam.m_Lens.FieldOfView = Mathf.Lerp(zoomedFOV, normalFOV, t / time);
            yield return null;
        }
    }

    //Rotate and move worldspace grid UI with camera
    private void MoveGrid()
    {
        gridUI.transform.Rotate(0, 90, 0);
        switch (currentPos)
        {
            case 1:
                //Move up 20 when it hits face 1 again
                gridUI.transform.position = new Vector3(0, gridUI.transform.position.y + 20, -40.1f);
                break;
            case 2:
                gridUI.transform.position = new Vector3(40.1f, gridUI.transform.position.y, 0);
                break;
            case 3:
                gridUI.transform.position = new Vector3(0, gridUI.transform.position.y, 40.1f);
                break;
            case 4:
                gridUI.transform.position = new Vector3(-40.1f, gridUI.transform.position.y, 0);
                break;
        }
    }

    public int GetState()
    {
        return currentPos;
    }
    
    public int GetFloor()
    {
        return floor;
    }
}
