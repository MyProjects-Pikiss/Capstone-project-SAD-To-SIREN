using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class carUpgrade_mod_s_script : MonoBehaviour
{
    [SerializeField]
    public TextAsset moduleLevel_database;

    public Sprite[] module_img_s = new Sprite[12];

    public GameObject[] moduleButtons = new GameObject[9];
    public Image[] module_icons = new Image[9];

    public GameObject[] moduleBase_btn = new GameObject[12];
    public Image[] moduleBase_icons = new Image[12];

    public GameObject moduleBase;
    public TMP_Text module_pos_text;
    public string selected_m_name;
    public GameObject installation;
    public TMP_Text installation_text;
    public TMP_Text insDes_text;
    public int module_position;
    public int module_icon;

    public GameObject moduleExplanation;
    public TMP_Text exHead_text;
    public TMP_Text exBody_text;
    public int module_code;

    public GameObject UpDes;
    public TMP_Text UDHead_text;
    public TMP_Text UDBody_text;
    public int UDcheck;
    public string targetName;

    // Start is called before the first frame update
    void Start()
    {
        module_update();
    }

    public void module_position_clicked(int index)
    {
        DataManager.Instance.LoadGameData0();
        bool foundModule = false;
        for (int i = 0; i < 11; i++)
        {
            if (DataManager.Instance.data0.module[i].m_position == index + 1)
            {
                module_code = DataManager.Instance.data0.module[i].m_code;
                module_position = DataManager.Instance.data0.module[i].m_position;
                module_btn_clicked(index + 1);
                foundModule = true;
                break;
            }
        }
        if (!foundModule)
        {
            moduleBase_clicked(index + 1);
        }
    }
    public void moduleBase_clicked(int index)
    {
        module_pos_text.text = "Position number: " + index.ToString();
        moduleBase.SetActive(true);
        moduleExplanation.SetActive(false);

        for (int i = 0; i < moduleBase_btn.Length - 1; i++)
        {
            if (i == 8) continue;
            moduleBase_icons[i].sprite = module_img_s[i];
        }
        module_position = index;
    }
    public void install_clicked(int index)
    {
        installation.SetActive(true);
        for (int i = 0; i < 11; i++)
        {
            if (DataManager.Instance.data0.module[i].m_code == index)
            {
                selected_m_name = DataManager.Instance.data0.module[i].m_name;
                break;
            }
        }
        string[] lines2 = moduleLevel_database.text.Substring(0, moduleLevel_database.text.Length - 1).Split('\n');
        for (int i = 0; i < lines2.Length; i++)
        {
            string[] row = lines2[i].Split('\t');
            if (index == int.Parse(row[4]))
            {
                insDes_text.text = row[3];
            }
        }
        installation_text.text = "Position: " + module_position + "\nModule: " + selected_m_name;
        module_icon = index;
    }
    public void yes_clicked()
    {
        DataManager.Instance.LoadGameData0();
        installation.SetActive(false);
        for (int i = 0; i < 11; i++)
        {
            if (DataManager.Instance.data0.module[i].m_code == module_icon)
            {
                DataManager.Instance.data0.module[i].m_position = module_position;
            }
        }
        DataManager.Instance.SaveGameData0();
        module_update();
    }
    public void no_clicked()
    {
        installation.SetActive(false);
    }

    public void module_btn_clicked(int index)
    {
        moduleBase.SetActive(false);
        moduleExplanation.SetActive(true);
        DataManager.Instance.LoadGameData0();
        string[] lines = moduleLevel_database.text.Substring(0, moduleLevel_database.text.Length - 1).Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            string[] row = lines[i].Split('\t');
            if(module_code == int.Parse(row[4]))
            {
                exHead_text.text = "Position: " + index.ToString() + "\nModule:" + row[0] + "\nLv." + DataManager.Instance.data0.module[i].m_level;
                exBody_text.text = "[Description]\n" + row[3];
            }
        }
    }

    public void mUpgrade_btn_clicked()
    {
        DataManager.Instance.LoadGameData0();
        UDcheck = 0;
        UpDes.SetActive(true);
        string[] lines = moduleLevel_database.text.Substring(0, moduleLevel_database.text.Length - 1).Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            int x = DataManager.Instance.data0.module[i].m_level + 1;
            string[] row = lines[i].Split('\t');
            if (module_code == int.Parse(row[4]))
            {
                UDHead_text.text = "Position: " + module_position.ToString() + "\nModule:" + row[0] + "\nLv." + DataManager.Instance.data0.module[i].m_level + " -> " + "Lv." + x + "\n" + "Required: " + row[0] + "-Lv." + x;
                UDBody_text.text = "[Description]\n" + row[3];
            }
        }
    }
    public void mDestroy_btn_clicked()
    {
        UDcheck = 1;
        DataManager.Instance.LoadGameData0();
        UpDes.SetActive(true);
        string[] lines = moduleLevel_database.text.Substring(0, moduleLevel_database.text.Length - 1).Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            string[] row = lines[i].Split('\t');
            if (module_code == int.Parse(row[4]))
            {
                int x = DataManager.Instance.data0.module[i].m_level + 1;
                UDHead_text.text = "Position: " + module_position.ToString() + "\nModule:" + row[0] + "\nLv." + DataManager.Instance.data0.module[i].m_level + "\n" + "Are you sure to destroy?";
                UDBody_text.text = "[Description]\n" + row[3];
            }
        }
    }

    public void yes2_clicked()
    {
        DataManager.Instance.LoadGameData0();
        if (UDcheck == 0)
        {
            string[] lines = moduleLevel_database.text.Substring(0, moduleLevel_database.text.Length - 1).Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                string[] row = lines[i].Split('\t');
                if (module_code == int.Parse(row[4]))
                {
                    int x = DataManager.Instance.data0.module[i].m_level + 1;
                    targetName = row[0] + "-lv" + x;
                    if (database_func_script_s.FindAmountByName(DataManager.Instance.data0.itemAmount, targetName) > 0)
                    {
                        int n = database_func_script_s.FindAmountByName(DataManager.Instance.data0.itemAmount, targetName);
                        DataManager.Instance.data0.module[i].m_level = x;
                        database_func_script_s.ChangeAmountByName(DataManager.Instance.data0.itemAmount, targetName, n-1);
                    }
                }
            }
            moduleExplanation.SetActive(false);
            DataManager.Instance.SaveGameData0();
            UpDes.SetActive(false);
        }
        if (UDcheck == 1)
        {
            for (int i = 0; i < 11; i++)
            {
                if (DataManager.Instance.data0.module[i].m_code == module_code)
                {
                    DataManager.Instance.data0.module[i].m_level = 0;
                    DataManager.Instance.data0.module[i].m_position = 0;
                }
            }
            DataManager.Instance.SaveGameData0();
            module_icons[module_position-1].sprite = module_img_s[0];
            UpDes.SetActive(false);
        }
    }
    public void no2_clicked()
    {
        UpDes.SetActive(false);
    }

    public void module_update()
    {
        DataManager.Instance.LoadGameData0();
        for (int i = 0; i < 11; i++)
        {
            if (DataManager.Instance.data0.module[i].m_position != 0)
            {
                module_icons[DataManager.Instance.data0.module[i].m_position - 1].sprite = module_img_s[DataManager.Instance.data0.module[i].m_code];
            }
        }
        for (int i = 0; i < module_icons.Length; i++)
        {
            if (module_icons[i].sprite == null)
            {
                module_icons[i].sprite = module_img_s[0];
            }
        }
    }
}
