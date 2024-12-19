using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

[System.Serializable]
public class Produce
{
    public string No_s, name, type, produceTime, produceAmount;
    public Dictionary<string, string> ingredientsDict = new Dictionary<string, string>();
    public Dictionary<string, string> modulesDict = new Dictionary<string, string>();
    public Dictionary<string, string> skillsDict = new Dictionary<string, string>();

    public Produce(string _No, string _name, string _type, string _produceAmount, string[] _ingredients, string[] _amounts, string _produceTime, string[] _modules, string[] _m_levels, string[] _skills, string[] _s_levels)
    {
        No_s = _No; name = _name; type = _type; produceTime = _produceTime; produceAmount = _produceAmount;

        for (int i = 0; i < _ingredients.Length; i++)
        {
            ingredientsDict[_ingredients[i]] = _amounts[i];
        }
        for (int i = 0; i < _modules.Length; i++)
        {
            modulesDict[_modules[i]] = _m_levels[i];
        }
        for (int i = 0; i < _skills.Length; i++)
        {
            skillsDict[_skills[i]] = _s_levels[i];
        }
    }
}

public class produce_mod_script : MonoBehaviour
{
    [SerializeField]
    private TextAsset produce_mod_database;
    private Dictionary<string, Dictionary<string, Produce>> produceTypesDict = new Dictionary<string, Dictionary<string, Produce>>();

    public TMP_Text[] typeTexts;
    public GameObject[] typeButtons;
    public TMP_Text[] nameTexts;
    public GameObject[] nameButtons;
    public TMP_Text[] ingredientTexts;
    public TMP_Text[] amountTexts;
    public GameObject[] ingredientButtons;
    public GameObject make_btn;
    public GameObject result_btn;
    public TMP_Text resultText;

    string targetType;
    private List<KeyValuePair<string, string>> ingredientList;
    private List<KeyValuePair<string, string>> moduleList;
    private List<KeyValuePair<string, string>> traitList;
    string targetName;
    public int target;
    public int produceTime2;

    public TMP_Text timeText;
    public TMP_Text[] moduleTexts;
    public TMP_Text[] traitTexts;

    private Dictionary<string, string> CreateIngredientsDict(string[] ingredients, string[] amounts)
    {
        Dictionary<string, string> ingredientsDict = new Dictionary<string, string>();
        for (int i = 0; i < ingredients.Length; i++)
        {
            ingredientsDict[ingredients[i]] = amounts[i];
        }
        return ingredientsDict;
    }

    // Start is called before the first frame update
    void Start()
    {
        string[] lines = produce_mod_database.text.Substring(0, produce_mod_database.text.Length - 1).Split('\n');

        for (int i = 0; i < lines.Length; i++)
        {
            string[] row = lines[i].Split('\t');
            string No_s = row[0];
            string type = row[1];
            string name = row[2];
            string produceAmount = row[3];
            string[] ingredients = new string[] { row[4], row[6], row[8], row[10], row[12], row[14], row[16], row[18], row[20], row[22] };
            string[] amounts = new string[] { row[5], row[7], row[9], row[11], row[13], row[15], row[17], row[19], row[21], row[23] };
            string produceTime = row[24];
            string[] modules = new string[] { row[25], row[27], row[29], row[31] };
            string[] m_levels = new string[] { row[26], row[28], row[30], row[32] };
            string[] skills = new string[] { row[33], row[35], row[37], row[39] };
            string[] s_levels = new string[] { row[34], row[36], row[38], row[40] };

            Produce produce = new Produce(No_s, name, type, produceAmount, ingredients, amounts, produceTime, modules, m_levels, skills, s_levels);

            if (!produceTypesDict.ContainsKey(type))
            {
                produceTypesDict[type] = new Dictionary<string, Produce>();
            }

            produceTypesDict[type][name] = produce;
        }

        UpdateProduceTypes();
    }

    public void result_btn_clicked()
    {
        result_btn.SetActive(false);
    }

    public void make_btn_clicked()
    {
        DataManager.Instance.LoadGameData0();
        int[] result = new int[ingredientList.Count - 1];
        string[] names = new string[ingredientList.Count - 1];
        int[] m_level = new int[moduleList.Count - 1];
        string[] m_names = new string[moduleList.Count - 1];
        int[] t_level = new int[traitList.Count - 1];
        string[] t_names = new string[traitList.Count - 1];
        bool check = false;
        for (int i = 0; i < ingredientTexts.Length; i++)
        {
            if (i < ingredientList.Count - 1)
            {
                var ingredient = ingredientList[i];
                result[i] = database_func_script_s.FindAmountByName(DataManager.Instance.data0.itemAmount, ingredient.Key) - int.Parse(ingredient.Value);
                names[i] = ingredient.Key;
            }
        }
        for (int i = 0; i < result.Length; i++)
        {
            if (result[i] < 0)
            {
                result_btn.SetActive(true);
                resultText.text = "Material shortage";
                check = true;
            }
        }
        for (int i = 0; i < moduleTexts.Length; i++)
        {
            if (i < moduleList.Count - 1)
            {
                var module = moduleList[i];
                m_level[i] = database_func_script_s.FindM_levelByName(DataManager.Instance.data0.module, module.Key) - int.Parse(module.Value);
                m_names[i] = module.Key;
            }
        }
        for (int i = 0; i < m_level.Length; i++)
        {
            if (m_level[i] < 0)
            {
                result_btn.SetActive(true);
                resultText.text = "Module level shortage";
                check = true;
            }
        }
        for (int i = 0; i < traitTexts.Length; i++)
        {
            if (i < traitList.Count - 1)
            {
                var trait = traitList[i];
                t_level[i] = database_func_script_s.FindT_levelByName(DataManager.Instance.data0.trait, trait.Key) - int.Parse(trait.Value);
                t_names[i] = trait.Key;
            }
        }
        for (int i = 0; i < t_level.Length; i++)
        {
            if (t_level[i] < 0)
            {
                result_btn.SetActive(true);
                resultText.text = "Trait level shortage";
                check = true;
            }
        }

        if (!check)
        {
            for (int i = 0; i < result.Length; i++)
            {
                database_func_script_s.ChangeAmountByName(DataManager.Instance.data0.itemAmount, names[i], result[i]);
            }
            int plus = database_func_script_s.FindPAmountByName(produce_mod_database, targetName);
            int plus2 = database_func_script_s.FindAmountByName(DataManager.Instance.data0.itemAmount, targetName) + plus;
            database_func_script_s.ChangeAmountByName(DataManager.Instance.data0.itemAmount, targetName, plus2);

            DataManager.Instance.SaveGameData0();
            status_script.status_produceTime(produceTime2);
            DataManager.Instance.LoadGameData0();
            OnNameButtonClicked(target);
        }
    }

    private void UpdateProduceTypes()
    {
        List<string> produceTypes = new List<string>(produceTypesDict.Keys);
        for (int i = 0; i < typeTexts.Length; i++)
        {
            if (i < produceTypes.Count)
            {
                typeTexts[i].text = produceTypes[i];
                typeButtons[i].SetActive(true);
            }
            else
            {
                typeTexts[i].text = "";
                typeButtons[i].SetActive(false);
            }
        }
    }

    public void OnTypeButtonClicked(int index)
    {
        targetType = typeTexts[index].text;
        List<string> produceNames = new List<string>(produceTypesDict[targetType].Keys);

        for (int i = 0; i < nameTexts.Length; i++)
        {
            if (i < produceNames.Count)
            {
                nameTexts[i].text = produceNames[i] + " (x" + database_func_script_s.FindPAmountByName(produce_mod_database, produceNames[i]) + ")";
                nameButtons[i].SetActive(true);
            }
            else
            {
                nameTexts[i].text = "";
                nameButtons[i].SetActive(false);
            }
        }
    }

    public void OnNameButtonClicked(int index)
    {
        DataManager.Instance.LoadGameData0();
        target = index;
        targetName = nameTexts[index].text;
        int startIndex = targetName.IndexOf('(');
        int endIndex = targetName.IndexOf(')');
        if (startIndex != -1 && endIndex != -1 && endIndex > startIndex)
        {
            targetName = targetName.Substring(0, startIndex).Trim();
        }
        // 선택한 유형의 사전을 가져옴
        if (produceTypesDict.TryGetValue(targetType, out Dictionary<string, Produce> produceDict))
        {
            // 사전에서 찾음
            if (produceDict.TryGetValue(targetName, out Produce selectedProduce))
            {
                // 이제 선택한 것의 재료와 갯수에 접근
                ingredientList = new List<KeyValuePair<string, string>>(selectedProduce.ingredientsDict);
                moduleList = new List<KeyValuePair<string, string>>(selectedProduce.modulesDict);
                traitList = new List<KeyValuePair<string, string>>(selectedProduce.skillsDict);
                for (int i = 0; i < ingredientTexts.Length; i++)
                {
                    if (i < ingredientList.Count-1)
                    {
                        var ingredient = ingredientList[i];
                        ingredientTexts[i].text = ingredient.Key;
                        amountTexts[i].text = database_func_script_s.FindAmountByName(DataManager.Instance.data0.itemAmount, ingredient.Key).ToString() + "/" + ingredient.Value;
                        ingredientButtons[i].SetActive(true);
                    }
                    else
                    {
                        ingredientTexts[i].text = "";
                        amountTexts[i].text = "";
                        ingredientButtons[i].SetActive(false);
                    }
                }
                for (int i = 0; i < moduleTexts.Length; i++)
                {
                    if (i < moduleList.Count-1)
                    {
                        var module = moduleList[i];
                        moduleTexts[i].text = "Required " + module.Key + "-Lv." + database_func_script_s.FindM_levelByName(DataManager.Instance.data0.module, module.Key).ToString() + "/" + module.Value;
                    }
                    else
                    {
                        moduleTexts[i].text = "";
                    }
                }
                for (int i = 0; i < traitTexts.Length; i++)
                {
                    if (i < traitList.Count-1)
                    {
                        var trait = traitList[i];
                        traitTexts[i].text = "Required " + trait.Key + "-Lv." + database_func_script_s.FindT_levelByName(DataManager.Instance.data0.trait, trait.Key).ToString() + "/" + trait.Value;
                    }
                    else
                    {
                        traitTexts[i].text = "";
                    }
                }
                produceTime2 = int.Parse(selectedProduce.produceTime);
                timeText.text = string.Format("Take {0:D2}:{1:D2}", produceTime2 / 60, produceTime2 % 60);
            }
            else
            {
                Debug.Log("해당 자료를 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.Log("해당 유형을 찾을 수 없습니다.");
        }
        make_btn.SetActive(true);
    }
}
