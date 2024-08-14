using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.Versioning;

public class Player_UI : MonoBehaviour
{
    [Header("Hitpoint/Stamina")]
    [SerializeField] private float health;
    [SerializeField] private float max_health = 100f;
    [SerializeField] private float stamina;
    [SerializeField] private float max_stamina = 100f;
    [SerializeField] private float bar_health_chipSpeed = 2.0f;
    [SerializeField] private float bar_stamina_chipSpeed = 6.0f;
    //Reference Healthbars
    public Image front_healthbar;
    public Image back_healthbar;
    //Reference Stamina Bars
    public Image front_staminabar;
    public Image back_staminabar;
    //For chip effect
    public float lerp_timer_health;
    public float lerp_timer_stamina;
    //To detect changes
    private float last_health;
    private float last_stamina;
    private bool stamina_can_sprint;

    [Header("Exposure")]
    public Slider exposure_meter;
    public TMP_Text TMP_exposure_level;
    //To detect changes
    private int exposure_level_last = 0;
    private float exposure_amount_last = 0.0f;

    [Header("Interact")]
    public TMP_Text TMP_interact_text;
    public TMP_Text TMP_interact_name;
    public TMP_Text TMP_interact_desc;

    [Header("Misc")]
    //Reference Level
    private Level level;
    //Reference Player
    private Player player;

    void Start()
    {
        //Reference Level
        level = GetComponentInParent<Level>();
        //Reference Player
        player = level.return_player();

        //Start with full health and stamina
        health = max_health;
        stamina = max_stamina;

        //Set last health and stamina to max
        last_health = max_health;
        last_stamina = max_stamina;

        //Set exposure meters to base
        exposure_meter.value = 0.0f;
        TMP_exposure_level.text = " 0 ";
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
        UpdateExposureLevel();
        UpdateInteractFrame();
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

    //Update Interact Frame
    private void UpdateInteractFrame()
    {
        //Grab from player
        Interactable current_interaction = player.getCurrentInteractable();
        //If not null
        if (current_interaction != null)
        {
            //Make Press X
            string interaction_key = level.interact_key.ToString();
            TMP_interact_text.text = "Press " + interaction_key;
            //Grab name
            string interaction_name = current_interaction.get_name();
            TMP_interact_name.text = interaction_name;
            //Grab desc
            string interaction_desc = current_interaction.get_desc();
            TMP_interact_desc.text = interaction_desc;
        }
        else
        {
            TMP_interact_text.text = "";
            TMP_interact_name.text = "";
            TMP_interact_desc.text = "";
        }

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

    //Update Exposure Level
    private void UpdateExposureLevel()
    {
        //Grab exposures from level
        int exposure_level = level.exposure_level;
        float exposure_amount = level.exposure_amount;

        //See if changes
        if (exposure_level != exposure_level_last)
        {
            //Change level text
            string text = exposure_level_text(exposure_level);
            TMP_exposure_level.text = text;
            //Change
            exposure_level_last = exposure_level;
        }

        //Exposure Amount
        //See changes
        if (exposure_amount != exposure_amount_last)
        {
            //If different change it up
            //Set to percentage first
            float value = exposure_amount / level.exposure_amount_max;
            exposure_meter.value = value;
            //Change
            exposure_amount_last = exposure_amount;
        }
    }

    //Exposure Level Text
    private string exposure_level_text(int level)
    {
        string output = "";

        switch (level)
        {
            case 0:
                output = "0";
                break;
            case 1:
                output = "I";
                break;
            case 2:
                output = "II";
                break;
            case 3:
                output = "III";
                break;
            default:
                output = "X";
                break;
        }

        return output;
    }
}
