using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;



// データファイル
[System.Serializable]
public class DataFile
{
    public DataFile()
    {
        Stage = new StageData[30];  // 一応30ステージ分生成
    }


    [System.Serializable]
    public struct StageData
    {
        public int Rank;    // ランク
        public float Time;  // 時間
    }

    public StageData[] Stage;   // ステージ分の配列
}

public class SaveDataManager : SingletonMonoBehaviour<SaveDataManager>
{
    // メインセーブデータ
    public DataFile MainData = null;

    // パス
    static private string Path = "Assets/SaveData/Data";


    private void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }
        DontDestroyOnLoad(this.gameObject); // シーンが変わっても死なない
    }

    public void Start()
    {
        LoadData();

        //Debug.Log(MainData.Stage[0].Rank);
        //Debug.Log(MainData.Stage[0].Time);
        //Debug.Log(MainData.Stage[1].Rank);
        //Debug.Log(MainData.Stage[1].Time);
        //Debug.Log(MainData.Stage[2].Time);
        //Debug.Log(MainData.Stage[2].Time);
    }


    public void SaveData()
    {
        DataFile Data;

        // セーブデータ作成
        if (MainData != null)
            Data = MainData;
        else
            Data = CreateDataFile();

        // バイナリ形式でシリアル化
        BinaryFormatter bf = new BinaryFormatter();

        // 指定したパスにファイルを作成
        FileStream file = File.Create(Path);

        // Closeが確実に呼ばれるように例外処理を用いる
        try
        {
            // 指定したオブジェクトを上で作成したストリームにシリアル化する
            bf.Serialize(file, Data);
        }
        finally
        {
            // ファイル操作には明示的な破棄が必要。Closeを忘れないようにする。
            if (file != null)
                file.Close();
        }
    }


    public void LoadData()
    {
        if (File.Exists(Path))
        {
            // バイナリ形式でデシリアライズ
            BinaryFormatter bf = new BinaryFormatter();

            // 指定したパスのファイルストリームを開く
            FileStream file = File.Open(Path, FileMode.Open);

            try
            {
                // 指定したファイルストリームをオブジェクトにデシリアライズ
                DataFile Data = (DataFile)bf.Deserialize(file);

                // 読み込んだデータを反映させる処理
                MainData = Data;
            }
            finally
            {
                // ファイル操作には明示的な破棄が必要。Closeを忘れないようにする。
                if (file != null)
                    file.Close();

                Debug.Log("セーブデータ読み込み完了");
            }
        }
        else
        {
            Debug.LogWarning("セーブデータがありません\n 新しいセーブデータを作成します");
            MainData = CreateDataFile();
            SaveData();
        }
    }


    // 空セーブデータを作成
    private DataFile CreateDataFile()
    {
        DataFile Data = new DataFile();

        //Data.Stage[0].Rank = 2;
        //Data.Stage[0].Time = 2;
        //Data.Stage[1].Rank = 3;
        //Data.Stage[1].Time = 3;

        return Data;
    }
}
