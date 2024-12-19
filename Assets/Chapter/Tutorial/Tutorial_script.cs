using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Tutorial_script : MonoBehaviour
{
    public GameObject begin_btn;
    public GameObject day_start;
    public TMP_Text dayText;
    public GameObject TypingManager_script;
    public GameObject csv_manager_script;
    public GameObject Textchat;
    private csv_manager_script tutorial_csv_Script; //텍스트

    public GameObject img_canv2;
    public Image img_canv;
    public Texture2D[] tutorial_img_s;

    void Start()
    {
        DataManager.Instance.LoadGameData0();
        dayText.text = "Day " + DataManager.Instance.data0.timeCheck / 1440;
        tutorial_csv_Script = csv_manager_script.GetComponent<csv_manager_script>(); //텍스트
        tutorial_csv_Script.SetCsvFileName("Tutorial_chat_data"); //텍스트
        day_start.SetActive(true);
        Typing_manager_script.instance.OnSentenceReached += CheckDesiredSentence; //텍스트
    }

    public void Begin_btn_clicked()
    {
        Textchat.SetActive(true);
        img_canv2.SetActive(true);
        day_start.SetActive(false);
    }
    void ShowImageAtIndex(int index)
    {
        if (index >= 0 && index < tutorial_img_s.Length)
        {
            img_canv.sprite = Sprite.Create(tutorial_img_s[index], new Rect(0, 0, tutorial_img_s[index].width, tutorial_img_s[index].height), new Vector2(0.5f, 0.5f));
        }
    }

    void CheckDesiredSentence(int sentenceNumber) //case n: n문장 시작시
    {
        switch (sentenceNumber)
        {
            case 0:
                ShowImageAtIndex(0);
                break;
            case 3:
                ShowImageAtIndex(1);
                break;
            case 4:
                SceneManager.LoadScene("vehicle_scen");
                break;

            default:
                break;
        }

    }
}
