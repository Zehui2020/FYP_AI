//----------------------------------------------------------------------
// ItemDisplay
//
// �A�C�e����\�����邽�߂̃N���X
//
// Data: 8/28/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class ItemDisplay : MonoBehaviour
{
    //[Header("UI Image for Item Sprite / �A�C�e���X�v���C�g�\���p")]
    public Image m_itemImage;
    //[Header("TextMeshProUGUI for Item Name / �A�C�e�����\���p")]
    public TextMeshProUGUI m_itemNameText;
    //[Header("TextMeshProUGUI for Item Price / �A�C�e�����i�\���p")]
    public TextMeshProUGUI m_itemPriceText;

    // ���݂̃A�C�e���f�[�^
    private ItemData m_currentItemData;
    // ���݂̃X�v���C�g���
    private Dictionary<string, Sprite> m_currentSpriteDictionary;

    // �A�C�e���̐ݒ�
    public void Setup(ItemData itemData, Dictionary<string, Sprite> spriteDictionary, string itemUnit)
    {
        // ���݂̃A�C�e���f�[�^��ݒ肷��
        m_currentItemData = itemData;
        // ���݂̃X�v���C�g����ݒ肷��
        m_currentSpriteDictionary = spriteDictionary;

        // �A�C�e���̃C���[�W���ݒ肳��Ă���ꍇ�A�A�C�e���f�[�^����X�v���C�g����������擾���Ă���ꍇ�̏���
        if (m_itemImage != null && spriteDictionary.TryGetValue(m_currentItemData.itemSprite, out Sprite sprite))
        {
            // �A�C�e���̃X�v���C�g��ݒ肷��
            m_itemImage.sprite = sprite;
        }

        // �A�C�e���̖��O���ݒ肳��Ă���ꍇ�̏���
        if (m_itemNameText != null)
        {
            // �A�C�e���̖��O��ݒ肷��
            m_itemNameText.text = m_currentItemData.itemName;
        }

        // �A�C�e���̉��i���ݒ肳��Ă���ꍇ�̏���
        if (m_itemPriceText != null)
        {
            // �A�C�e���̉��i��ݒ肷��
            m_itemPriceText.text = m_currentItemData.itemPrice.ToString() + itemUnit;
        }
    }

    // ���݂̃A�C�e���f�[�^���擾
    public ItemData GetCurrentItemData()
    {
        return m_currentItemData;
    }

    // ���݂̃X�v���C�g�̏����擾
    public Dictionary<string, Sprite> GetCurrentSpriteDictionary()
    {
        return m_currentSpriteDictionary;
    }
}
