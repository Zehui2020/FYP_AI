//----------------------------------------------------------------------
// ItemShopManager
//
// �A�C�e�����Ǘ�����N���X
//
// Data: 8/28/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEditor.Rendering;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using System.Collections;

public class ItemShopManager : MonoBehaviour
{
    [Header("�A�C�e���̏����z�u���W")]
    public Vector2 m_itemStartPosition;

    [Header("Item Spacing / �A�C�e���Ԋu")]
    public Vector2 m_itemSpacing;

    [Header("Number of Columns / ��̐�")]
    public int m_numberOfColumns = 4; // Defaul: 4

    [Header("�A�C�e���̃X�P�[��")]
    public float m_itemScale = 1.0f; // Defaul: 1.0

    // �A�C�e���V���b�vUI�Ǘ��N���X
    ItemShopUIHandler m_itemShopUIHandler;

    // �A�C�e���̒P��
    private string m_itemUnit = " G";

    // �A�C�e���f�[�^�z��
    private ItemData[] m_itemDataArray;

    // �A�C�e���ǂݍ��݃N���X
    public ItemLoader m_itemLoader;


    private IEnumerator Start()
    {
        // �A�C�e���V���b�vUI�Ǘ��N���X���ݒ肳��Ă��Ȃ��ꍇ�̏���
        if (m_itemShopUIHandler == null)
        {
            // �A�C�e���V���b�vUI�Ǘ��N���X��ݒ肷��
            m_itemShopUIHandler = FindAnyObjectByType<ItemShopUIHandler>();
        }

        // �A�C�e���ǂݍ��݃N���X���ݒ肳��Ă��Ȃ��ꍇ�̏���
        if (m_itemLoader == null)
        {
            // �A�C�e���ǂݍ��݃N���X��ݒ肷��
            m_itemLoader = FindAnyObjectByType<ItemLoader>();
        }

        // �o���{�^�����ݒ肳��Ă���ꍇ�̏���
        if (m_itemShopUIHandler.m_exitTextButton != null)
        {
            // �{�^�����N���b�N���ꂽ����SwitchShopDisplay���Ăяo��
            m_itemShopUIHandler.m_exitTextButton.onClick.AddListener(SwitchShopDisplay);
        }

        // �A�C�e���ǂݍ��݃N���X���ݒ肳��Ă���ꍇ�̏���
        if (m_itemLoader != null)
        {
            // �A�C�e���ǂݍ��݂���������܂ő҂�
            yield return new WaitUntil(() => m_itemLoader.IsDataLoaded());

            // �A�C�e����\������
            ShowShop();
        }

        // �f�o�b�O�{�^��
        if (m_itemShopUIHandler.m_debugButton != null)
        {
            m_itemShopUIHandler.m_debugButton.onClick.AddListener(SwitchShopDisplay);
        }
    }


    // �V���b�v�̕\���؂�ւ�
    public void SwitchShopDisplay()
    {
        // �V���b�v���\������Ă���ꍇ�̏���
        if (m_itemShopUIHandler.m_itemShopBackground.activeSelf)
        {
            // �V���b�v���\������
            HideShop();
        }
        // �V���b�v����\���ɂȂ��Ă���ꍇ�̏���
        else
        {
            // �V���b�v��\���ɂ���
            ShowShop();
        }
    }

    // �V���b�v��\��
    private void ShowShop()
    {
        // �A�C�e�����Đݒ肷��
        SetItem();
        // �V���b�v��\������
        m_itemShopUIHandler.m_itemShopBackground.SetActive(true);
    }

    // �V���b�v���\��
    private void HideShop()
    {
        // �V���b�v���\���ɂ���
        m_itemShopUIHandler.m_itemShopBackground.SetActive(false);
        // �A�C�e�����폜����
        ClearItems();
    }

    // �A�C�e���̐ݒ�
    private void SetItem()
    {
        // �V���b�v��RectTransform���擾
        RectTransform parentRectTransform = m_itemShopUIHandler.m_itemShopBackground.GetComponent<RectTransform>();
        float parentWidth = parentRectTransform.rect.width;
        float parentHeight = parentRectTransform.rect.height;

        // �A�C�e���f�[�^�z����擾
        m_itemDataArray = m_itemLoader.GetItemDataArray();

        if (m_itemDataArray != null && m_itemDataArray.Length > 0)
        {
            // �A�C�e����z�u����
            ArrangeItems(parentWidth, parentHeight);
        }
    }

    // �A�C�e���̔z�u
    private void ArrangeItems(float parentWidth, float parentHeight)
    {
        int row = 0;
        int column = 0;
        float itemStartXPosition = m_itemStartPosition.x;
        float itemStartYPosition = m_itemStartPosition.y;

        foreach (ItemData itemData in m_itemDataArray)
        {
            // �A�C�e���̐����Ɛݒ肷��
            RectTransform rectTransform = CreateItem(itemData);

            // �A�C�e����z�u����
            PlaceItem(rectTransform, ref itemStartXPosition, ref itemStartYPosition, row, column);

            // ��ƍs�̍X�V������
            UpdateRowAndColumn(ref row, ref column, rectTransform, ref itemStartXPosition, ref itemStartYPosition);
        }
    }

    // �A�C�e���̐���
    private RectTransform CreateItem(ItemData itemData)
    {
        GameObject itemPrefab = Instantiate(m_itemShopUIHandler.m_itemPrefab, m_itemShopUIHandler.m_itemShop.transform);

        ItemDisplay itemDisplay = itemPrefab.GetComponent<ItemDisplay>();
        if (itemDisplay != null)
        {
            itemDisplay.Setup(itemData, m_itemLoader.GetSpriteDictionary(), m_itemUnit);
        }

        RectTransform rectTransform = itemPrefab.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.0f, 1.0f);
        rectTransform.anchorMax = new Vector2(0.0f, 1.0f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.localScale *= m_itemScale;

        return rectTransform;
    }

    // �A�C�e���̔z�u
    private void PlaceItem(RectTransform rectTransform, ref float itemStartXPosition, ref float itemStartYPosition, int row, int column)
    {
        rectTransform.anchoredPosition = new Vector2(
            itemStartXPosition + rectTransform.sizeDelta.x * rectTransform.pivot.x,
            -itemStartYPosition - rectTransform.sizeDelta.y * rectTransform.pivot.y
        );
    }

    // �s�Ɨ�̍X�V
    private void UpdateRowAndColumn(ref int row, ref int column, RectTransform rectTransform, ref float itemStartXPosition, ref float itemStartYPosition)
    {
        column++;
        if (column >= m_numberOfColumns)
        {
            column = 0;
            row++;
            itemStartXPosition = m_itemStartPosition.x;
            itemStartYPosition += rectTransform.sizeDelta.y + m_itemSpacing.y;
        }
        else
        {
            itemStartXPosition += rectTransform.sizeDelta.x + m_itemSpacing.x;
        }
    }

    // �A�C�e���̍폜
    private void ClearItems()
    {
        // �V���b�v�̃A�C�e�����擾����(�A�C�e����ݒ肵�Ă���e��ItemShopObject)
        Transform shopCanvas = m_itemShopUIHandler.m_itemShopBackground.transform.Find("ItemShopObject");
        // �A�C�e�����X�g�̐e�I�u�W�F�N�g���̑S�Ă̎q�I�u�W�F�N�g���폜����
        foreach (Transform child in shopCanvas)
        {
            Destroy(child.gameObject);
        }
    }

    // �A�C�e���P�ʂ̎擾
    public string GetItemUnit()
    {
        return m_itemUnit;
    }

    // �A�C�e���f�[�^���X�g��Ԃ����\�b�h
    public ItemDataList GetItemDataList()
    {
        // ItemDataList��V�����쐬���A���݂̃A�C�e���f�[�^��ݒ肷��
        ItemDataList itemDataList = new ItemDataList();
        itemDataList.items = m_itemDataArray;
        return itemDataList;
    }
}