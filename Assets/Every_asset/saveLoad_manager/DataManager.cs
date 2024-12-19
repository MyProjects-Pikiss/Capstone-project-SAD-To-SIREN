using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    static GameObject container;

    // ---싱글톤으로 선언--- //
    static DataManager instance;
    public static DataManager Instance
    {
        get
        {
            if (!instance)
            {
                container = new GameObject();
                container.name = "DataManager";
                instance = container.AddComponent(typeof(DataManager)) as DataManager;
                DontDestroyOnLoad(container);
            }
            return instance;
        }
    }

    // --- 게임 데이터 파일이름 설정 ("원하는 이름(영문).json") --- //
    string DataNumName = "GameDataNum.json";
    string GameDataFileName0 = "GameDataA.json";
    string GameDataFileName1 = "GameDataB.json";
    string GameDataFileName2 = "GameDataC.json";
    string GameDataFileName3 = "GameDataD.json";

    // --- 저장용 클래스 변수 --- //
    public DataNumber dataNumber = new DataNumber();
    public Data0 data0 = new Data0();
    public Data0 data1 = new Data0();
    public Data0 data2 = new Data0();
    public Data0 data3 = new Data0();

    public void LoadDataNum()
    {
        string filePath = Application.persistentDataPath + "/" + DataNumName;

        // 저장된 파일이 있다면
        if (File.Exists(filePath))
        {
            // 저장된 파일 읽어오고 Json을 클래스 형식으로 전환해서 할당
            string FromJsonData = File.ReadAllText(filePath);
            dataNumber = JsonUtility.FromJson<DataNumber>(FromJsonData);
        }
    }
    public void LoadGameData0()
    {
        string filePath = Application.persistentDataPath + "/" + GameDataFileName0;

        if (File.Exists(filePath))
        {
            string FromJsonData = File.ReadAllText(filePath);
            data0 = JsonUtility.FromJson<Data0>(FromJsonData);
        }
    }
    public void LoadGameData1()
    {
        string filePath = Application.persistentDataPath + "/" + GameDataFileName1;

        if (File.Exists(filePath))
        {
            string FromJsonData = File.ReadAllText(filePath);
            data1 = JsonUtility.FromJson<Data0>(FromJsonData);
        }
    }
    public void LoadGameData2()
    {
        string filePath = Application.persistentDataPath + "/" + GameDataFileName2;

        if (File.Exists(filePath))
        {
            string FromJsonData = File.ReadAllText(filePath);
            data2 = JsonUtility.FromJson<Data0>(FromJsonData);
        }
    }
    public void LoadGameData3()
    {
        string filePath = Application.persistentDataPath + "/" + GameDataFileName3;

        if (File.Exists(filePath))
        {
            string FromJsonData = File.ReadAllText(filePath);
            data3 = JsonUtility.FromJson<Data0>(FromJsonData);
        }
    }


    public void SaveDataNum()
    {
        // 클래스를 Json 형식으로 전환 (true : 가독성 좋게 작성)
        string ToJsonData = JsonUtility.ToJson(dataNumber, true);
        string filePath = Application.persistentDataPath + "/" + DataNumName;

        // 이미 저장된 파일이 있다면 덮어쓰고, 없다면 새로 만들어서 저장
        File.WriteAllText(filePath, ToJsonData);
    }
    public void SaveGameData0()
    {
        string ToJsonData = JsonUtility.ToJson(data0, true);
        string filePath = Application.persistentDataPath + "/" + GameDataFileName0;

        File.WriteAllText(filePath, ToJsonData);
    }
    public void SaveGameData1()
    {
        string ToJsonData = JsonUtility.ToJson(data1, true);
        string filePath = Application.persistentDataPath + "/" + GameDataFileName1;

        File.WriteAllText(filePath, ToJsonData);
    }
    public void SaveGameData2()
    {
        string ToJsonData = JsonUtility.ToJson(data2, true);
        string filePath = Application.persistentDataPath + "/" + GameDataFileName2;

        File.WriteAllText(filePath, ToJsonData);
    }
    public void SaveGameData3()
    {
        string ToJsonData = JsonUtility.ToJson(data3, true);
        string filePath = Application.persistentDataPath + "/" + GameDataFileName3;

        File.WriteAllText(filePath, ToJsonData);
    }
}