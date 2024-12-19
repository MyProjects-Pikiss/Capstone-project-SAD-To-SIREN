using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class database_func_script_s : MonoBehaviour
{
    public static int FindAmountByName(List<ItemAmount> itemAmountList, string itemName)
    {
        foreach (var itemAmount in itemAmountList)
        {
            if (itemAmount.name == itemName)
            {
                return itemAmount.amount;
            }
        }
        // 해당 이름의 항목을 찾지 못한 경우 기본값 또는 예외 처리 등을 수행할 수 있음.
        return 0;
    }

    public static void ChangeAmountByName(List<ItemAmount> itemAmountList, string itemName, int newAmount)
    {
        for (int i = 0; i < itemAmountList.Count; i++)
        {
            if (itemAmountList[i].name == itemName)
            {
                // 해당 아이템을 찾은 경우, 새로운 ItemAmount 객체를 생성하여 대입
                itemAmountList[i] = new ItemAmount { name = itemName, amount = newAmount };
                break; // 해당 아이템을 찾았으므로 루프 종료
            }
        }
    }

    public static int FindM_levelByName(List<Module> moduleList, string moduleName)
    {
        foreach (var module in moduleList)
        {
            if (module.m_name == moduleName)
            {
                return module.m_level;
            }
        }
        // 해당 이름의 항목을 찾지 못한 경우 기본값 또는 예외 처리 등을 수행할 수 있음.
        return 0;
    }

    public static int FindT_levelByName(List<Trait> traitList, string traitName)
    {
        foreach (var trait in traitList)
        {
            if (trait.t_na_name == traitName)
            {
                return trait.t_level;
            }
            if (trait.t_po_name == traitName)
            {
                return trait.t_level;
            }
        }
        // 해당 이름의 항목을 찾지 못한 경우 기본값 또는 예외 처리 등을 수행할 수 있음.
        return 0;
    }

    public static int CalculationEnemySpawn(TextAsset item_s_database, List<ItemAmount> itemAmountList, List<Drone> drone, int spawnRate)
    {
        int total = 0;

        string[] lines2 = item_s_database.text.Substring(0, item_s_database.text.Length - 1).Split('\n');
        for (int i = 0; i < itemAmountList.Count; i++)
        {
            for (int x = 0; x < lines2.Length; x++)
            {
                string[] row = lines2[x].Split('\t');
                string name = row[2];
                int value = int.Parse(row[4]);
                if (itemAmountList[x].name == name)
                {
                    total += value * itemAmountList[x].amount;
                }
            }
        }
        for (int j = 0; j < 5; j++)
        {
            for (int i = 0; i < drone[j].d_itemAmount.Count; i++)
            {
                for (int x = 0; x < lines2.Length; x++)
                {
                    string[] row = lines2[x].Split('\t');
                    string name = row[2];
                    int value = int.Parse(row[4]);
                    if (drone[j].d_itemAmount[x].name == name)
                    {
                        total += value * drone[j].d_itemAmount[x].amount;
                    }
                }
            }
            for (int i = 0; i < drone[j].d_gear.Count; i++) 
            {
                for (int x = 0; x < lines2.Length; x++)
                {
                    string[] row = lines2[x].Split('\t');
                    string name = row[2];
                    int value = int.Parse(row[4]);
                    if (drone[j].d_gear[i] == name)
                    {
                        total += value;
                    }
                }
            }
        }
        //마지막 계산
        int last = total / 3000;
        return last;
    }

    public static List<int> RecursiveCalculationEnemy(List<int> pLevelList, string[] lines, int total, List<int> Return)
    {
        for (int i = 0; i < pLevelList.Count; i++)
        {
            for (int x = 0; x < lines.Length; x++)
            {
                string[] row = lines[x].Split('\t');
                int no = int.Parse(row[0]);
                int p_level = int.Parse(row[3]);
                if (p_level == pLevelList[i])
                {
                    if (total - p_level >= 0)
                    {
                        total -= p_level;
                        Return.Add(no);
                        return RecursiveCalculationEnemy(pLevelList, lines, total, Return);
                    }
                    else
                    {
                        return Return;
                    }
                }
            }
        }
        return Return;
    }

    public static List<int> CalculationListEnemy(TextAsset enemy_database, TextAsset worldMap_database, int total, int mapCode)
    {
        List<int> Return = new List<int>();
        List<int> pLevelList = new List<int>();
        string type1 = "";
        string type2 = "";

        string[] lines2 = worldMap_database.text.Substring(0, worldMap_database.text.Length - 1).Split('\n');
        for (int x = 0; x < lines2.Length; x++)
        {
            string[] row = lines2[x].Split('\t');
            int info = int.Parse(row[2]);
            if(mapCode == info)
            {
                type1 = row[4];
                type2 = row[5];
            }
        }

        string[] lines = enemy_database.text.Substring(0, enemy_database.text.Length - 1).Split('\n');
        for (int x = 0; x < lines.Length; x++)
        {
            string[] row = lines[x].Split('\t');
            string type_e = row[1];
            int p_level = int.Parse(row[3]);
            if(type1 == type_e || type2 == type_e)
                pLevelList.Add(p_level);
        }

        pLevelList.Sort();
        pLevelList.Reverse();

        //최대 +-20% 랜덤화
        int randomFactor = Random.Range(-20, 21);
        int change = (total * randomFactor) / 100;
        int newValue = total + change;

        return RecursiveCalculationEnemy(pLevelList, lines, newValue, Return);
    }


    public static List<string> RecursiveCalculationItem(List<List<int>> masterList, string[] lines, int[] type_is, List<string> Return)
    {
        Random.InitState(System.DateTime.Now.Millisecond);
        foreach (var sublist in masterList)
        {
            for (int i = 0; i < sublist.Count; i++)
            {
                int temp = sublist[i];
                int randomIndex = Random.Range(i, sublist.Count);
                sublist[i] = sublist[randomIndex];
                sublist[randomIndex] = temp;
            }
        }

        for (int i = 0; i < type_is.Length; i++)
        {
            for (int j = 0; j < masterList[i].Count; j++)
            {
                for (int x = 0; x < lines.Length; x++)
                {
                    string[] row = lines[x].Split('\t');
                    int no = int.Parse(row[0]);
                    string name = row[2];
                    int type_i = ReturnNumByItemType(row[1]);
                    int value = int.Parse(row[4]);
                    if (no == masterList[i][j])
                    {
                        if (type_is[i] - value >= 0)
                        {
                            type_is[i] -= value;
                            Return.Add(name);
                            return RecursiveCalculationItem(masterList, lines, type_is, Return);
                        }
                    }
                }
            }
        }
        return Return;
    }
    public static List<string> CalculationListItems(TextAsset item_s_database, TextAsset worldMap_database, int mapCode)
    {
        List<string> Return = new List<string>();
        List<int> value = new List<int>();

        List<int> meds = new List<int>();
        List<int> foods = new List<int>();
        List<int> ingrs = new List<int>();
        List<int> artis = new List<int>();
        List<List<int>> masterList = new List<List<int>>();
        masterList.Add(meds);
        masterList.Add(foods);
        masterList.Add(ingrs);
        masterList.Add(artis);
        int med = 0; int food = 0; int ingr = 0; int arti = 0;
        string[] lines2 = worldMap_database.text.Substring(0, worldMap_database.text.Length - 1).Split('\n');
        for (int x = 0; x < lines2.Length; x++)
        {
            string[] row = lines2[x].Split('\t');
            int info = int.Parse(row[2]);
            if (mapCode == info)
            {
                med = int.Parse(row[7]); food = int.Parse(row[8]); ingr = int.Parse(row[9]); arti = int.Parse(row[10]);
            }
        }

        //밸류 강화
        med = med * 4000; food = food * 2000; ingr = ingr * 1200; arti = arti * 4000;

        int[] type_is = {med, food, ingr, arti};

        string[] lines = item_s_database.text.Substring(0, item_s_database.text.Length - 1).Split('\n');
        for (int x = 0; x < lines.Length; x++)
        {
            string[] row = lines[x].Split('\t');
            int No = int.Parse(row[0]);
            string type_i = row[1];
            int index = ReturnNumByItemType(type_i);
            if(index != -1)
            {
                masterList[index].Add(No);
            }
        }

        return RecursiveCalculationItem(masterList, lines, type_is, Return);
    }

    public static int ReturnNumByItemType(string itemType)
    {
        switch (itemType.ToLower())
        {
            case "medicine":
                return 0;
            case "food":
                return 1;
            case "ingredient":
                return 2;
            case "artifact":
                return 3;
            default:
                return -1;
        }
    }

    public static int FindPAmountByName(TextAsset produce_mod_database, string name)
    {
        string[] lines = produce_mod_database.text.Substring(0, produce_mod_database.text.Length - 1).Split('\n');

        for (int i = 0; i < lines.Length; i++)
        {
            string[] row = lines[i].Split('\t');
            string name2 = row[2];
            if(name2 == name)
            {
                return int.Parse(row[3]);
            }
        }
        return -99999;
    }

    public static void RenameFilesWithPartialChange(string oldPrefix, string searchSubstring, string replaceSubstring)
    {// a로 시작하는 파일이름중에 b를 c로 바꿈
        DirectoryInfo directoryInfo = new DirectoryInfo(Application.persistentDataPath);
        FileInfo[] files = directoryInfo.GetFiles(oldPrefix + "*.json");

        foreach (FileInfo file in files)
        {
            // 기존 파일 이름에서 일부 추출
            string existingPart = file.Name.Substring(oldPrefix.Length);

            // 일부를 수정
            string modifiedPart = existingPart.Replace(searchSubstring, replaceSubstring);

            // 새로운 파일 이름 생성
            string newFileName = oldPrefix + modifiedPart;
            string newFilePath = Path.Combine(Application.persistentDataPath, newFileName);

            // 파일 이름 변경
            file.MoveTo(newFilePath);

            Debug.Log("File renamed from " + file.Name + " to " + newFileName);
        }
    }
    public static void CopyFilesWithPartialChange(string oldPrefix, string searchSubstring, string replaceSubstring)
    {// a로 시작하는 파일이름중에 b를 c로 바꿔 복사
        DirectoryInfo directoryInfo = new DirectoryInfo(Application.persistentDataPath);
        FileInfo[] files = directoryInfo.GetFiles(oldPrefix + "*.json");

        foreach (FileInfo file in files)
        {
            // 기존 파일 이름에서 일부 추출
            string existingPart = file.Name.Substring(oldPrefix.Length);

            // 일부를 수정
            string modifiedPart = existingPart.Replace(searchSubstring, replaceSubstring);

            // 새로운 파일 이름 생성
            string newFileName = oldPrefix + modifiedPart;
            string newFilePath = Path.Combine(Application.persistentDataPath, newFileName);

            // 이미 존재하는 파일이 있다면 삭제
            if (File.Exists(newFilePath))
            {
                File.Delete(newFilePath);
                Debug.Log("Existing file deleted: " + newFilePath);
            }

            // 파일 복사
            File.Copy(file.FullName, newFilePath);

            Debug.Log("File copied from " + file.FullName + " to " + newFilePath);
        }
    }

}

