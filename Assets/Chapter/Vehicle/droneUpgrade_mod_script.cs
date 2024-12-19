using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Linq;
using TMPro;

public class droneUpgrade_mod_script : MonoBehaviour
{
    [SerializeField]
    public TextAsset drone_item_database;
    [SerializeField]
    public TextAsset item_s_database;

    public GameObject dronePrefab;
    public GameObject gearPrefab;
    public Transform droneParent;
    public Transform gearParent;
    public int numberOfDrones;
    public int numberOfGears;
    private List<GameObject> droneList1 = new List<GameObject>();
    private List<GameObject> gearList1 = new List<GameObject>();
    private List<string> droneList;
    private List<string> gearList;

    public GameObject checkView;
    public GameObject drone_yes;
    public TMP_Text checkText;
    public string droneSelect;
    public GameObject[] dronePosition = new GameObject[5];
    public int position;
    public int position2;
    public int garageLevel;

    public GameObject droneView;
    public TMP_Text droneName;
    public TMP_Text droneHP;
    public TMP_Text droneArmor;
    public TMP_Text droneSpeed;
    public TMP_Text droneWeight;
    public TMP_Text droneAp;

    public GameObject[] droneGears = new GameObject[9];
    public int[] gearSpace = new int[9];
    public int gearPosition;
    public string gearSelect;
    public int YNcheck;
    public GameObject gearInfo;
    public GameObject gearReload_btn;
    public GameObject gearReload;
    public TMP_Text gearInfo_name;
    public TMP_Text gearInfo_description;
    public int gearPosition2;
    public GameObject d_inventory;

    public GameObject reloadPrefab;
    public Transform reloadParent;
    private List<GameObject> reloadList1 = new List<GameObject>();

    public GameObject drone_view;

    public GameObject repair_view;
    public Slider repair_slider;

    // Start is called before the first frame update
    void Start()
    {
        droneList = new List<string>();
        gearList = new List<string>();
        position = -1;
        gearPosition = -1;
        gearPosition2 = -1;
        DataManager.Instance.LoadGameData0();
        garageLevel = database_func_script_s.FindM_levelByName(DataManager.Instance.data0.module, "garage");
        for (int i = 0; i < 5; i++)
        {
            if (i < garageLevel + 2)
            {
                dronePosition[i].SetActive(true);
            }
            else
            {
                dronePosition[i].SetActive(false);
            }
        }

        string[] lines = drone_item_database.text.Substring(0, drone_item_database.text.Length - 1).Split('\n');

        for (int i = 0; i < lines.Length; i++)
        {
            string[] row = lines[i].Split('\t');
            string No_s = row[0];
            string type_g = row[1];
            string name = row[2];
            string amount_s = database_func_script_s.FindAmountByName(DataManager.Instance.data0.itemAmount, name).ToString();
            if (type_g == "drone")
            {
                if (int.Parse(amount_s) > 0)
                {
                    droneList.Add(name);
                }
            }
            else
            {
                if (int.Parse(amount_s) > 0)
                {
                    gearList.Add(name);
                }
            }
        }
        ClearButtons();
        ChangedroneCount(droneList.Count);
        FillDroneListsWithData(droneList);
        ChangeGearCount(gearList.Count);
        FillGearListsWithData(gearList);
    }

    public void gearListUpdate()
    {
        Start();
    }

    public void drone_yes_clicked()
    {
        string[] lines2 = item_s_database.text.Substring(0, item_s_database.text.Length - 1).Split('\n');
        if (YNcheck == 0)
        {
            for (int i = 0; i < lines2.Length; i++)
            {
                string[] row = lines2[i].Split('\t');
                string No_s = row[0];
                string type_c = row[1];
                string name = row[2];
                if (name == droneSelect)
                {
                    int d_hp = int.Parse(row[7]);
                    int d_armor = int.Parse(row[9]);
                    int d_speed = int.Parse(row[11]);
                    int d_maxWeight = int.Parse(row[13]);
                    int ap = int.Parse(row[15]);

                    DataManager.Instance.data0.drone[position].d_info = int.Parse(No_s);
                    DataManager.Instance.data0.drone[position].d_name = name;
                    DataManager.Instance.data0.drone[position].d_max_hp = d_hp;
                    DataManager.Instance.data0.drone[position].d_hp = d_hp;
                    DataManager.Instance.data0.drone[position].d_armor = d_armor;
                    DataManager.Instance.data0.drone[position].d_speed = d_speed;
                    DataManager.Instance.data0.drone[position].d_max_weight = d_maxWeight;
                    DataManager.Instance.data0.drone[position].d_weight = 0;
                    DataManager.Instance.data0.drone[position].d_ap = ap;
                    DataManager.Instance.data0.drone[position].d_gear = new List<string>();
                    DataManager.Instance.data0.drone[position].d_magazine = new List<magazine>();
                    for (int y = 0; y < 9; y++)
                    {
                        DataManager.Instance.data0.drone[position].d_gear.Add("0");
                    }
                    int x = database_func_script_s.FindAmountByName(DataManager.Instance.data0.itemAmount, droneSelect);
                    database_func_script_s.ChangeAmountByName(DataManager.Instance.data0.itemAmount, droneSelect, x - 1);
                }
            }
        }
        if(YNcheck == 1)
        {
            DataManager.Instance.data0.drone[position2].d_gear[gearPosition] = gearSelect;
            for (int i = 0; i < lines2.Length; i++)
            {
                string[] row = lines2[i].Split('\t');
                if (row[2] == gearSelect)
                {
                    magazine m_gear = new magazine();
                    m_gear.weapon = gearSelect;
                    m_gear.ammunition = "0";
                    m_gear.max = int.Parse(row[7]);
                    m_gear.current = 0;
                    DataManager.Instance.data0.drone[position2].d_magazine.Add(m_gear);
                }
            }
            int x = database_func_script_s.FindAmountByName(DataManager.Instance.data0.itemAmount, gearSelect);
            database_func_script_s.ChangeAmountByName(DataManager.Instance.data0.itemAmount, gearSelect, x - 1);
        }

        for (int i = 0; i < 5; i++)
        {
            if (i < garageLevel + 2)
            {
                dronePosition[i].SetActive(true);
            }
            else
            {
                dronePosition[i].SetActive(false);
            }
        }
        DataManager.Instance.SaveGameData0();
        checkView.SetActive(false);
        gearListUpdate();
    }
    public void drone_no_clicked()
    {
        for (int i = 0; i < 5; i++)
        {
            if (i < garageLevel + 2)
            {
                dronePosition[i].SetActive(true);
            }
            else
            {
                dronePosition[i].SetActive(false);
            }
        }
        drone_yes.SetActive(true);
        checkView.SetActive(false);
    }
    public void ChangedroneCount(int newDroneCount)
    {
        numberOfDrones = newDroneCount;
        GenerateButtons();
    }
    public void ChangeGearCount(int newGearCount)
    {
        numberOfGears = newGearCount;
        GenerateGearButtons();
    }

    public void ButtonClicked(int index)
    {
        DataManager.Instance.LoadGameData0();
        droneSelect = droneList[index];
        YNcheck = 0;
        checkView.SetActive(true);
        checkText.text = "Do you want to deploy?\n" + "[" + droneSelect + "]\n" + "Position: " + (position + 1);
        if (position == -1)
        {
            checkText.text = "You have to choose empty drone position";
            drone_yes.SetActive(false);
        }
        for (int i = 0; i < 5; i++)
        {
            dronePosition[i].SetActive(false);
        }
    }
    public void ButtonClicked2(int index)
    {
        if (gearPosition == -1)
        {
            checkView.SetActive(true);
            checkText.text = "You have to choose gear position";
            drone_yes.SetActive(false);
        }
        else
        {
            DataManager.Instance.LoadGameData0();
            gearSelect = gearList[index];
            checkView.SetActive(true);
            YNcheck = 1;
            string gearPosiName = FindGear(gearPosition);
            string[] lines = drone_item_database.text.Substring(0, drone_item_database.text.Length - 1).Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                string[] row = lines[i].Split('\t');
                string name = row[2];
                if (name == gearSelect)
                {
                    if (int.Parse(row[gearPosition + 3]) == 1)
                    {
                        checkText.text = "Do you want to attach?\n" + "[" + gearSelect + "]\n" + "Position: " + gearPosiName;
                    }
                    else
                    {
                        checkText.text = "Cannot be attached in the wrong position\n" + "[" + gearSelect + "]\n" + "Position: " + gearPosiName;
                        drone_yes.SetActive(false);
                    }
                }
            }
        }
    }

    public void dronePosition_clicked(int index)
    {
        DataManager.Instance.LoadGameData0();
        if (DataManager.Instance.data0.drone[index].d_name == "")
        {
            position = index;
            droneView.SetActive(false);
        }
        else
        {
            position = -1;
            position2 = index;
            droneView.SetActive(true);
            droneName.text = "[" + DataManager.Instance.data0.drone[index].d_name + "]";
            droneHP.text = "HP: " + DataManager.Instance.data0.drone[index].d_hp + "/" + DataManager.Instance.data0.drone[index].d_max_hp;
            droneArmor.text = "Armor: " + DataManager.Instance.data0.drone[index].d_armor;
            droneSpeed.text = "Speed: " + DataManager.Instance.data0.drone[index].d_speed;
            droneWeight.text = "Weight: " + DataManager.Instance.data0.drone[index].d_weight + "/" + DataManager.Instance.data0.drone[index].d_max_weight;
            droneAp.text = "AP: " + DataManager.Instance.data0.drone[index].d_ap;

            string[] lines3 = drone_item_database.text.Substring(0, drone_item_database.text.Length - 1).Split('\n');
            for (int i = 0; i < lines3.Length; i++)
            {
                string[] row = lines3[i].Split('\t');
                if (DataManager.Instance.data0.drone[index].d_name == row[2])
                {
                    for (int x = 0; x < 9; x++)
                    {
                        gearSpace[x] = int.Parse(row[x+3]);
                    }
                }
            }
            for (int i = 0; i < 9; i++)
            {
                if (gearSpace[i] == 1)
                {
                    droneGears[i].SetActive(true);

                }
                else
                {
                    droneGears[i].SetActive(false);
                }
            }
        }
    }

    public void droneGear_clicked(int index)
    {
        DataManager.Instance.LoadGameData0();
        if (DataManager.Instance.data0.drone[position2].d_gear[index] == "0")
        {
            gearPosition = index;
            gearReload_btn.SetActive(false);
        }
        else
        {
            gearPosition2 = index;
            gearInfo.SetActive(true);
            gearReload_btn.SetActive(true);
            gearInfo_name.text = DataManager.Instance.data0.drone[position2].d_gear[index];
            string targetKey = DataManager.Instance.data0.drone[position2].d_gear[index];

            magazine foundMagazine = DataManager.Instance.data0.drone[position2].d_magazine.FirstOrDefault(mag => mag.weapon == targetKey);

            string t_name = foundMagazine.ammunition;
            if (t_name == "0")
            {
                t_name = "Not loaded";
            }
            gearInfo_description.text = t_name + "\n Magazine: " + foundMagazine.current + "/" + foundMagazine.max;
        }

        DataManager.Instance.SaveGameData0();
    }

    public void droneReload_clicked()
    {
        DataManager.Instance.LoadGameData0();
        string gear = DataManager.Instance.data0.drone[position2].d_gear[gearPosition2];
        string type = "0";
        // 정규 표현식을 사용하여 [ ] 사이의 문자열을 추출
        string pattern = @"\[(.*?)\]";
        MatchCollection matches = Regex.Matches(gear, pattern);
        foreach (Match match in matches)
        {
            type = match.Groups[1].Value;
        }
        List<ItemAmount> filteredItems = DataManager.Instance.data0.drone[position2].d_itemAmount
            .Where(item => item.name.Contains(type) && item.amount >= 1)
            .ToList();
        gearReload.SetActive(true);
        GenerateReloadButtons(filteredItems);
    }

    public void droneDettach_clicked()
    {
        DataManager.Instance.LoadGameData0();
        int x = database_func_script_s.FindAmountByName(DataManager.Instance.data0.itemAmount, DataManager.Instance.data0.drone[position2].d_gear[gearPosition2]);
        database_func_script_s.ChangeAmountByName(DataManager.Instance.data0.itemAmount, DataManager.Instance.data0.drone[position2].d_gear[gearPosition2], x + 1);
        string gearToRemove = DataManager.Instance.data0.drone[position2].d_gear[gearPosition2];
        DataManager.Instance.data0.drone[position2].d_magazine.RemoveAll(mag => mag.weapon == gearToRemove);
        DataManager.Instance.data0.drone[position2].d_gear[gearPosition2] = "0";
        DataManager.Instance.SaveGameData0();
        gearInfo.SetActive(false);
        gearListUpdate();
    }

    public void droneUndeploy_clicked()
    {
        DataManager.Instance.LoadGameData0();
        int x = database_func_script_s.FindAmountByName(DataManager.Instance.data0.itemAmount, DataManager.Instance.data0.drone[position2].d_name);
        database_func_script_s.ChangeAmountByName(DataManager.Instance.data0.itemAmount, DataManager.Instance.data0.drone[position2].d_name, x + 1);

        Drone newDrone = new Drone();
        newDrone.d_code = DataManager.Instance.data0.drone[position2].d_code;
        newDrone.d_itemAmount = new List<ItemAmount>();
        newDrone.d_magazine = new List<magazine>();

        string[] lines = item_s_database.text.Substring(0, item_s_database.text.Length - 1).Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            string[] row = lines[i].Split('\t');
            newDrone.d_itemAmount.Add(new ItemAmount { name = row[2], amount = 0 });
        }

        DataManager.Instance.data0.drone[position2] = newDrone;
        DataManager.Instance.SaveGameData0();
        droneView.SetActive(false);
        gearListUpdate();
    }

    public void toDroneInventory()
    {
        d_inventory.SetActive(true);
    }

    private string FindGear(int index)
    {
        switch (index)
        {
            case 0:
                return "Right Hand";
            case 1:
                return "Left Hand";
            case 2:
                return "Body";
            case 3:
                return "Head";
            case 4:
                return "Leg";
            case 5:
                return "Back";
            case 6:
                return "Artifact1";
            case 7:
                return "Artifact2";
            case 8:
                return "Artifact3";
            default:
                Debug.LogError("Invalid Gear Position: " + index);
                return "Unknown";
        }
    }

    private void ClearButtons()
    {
        foreach (GameObject button in droneList1)
        {
            Destroy(button);
        }
        droneList1.Clear();

        foreach (GameObject button in gearList1)
        {
            Destroy(button);
        }
        gearList1.Clear();
    }
    private void ClearButtons2()
    {
        foreach (GameObject button in reloadList1)
        {
            Destroy(button);
        }
        reloadList1.Clear();
    }

    private void FillDroneListsWithData(List<string> droneList)
    {
        for (int i = 0; i < droneList1.Count; i++)
        {
            if (i < droneList.Count)
            {
                string name = droneList[i];
                i = i * 2;
                droneList1[i].GetComponentInChildren<TMP_Text>().text = name;
                i = i / 2;
            }
        }
    }
    private void FillGearListsWithData(List<string> gearList)
    {
        for (int i = 0; i < gearList1.Count; i++)
        {
            if (i < gearList.Count)
            {
                string name = gearList[i];
                i = i * 2;
                gearList1[i].GetComponentInChildren<TMP_Text>().text = name;
                i = i / 2;
            }
        }
    }

    private void GenerateButtons()
    {
        RectTransform contentRect = droneParent.GetComponent<RectTransform>();
        contentRect.pivot = new Vector2(0, 1);

        // 버튼 높이를 가져와서 버튼 개수만큼 더한 높이를 계산
        int totalButtonHeight = numberOfDrones * 50;

        // 버튼 시작 Y 포지션 값
        int startYPosition = totalButtonHeight;

        // 지정한 개수만큼 버튼 생성
        for (int i = 0; i < numberOfDrones; i++)
        {
            int buttonIndex = i;
            GameObject buttonInstance = Instantiate(dronePrefab, droneParent);
            buttonInstance.name = "Button " + i;
            buttonInstance.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, startYPosition / 2 - i * 50);
            buttonInstance.GetComponent<Button>().onClick.AddListener(() => ButtonClicked(buttonIndex));
            TMP_Text buttonText1 = buttonInstance.GetComponentInChildren<TMP_Text>();
            droneList1.Add(buttonText1.gameObject);
            droneList1.Add(buttonInstance);
        }

        // Content 영역의 크기를 조정
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, totalButtonHeight + 50);
    }
    private void GenerateGearButtons()
    {
        RectTransform contentRect = gearParent.GetComponent<RectTransform>();
        contentRect.pivot = new Vector2(0, 1);

        // 버튼 너비를 가져와서 버튼 개수만큼 더한 너비를 계산
        int totalButtonWidth = numberOfGears * 250;

        // 버튼 시작 X 포지션 값
        int startXPosition = totalButtonWidth;

        // 지정한 개수만큼 버튼 생성
        for (int i = 0; i < numberOfGears; i++)
        {
            int buttonIndex = i;
            GameObject buttonInstance = Instantiate(gearPrefab, gearParent);
            buttonInstance.name = "Button " + i;
            buttonInstance.GetComponent<RectTransform>().anchoredPosition = new Vector2(startXPosition / -2 + i * 250, 0);
            buttonInstance.GetComponent<Button>().onClick.AddListener(() => ButtonClicked2(buttonIndex));
            TMP_Text buttonText1 = buttonInstance.GetComponentInChildren<TMP_Text>();
            gearList1.Add(buttonText1.gameObject);
            gearList1.Add(buttonInstance);
        }

        // Content 영역의 크기를 조정
        contentRect.sizeDelta = new Vector2(totalButtonWidth - 750, contentRect.sizeDelta.y);
    }
    private void GenerateReloadButtons(List<ItemAmount> items)
    {
        RectTransform contentRect = reloadParent.GetComponent<RectTransform>();
        contentRect.pivot = new Vector2(0, 1);

        // 버튼 시작 X 포지션 값
        int startXPosition = 0;
        int startYPosition = 500;

        // 지정한 개수만큼 버튼 생성
        for (int i = 0; i < items.Count(); i++)
        {
            if (i % 2 == 0) { startXPosition -= 330; startYPosition -= 200; }
            else { startXPosition += 330; }
            string name = items[i].name;
            GameObject buttonInstance = Instantiate(reloadPrefab, reloadParent);
            buttonInstance.name = "Button_R" + i;
            buttonInstance.GetComponent<RectTransform>().anchoredPosition = new Vector2(startXPosition, startYPosition);
            buttonInstance.GetComponent<Button>().onClick.AddListener(() => ammunitionSelect(name));
            TMP_Text[] textComponents = buttonInstance.GetComponentsInChildren<TMP_Text>();
            textComponents[0].text = items[i].name;
            textComponents[1].text = "Amount: " + items[i].amount.ToString();
            reloadList1.Add(buttonInstance);
        }

        // Content 영역의 크기를 조정
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, contentRect.sizeDelta.y);
    }
    public void ammunitionSelect(string a_name)
    {
        Drone drone1 = DataManager.Instance.data0.drone[position2];
        string gear1 = DataManager.Instance.data0.drone[position2].d_gear[gearPosition2];
        magazine magazine1 = drone1.d_magazine.FirstOrDefault(mag => mag.weapon == gear1);
        int amount = database_func_script_s.FindAmountByName(drone1.d_itemAmount, a_name);
        if (magazine1.current != 0) {
            if (magazine1.ammunition == a_name)
            {
                amount += magazine1.current;
            }
            else
            {
                int plus = database_func_script_s.FindAmountByName(drone1.d_itemAmount, magazine1.ammunition);
                plus += magazine1.current;
                database_func_script_s.ChangeAmountByName(drone1.d_itemAmount, magazine1.ammunition, plus);
            }
        }
        if (amount >= magazine1.max)
        {
            amount -= magazine1.max;
            magazine1.current = magazine1.max;
            magazine1.ammunition = a_name;
        }
        else
        {
            magazine1.current = amount;
            amount = 0;
            magazine1.ammunition = a_name;
        }
        database_func_script_s.ChangeAmountByName(drone1.d_itemAmount, a_name, amount);
        DataManager.Instance.SaveGameData0();
        ClearButtons2();
        gearReload.SetActive(false);
        gearInfo.SetActive(false);
    }
    public void reloadExit()
    {
        gearReload.SetActive(false);
        ClearButtons2();
    }
    public void droneExit()
    {
        gearInfo.SetActive(false);
        gearReload.SetActive(false);
        drone_view.SetActive(false);
    }
}
