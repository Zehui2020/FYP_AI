//----------------------------------------------------------------------
// ItemLoader
//
// �A�C�e����ǂݍ��ރN���X
//
// Data: 9/13/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class ItemData
{
    // json�t�@�C���œǂݍ��ނ��ߖ��O��json�t�@�C���Ɠ������O���g�p����
    [Header("Item Sprite Path / �A�C�e���X�v���C�g�̃p�X")]
    public string itemSprite;
    [Header("Item Name / �A�C�e����")]
    public string itemName;
    [Header("Item Price / �A�C�e���̒l�i")]
    public int itemPrice;
    [Header("�A�C�e���̌�")]
    public int itemQuantity;
}

[System.Serializable]
public class ItemDataList
{
    // json�t�@�C���œǂݍ��ނ��ߖ��O��json�t�@�C���Ɠ������O���g�p����
    [Header("List of Items / �A�C�e���̃��X�g")]
    public ItemData[] items;
}

public class ItemLoader : MonoBehaviour
{
    [Header("Path to JSON File / JSON�t�@�C���̃p�X")]
    public string m_jsonFilePath = "items";

    // �A�C�e���f�[�^�̔z��
    private ItemData[] m_itemDataArray;
    // �A�C�e���f�[�^
    public ItemData m_itemData;
    // �A�C�e���f�[�^���X�g
    public ItemDataList m_itemDataList;
    // �X�v���C�g�𖼑O�ŊǗ����鎫��
    private Dictionary<string, Sprite> m_spriteDictionary;

    private void Start()
    {
        // �X�v���C�g���ꊇ�Ń��[�h���Ď����Ɋi�[����
        LoadAllSprites();

        // JSON�t�@�C������A�C�e���f�[�^��ǂݍ���
        m_itemDataList = LoadItemData();
        // �A�C�e���f�[�^���X�g���ݒ肳��Ă���ꍇ�̏���
        if (m_itemDataList != null)
        {
            // �A�C�e����ݒ肷��
            m_itemDataArray = m_itemDataList.items;
        }

    }

    // ���ׂẴX�N���v�g��ǂݍ���
    private void LoadAllSprites()
    {
        // "items"�t�H���_����S�ẴX�v���C�g��ǂݍ���
        Sprite[] sprites = Resources.LoadAll<Sprite>("items");

        // �X�v���C�g�̖��O�ƃX�v���C�g�{�̂��i�[���鎫��������������
        m_spriteDictionary = new Dictionary<string, Sprite>();

        // �ǂݍ��񂾃X�v���C�g�������ɒǉ�����
        foreach (Sprite sprite in sprites)
        {
            m_spriteDictionary[sprite.name] = sprite;
        }
    }

    // �A�C�e����ǂݍ���
    private ItemDataList LoadItemData()
    {
        // Resources�t�H���_����JSON�t�@�C����ǂݍ���
        TextAsset jsonText = Resources.Load<TextAsset>(m_jsonFilePath);

        // JSON�f�[�^��ItemDataList�ɕϊ�
        ItemDataList itemDataList = JsonUtility.FromJson<ItemDataList>(jsonText.text);
        return itemDataList;
    }

    // �f�[�^�����[�h����Ă��邩���m�F
    public bool IsDataLoaded()
    {
        // �A�C�e���f�[�^�ƃX�v���C�g���������[�h����Ă��邩�ǂ������m�F����
        return m_itemDataArray != null && m_itemDataArray.Length > 0 && m_spriteDictionary != null && m_spriteDictionary.Count > 0;
    }

    // �A�C�e���f�[�^�擾
    public ItemData GetItemData()
    {
        return m_itemData;
    }
    // �A�C�e���f�[�^�z��擾
    public ItemData[] GetItemDataArray()
    {
        return m_itemDataArray;
    }
    // �X�v���C�g�擾
    public Dictionary<string, Sprite> GetSpriteDictionary()
    {
        return m_spriteDictionary;
    }
}