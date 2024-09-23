//----------------------------------------------------------------------
// PlayerWallet
//
// �v���C���[�̏��������Ǘ�����N���X
//
// Data: 2024/08/30
// Author: Shimba Sakai
//----------------------------------------------------------------------

using System.IO;
using UnityEngine;

public class PlayerWallet : MonoBehaviour
{
    // �v���C���[�̏�����
    public static PlayerWallet Instance { get; private set; }

    // �Q�[�����J�n���ꂽ�Ƃ��̂�Resources��json�t�@�C������ǂݍ���
    [Header("JSON File Name in Resources / Resources�t�H���_����JSON�t�@�C����")]
    public string m_jsonFileName = "money";

    // ���݂̃v���C���[�̏�����
    private int m_currentMoney;

    void Start()
    {
        // �ۑ���m�F�p
        Debug.Log("Persistent Data Path: " + Application.persistentDataPath);
    }

    void Awake()
    {
        // �V���O���g���p�^�[���̎���: �C���X�^���X���ݒ肳��Ă��Ȃ��ꍇ�̏���
        if (Instance == null)
        {
            // �C���X�^���X�Ƃ��ăI�u�W�F�N�g��ݒ肷��
            Instance = this;
            // �V�[�����؂�ւ���Ă��I�u�W�F�N�g��j�󂵂Ȃ�
            DontDestroyOnLoad(gameObject);
            // �N������JSON���珊������ǂݍ���
            LoadMoney();
        }
        else
        {
            // �d������C���X�^���X��j�󂷂�
            Destroy(gameObject);
        }
    }

    // �v���C���[�̏��������擾���鏈��
    public int GetMoney()
    {
        // �v���C���[�̌��݂̏��������擾����
        return m_currentMoney;
    }

    // �v���C���[�̏�������ǉ����鏈��
    public void AddMoney(int amount)
    {
        // �v���C���[�̏�������ǉ�������ɕۑ�����
        m_currentMoney += amount;
        // �v���C���[�̏�������ۑ�����
        SaveMoney();
    }

    // �v���C���[�̏��������g�p���鏈��
    public bool SpendMoney(int amount)
    {
        // �v���C���[�̏��������w���z�ȏ�̏ꍇ�̏���
        if (m_currentMoney >= amount)
        {
            // �v���C���[�̏���������w�肳�ꂽ�z�����炷
            m_currentMoney -= amount;
            // �v���C���[�̏�������ۑ�����
            SaveMoney();
            // �w���𐬌�������
            return true;
        }
        // �v���C���[�̏��������w���z�����̏ꍇ�̏���
        else
        {
            // �w�������s������
            return false;
        }
    }

    // �v���C���[�̏�������ۑ����鏈��
    private void SaveMoney()
    {
        // ��������ۑ����邽�߂̃f�[�^�N���X�̃C���X�^���X���쐬���A���݂̏�������ݒ肷��
        var data = new PlayerWalletData { money = m_currentMoney };

        // �f�[�^�N���X��JSON�`���̕�����ɕϊ�����
        string json = JsonUtility.ToJson(data);

        // JSON�f�[�^��ۑ�����t�@�C���̃p�X�����肷��
        string path = Path.Combine(Application.persistentDataPath, m_jsonFileName + ".json");

        // JSON�f�[�^���t�@�C���ɏ�������
        File.WriteAllText(path, json);
    }

    // �v���C���[�̏�������ǂݍ��ޏ���
    private void LoadMoney()
    {
        // �������f�[�^��ۑ�����t�@�C���̃p�X�����肷��
        string path = Path.Combine(Application.persistentDataPath, m_jsonFileName + ".json");

        // �w�肵���p�X�Ƀt�@�C�������݂���ꍇ�̏���
        if (File.Exists(path))
        {
            // �t�@�C������JSON�f�[�^��ǂݍ���
            string json = File.ReadAllText(path);

            // JSON�f�[�^���v���C���[�̏������f�[�^�ɕϊ�����
            PlayerWalletData data = JsonUtility.FromJson<PlayerWalletData>(json);

            // �ǂݍ��񂾏������f�[�^�����݂̏������Ƃ��Đݒ肷��
            m_currentMoney = data.money;
        }
        // �t�@�C�������݂��Ȃ��ꍇ�̏���
        else
        {
            // �f�t�H���g�̏������i10000G�j��ݒ肷��
            m_currentMoney = 10000;
        }
    }
}

// �v���C���[�̏�������ۑ����邽�߂̃f�[�^�N���X
[System.Serializable]
public class PlayerWalletData
{
    public int money;
}
