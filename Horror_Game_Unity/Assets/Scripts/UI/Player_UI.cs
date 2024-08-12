using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;

public class Player_UI : MonoBehaviour
{
    [Header("Hitpoint/Stamina")]
    [SerializeField] private float health;
    [SerializeField] private float max_health = 100f;
    [SerializeField] private float stamina;
    [SerializeField] private float max_stamina = 100f;
    [SerializeField] private float bar_health_chipSpeed = 2.0f;
    [SerializeField] private float bar_stamina_chipSpeed = 6.0f;
    private bool stamina_can_sprint;

    //Reference Level
    private Level level;
    //Reference Healthbars
    public Image front_healthbar;
    public Image back_healthbar;
    //Reference Stamina Bars
    public Image front_staminabar;
    public Image back_staminabar;
    //To detect changes
    private float last_health;
    private float last_stamina;
    //For chip effect
    public float lerp_timer_health;
    public float lerp_timer_stamina;

    void Start()
    {
        //Reference Level
        level = GetComponentInParent<Level>();

        //Start with full health and stamina
        health = max_health;
        stamina = max_stamina;

        //Set last health and stamina to max
        last_health = max_health;
        last_stamina = max_stamina;
    }

    public void UpdateUIHealthStamina(float current_health, float current_stamina)
    {
        health = current_health;
        stamina = current_stamina;
    }

    // Update is called once per frame
    void Update()
    {
        //Clamp health and stamina
        health = Mathf.Clamp(health, 0, max_health);
        stamina = Mathf.Clamp(stamina, 0, max_stamina);
        UpdateHealthUI();
        UpdateStaminaUI();
    }

    public void ResetHealthLerp()
    {
        lerp_timer_health = 0.0f;
    }

    public void ResetStaminaLerp()
    {
        lerp_timer_health = 0.0f;
    }

    public void SetSprintAllowed(bool state)
    {
        stamina_can_sprint = state;
    }

    //Update Health UI
    private void UpdateHealthUI()
    {
        //Fill colors
        float health_fill_foreground = front_healthbar.fillAmount;
        float health_fill_background = back_healthbar.fillAmount;

        //Health as percentage 
        float health_percent = health / max_health;

        //Check if changes occured
        //Health changes
        if (health != last_health)
        {

            //Check if heal or damage
            if (health > last_health)
            {
                //Set color
                back_healthbar.color = Color.green;

                //Set back healthbar
                back_healthbar.fillAmount = health_percent;

                //Lerp timer
                lerp_timer_health += Time.deltaTime;
                float percentComplete = lerp_timer_health / bar_health_chipSpeed;

                //Small effect for better animation
                percentComplete = percentComplete * percentComplete;

                //Heal Effect
                front_healthbar.fillAmount = Mathf.Lerp(health_fill_background, health_percent, percentComplete);
            }
            else
            {
                //Set color
                back_healthbar.color = Color.red;

                //Set front healthbar
                front_healthbar.fillAmount = health_percent;

                //Lerp timer for chip effect
                lerp_timer_health += Time.deltaTime;
                float percentComplete = lerp_timer_health / bar_health_chipSpeed;

                //Small effect for better animation
                percentComplete = percentComplete * percentComplete;

                //Damage Effect
                back_healthbar.fillAmount = Mathf.Lerp(health_fill_foreground, health_percent, percentComplete);
            }
           }
        }

    //Update Stamina UI
        private void UpdateStaminaUI()
        {
        
            //Fill colors
            float stamina_fill_foreground = front_staminabar.fillAmount;
            float stamina_fill_background = back_staminabar.fillAmount;

            //Stamina as percentage 
            float stamina_percent = stamina / max_stamina;


            //Check if stamina changed
            if (stamina != last_stamina)
            {
                //Color change if sprint is allowed or not
                if (stamina_can_sprint == true)
                {
                    //Reset color
                    front_staminabar.color = Color.white;
                }
                else
                {
                //Set to red
                    front_staminabar.color = Color.red;
                }

                //Check if loss or gain
                if (stamina > last_stamina)
                {
                    //Gain
                    //Set color
                    back_staminabar.color = Color.green;

                    //Set back healthbar
                    back_staminabar.fillAmount = stamina_percent;

                    //Lerp timer
                    lerp_timer_stamina += Time.deltaTime;
                    float percentComplete = lerp_timer_stamina / bar_stamina_chipSpeed;

                    //Small effect for better animation
                    percentComplete = percentComplete * percentComplete;

                    //Gain Effect
                    front_staminabar.fillAmount = Mathf.Lerp(stamina_fill_background, stamina_percent, percentComplete);
                }
                
                else
                {
                    //Gain
                    //Set color
                    back_staminabar.color = Color.red;

                    //Set back healthbar
                    front_staminabar.fillAmount = stamina_percent;

                    //Lerp timer
                    lerp_timer_stamina += Time.deltaTime;
                    float percentComplete = lerp_timer_stamina / bar_stamina_chipSpeed;

                    //Small effect for better animation
                    percentComplete = percentComplete * percentComplete;

                    //Loss Effect
                    back_staminabar.fillAmount = Mathf.Lerp(stamina_fill_foreground, stamina_percent, percentComplete);
                }
            }
        }
}
