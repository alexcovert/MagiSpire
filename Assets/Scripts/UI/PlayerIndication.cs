﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerIndication : MonoBehaviour {
    [SerializeField] private Camera camTop;
    [SerializeField] GameObject playerOne;
    [SerializeField] GameObject playerTwo;
    [SerializeField] TextMeshProUGUI topText;
    [SerializeField] TextMeshProUGUI botText;

    
    private InputManager inputManager;
    private int numTimesRotated = 0;
    [SerializeField] private GameObject tower;
    private PauseMenu pause;
    [SerializeField] private GameObject gameManager;

    private int floorTop = 2;
    private int floorBot = 1;

    private int floorDifTop = 0;
    private int floorDifBot = 0;

    private int playerOneCurrentFace = 1;
    private int playerOneLastFace = 1;

    // Use this for initialization
    void Start () {
        inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();
        pause = gameManager.GetComponent<PauseMenu>();
    }
	
	// Update is called once per frame
	void Update () {
        floorTop = camTop.GetComponent<CameraTwoRotator>().GetFloor();
        floorBot = playerOne.GetComponent<CameraOneRotator>().GetFloor();

        floorDifTop = floorTop - floorBot;
        floorDifBot = floorBot - floorTop;

        numTimesRotated = playerTwo.GetComponent<PlaceTrap>().GetNumRotated();
        
        if(playerOneCurrentFace != playerOne.GetComponent<CameraOneRotator>().GetState())
        {
            playerOneLastFace = playerOneCurrentFace;
            playerOneCurrentFace = playerOne.GetComponent<CameraOneRotator>().GetState();
        }
        if(playerOneLastFace > playerOneCurrentFace)
        {
            if (floorDifBot < 0)
            {
                botText.text = "Ollie: " + (-floorDifBot) + " floor above";
            }
            else if (floorDifBot > 0)
            {
                botText.text = "Ollie: " + floorDifBot + " floor below";
            }
            else
            {
                botText.text = "Ollie: on same floor";
            }
            botText.CrossFadeAlpha(1, 2.0f, false);
        }
        if (inputManager.GetButtonDown(InputCommand.TopPlayerRotate) && !pause.GameIsPaused && numTimesRotated%3 == 0 &&numTimesRotated < 4 * (tower.GetComponentInChildren<NumberOfFloors>().NumFloors - 1) - 1)
        {
            if (floorDifTop > 0)
            {
                topText.text = "Speccy: " + (floorDifTop) + " floor below";
            }
            else if (floorDifTop < 0)
            {
                topText.text = "Speccy: " + (-floorDifTop) + "floor above";
            }
            else
            {
                topText.text = "Speccy: on same floor";
            }
            topText.CrossFadeAlpha(1, 2.0f, false);
        }
            
	}
    private IEnumerator BotFading()
    {
        if (floorDifBot < 0)
        {
            botText.text = "Ollie: " + (-floorDifBot) + " floor above";
        }
        else if (floorDifBot > 0)
        {
            botText.text = "Ollie: " + floorDifBot + " floor below";
        }
        else
        {
            botText.text = "Ollie: on same floor";
        }
        botText.CrossFadeAlpha(1, 2.0f, false);
        
        yield return new WaitForSeconds(2);

        botText.CrossFadeAlpha(0, 2.0f, false);
    }

    private IEnumerator TopFading()
    {
        if(floorDifTop > 0)
        {
            topText.text = "Speccy: " + (floorDifTop) + " floor below";
        }
        else if (floorDifTop< 0)
        {
            topText.text = "Speccy: " + (-floorDifTop) + "floor above";
        }
        else
        {
            topText.text = "Speccy: on same floor";
        }
        topText.CrossFadeAlpha(1, 2.0f, false);
        yield return new WaitForSeconds(2);
    }
}
