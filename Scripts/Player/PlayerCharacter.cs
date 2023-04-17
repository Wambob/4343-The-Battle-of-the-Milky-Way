using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacter : Ship
{
    #region Variables

    [SerializeField] private ShipData[] shipData;
    [SerializeField] private Ability[] abilities;
    [SerializeField] private float blendSpeed, powerDuration, healingChance, healAmount;

    public delegate void AbilityEvent();
    public event AbilityEvent onAbility;
    public Vector2 moveInput;
    public float abilityTimer, abilityCooldown, upgradePercent;

    private Player shipInputs;
    private InputAction move, shoot, ability;
    private InputAction[] inputActions;
    private Animator anim;
    private IEnumerator powerUpIE;
    private bool shooting;

    #endregion

    private void Awake()
    {
        //Instantiate input actions
        shipInputs = new Player();
        inputActions = new InputAction[3];

        move = shipInputs.Ship.Move;
        shoot = shipInputs.Ship.Shoot;
        ability = shipInputs.Ship.Roll;

        //Place actions in an array
        inputActions[0] = move;
        inputActions[1] = shoot;
        inputActions[2] = ability;

        ability.performed += OnAbility;
        shoot.performed += OnShoot;
        shoot.canceled += OffShoot;
    }

    private void OffShoot(InputAction.CallbackContext obj)
    {
        shooting = false;
    }

    private void OnShoot(InputAction.CallbackContext obj)
    {
        shooting = true;
    }

    private void OnAbility(InputAction.CallbackContext obj)
    {
        onAbility();
    }

    private void OnEnable()
    {
        foreach(InputAction action in inputActions)
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
        //Load data from gamedata based on ship currently equipped
        maxHealth = shipData[GameData.instance.saveData.shipEquipped].health;
        speed = shipData[GameData.instance.saveData.shipEquipped].speed;
        blendSpeed = shipData[GameData.instance.saveData.shipEquipped].blendSpeed;
        abilityCooldown = shipData[GameData.instance.saveData.shipEquipped].coolDown;
        equippedWeapon = arsenal[0];
        anim = GetComponent<Animator>();
        abilities[GameData.instance.saveData.shipEquipped].enabled = true;
        abilities[GameData.instance.saveData.shipEquipped].InitialiseAbility();

        //Load upgrade data
        if (GameData.instance.saveData.upgradesBought[0, GameData.instance.saveData.shipEquipped])
        {
            maxHealth *= 1 + upgradePercent;
        }
        if (GameData.instance.saveData.upgradesBought[1, GameData.instance.saveData.shipEquipped])
        {
            speed *= 1 + upgradePercent;
        }
        health = maxHealth;
        onDie += Dead;
    }

    private void Update()
    {
        //Countdown ability timer
        if (abilityTimer > 0)
        {
            UI.instance.UpdateBar(UI.BarType.Ability, (abilityCooldown - abilityTimer) / abilityCooldown);
            abilityTimer -= Time.deltaTime;
        }

        //Countdown weapon cooldown timer
        if (weaponCooldownTimer < equippedWeapon.cooldown)
        {
            weaponCooldownTimer += Time.deltaTime;
        }

        //Get movement input
        //Lerp for a smooth transition
        moveInput = Vector2.Lerp(moveInput, move.ReadValue<Vector2>(), blendSpeed * Time.deltaTime);

        //Apply transformation
        transform.position += (transform.forward * moveInput.y + transform.right * moveInput.x) * speed * Time.deltaTime;

        transform.localPosition = GameplayManager.instance.GetValidPosition(transform.localPosition);

        SetRoll(moveInput.x, moveInput.y);

        //Fire weapon if firing input true and able to
        if (weaponCooldownTimer >= equippedWeapon.cooldown && shooting)
        {
            Shoot(equippedWeapon, transform.forward);

            weaponCooldownTimer = 0;
        }
    }

    public IEnumerator PowerUp()
    {
        //Equip a different weapon for a given amount of time
        equippedWeapon = arsenal[Random.Range(1, arsenal.Length)];
        UI.instance.CallForNotification(equippedWeapon.name + " weapon equipped!");
        yield return new WaitForSeconds(powerDuration);
        equippedWeapon = arsenal[0];
    }

    public void StartPowerUp()
    {
        //Determine whether powerup is a health pickup or weapon swap
        if (Random.Range(0f, 1f) <= healingChance)
        {
            UI.instance.CallForNotification("Healed +" + ((int)((healAmount / maxHealth) * 100)) + "%");
            ChangeHealth(healAmount);
        }
        else
        {
            if (powerUpIE != null)
            {
                StopCoroutine(powerUpIE);
            }
            powerUpIE = PowerUp();
            StartCoroutine(powerUpIE);
        }
    }

    private void Dead()
    {
        CameraShake.camShake.AddShake(1, 1);
        GameplayManager.instance.CallForExplosion(transform.position);
        foreach (InputAction action in inputActions)
        {
            action.Disable();
        }
        GameplayManager.instance.LevelFinished(false);
    }
}

[System.Serializable]
public class ShipData
{
    public string name;
    public float health, speed, blendSpeed, coolDown;
}
