using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.Versioning;
using System.ComponentModel.Design;

public class Player_UI : MonoBehaviour
{
    [Header("Hitpoint/Stamina")]
    [SerializeField] private float health;
    [SerializeField] private float max_health = 100f;
    [SerializeField] private float stamina;
    [SerializeField] private float max_stamina = 100f;
    [SerializeField] private float bar_health_chipSpeed = 2.0f;
    [SerializeField] private float bar_stamina_chipSpeed = 6.0f;

    [Header("References")]
    [Header("Health Bar")]
    [SerializeField] public GameObject main_UI;
    //Reference Healthbars
    [SerializeField] public Image front_healthbar;
    [SerializeField] public Image back_healthbar;
    [SerializeField] public Image invul_shield;
    [SerializeField] public float lerp_timer_health;
    [SerializeField] public float lerp_timer_stamina;
    [Header("Damage Frame")]
    [SerializeField] public GameObject damage_frame_UI;
    [SerializeField] public Image red_screen;
    [SerializeField] public float blood_screen_fade_duration = 0.8f;
    [Header("Stamina Bar")]
    //Reference Stamina Bars
    [SerializeField] public Image front_staminabar;
    [SerializeField] public Image back_staminabar;
    [SerializeField] private float last_health;
    [SerializeField] private float last_stamina;
    [SerializeField] private bool stamina_can_sprint;
    [Header("Death Screen")]
    //Reference Death Screen
    [SerializeField] public GameObject death_screen_UI;
    [SerializeField] public Image black_screen;
    [SerializeField] public Button death_restart_button;
    [SerializeField] public Button death_main_menu_button;
    [SerializeField] public float black_screen_fade_duration = 1f;
    [Header("Pause")]
    [SerializeField] public GameObject pause_UI;
    [SerializeField] public Button menu_continue_button;
    [SerializeField] public Button menu_restart_button;
    [SerializeField] public Button menu_main_menu_button;
    [Header("Exposure")]
    [SerializeField] public Slider exposure_meter;
    [SerializeField] public TMP_Text TMP_exposure_level;
    [SerializeField] private int exposure_level_last = 0;
    [SerializeField] private float exposure_amount_last = 0.0f;
    [SerializeField] public GameObject exposure_alert_UI;
    [SerializeField] public TMP_Text TMP_exposure_increase_alert;
    [SerializeField] public TMP_Text TMP_exposure_decrease_alert;
    [SerializeField] private string exposure_alert_increase = "EXPOSURE LEVEL INCREASED";
    [SerializeField] private string exposure_alert_decrease = "EXPOSURE LEVEL DECREASED";

    [Header("Interact")]
    [SerializeField] public GameObject interact_UI;
    public TMP_Text TMP_interact_text;
    public TMP_Text TMP_interact_name;
    public TMP_Text TMP_interact_desc;
    public Slider interact_bar;

    [Header("Misc")]
    //Reference Level
    [SerializeField] private Level level;
    //Reference Player
    [SerializeField] private Player player;

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

        //Set exposure alerts off
        TMP_exposure_increase_alert.text = " ";
        TMP_exposure_decrease_alert.text = " ";

        //Hide slider
        interact_bar.gameObject.SetActive(false);
        //Hide shield
        invul_shield.gameObject.SetActive(false);
        //Set damage blood frame off
        damage_frame_UI.gameObject.SetActive(false);
        //Set death screen inacitive
        death_screen_UI.gameObject.SetActive(false);
        //Set pause off
        pause_UI.gameObject.SetActive(false);
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
        UpdatePause();
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

            //Get from player whether interaction is constant or instant
            if (current_interaction.instant_interact() == true)
            {
                //Hide bar
                interact_bar.gameObject.SetActive(false);
                //Reset Value in case
                interact_bar.value = 0.0f;
            }
            else
            {
                //Show Bar
                interact_bar.gameObject.SetActive(true);
                //Get values
                float current_value = current_interaction.interact_value();
                if (current_value <= 0)
                {
                    //Workaround
                    current_value = 0.01f;
                }
                float max_value = current_interaction.interact_value_max();
                //Get percentage
                float percent_value = current_value / max_value;
                //Set value
                interact_bar.value = percent_value;
            }
        }
        else
        {
            TMP_interact_text.text = "";
            TMP_interact_name.text = "";
            TMP_interact_desc.text = "";
            //Hide bar
            interact_bar.gameObject.SetActive(false);
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
            //Alert
            if (exposure_level > exposure_level_last)
            {
                //Increase alert
                exposure_alert(1);
            }
            else
            {
                //Decrease alert
                exposure_alert(2);
            }

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

    public void UpdatePause()
    {
        //Detect if game paused and player alive
        if (level.paused)
        {
            if (player.player_alive == true)
            {
                //Activate pause
                main_UI.gameObject.SetActive(false);
                exposure_alert_UI.gameObject.SetActive(false);
                interact_UI.gameObject.SetActive(false);
                pause_UI.gameObject.SetActive(true);
                death_screen_UI.gameObject.SetActive(false);
            }
        }
        if (!level.paused && player.player_alive == true)
        {
            UnityEngine.Debug.Log("Not Paused");
            //Deactivate pause
            main_UI.gameObject.SetActive(true);
            exposure_alert_UI.gameObject.SetActive(true);
            interact_UI.gameObject.SetActive(true);
            pause_UI.gameObject.SetActive(false);
            death_screen_UI.gameObject.SetActive(false);
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

    //Exposure Alert
    private void exposure_alert(int x)
    {
        //1 or 2
        //1 being increase and 2 being decreased

        switch (x)
        {
            case 1:
                TMP_exposure_increase_alert.text = exposure_alert_increase;
                TMP_exposure_decrease_alert.text = " ";
                //Start enumerator
                StartCoroutine(alert_cooldown(1));
                break;
            case 2:
                //Set exposure alerts off
                TMP_exposure_increase_alert.text = " ";
                TMP_exposure_decrease_alert.text = exposure_alert_decrease;
                //Start enumerator
                StartCoroutine(alert_cooldown(2));
                break;
            default:
                //You're not supposed to be here.
                break;

        }
    }

    public void show_hide_shield(int x)
    {
        switch (x)
        {
            case 0:
                //Hide shield
                invul_shield.gameObject.SetActive(false);
                break;
            case 1:
                //Show shield
                invul_shield.gameObject.SetActive(true);
                break;
            default:
                //Not supposed to be here
                break;
        }
    }

    public void player_death()
    {
        //Disable everything
        //Health, Stamina, Exposure
        damage_frame_UI.gameObject.SetActive(false);
        main_UI.gameObject.SetActive(false);
        exposure_alert_UI.gameObject.SetActive(false);
        interact_UI.gameObject.SetActive(false);
        pause_UI.gameObject.SetActive(false);

        //Set death screen active
        death_screen_UI.gameObject.SetActive(true);
        //Fade into existence
        StartCoroutine(fade_from_black());
        //Reset cursors
        level.UnlockCursor();
    }

    public void Damage_RedFrame()
    {
        //Show
        damage_frame_UI.gameObject.SetActive(true);
        //Fadeaway
        StartCoroutine (Red_FadeAway());
    }

    //Damage Frame Red fade away
    IEnumerator Red_FadeAway()
    {
        //Black screen first
        float elapsedTime = 0f;
        Color color = red_screen.color;

        while (elapsedTime < blood_screen_fade_duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0.6f, 0f, elapsedTime / blood_screen_fade_duration);
            color.a = alpha;
            red_screen.color = color;
            yield return null;
        }

        // Ensure final value is set
        color.a = 1f;
        red_screen.color = color;
        damage_frame_UI.gameObject.SetActive(false);
        //Done
    }

    //Exposure Alert cooldown
    IEnumerator alert_cooldown(int x)
    {
        int countdown = 3;
        while (countdown > 0)
        {
            yield return new WaitForSeconds(1f);
            countdown--;
        }

        if (x == 1)
        {
            TMP_exposure_increase_alert.text = " ";
        }
        
        if (x == 2)
        {
            TMP_exposure_decrease_alert.text = " ";
        }
    }

    //Fade into Black
    IEnumerator fade_from_black()
    {
        //Black screen first
        float elapsedTime = 0f;
        Color color = black_screen.color;

        while (elapsedTime < black_screen_fade_duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / black_screen_fade_duration);
            color.a = alpha;
            black_screen.color = color;
            yield return null;
        }

        // Ensure final value is set
        color.a = 1f;
        black_screen.color = color;
        //Done
    }
}
