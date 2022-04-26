using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;



// �f�[�^�t�@�C��
[System.Serializable]
public class DataFile
{
    public DataFile()
    {
        Stage = new StageData[30];  // �ꉞ30�X�e�[�W������
    }


    [System.Serializable]
    public struct StageData
    {
        public int Rank;    // �����N
        public float Time;  // ����
    }

    public StageData[] Stage;   // �X�e�[�W���̔z��
}

public class SaveDataManager : SingletonMonoBehaviour<SaveDataManager>
{
    // ���C���Z�[�u�f�[�^
    public DataFile MainData = null;

    // �p�X
    static private string Path = "Assets/SaveData/Data";


    private void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }
        DontDestroyOnLoad(this.gameObject); // �V�[�����ς���Ă����ȂȂ�
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

        // �Z�[�u�f�[�^�쐬
        if (MainData != null)
            Data = MainData;
        else
            Data = CreateDataFile();

        // �o�C�i���`���ŃV���A����
        BinaryFormatter bf = new BinaryFormatter();

        // �w�肵���p�X�Ƀt�@�C�����쐬
        FileStream file = File.Create(Path);

        // Close���m���ɌĂ΂��悤�ɗ�O������p����
        try
        {
            // �w�肵���I�u�W�F�N�g����ō쐬�����X�g���[���ɃV���A��������
            bf.Serialize(file, Data);
        }
        finally
        {
            // �t�@�C������ɂ͖����I�Ȕj�����K�v�BClose��Y��Ȃ��悤�ɂ���B
            if (file != null)
                file.Close();
        }
    }


    public void LoadData()
    {
        if (File.Exists(Path))
        {
            // �o�C�i���`���Ńf�V���A���C�Y
            BinaryFormatter bf = new BinaryFormatter();

            // �w�肵���p�X�̃t�@�C���X�g���[�����J��
            FileStream file = File.Open(Path, FileMode.Open);

            try
            {
                // �w�肵���t�@�C���X�g���[�����I�u�W�F�N�g�Ƀf�V���A���C�Y
                DataFile Data = (DataFile)bf.Deserialize(file);

                // �ǂݍ��񂾃f�[�^�𔽉f�����鏈��
                MainData = Data;
            }
            finally
            {
                // �t�@�C������ɂ͖����I�Ȕj�����K�v�BClose��Y��Ȃ��悤�ɂ���B
                if (file != null)
                    file.Close();

                Debug.Log("�Z�[�u�f�[�^�ǂݍ��݊���");
            }
        }
        else
        {
            Debug.LogWarning("�Z�[�u�f�[�^������܂���\n �V�����Z�[�u�f�[�^���쐬���܂�");
            MainData = CreateDataFile();
            SaveData();
        }
    }


    // ��Z�[�u�f�[�^���쐬
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
