using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class vehicle_timeCheck_script : MonoBehaviour
{

    public Image time_icon_img;
    public TMP_Text dayText_text;
    public TMP_Text timeText_text;

    // Start is called before the first frame update
    void Start()
    {
        timeCheck_update();
    }

    public void timeCheck_update()
    {
        DataManager.Instance.LoadGameData0();
        int time = DataManager.Instance.data0.timeCheck;
        float timeFill = ((float)(time % 1440)) / 1440;
        time_icon_img.fillAmount = timeFill;
        dayText_text.text = "Day: " + (time / 1440).ToString();
        timeText_text.text = string.Format("{0:D2}:{1:D2}", (time / 60) % 24, time % 60);
    }
}
