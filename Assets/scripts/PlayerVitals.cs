using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerVitals : MonoBehaviour
{
    public Slider healthSlider;
    public int maxHealth;
    public int healthFallRate;

    public Slider staminaSlider;
    public int maxStamina;
    private int staminaFallRate;
    public int staminaFallMult;
    private int staminaRegainRate;
    public int staminaRegainMult;

    public Slider hungerSlider;
    public int maxHunger;
    public int hungerFallRate;


    public Slider thristSlider;
    public int maxThrist;
    public int thirstFallRate;

    private CharacterController charController;
    private FirstPersonController playerController;

    void Start()
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;

        thristSlider.maxValue = maxThrist;
        thristSlider.value = maxThrist;

        hungerSlider.maxValue = maxHunger;
        hungerSlider.value = maxHunger;

        staminaSlider.maxValue = maxStamina;
        staminaSlider.value = maxStamina;
        staminaFallRate = 1;
        staminaRegainRate = 1;

        charController = GetComponent<CharacterController>();
        playerController = GetComponent<FirstPersonController>();
    }
    void Update()
    {
        //health degrade
        if(hungerSlider.value<=0 && thristSlider.value<=0)
        {
            healthSlider.value -= Time.deltaTime / healthFallRate * 2;
        }
        else if(hungerSlider.value <=0 || thristSlider.value<=0)
        {
            healthSlider.value -= Time.deltaTime / healthFallRate * 2;

        }
        if(healthSlider.value<=0)
        {
            //player death. DO SOMETHING
        }

        //hungerController
        if(hungerSlider.value>=0)
        {
            hungerSlider.value -= Time.deltaTime / hungerFallRate;
        }

        //thirstController
        if (thristSlider.value >= 0)
        {
            thristSlider.value -= Time.deltaTime / thirstFallRate;
        }
        //thrist dependency on stamina
        if(staminaSlider.value <=0)
        {
            thristSlider.value -= Time.deltaTime * 2 / thirstFallRate;
        }
        else if (staminaSlider.value >=0)
        {
            thristSlider.value -= Time.deltaTime / thirstFallRate ;
        }

        //this controlls the stamina system
        if (charController.velocity.magnitude > 0 && Input.GetKey(KeyCode.LeftShift))
        {
            staminaSlider.value -= Time.deltaTime / staminaFallRate * staminaFallMult;
        }
        else
        {
            staminaSlider.value += Time.deltaTime / staminaRegainRate * staminaRegainMult;
        }
        if(staminaSlider.value >= maxStamina)
        {
            staminaSlider.value = maxStamina;
        }
        else if (staminaSlider.value <= 0)
        {
            staminaSlider.value = 0;
            playerController.m_RunSpeed = playerController.m_WalkSpeed;
        }
        else if (staminaSlider.value >= 0)
        {
            playerController.m_RunSpeed = playerController.m_RunSpeedNorm;
        }
    }

}
