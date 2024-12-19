using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class status_script : MonoBehaviour
{
    public Image hp_icon_img;
    public Image stamina_icon_img;
    public Image hunger_icon_img;
    public Image thirst_icon_img;
    public Image stress_icon_img;
    public Image radio_icon_img;

    public Color green_color = new Color(76/255f, 175 / 255f, 80 / 255f);
    public Color white_color = new Color(1f, 1f, 1f);
    public Color yellow_color = new Color(249 / 255f, 168 / 255f, 37 / 255f);
    public Color red_color = new Color(211 / 255f, 47 / 255f, 47 / 255f);
    int hp, stamina, hunger, thirst, stress, radio;
    public GameObject[] status_btn = new GameObject[6];
    public GameObject description_btn;
    public TMP_Text description_text;


    private void Start()
    {
        status_number_update();
        status_img_update_lv1();
    }
    public void statusUpdate()
    {
        status_number_update();
        status_img_update_lv1();
    }

    public void status_number_update()
    {
        DataManager.Instance.LoadGameData0();
        hp = DataManager.Instance.data0.status[0];
        stamina = DataManager.Instance.data0.status[1];
        hunger = DataManager.Instance.data0.status[2];
        thirst = DataManager.Instance.data0.status[3];
        stress = DataManager.Instance.data0.status[4];
        radio = DataManager.Instance.data0.status[5];
    }

    public static void status_produceTime(int time) //´ÜÀ§(ºÐ)
    {
        DataManager.Instance.LoadGameData0();
        int hp_t = DataManager.Instance.data0.status[0];
        int stamina_t = DataManager.Instance.data0.status[1];
        int hunger_t = DataManager.Instance.data0.status[2];
        int thirst_t = DataManager.Instance.data0.status[3];
        int stress_t = DataManager.Instance.data0.status[4];
        int radio_t = DataManager.Instance.data0.status[5];

        if(time >= 60)
        {
            DataManager.Instance.LoadGameData0();
            if (hunger_t <= 20) hp_t -= 2;
            if (thirst_t <= 20) hp_t -= 2;
            if (stamina_t <= 10) hp_t -= 4;
            if (radio_t <= 40) hp_t -= 4;
            if (stress_t <= 20) hp_t -= 2;

            hunger_t -= 4;
            thirst_t -= 6;
            stamina_t -= 3;
            stress_t -= 1;

            if (hp_t >= 100) hp_t = 100;
            if (stamina_t >= 100) stamina_t = 100;
            if (hunger_t >= 100) hunger_t = 100;
            if (thirst_t >= 100) thirst_t = 100;
            if (stress_t >= 100) stress_t = 100;
            if (radio_t >= 100) radio_t = 100;

            DataManager.Instance.data0.status[0] = hp_t;
            DataManager.Instance.data0.status[1] = stamina_t;
            DataManager.Instance.data0.status[2] = hunger_t;
            DataManager.Instance.data0.status[3] = thirst_t;
            DataManager.Instance.data0.status[4] = stress_t;
            DataManager.Instance.data0.status[5] = radio_t;

            DataManager.Instance.data0.timeCheck += 60;
            DataManager.Instance.SaveGameData0();

            status_produceTime(time - 60);
        }
        else if (time >= 30)
        {
            DataManager.Instance.LoadGameData0();
            if (hunger_t <= 20) hp_t -= 1;
            if (thirst_t <= 20) hp_t -= 1;
            if (stamina_t <= 10) hp_t -= 2;
            if (radio_t <= 40) hp_t -= 2;
            if (stress_t <= 20) hp_t -= 1;

            hunger_t -= 2;
            thirst_t -= 3;
            stamina_t -= 1;

            if (hp_t >= 100) hp_t = 100;
            if (stamina_t >= 100) stamina_t = 100;
            if (hunger_t >= 100) hunger_t = 100;
            if (thirst_t >= 100) thirst_t = 100;
            if (stress_t >= 100) stress_t = 100;
            if (radio_t >= 100) radio_t = 100;

            DataManager.Instance.data0.status[0] = hp_t;
            DataManager.Instance.data0.status[1] = stamina_t;
            DataManager.Instance.data0.status[2] = hunger_t;
            DataManager.Instance.data0.status[3] = thirst_t;
            DataManager.Instance.data0.status[4] = stress_t;
            DataManager.Instance.data0.status[5] = radio_t;

            DataManager.Instance.data0.timeCheck += 30;
            DataManager.Instance.SaveGameData0();

            status_produceTime(time - 30);
        }
        else if (time >= 10)
        {
            DataManager.Instance.LoadGameData0();
            if (stamina_t <= 10) hp_t -= 1;
            if (radio_t <= 40) hp_t -= 1;

            hunger_t -= 1;
            thirst_t -= 1;

            if (hp_t >= 100) hp_t = 100;
            if (stamina_t >= 100) stamina_t = 100;
            if (hunger_t >= 100) hunger_t = 100;
            if (thirst_t >= 100) thirst_t = 100;
            if (stress_t >= 100) stress_t = 100;
            if (radio_t >= 100) radio_t = 100;

            DataManager.Instance.data0.status[0] = hp_t;
            DataManager.Instance.data0.status[1] = stamina_t;
            DataManager.Instance.data0.status[2] = hunger_t;
            DataManager.Instance.data0.status[3] = thirst_t;
            DataManager.Instance.data0.status[4] = stress_t;
            DataManager.Instance.data0.status[5] = radio_t;

            DataManager.Instance.data0.timeCheck += 10;
            DataManager.Instance.SaveGameData0();

            status_produceTime(time - 10);
        }
        else if (time < 10)
        {
            DataManager.Instance.LoadGameData0();
            DataManager.Instance.data0.timeCheck += time;
            DataManager.Instance.SaveGameData0();
        }
    }

    public void status_img_update_lv1()
    {
        if (hp >= 50) { hp_icon_img.color = white_color; }
        if (hp < 50) { hp_icon_img.color = yellow_color; }
        if (hp < 20) { hp_icon_img.color = red_color; }

        if (stamina >= 80) { stamina_icon_img.color = green_color; }
        if (stamina < 80) { stamina_icon_img.color = white_color; }
        if (stamina < 50) { stamina_icon_img.color = yellow_color; }
        if (stamina < 20) { stamina_icon_img.color = red_color; }
        if (hunger >= 80) { hunger_icon_img.color = green_color; }
        if (hunger < 80) { hunger_icon_img.color = white_color; }
        if (hunger < 50) { hunger_icon_img.color = yellow_color; }
        if (hunger < 20) { hunger_icon_img.color = red_color; }
        if (thirst >= 80) { thirst_icon_img.color = green_color; }
        if (thirst < 80) { thirst_icon_img.color = white_color; }
        if (thirst < 50) { thirst_icon_img.color = yellow_color; }
        if (thirst < 20) { thirst_icon_img.color = red_color; }
        if (stress >= 80) { stress_icon_img.color = green_color; }
        if (stress < 80) { stress_icon_img.color = white_color; }
        if (stress < 50) { stress_icon_img.color = yellow_color; }
        if (stress < 20) { stress_icon_img.color = red_color; }

        if (radio >= 40) { radio_icon_img.color = white_color; }
        if (radio < 40) { radio_icon_img.color = red_color; }
    }

    public void status_btn_clicked_lv1(int index)
    {
        if (index == 0)
        {
            if (hp >= 50) { description_text.text = "Everything's fine"; }
            if (hp < 50) { description_text.text = "I feel a little sick"; }
            if (hp < 20) { description_text.text = "I could die if I don't receive any care"; }
        }

        if (index == 1)
        {
            if (stamina >= 80) { description_text.text = "I'm full of energy"; }
            if (stamina < 80) { description_text.text = "I'm not tired"; }
            if (stamina < 50) { description_text.text = "I'm a little sleepy"; }
            if (stamina < 20) { description_text.text = "My eyes are closing"; }
        }
        if (index == 2)
        {
            if (hunger >= 80) { description_text.text = "I don't need to eat"; }
            if (hunger < 80) { description_text.text = "I can have a snack or so"; }
            if (hunger < 50) { description_text.text = "I wish I could have some food"; }
            if (hunger < 20) { description_text.text = "I'm so hungry that I could die"; }
        }
        if (index == 3)
        {
            if (thirst >= 80) { description_text.text = "I don't need to drink"; }
            if (thirst < 80) { description_text.text = "Should I drink? Hmm, never mind"; }
            if (thirst < 50) { description_text.text = "I want something to drink"; }
            if (thirst < 20) { description_text.text = "I need to drink right now"; }
        }
        if (index == 4)
        {
            if (stress >= 80) { description_text.text = "What a beautiful day"; }
            if (stress < 80) { description_text.text = "Just another day"; }
            if (stress < 50) { description_text.text = "What am I doing here"; }
            if (stress < 20) { description_text.text = "Damn, I'm so depressed"; }
        }
        if (index == 5)
        {
            if (radio >= 40) { description_text.text = "I'm in the same condition as usual"; }
            if (radio < 40) { description_text.text = "My stomach is churning, and my head feels dizzy"; }
        }
        description_btn.SetActive(true);
    }
    public void description_btn_clicked()
    {
        description_btn.SetActive(false);
    }
}