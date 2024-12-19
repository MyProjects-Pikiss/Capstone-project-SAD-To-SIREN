using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Mainmenu_script : MonoBehaviour
{
    public Sprite[] droneSprites = new Sprite[5];

    public GameObject Newgame_btn;
    public GameObject Main_btn_s;
    public GameObject Newgame_u_btn_s;
    public GameObject Continue_btn;
    public GameObject Tutorial_btn;
    public GameObject Back_btn;
    public GameObject End_btn;

    public GameObject Continue_u_btn_s;
    public GameObject Continue_btn2;
    public GameObject Load_btn;
    public GameObject back_btn2;

    public GameObject loadData_s;

    public TMP_Text data0_text;
    public TMP_Text data1_text;
    public TMP_Text data2_text;
    public TMP_Text data3_text;

    public TextAsset itemAmount_database;
    public TextAsset item_s_database;
    public TextAsset moduleLevel_database;
    public TextAsset traitLevel_database;

    void Start()
    {
        if (!File.Exists(Application.persistentDataPath + "/" + "GameDataNum.json"))
        {
            DataManager.Instance.LoadDataNum();
            DataManager.Instance.dataNumber.Adata = -1;
            DataManager.Instance.dataNumber.Bdata = -1;
            DataManager.Instance.dataNumber.Cdata = -1;
            DataManager.Instance.dataNumber.Ddata = -1;
            DataManager.Instance.SaveDataNum();
        }
    }

    public void Newgame_btn_clicked()
    {
        Newgame_u_btn_s.SetActive(true);
        Main_btn_s.SetActive(false);
    }
    public void Continue_btn_clicked()
    {
        Main_btn_s.SetActive(false);
        Continue_u_btn_s.SetActive(true);
    }
    public void Back_btn_clicked()
    {
        Newgame_u_btn_s.SetActive(false);
        Main_btn_s.SetActive(true);
    }
    public void End_btn_clicked()
    {
        Application.Quit();
    }

    public void Continue_btn2_clicked()
    {
        SceneManager.LoadScene("vehicle_scen");
    }
    public void Load_btn_clicked()
    {
        loadData_s.SetActive(true);
        dataLoad();
    }
    public void Back_btn2_clicked()
    {
        Main_btn_s.SetActive(true);
        Continue_u_btn_s.SetActive(false);
    }

    public void loadBack_clicked()
    {
        loadData_s.SetActive(false);
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
    public void Tutorial_btn_clicked()
    {
        first_data_save();
        SceneManager.LoadScene("Tutorial_scen");
    }
    public void Start_btn_clicked()
    {
        first_data_save();
        SceneManager.LoadScene("vehicle_scen");
    }
    public void first_data_save()
    {
        DataManager.Instance.LoadDataNum();
        DataManager.Instance.data0.dataNumber = DataManager.Instance.dataNumber.NewNumFind();
        DataManager.Instance.dataNumber.Adata = DataManager.Instance.data0.dataNumber;
        DataManager.Instance.SaveDataNum();
        DataManager.Instance.data0.timeCheck = 0;
        DataManager.Instance.data0.itemAmount = new List<ItemAmount>(); // √ ±‚»≠
        DataManager.Instance.data0.questLine = new List<int>();
        DataManager.Instance.data0.status = new List<int>();
        DataManager.Instance.data0.module = new List<Module>();
        DataManager.Instance.data0.trait = new List<Trait>();
        DataManager.Instance.data0.drone = new List<Drone>();

        for (int i = 0; i < 5; i++)
        {
            Drone newDrone = new Drone();
            newDrone.d_code = i;
            newDrone.d_itemAmount = new List<ItemAmount>();
            newDrone.d_magazine = new List<magazine>();
            DataManager.Instance.data0.drone.Add(newDrone);
        }
        string[] lines = item_s_database.text.Substring(0, item_s_database.text.Length - 1).Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            string[] row = lines[i].Split('\t');
            DataManager.Instance.data0.itemAmount.Add(new ItemAmount { name = row[2], amount = 0 });
            for (int x = 0; x < 5; x++)
            {
                DataManager.Instance.data0.drone[x].d_itemAmount.Add(new ItemAmount { name = row[2], amount = 0 });
            }
        }
        string[] lines2 = itemAmount_database.text.Substring(0, itemAmount_database.text.Length - 1).Split('\n');
        for (int i = 0; i < lines2.Length; i++)
        {
            string[] row = lines2[i].Split('\t');
            database_func_script_s.ChangeAmountByName(DataManager.Instance.data0.itemAmount, row[0], int.Parse(row[1]));
        }
        for (int i = 0; i < 6; i++)
        {
            DataManager.Instance.data0.status.Add(100);
        }
        string[] lines3 = moduleLevel_database.text.Substring(0, moduleLevel_database.text.Length - 1).Split('\n');
        for (int i = 0; i < lines3.Length; i++)
        {
            string[] row = lines3[i].Split('\t');
            DataManager.Instance.data0.module.Add(new Module { m_name = row[0], m_level = int.Parse(row[1]), m_position = int.Parse(row[2]), m_code = int.Parse(row[4])});
        }
        string[] lines4 = traitLevel_database.text.Substring(0, traitLevel_database.text.Length - 1).Split('\n');
        for (int i = 0; i < lines4.Length; i++)
        {
            string[] row = lines4[i].Split('\t');
            DataManager.Instance.data0.trait.Add(new Trait { t_na_name = row[0], t_level = int.Parse(row[1]), t_po_name = row[2]});
        }
        DataManager.Instance.SaveGameData0();
    }

    public void MASL()
    {
        DataManager.Instance.LoadGameData0();
        SceneManager.LoadScene("vehicle_scen");
    }
    public void MBSL()
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
    public void MCSL()
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
    public void MDSL()
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
