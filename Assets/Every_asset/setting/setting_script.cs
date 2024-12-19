using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class setting_script : MonoBehaviour
{
    public GameObject setting_s;
    public GameObject setList;
    public GameObject buttonA;
    public GameObject tutorial_s;
    public GameObject imgList_s;
    public bool settingCheck;

    public Texture2D[] status_array;
    public Texture2D[] vehicle_array;
    public Texture2D[] world_array;
    public Texture2D[] field_array;
    public Texture2D[] drone_array;

    public Image tutorial_img;
    public Texture2D[] selectedImageArray;
    private int tutorialIndex;
    public TMP_Text pageNum;

    public GameObject SL;
    public GameObject SLback;
    public bool SLCheck;
    public TMP_Text[] dataText;
    public Button[] dataBTN;
    public TMP_Text data0_text;
    public TMP_Text data1_text;
    public TMP_Text data2_text;
    public TMP_Text data3_text;

    void Start()
    {
        settingCheck = true;
        settingOnOff();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            settingOnOff();
        }
    }

    public void settingOnOff()
    {
        if (!settingCheck)
        {
            setting_s.SetActive(true);
            settingCheck = true;
        }
        else
        {
            setting_s.SetActive(false);
            settingCheck = false;
            tutorial_s.SetActive(false);
            imgList_s.SetActive(false);
            buttonA.SetActive(true);
        }
    }
    public void goMainmenu_clicked()
    {
        SceneManager.LoadScene("Mainmenu_scen");
    }

    public void tutorial_clicked()
    {
        buttonA.SetActive(false);
        tutorial_s.SetActive(true);
    }
    public void returnToA_clicked()
    {
        buttonA.SetActive(true);
        tutorial_s.SetActive(false);
        imgList_s.SetActive(false);
    }
    public void status_clicked()
    {
        imgList_s.SetActive(true);
        tutorialIndex = 0;
        selectedImageArray = status_array;
        ShowImageAtIndex(tutorialIndex);
    }
    public void vehicle_clicked()
    {
        imgList_s.SetActive(true);
        tutorialIndex = 0;
        selectedImageArray = vehicle_array;
        ShowImageAtIndex(tutorialIndex);
    }
    public void world_clicked()
    {
        imgList_s.SetActive(true);
        tutorialIndex = 0;
        selectedImageArray = world_array;
        ShowImageAtIndex(tutorialIndex);
    }
    public void field_clicked()
    {
        imgList_s.SetActive(true);
        tutorialIndex = 0;
        selectedImageArray = field_array;
        ShowImageAtIndex(tutorialIndex);
    }
    public void drone_clicked()
    {
        imgList_s.SetActive(true);
        tutorialIndex = 0;
        selectedImageArray = drone_array;
        ShowImageAtIndex(tutorialIndex);
    }
    void ShowImageAtIndex(int index)
    {
        if (index >= 0 && index < selectedImageArray.Length)
        {
            tutorial_img.sprite = Sprite.Create(selectedImageArray[index], new Rect(0, 0, selectedImageArray[index].width, selectedImageArray[index].height), new Vector2(0.5f, 0.5f));
        }
        pageNum.text = (tutorialIndex + 1).ToString() + "/" + (selectedImageArray.Length).ToString();
    }
    public void ShowNextImage()
    {
        tutorialIndex = (tutorialIndex + 1) % selectedImageArray.Length;
        ShowImageAtIndex(tutorialIndex);
    }
    public void ShowPreviousImage()
    {
        tutorialIndex = (tutorialIndex - 1 + selectedImageArray.Length) % selectedImageArray.Length;
        ShowImageAtIndex(tutorialIndex);
    }
    public void ShowOut()
    {
        imgList_s.SetActive(false);
    }

    public void goSL_clicked()
    {
        SL.SetActive(true);
        setList.SetActive(false);
        SLCheck = true;
        dataColorChange();
        dataLoad();
    }
    public void SLback_clicked()
    {
        SL.SetActive(false);
        setList.SetActive(true);
    }
    public void dataColorChange()
    {
        if (SLCheck)
        {
            for (int i = 1; i < 4; i++)
            {
                dataBTN[i].image.color = Color.green;
                dataText[i].color = Color.green;
            }
        }
        else
        {
            for (int i = 1; i < 4; i++)
            {
                dataBTN[i].image.color = Color.cyan;
                dataText[i].color = Color.cyan;
            }
        }
    }
    public void save_clicked()
    {
        SLCheck = false;
        dataColorChange();
    }
    public void load_clicked()
    {
        SLCheck = true;
        dataColorChange();
    }
    public void dataLoad()
    {
        DataManager.Instance.LoadGameData0();
        DataManager.Instance.LoadGameData1();
        DataManager.Instance.LoadGameData2();
        DataManager.Instance.LoadGameData3();
        data0_text.text = "Day: " + (DataManager.Instance.data0.timeCheck / 1440).ToString() + " | Time: " + (DataManager.Instance.data0.timeCheck / 60 % 24).ToString();
        data1_text.text = "Day: " + (DataManager.Instance.data1.timeCheck / 1440).ToString() + " | Time: " + (DataManager.Instance.data1.timeCheck / 60 % 24).ToString();
        data2_text.text = "Day: " + (DataManager.Instance.data2.timeCheck / 1440).ToString() + " | Time: " + (DataManager.Instance.data2.timeCheck / 60 % 24).ToString();
        data3_text.text = "Day: " + (DataManager.Instance.data3.timeCheck / 1440).ToString() + " | Time: " + (DataManager.Instance.data3.timeCheck / 60 % 24).ToString();
    }
    public void BSL()
    {
        if (!SLCheck)
        {
            DataManager.Instance.LoadGameData0();
            DataManager.Instance.LoadGameData1();
            DataManager.Instance.LoadDataNum();
            string A = DataManager.Instance.data0.dataNumber.ToString();
            DataManager.Instance.data1 = DataManager.Instance.data0;
            DataManager.Instance.data1.dataNumber = DataManager.Instance.dataNumber.NewNumFind();
            string B = DataManager.Instance.data1.dataNumber.ToString();
            DataManager.Instance.dataNumber.Bdata = DataManager.Instance.data1.dataNumber;
            database_func_script_s.CopyFilesWithPartialChange("savedWorldMap", A, B);
            DataManager.Instance.SaveGameData1();
            DataManager.Instance.SaveDataNum();
            dataLoad();
        }
        else
        {
            DataManager.Instance.LoadGameData0();
            DataManager.Instance.LoadGameData1();
            DataManager.Instance.LoadDataNum();
            DataManager.Instance.data0 = DataManager.Instance.data1;
            DataManager.Instance.dataNumber.Adata = DataManager.Instance.dataNumber.Bdata;
            DataManager.Instance.SaveGameData0();
            DataManager.Instance.SaveDataNum();
            SceneManager.LoadScene("vehicle_scen");
        }
    }
    public void CSL()
    {
        if (!SLCheck)
        {
            DataManager.Instance.LoadGameData0();
            DataManager.Instance.LoadGameData2();
            DataManager.Instance.LoadDataNum();
            string A = DataManager.Instance.data0.dataNumber.ToString();
            DataManager.Instance.data2 = DataManager.Instance.data0;
            DataManager.Instance.data2.dataNumber = DataManager.Instance.dataNumber.NewNumFind();
            string B = DataManager.Instance.data2.dataNumber.ToString();
            DataManager.Instance.dataNumber.Cdata = DataManager.Instance.data2.dataNumber;
            database_func_script_s.CopyFilesWithPartialChange("savedWorldMap", A, B);
            DataManager.Instance.SaveGameData2();
            DataManager.Instance.SaveDataNum();
            dataLoad();
        }
        else
        {
            DataManager.Instance.LoadGameData0();
            DataManager.Instance.LoadGameData2();
            DataManager.Instance.LoadDataNum();
            DataManager.Instance.data0 = DataManager.Instance.data2;
            DataManager.Instance.dataNumber.Adata = DataManager.Instance.dataNumber.Cdata;
            DataManager.Instance.SaveGameData0();
            DataManager.Instance.SaveDataNum();
            SceneManager.LoadScene("vehicle_scen");
        }
    }
    public void DSL()
    {
        if (!SLCheck)
        {
            DataManager.Instance.LoadGameData0();
            DataManager.Instance.LoadGameData3();
            DataManager.Instance.LoadDataNum();
            string A = DataManager.Instance.data0.dataNumber.ToString();
            DataManager.Instance.data3 = DataManager.Instance.data0;
            DataManager.Instance.data3.dataNumber = DataManager.Instance.dataNumber.NewNumFind();
            string B = DataManager.Instance.data3.dataNumber.ToString();
            DataManager.Instance.dataNumber.Ddata = DataManager.Instance.data3.dataNumber;
            database_func_script_s.CopyFilesWithPartialChange("savedWorldMap", A, B);
            DataManager.Instance.SaveGameData3();
            DataManager.Instance.SaveDataNum();
            dataLoad();
        }
        else
        {
            DataManager.Instance.LoadGameData0();
            DataManager.Instance.LoadGameData3();
            DataManager.Instance.LoadDataNum();
            DataManager.Instance.data0 = DataManager.Instance.data3;
            DataManager.Instance.dataNumber.Adata = DataManager.Instance.dataNumber.Ddata;
            DataManager.Instance.SaveGameData0();
            DataManager.Instance.SaveDataNum();
            SceneManager.LoadScene("vehicle_scen");
        }
    }
}