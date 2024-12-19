using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using System.Linq;

[System.Serializable]
public class item_s
{
    public string name, type_c, explanation;
    public string No_s, weight_s, value_s, amount_s;
    public Dictionary<string, string> statsDict = new Dictionary<string, string>();

    public item_s(string _name, string _type_c, string _explanation,
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

public class inventory_mod_script : MonoBehaviour
{

    [SerializeField]
    public TextAsset item_s_database;
    private Dictionary<string, item_s> itemDict;

    public GameObject buttonPrefab;
    public GameObject buttonPrefab2;
    public GameObject buttonPrefab3;
    public GameObject buttonPrefab4;
    public Transform buttonParent;
    public int numberOfButtons;
    private List<GameObject> buttonsList1 = new List<GameObject>();
    private List<GameObject> buttonsList2 = new List<GameObject>();
    private List<GameObject> buttonsList3 = new List<GameObject>();
    private List<GameObject> buttonsList4 = new List<GameObject>();

    public GameObject type_sort;
    public Transform buttonParent_type;
    public GameObject buttonParent_type2;
    public GameObject buttonPrefab5;
    private bool isButtonParentTypeActive = false;
    public GameObject amount_sort;
    public GameObject weight_sort;

    private void Start()
    {
        DataManager.Instance.LoadGameData0();
        itemDict = new Dictionary<string, item_s>();
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
            if(database_func_script_s.FindAmountByName(DataManager.Instance.data0.itemAmount, name) != 0) itemDict.Add(name, item);
        }

        // itemDict���� type_c�� ������ �����Ͽ� �ߺ��� ������ ����Ʈ�� ����
        List<string> uniqueTypeList = itemDict.Values.Select(item => item.type_c).Distinct().ToList();

        CreateTypeButton("all", 0);
        // ������ type_c�� ������ŭ ��ư ����
        for (int i = 1; i < uniqueTypeList.Count+1; i++)
        {
            CreateTypeButton(uniqueTypeList[i-1], i);
        }

        itemDict = itemDict.OrderBy(item => item.Value.type_c).ToDictionary(item => item.Key, item => item.Value);
        ChangeButtonCount(itemDict.Count);
        FillButtonsListsWithData(itemDict);
    }
    public void listUpdate()
    {
        Start();
        TypeButtonClicked("all");
    }

    public void type_sort_Clicked()
    {
        isButtonParentTypeActive = !isButtonParentTypeActive;
        buttonParent_type2.SetActive(isButtonParentTypeActive);
    }
    public void amount_sort_Clicked()
    {
        SortByAmountDescending();
    }
    public void weight_sort_Clicked()
    {
        SortByWeightDescending();
    }

    private void SortByAmountDescending()
    {
        // "amount_s" ���� �������� itemDict�� ������������ ����
        itemDict = itemDict.OrderByDescending(item => int.Parse(item.Value.amount_s)).ToDictionary(item => item.Key, item => item.Value);

        // ������ ��ư���� �����ϰ� ���ĵ� �����ͷ� ���ο� ��ư���� ä���
        ClearButtons();
        ChangeButtonCount(itemDict.Count);
        FillButtonsListsWithData(itemDict);
    }

    private void SortByWeightDescending()
    {
        // itemDict�� "amount_s"�� "weight_s"�� ���� ���� ���� ������������ ����
        itemDict = itemDict.OrderByDescending(item => int.Parse(item.Value.amount_s) * int.Parse(item.Value.weight_s)).ToDictionary(item => item.Key, item => item.Value);

        // ������ ��ư���� �����ϰ� ���ĵ� �����ͷ� ���ο� ��ư���� ä���
        ClearButtons();
        ChangeButtonCount(itemDict.Count);
        FillButtonsListsWithData(itemDict);
    }

    private void CreateTypeButton(string type, int index)
    {
        // ��ư ����
        GameObject buttonInstance = Instantiate(buttonPrefab5, buttonParent_type);
        buttonInstance.name = "TypeButton " + index;
        buttonInstance.GetComponent<RectTransform>().anchoredPosition = new Vector2(150, index * -70 - 35);
        buttonInstance.GetComponent<Button>().onClick.AddListener(() => TypeButtonClicked(type));

        TMP_Text buttonText = buttonInstance.GetComponentInChildren<TMP_Text>();
        buttonText.text = type;
    }

    private void TypeButtonClicked(string type)
    {
        if (type == "all")
        {
            // ������ ��ư���� ����
            ClearButtons();

            // ȭ�鿡 ǥ���� ��ư ���� ���� �� ��ư ������ ����
            ChangeButtonCount(itemDict.Count);

            // ��ư ������ ä���
            FillButtonsListsWithData(itemDict);
        }
        else
        {
            // �ش� Ÿ��(type_c)�� �ش��ϴ� �����۵鸸 ���͸��Ͽ� ���ο� Dictionary�� ����
            Dictionary<string, item_s> filteredItems = itemDict.Where(item => item.Value.type_c == type).ToDictionary(item => item.Key, item => item.Value);

            // ������ ��ư���� ����
            ClearButtons();

            // ȭ�鿡 ǥ���� ��ư ���� ���� �� ��ư ������ ����
            ChangeButtonCount(filteredItems.Count);

            // ��ư ������ ä���
            FillButtonsListsWithData(filteredItems);
        }
    }


    private void ClearButtons()
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

    private void FillButtonsListsWithData(Dictionary<string, item_s> itemDict)
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

    private void ButtonClicked(int buttonIndex)
    {
        Debug.Log("Button " + buttonIndex + " clicked!");
    }

    public void ChangeButtonCount(int newButtonCount)
    {
        numberOfButtons = newButtonCount;
        GenerateButtons();
    }

    private void GenerateButtons()
    {
        // ��ũ�� �䰡 ����� �����Ǿ����� Ȯ��
        if (buttonParent == null)
        {
            Debug.LogError("Button parent (Content Transform of Scroll View) is not set!");
            return;
        }

        // ��ư ������ ���� Ȯ��
        if (buttonPrefab == null)
        {
            Debug.LogError("buttonPrefab error");
            return;
        }

        RectTransform contentRect = buttonParent.GetComponent<RectTransform>();
        contentRect.pivot = new Vector2(0, 1);

        // ��ư ���̸� �����ͼ� ��ư ������ŭ ���� ���̸� ���
        int totalButtonHeight = numberOfButtons * 50;

        // ��ư ���� Y ������ ��
        int startYPosition = totalButtonHeight;

        // ������ ������ŭ ��ư ����
        for (int i = 0; i < numberOfButtons; i++)
        {
            int buttonIndex = i;
            // ��ư 1 ����
            GameObject buttonInstance = Instantiate(buttonPrefab, buttonParent);
            buttonInstance.name = "Button " + i;
            buttonInstance.GetComponent<RectTransform>().anchoredPosition = new Vector2(20, startYPosition/2 - i * 50);
            buttonInstance.GetComponent<Button>().onClick.AddListener(() => ButtonClicked(buttonIndex));
            TMP_Text buttonText1 = buttonInstance.GetComponentInChildren<TMP_Text>();
            buttonsList1.Add(buttonText1.gameObject);
            buttonsList1.Add(buttonInstance);

            // ��ư 2 ����
            GameObject buttonInstance2 = Instantiate(buttonPrefab2, buttonParent);
            buttonInstance2.name = "Button_t " + i;
            buttonInstance2.GetComponent<RectTransform>().anchoredPosition = new Vector2(-500, startYPosition / 2 - i * 50);
            TMP_Text buttonText2 = buttonInstance2.GetComponentInChildren<TMP_Text>();
            buttonsList2.Add(buttonText2.gameObject);
            buttonsList2.Add(buttonInstance2);

            // ��ư 3 ����
            GameObject buttonInstance3 = Instantiate(buttonPrefab3, buttonParent);
            buttonInstance3.name = "Button_n " + i;
            buttonInstance3.GetComponent<RectTransform>().anchoredPosition = new Vector2(450, startYPosition / 2 - i * 50);
            TMP_Text buttonText3 = buttonInstance3.GetComponentInChildren<TMP_Text>();
            buttonsList3.Add(buttonText3.gameObject);
            buttonsList3.Add(buttonInstance3);

            // ��ư 4 ����
            GameObject buttonInstance4 = Instantiate(buttonPrefab4, buttonParent);
            buttonInstance4.name = "Button_w " + i;
            buttonInstance4.GetComponent<RectTransform>().anchoredPosition = new Vector2(580, startYPosition / 2 - i * 50);
            TMP_Text buttonText4 = buttonInstance4.GetComponentInChildren<TMP_Text>();
            buttonsList4.Add(buttonText4.gameObject);
            buttonsList4.Add(buttonInstance4);
        }

        // Content ������ ũ�⸦ ����
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, totalButtonHeight + 50);
    }
}