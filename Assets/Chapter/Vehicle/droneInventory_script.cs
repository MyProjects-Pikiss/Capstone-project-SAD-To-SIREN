using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using System.Linq;

[System.Serializable]
public class item_s2
{
    public string name, type_c, explanation;
    public string No_s, weight_s, value_s, amount_s;
    public Dictionary<string, string> statsDict = new Dictionary<string, string>();

    public item_s2(string _name, string _type_c, string _explanation,
        string _No, string _weight_s, string _value_s, string _amount_s, string[] _stats, string[] _statValues)
    {
        name = _name; type_c = _type_c; explanation = _explanation;
        No_s = _No; weight_s = _weight_s; value_s = _value_s; amount_s = _amount_s;

        for (int i = 0; i < _stats.Length; i++)
        {
            statsDict[_stats[i]] = _statValues[i];
        }
    }
}

public class droneInventory_script : MonoBehaviour
{
    [SerializeField]
    public TextAsset item_s_database;

    public GameObject d_inventory;
    public GameObject buttonPrefab7;
    public GameObject buttonPrefab8;
    public GameObject buttonPrefab9;
    public GameObject buttonPrefab10;
    public GameObject buttonPrefab5;
    public GameObject buttonPrefab6;
    public Transform buttonParent1;
    public Transform buttonParent2;
    public Transform buttonParent_type1;
    public Transform buttonParent_type2;
    public GameObject buttonParent_type3;
    public GameObject buttonParent_type4;
    public int numberOfButtons1;
    public int numberOfButtons2;
    private List<GameObject> buttonsList1 = new List<GameObject>();
    private List<GameObject> buttonsList2 = new List<GameObject>();
    private List<GameObject> buttonsList3 = new List<GameObject>();
    private List<GameObject> buttonsList4 = new List<GameObject>();
    private List<GameObject> buttonsList5 = new List<GameObject>();
    private List<GameObject> buttonsList6 = new List<GameObject>();
    private List<GameObject> buttonsList7 = new List<GameObject>();
    private List<GameObject> buttonsList8 = new List<GameObject>();
    private bool isButtonParentTypeActive1 = false;
    private bool isButtonParentTypeActive2 = false;

    public int droneNumber;
    private Dictionary<string, item_s> itemDict1;
    private Dictionary<string, item_s2> itemDict2;

    public TMP_Text moveText;

    // Start is called before the first frame update
    void Start()
    {
        DataManager.Instance.LoadGameData0();
        itemDict1 = new Dictionary<string, item_s>();
        itemDict2 = new Dictionary<string, item_s2>();
        string[] lines = item_s_database.text.Substring(0, item_s_database.text.Length - 1).Split('\n');

        for (int i = 0; i < lines.Length; i++)
        {
            string[] row = lines[i].Split('\t');
            string No_s = row[0];
            string type_c = row[1];
            string name = row[2];
            string amount_s = database_func_script_s.FindAmountByName(DataManager.Instance.data0.itemAmount, name).ToString();
            string weight = row[3];
            string value = row[4];
            string explanation = row[5];
            string[] stats = new string[] { row[6], row[8], row[10], row[12], row[14] };
            string[] statValues = new string[] { row[7], row[9], row[11], row[13], row[15] };

            item_s item = new item_s(name, type_c, explanation, No_s, weight, value, amount_s, stats, statValues);
            if (database_func_script_s.FindAmountByName(DataManager.Instance.data0.itemAmount, name) != 0) itemDict1.Add(name, item);
        }
        string[] lines2 = item_s_database.text.Substring(0, item_s_database.text.Length - 1).Split('\n');

        for (int i = 0; i < lines2.Length; i++)
        {
            string[] row = lines2[i].Split('\t');
            string No_s = row[0];
            string type_c = row[1];
            string name = row[2];
            string amount_s = database_func_script_s.FindAmountByName(DataManager.Instance.data0.drone[droneNumber].d_itemAmount, name).ToString();
            string weight = row[3];
            string value = row[4];
            string explanation = row[5];
            string[] stats = new string[] { row[6], row[8], row[10], row[12], row[14] };
            string[] statValues = new string[] { row[7], row[9], row[11], row[13], row[15] };

            item_s2 item2 = new item_s2(name, type_c, explanation, No_s, weight, value, amount_s, stats, statValues);
            if (database_func_script_s.FindAmountByName(DataManager.Instance.data0.drone[droneNumber].d_itemAmount, name) != 0) itemDict2.Add(name, item2);
        }

        // itemDict에서 type_c의 종류를 추출하여 중복을 제거한 리스트를 생성
        List<string> uniqueTypeList1 = itemDict1.Values.Select(item => item.type_c).Distinct().ToList();
        List<string> uniqueTypeList2 = itemDict2.Values.Select(item => item.type_c).Distinct().ToList();

        CreateTypeButton1("all", 0);
        CreateTypeButton2("all", 0);
        // 추출한 type_c의 종류만큼 버튼 생성
        for (int i = 1; i < uniqueTypeList1.Count + 1; i++)
        {
            CreateTypeButton1(uniqueTypeList1[i - 1], i);
        }
        for (int i = 1; i < uniqueTypeList2.Count + 1; i++)
        {
            CreateTypeButton2(uniqueTypeList2[i - 1], i);
        }
        itemDict1 = itemDict1.OrderBy(item => item.Value.type_c).ToDictionary(item => item.Key, item => item.Value);
        ChangeButtonCount1(itemDict1.Count);
        itemDict2 = itemDict2.OrderBy(item => item.Value.type_c).ToDictionary(item => item.Key, item => item.Value);
        ChangeButtonCount2(itemDict2.Count);
    }

    public void listUpdate()
    {
        Start();
        TypeButtonClicked1("all");
        TypeButtonClicked2("all");
    }
    public void getDrone(int index)
    {
        DataManager.Instance.LoadGameData0();
        droneNumber = index;
    }

    private void ButtonClicked1(int buttonIndex)
    {
        DataManager.Instance.LoadGameData0();
        buttonIndex = buttonIndex * 2;
        moveText.text = buttonsList1[buttonIndex].GetComponentInChildren<TMP_Text>().text;
        int x = database_func_script_s.FindAmountByName(DataManager.Instance.data0.itemAmount, moveText.text);
        database_func_script_s.ChangeAmountByName(DataManager.Instance.data0.itemAmount, moveText.text, x - 1);
        int y = database_func_script_s.FindAmountByName(DataManager.Instance.data0.drone[droneNumber].d_itemAmount, moveText.text);
        database_func_script_s.ChangeAmountByName(DataManager.Instance.data0.drone[droneNumber].d_itemAmount, moveText.text, y + 1);
        DataManager.Instance.SaveGameData0();
        listUpdate();
    }
    private void ButtonClicked2(int buttonIndex)
    {
        DataManager.Instance.LoadGameData0();
        buttonIndex = buttonIndex * 2;
        moveText.text = buttonsList5[buttonIndex].GetComponentInChildren<TMP_Text>().text;
        int x = database_func_script_s.FindAmountByName(DataManager.Instance.data0.itemAmount, moveText.text);
        database_func_script_s.ChangeAmountByName(DataManager.Instance.data0.itemAmount, moveText.text, x + 1);
        int y = database_func_script_s.FindAmountByName(DataManager.Instance.data0.drone[droneNumber].d_itemAmount, moveText.text);
        database_func_script_s.ChangeAmountByName(DataManager.Instance.data0.drone[droneNumber].d_itemAmount, moveText.text, y - 1);
        DataManager.Instance.SaveGameData0();
        listUpdate();
    }
    public void toDroneView()
    {
        d_inventory.SetActive(false);
    }

    public void amount_sort_Clicked1()
    {
        SortByAmountDescending1();
    }
    public void weight_sort_Clicked1()
    {
        SortByWeightDescending1();
    }
    public void amount_sort_Clicked2()
    {
        SortByAmountDescending2();
    }
    public void weight_sort_Clicked2()
    {
        SortByWeightDescending2();
    }

    private void SortByAmountDescending1()
    {
        // "amount_s" 값을 기준으로 itemDict를 내림차순으로 정렬
        itemDict1 = itemDict1.OrderByDescending(item => int.Parse(item.Value.amount_s)).ToDictionary(item => item.Key, item => item.Value);

        // 기존의 버튼들을 제거하고 정렬된 데이터로 새로운 버튼들을 채우기
        ClearButtons1();
        ChangeButtonCount1(itemDict1.Count);
        FillButtonsListsWithData1(itemDict1);
    }

    private void SortByWeightDescending1()
    {
        // itemDict를 "amount_s"와 "weight_s"를 곱한 값에 따라 내림차순으로 정렬
        itemDict1 = itemDict1.OrderByDescending(item => int.Parse(item.Value.amount_s) * int.Parse(item.Value.weight_s)).ToDictionary(item => item.Key, item => item.Value);

        // 기존의 버튼들을 제거하고 정렬된 데이터로 새로운 버튼들을 채우기
        ClearButtons1();
        ChangeButtonCount1(itemDict1.Count);
        FillButtonsListsWithData1(itemDict1);
    }
    private void SortByAmountDescending2()
    {
        // "amount_s" 값을 기준으로 itemDict를 내림차순으로 정렬
        itemDict2 = itemDict2.OrderByDescending(item => int.Parse(item.Value.amount_s)).ToDictionary(item => item.Key, item => item.Value);

        // 기존의 버튼들을 제거하고 정렬된 데이터로 새로운 버튼들을 채우기
        ClearButtons2();
        ChangeButtonCount2(itemDict2.Count);
        FillButtonsListsWithData2(itemDict2);
    }

    private void SortByWeightDescending2()
    {
        // itemDict를 "amount_s"와 "weight_s"를 곱한 값에 따라 내림차순으로 정렬
        itemDict2 = itemDict2.OrderByDescending(item => int.Parse(item.Value.amount_s) * int.Parse(item.Value.weight_s)).ToDictionary(item => item.Key, item => item.Value);

        // 기존의 버튼들을 제거하고 정렬된 데이터로 새로운 버튼들을 채우기
        ClearButtons2();
        ChangeButtonCount2(itemDict2.Count);
        FillButtonsListsWithData2(itemDict2);
    }

    public void ChangeButtonCount1(int newButtonCount)
    {
        numberOfButtons1 = newButtonCount;
        GenerateButtons1();
    }
    public void ChangeButtonCount2(int newButtonCount)
    {
        numberOfButtons2 = newButtonCount;
        GenerateButtons2();
    }
    private void GenerateButtons1()
    {
        RectTransform contentRect = buttonParent1.GetComponent<RectTransform>();
        contentRect.pivot = new Vector2(0, 1);

        // 버튼 높이를 가져와서 버튼 개수만큼 더한 높이를 계산
        int totalButtonHeight = numberOfButtons1 * 50;

        // 버튼 시작 Y 포지션 값
        int startYPosition = totalButtonHeight;

        // 지정한 개수만큼 버튼 생성
        for (int i = 0; i < numberOfButtons1; i++)
        {
            int buttonIndex = i;
            // 버튼 1 생성
            GameObject buttonInstance = Instantiate(buttonPrefab7, buttonParent1);
            buttonInstance.name = "Button " + i;
            buttonInstance.GetComponent<RectTransform>().anchoredPosition = new Vector2(-40, startYPosition / 2 - i * 50);
            buttonInstance.GetComponent<Button>().onClick.AddListener(() => ButtonClicked1(buttonIndex));
            TMP_Text buttonText1 = buttonInstance.GetComponentInChildren<TMP_Text>();
            buttonsList1.Add(buttonText1.gameObject);
            buttonsList1.Add(buttonInstance);

            // 버튼 2 생성
            GameObject buttonInstance2 = Instantiate(buttonPrefab8, buttonParent1);
            buttonInstance2.name = "Button_t " + i;
            buttonInstance2.GetComponent<RectTransform>().anchoredPosition = new Vector2(-300, startYPosition / 2 - i * 50);
            TMP_Text buttonText2 = buttonInstance2.GetComponentInChildren<TMP_Text>();
            buttonsList2.Add(buttonText2.gameObject);
            buttonsList2.Add(buttonInstance2);

            // 버튼 3 생성
            GameObject buttonInstance3 = Instantiate(buttonPrefab9, buttonParent1);
            buttonInstance3.name = "Button_n " + i;
            buttonInstance3.GetComponent<RectTransform>().anchoredPosition = new Vector2(190, startYPosition / 2 - i * 50);
            TMP_Text buttonText3 = buttonInstance3.GetComponentInChildren<TMP_Text>();
            buttonsList3.Add(buttonText3.gameObject);
            buttonsList3.Add(buttonInstance3);

            // 버튼 4 생성
            GameObject buttonInstance4 = Instantiate(buttonPrefab10, buttonParent1);
            buttonInstance4.name = "Button_w " + i;
            buttonInstance4.GetComponent<RectTransform>().anchoredPosition = new Vector2(330, startYPosition / 2 - i * 50);
            TMP_Text buttonText4 = buttonInstance4.GetComponentInChildren<TMP_Text>();
            buttonsList4.Add(buttonText4.gameObject);
            buttonsList4.Add(buttonInstance4);
        }

        // Content 영역의 크기를 조정
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, totalButtonHeight + 50);
    }
    private void GenerateButtons2()
    {
        RectTransform contentRect = buttonParent2.GetComponent<RectTransform>();
        contentRect.pivot = new Vector2(0, 1);

        // 버튼 높이를 가져와서 버튼 개수만큼 더한 높이를 계산
        int totalButtonHeight = numberOfButtons2 * 50;

        // 버튼 시작 Y 포지션 값
        int startYPosition = totalButtonHeight;

        // 지정한 개수만큼 버튼 생성
        for (int i = 0; i < numberOfButtons2; i++)
        {
            int buttonIndex = i;
            // 버튼 1 생성
            GameObject buttonInstance = Instantiate(buttonPrefab7, buttonParent2);
            buttonInstance.name = "Button " + i;
            buttonInstance.GetComponent<RectTransform>().anchoredPosition = new Vector2(-40, startYPosition / 2 - i * 50);
            buttonInstance.GetComponent<Button>().onClick.AddListener(() => ButtonClicked2(buttonIndex));
            TMP_Text buttonText5 = buttonInstance.GetComponentInChildren<TMP_Text>();
            buttonsList5.Add(buttonText5.gameObject);
            buttonsList5.Add(buttonInstance);

            // 버튼 2 생성
            GameObject buttonInstance2 = Instantiate(buttonPrefab8, buttonParent2);
            buttonInstance2.name = "Button_t " + i;
            buttonInstance2.GetComponent<RectTransform>().anchoredPosition = new Vector2(-300, startYPosition / 2 - i * 50);
            TMP_Text buttonText6 = buttonInstance2.GetComponentInChildren<TMP_Text>();
            buttonsList6.Add(buttonText6.gameObject);
            buttonsList6.Add(buttonInstance2);

            // 버튼 3 생성
            GameObject buttonInstance3 = Instantiate(buttonPrefab9, buttonParent2);
            buttonInstance3.name = "Button_n " + i;
            buttonInstance3.GetComponent<RectTransform>().anchoredPosition = new Vector2(190, startYPosition / 2 - i * 50);
            TMP_Text buttonText7 = buttonInstance3.GetComponentInChildren<TMP_Text>();
            buttonsList7.Add(buttonText7.gameObject);
            buttonsList7.Add(buttonInstance3);

            // 버튼 4 생성
            GameObject buttonInstance4 = Instantiate(buttonPrefab10, buttonParent2);
            buttonInstance4.name = "Button_w " + i;
            buttonInstance4.GetComponent<RectTransform>().anchoredPosition = new Vector2(330, startYPosition / 2 - i * 50);
            TMP_Text buttonText8 = buttonInstance4.GetComponentInChildren<TMP_Text>();
            buttonsList8.Add(buttonText8.gameObject);
            buttonsList8.Add(buttonInstance4);
        }

        // Content 영역의 크기를 조정
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, totalButtonHeight + 50);
    }

    public void type_sort_Clicked1()
    {
        isButtonParentTypeActive1 = !isButtonParentTypeActive1;
        buttonParent_type3.SetActive(isButtonParentTypeActive1);
    }
    public void type_sort_Clicked2()
    {
        isButtonParentTypeActive2 = !isButtonParentTypeActive2;
        buttonParent_type4.SetActive(isButtonParentTypeActive2);
    }
    private void TypeButtonClicked1(string type)
    {
        if (type == "all")
        {
            // 기존의 버튼들을 제거
            ClearButtons1();

            // 화면에 표시할 버튼 개수 변경 및 버튼 데이터 갱신
            ChangeButtonCount1(itemDict1.Count);

            // 버튼 데이터 채우기
            FillButtonsListsWithData1(itemDict1);
        }
        else
        {
            // 해당 타입(type_c)에 해당하는 아이템들만 필터링하여 새로운 Dictionary를 생성
            Dictionary<string, item_s> filteredItems = itemDict1.Where(item => item.Value.type_c == type).ToDictionary(item => item.Key, item => item.Value);

            // 기존의 버튼들을 제거
            ClearButtons1();

            // 화면에 표시할 버튼 개수 변경 및 버튼 데이터 갱신
            ChangeButtonCount1(filteredItems.Count);

            // 버튼 데이터 채우기
            FillButtonsListsWithData1(filteredItems);
        }
    }
    private void TypeButtonClicked2(string type)
    {
        if (type == "all")
        {
            // 기존의 버튼들을 제거
            ClearButtons2();

            // 화면에 표시할 버튼 개수 변경 및 버튼 데이터 갱신
            ChangeButtonCount2(itemDict2.Count);

            // 버튼 데이터 채우기
            FillButtonsListsWithData2(itemDict2);
        }
        else
        {
            // 해당 타입(type_c)에 해당하는 아이템들만 필터링하여 새로운 Dictionary를 생성
            Dictionary<string, item_s2> filteredItems2 = itemDict2.Where(item => item.Value.type_c == type).ToDictionary(item => item.Key, item => item.Value);

            // 기존의 버튼들을 제거
            ClearButtons2();

            // 화면에 표시할 버튼 개수 변경 및 버튼 데이터 갱신
            ChangeButtonCount2(filteredItems2.Count);

            // 버튼 데이터 채우기
            FillButtonsListsWithData2(filteredItems2);
        }
    }
    private void ClearButtons1()
    {
        foreach (GameObject button in buttonsList1)
        {
            Destroy(button);
        }
        buttonsList1.Clear();

        foreach (GameObject button in buttonsList2)
        {
            Destroy(button);
        }
        buttonsList2.Clear();

        foreach (GameObject button in buttonsList3)
        {
            Destroy(button);
        }
        buttonsList3.Clear();
        foreach (GameObject button in buttonsList4)
        {
            Destroy(button);
        }
        buttonsList4.Clear();
    }
    private void ClearButtons2()
    {
        foreach (GameObject button in buttonsList5)
        {
            Destroy(button);
        }
        buttonsList5.Clear();

        foreach (GameObject button in buttonsList6)
        {
            Destroy(button);
        }
        buttonsList6.Clear();

        foreach (GameObject button in buttonsList7)
        {
            Destroy(button);
        }
        buttonsList7.Clear();
        foreach (GameObject button in buttonsList8)
        {
            Destroy(button);
        }
        buttonsList8.Clear();
    }
    private void FillButtonsListsWithData1(Dictionary<string, item_s> itemDict)
    {
        for (int i = 0; i < buttonsList1.Count; i++)
        {
            if (i < itemDict.Count)
            {
                var item = itemDict.ElementAt(i);
                string name = item.Key;
                item_s itemInfo = item.Value;

                i = i * 2;
                buttonsList1[i].GetComponentInChildren<TMP_Text>().text = name;
                buttonsList2[i].GetComponentInChildren<TMP_Text>().text = itemInfo.type_c;
                buttonsList3[i].GetComponentInChildren<TMP_Text>().text = itemInfo.amount_s;
                buttonsList4[i].GetComponentInChildren<TMP_Text>().text = (int.Parse(itemInfo.weight_s) * int.Parse(itemInfo.amount_s)).ToString();
                i = i / 2;
            }
        }
    }
    private void FillButtonsListsWithData2(Dictionary<string, item_s2> itemDict)
    {
        for (int i = 0; i < buttonsList5.Count; i++)
        {
            if (i < itemDict.Count)
            {
                var item = itemDict.ElementAt(i);
                string name = item.Key;
                item_s2 itemInfo = item.Value;

                i = i * 2;
                buttonsList5[i].GetComponentInChildren<TMP_Text>().text = name;
                buttonsList6[i].GetComponentInChildren<TMP_Text>().text = itemInfo.type_c;
                buttonsList7[i].GetComponentInChildren<TMP_Text>().text = itemInfo.amount_s;
                buttonsList8[i].GetComponentInChildren<TMP_Text>().text = (int.Parse(itemInfo.weight_s) * int.Parse(itemInfo.amount_s)).ToString();
                i = i / 2;
            }
        }
    }
    private void CreateTypeButton1(string type, int index)
    {
        // 버튼 생성
        GameObject buttonInstance = Instantiate(buttonPrefab5, buttonParent_type1);
        buttonInstance.name = "TypeButton " + index;
        buttonInstance.GetComponent<RectTransform>().anchoredPosition = new Vector2(100, index * -90 - 45);
        buttonInstance.GetComponent<Button>().onClick.AddListener(() => TypeButtonClicked1(type));

        TMP_Text buttonText = buttonInstance.GetComponentInChildren<TMP_Text>();
        buttonText.text = type;
    }
    private void CreateTypeButton2(string type, int index)
    {
        // 버튼 생성
        GameObject buttonInstance = Instantiate(buttonPrefab6, buttonParent_type2);
        buttonInstance.name = "TypeButton " + index;
        buttonInstance.GetComponent<RectTransform>().anchoredPosition = new Vector2(100, index * -90 - 45);
        buttonInstance.GetComponent<Button>().onClick.AddListener(() => TypeButtonClicked2(type));

        TMP_Text buttonText = buttonInstance.GetComponentInChildren<TMP_Text>();
        buttonText.text = type;
    }
}
