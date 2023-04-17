using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private TutorialData[] tutorial;
    [SerializeField] private TextMeshProUGUI instructions;
    [SerializeField] private EnemyShip enemy;

    private Player shipInputs;
    private InputAction move, shoot, ability;
    private InputAction[] inputActions;

    private void Awake()
    {
        //Get input actions to add events
        shipInputs = new Player();
        inputActions = new InputAction[3];

        move = shipInputs.Ship.Move;
        shoot = shipInputs.Ship.Shoot;
        ability = shipInputs.Ship.Roll;

        inputActions[0] = move;
        inputActions[1] = shoot;
        inputActions[2] = ability;

        ability.performed += OnAbility;
        shoot.performed += OnShoot;
        move.performed += OnMove;
    }

    private void OnMove(InputAction.CallbackContext obj)
    {
        tutorial[0].complete = true;
        ChangeInstruction();
    }

    private void OnShoot(InputAction.CallbackContext obj)
    {
        tutorial[1].complete = true;
        ChangeInstruction();
    }

    private void OnAbility(InputAction.CallbackContext obj)
    {
        tutorial[2].complete = true;
        ChangeInstruction();
    }

    private void OnEnable()
    {
        foreach (InputAction action in inputActions)
        {
            action.Enable();
        }
    }

    private void OnDisable()
    {
        foreach (InputAction action in inputActions)
        {
            action.Disable();
        }
    }

    private void Start()
    {
        //Skip tutorial if completed already
        if (GameData.instance.saveData.tutorialComplete)
        {
            UI.instance.LoadScene(1);
        }

        //Display first part of tutorial
        instructions.text = tutorial[0].instructionText;
        enemy.onDie += EnemyDefeated;
    }

    private void EnemyDefeated()
    {
        tutorial[3].complete = true;
        ChangeInstruction();
    }

    private void ChangeInstruction()
    {
        //Run through tutorial steps until incomplete part is found
        //Then display the incomplete step
        //Completes the tutorial if all steps completed
        for (int i = 0; i < tutorial.Length; i += 1)
        {
            if (!tutorial[i].complete)
            {
                instructions.text = tutorial[i].instructionText;
                break;
            }
            else if (i == tutorial.Length - 1)
            {
                instructions.text = (" ");
                foreach (InputAction action in inputActions)
                {
                    action.Disable();
                }
                GameData.instance.saveData.tutorialComplete = true;
                GameplayManager.instance.LevelFinished(true);
            }
        }
    }
}

[System.Serializable]
public class TutorialData
{
    public string name;
    [TextArea]
    public string instructionText;
    public bool complete;
}
