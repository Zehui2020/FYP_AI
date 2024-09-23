//----------------------------------------------------------------------
// ItemPurchaseDisplay
//
// �A�C�e���̍w���\�����Ǘ�����N���X
//
// Data: 2/9/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.EventSystems;

public class ItemPurchaseDisplay : MonoBehaviour
{
    [Header("�A�C�e���̃C���[�W�摜��`�悷����W")]
    public Vector2 m_ItemImageTexturePosition;

    // �A�C�e���f�[�^
    private ItemData m_itemData;
    // ���݂̃A�C�e���I�u�W�F�N�g
    private GameObject m_currentItemObject;
    // �|�b�v�A�b�v���Ă��邩�ǂ����̃t���O
    private bool m_isPopupActive = false;
    // �A�C�e���̌�
    private int m_itemQuantity = 1;
    // ���݂̃A�C�e���̌�
    private int m_currentItemMQuantity;
    // �A�C�e���̌��̍ŏ��l
    private const int ITEMQUANTITY_MIN = 1;
    // �A�C�e���̌��̏����l
    private const int ITEMQUANTITY_INITIAL = 1;
    // �A�C�e���̍��v���z
    private int m_itemTotalPrice;


    // �{�^��������������Ă��邩�̃t���O
    private bool m_isPlusButtonHeld = false;
    private bool m_isMinusButtonHeld = false;
    private float m_holdDuration = 0.0f;
    private const float HOLD_THRESHOLD = 0.5f;  // ��������臒l
    private const float HOLD_REPEAT_RATE = 0.1f;  // ��������̌J��Ԃ����x

    // �A�C�e���}�l�[�W���[�N���X
    public ItemShopManager m_itemManager;

    // �w���z�\���p�N���X
    TextFadeAndMove m_textFadeAndMove;

    // �A�C�e���V���b�vUI�Ǘ��N���X
    ItemShopUIHandler m_itemShopUIHandler;

    void Start()
    {
        // �A�C�e���V���b�vUI�Ǘ��N���X���ݒ肳��Ă��Ȃ��ꍇ�̏���
        if (m_itemShopUIHandler == null)
        {
            // �A�C�e���V���b�vUI�Ǘ��N���X��ݒ肷��
            m_itemShopUIHandler = FindAnyObjectByType<ItemShopUIHandler>();
        }

        // �A�C�e���}�l�[�W���[�N���X���ݒ肳��Ă��Ȃ��ꍇ�̏���
        if (m_itemManager == null)
        {
            // �A�C�e���}�l�[�W���[�N���X��ݒ肷��
            m_itemManager = FindAnyObjectByType<ItemShopManager>();
        }

        // �A�C�e���w���z�\���p�N���X���ݒ肳��Ă��Ȃ��ꍇ�̏���
        if(m_textFadeAndMove == null)
        {
            // �A�C�e���w���z�\���p�N���X��ݒ肷��
            m_textFadeAndMove = FindAnyObjectByType<TextFadeAndMove>();
        }

        // �|�b�v�A�b�v�p�l�����\���ɂ���
        m_itemShopUIHandler.m_itemShopBackground.SetActive(false);
        // �w���{�^���������ꂽ���̏���
        m_itemShopUIHandler.m_purchaseButton.onClick.AddListener(PurchaseButtonClicked);
        // �L�����Z���{�^���������ꂽ���̏���
        m_itemShopUIHandler.m_cancelButton.onClick.AddListener(CancelButtonClicked);

        // �A�C�e���̌��𑝂₷�{�^���������ꂽ���̏���
        m_itemShopUIHandler.m_plusButton.onClick.AddListener(IncreaseQuantity);

        // +�{�^����EventTrigger��ǉ����āA���������̏�����ݒ�
        var plusButtonTrigger = m_itemShopUIHandler.m_plusButton.gameObject.AddComponent<EventTrigger>();

        // PointerDown�C�x���g�i�{�^���������ꂽ���j�̏���
        var plusDownEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        plusDownEntry.callback.AddListener((data) => { StartButtonHold(true); }); // �������J�n
        plusButtonTrigger.triggers.Add(plusDownEntry);

        // PointerUp�C�x���g�i�{�^���������ꂽ���j�̏���
        var plusUpEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        plusUpEntry.callback.AddListener((data) => { StopButtonHold(); }); // ��������~
        plusButtonTrigger.triggers.Add(plusUpEntry);

        // �A�C�e���̌������炷�{�^���������ꂽ���̏���
        m_itemShopUIHandler.m_minusButton.onClick.AddListener(DecreaseQuantity);

        // -�{�^����EventTrigger��ǉ����āA���������̏�����ݒ�
        var minusButtonTrigger = m_itemShopUIHandler.m_minusButton.gameObject.AddComponent<EventTrigger>();

        // PointerDown�C�x���g�i�{�^���������ꂽ���j�̏���
        var minusDownEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        minusDownEntry.callback.AddListener((data) => { StartButtonHold(false); }); // �������J�n
        minusButtonTrigger.triggers.Add(minusDownEntry);

        // PointerUp�C�x���g�i�{�^���������ꂽ���j�̏���
        var minusUpEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        minusUpEntry.callback.AddListener((data) => { StopButtonHold(); }); // ��������~
        minusButtonTrigger.triggers.Add(minusUpEntry);


        // ������Ԃōw����ʂ��\���ɂ���
        m_itemShopUIHandler.m_purchaseScreenBackground.SetActive(false);
        // ������Ԃōw�����ʂ̃��b�Z�[�W���\���ɂ���
        m_itemShopUIHandler.m_itemPurchaseMessage.gameObject.SetActive(false);


    }

    // �A�C�e�����N���b�N�������̏���
    public void OnItemClicked(ItemData itemdata, Dictionary<string, Sprite> spriteDictionary)
    {
        // �|�b�v�A�b�v���\������Ă���Ԃ͑��̃A�C�e�����N���b�N�ł��Ȃ��悤�ɂ���
        if (m_isPopupActive)
        {
            return;
        }

        // �O��̃A�C�e�����c���Ă���ꍇ�̏���
        if (m_currentItemObject != null)
        {
            // �O��̃A�C�e�����폜����
            Destroy(m_currentItemObject);
        }

        // �A�C�e���̃C���[�W�摜��RectTransform���擾����
        RectTransform rectTransform = m_itemShopUIHandler.m_forDisplayingItemImages.GetComponent<RectTransform>();

        // �A���J�[�ƃs�{�b�g�̐ݒ�𒆉��ɕύX����
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        // �e�I�u�W�F�N�g����Ƃ��A�w�肵�����W���ړ�������
        rectTransform.anchoredPosition += new Vector2(m_ItemImageTexturePosition.x, -m_ItemImageTexturePosition.y);

        // �A�C�e���f�[�^��ݒ肷��
        m_itemData = itemdata;

        // �A�C�e���̃C���[�W�摜���ݒ肳��Ă���ꍇ�A�A�C�e���f�[�^����X�v���C�g����������擾�ł���ꍇ�̏���
        if (m_itemShopUIHandler.m_forDisplayingItemImages != null && spriteDictionary.TryGetValue(m_itemData.itemSprite, out Sprite sprite))
        {
            // �A�C�e���̃C���[�W�摜��ݒ肷��
            m_itemShopUIHandler.m_forDisplayingItemImages.sprite = sprite;
        }
        // �A�C�e���̖��O��ݒ肷��
        m_itemShopUIHandler.m_itemName.text = itemdata.itemName;
        // �A�C�e��1�̒l�i��ݒ肷��
        m_itemShopUIHandler.m_itemPerPrice.text = itemdata.itemPrice.ToString() + m_itemManager.GetItemUnit();
        // �A�C�e���̌���ݒ肷��
        m_currentItemMQuantity = itemdata.itemQuantity;
        // �A�C�e���̌����X�V����
        UpdateItemQuantity();
        // ���v���z���X�V����
        UpdateTotalPrice();
        // �|�b�v�A�b�v�p�l����\������
        m_itemShopUIHandler.m_purchaseScreenBackground.SetActive(true);
        // �|�b�v�A�b�v���A�N�e�B�u�ɐݒ肷��
        m_isPopupActive = true;
    }

    // �w������A�C�e���̌��𑝂₷
    private void IncreaseQuantity()
    {
        // �A�C�e���̌��̏���l�����߂�
        if (m_itemQuantity < m_currentItemMQuantity)
        {
            // �A�C�e���̌��𑝂₷
            m_itemQuantity++;
            UpdateItemQuantity();
            // ���v���z���X�V����
            UpdateTotalPrice();
        }

    }

    // �w������A�C�e���̌������炷
    private void DecreaseQuantity()
    {
        // �A�C�e���̌����ŏ��l�����ɂȂ����ꍇ�̏���
        if (m_itemQuantity > ITEMQUANTITY_MIN)
        {
            // �A�C�e���̌������炷
            m_itemQuantity--;
            UpdateItemQuantity();
            // ���v���z���X�V����
            UpdateTotalPrice();
        }
    }

    // �A�C�e���̌����X�V����
    private void UpdateItemQuantity()
    {
        // �A�C�e���̌���\������e�L�X�g���ݒ肳��Ă���ꍇ�̏���
        if (m_itemShopUIHandler.m_itemQuantity != null)
        {
            // �A�C�e���̌���ݒ肷��
            m_itemShopUIHandler.m_itemQuantity.text = "x" + m_itemQuantity.ToString();
        }
    }

    // ���v���z���X�V����
    private void UpdateTotalPrice()
    {
        // �A�C�e���f�[�^���ݒ肳��Ă���ꍇ�̏���
        if (m_itemData != null)
        {
            // ���v���z��ݒ肷��
            m_itemTotalPrice = m_itemData.itemPrice * m_itemQuantity;
            m_itemShopUIHandler.m_itemTotalPrice.text = m_itemTotalPrice.ToString() + m_itemManager.GetItemUnit(); ;

            // �v���C���[�̏����������w�����z�����������ꍇ�̏���
            if (PlayerWallet.Instance.GetMoney() < m_itemTotalPrice)
            {
                // �e�L�X�g�̐F��ԐF�ɂ���
                m_itemShopUIHandler.m_itemTotalPrice.color = Color.red;
            }
            // �v���C���[�̏����������w�����z���Ⴉ�����ꍇ�̏���
            else
            {
                // �e�L�X�g�̐F�𔒐F�ɂ���
                m_itemShopUIHandler.m_itemTotalPrice.color = Color.white;
            }
        }
    }

    // �w���{�^���̏���
    public void PurchaseButtonClicked()
    {
        // �v���C���[�̏��������w�����z��菭�Ȃ��ꍇ�̏���
        if (m_itemTotalPrice > PlayerWallet.Instance.GetMoney())
        {
            // �w���Ɏ��s���܂���
            m_itemShopUIHandler.m_itemPurchaseMessage.text = "Purchase failed";
            m_itemShopUIHandler.m_itemPurchaseMessage.color = Color.red;
        }
        else
        {
            // �A�C�e�����w������
            PurchaseItem(m_itemQuantity);
            // �w���ɐ������܂���
            m_itemShopUIHandler.m_itemPurchaseMessage.text = "Purchase successful";
            m_itemShopUIHandler.m_itemPurchaseMessage.color = Color.green;


            // �w�����z���e�L�X�g�Ƃ��ĕ\��
            StartCoroutine(m_textFadeAndMove.FadeMoveAndResetText("-", m_itemTotalPrice, m_itemManager.GetItemUnit()));

        }

        // �w�����ʂ̃��b�Z�[�W��\������
        m_itemShopUIHandler.m_itemPurchaseMessage.gameObject.SetActive(true);

        // ���b�Z�[�W����莞�Ԍ�(�b)�ɔ�\���ɂ���
        StartCoroutine(HideMessageAfterDelay(2.0f));
    }

    // �{�^��������������n�߂����̏���
    private void StartButtonHold(bool isPlusButton)
    {
        // �������t���O��ݒ肷��
        if (isPlusButton)
        {
            m_isPlusButtonHeld = true;
        }
        else
        {
            m_isMinusButtonHeld = true;
        }

        // �R���[�`�����J�n����
        StartCoroutine(HoldButtonCoroutine(isPlusButton));
    }

    // �{�^�������������ꂽ���̏���
    private IEnumerator HoldButtonCoroutine(bool isPlusButton)
    {
        // ��莞�Ԓ����������܂őҋ@
        yield return new WaitForSeconds(HOLD_THRESHOLD);

        // �{�^���������ꑱ���Ă���Ԃ̏���
        while (isPlusButton ? m_isPlusButtonHeld : m_isMinusButtonHeld)
        {
            // �A�C�e���̌��𑝌�����
            if (isPlusButton)
            {
                IncreaseQuantity();
            }
            else
            {
                DecreaseQuantity();
            }

            // �J��Ԃ����x�𐧌�
            yield return new WaitForSeconds(HOLD_REPEAT_RATE);
        }
    }

    // �{�^���������ꂽ���̏���
    private void StopButtonHold()
    {
        // �������t���O����������
        m_isPlusButtonHeld = false;
        m_isMinusButtonHeld = false;
    }


    // �w������A�C�e���̏���
    private void PurchaseItem(int quantity)
    {
        // �A�C�e���f�[�^���ݒ肳��Ă���ꍇ�̏���
        if (m_itemData != null)
        {
            // �v���C���[�̏��������猸�炷
            PlayerWallet.Instance.SpendMoney(m_itemData.itemPrice * quantity);
        }
        // ���������I�u�W�F�N�g���폜����
        DestroyObject();
        // �A�C�e���̌�������������
        m_itemQuantity = ITEMQUANTITY_INITIAL;
    }

    // �L�����Z���{�^���̏���
    public void CancelButtonClicked()
    {
        // �A�C�e�����w�����邱�Ƃ���߂�
        CancelItem();
    }

    // �L�����Z������A�C�e���̏���
    void CancelItem()
    {
        // ���������I�u�W�F�N�g���폜����
        DestroyObject();
        // �A�C�e���̌�������������
        m_itemQuantity = ITEMQUANTITY_INITIAL;
    }

    // ��莞�Ԍo�ߌ�Ƀ��b�Z�[�W���\���ɂ��鏈��
    private IEnumerator HideMessageAfterDelay(float delay)
    {
        // �w�肵�����ԑҋ@����
        yield return new WaitForSeconds(delay);

        // ���b�Z�[�W���\���ɂ���
        m_itemShopUIHandler.m_itemPurchaseMessage.gameObject.SetActive(false);
    }

    // �I�u�W�F�N�g����������
    void DestroyObject()
    {
        // �|�b�v�A�b�v���\���ɂ���
        m_itemShopUIHandler.m_purchaseScreenBackground.SetActive(false);

        // �A�C�e���摜���ݒ肳��Ă���ꍇ�̏���
        if (m_itemShopUIHandler.m_forDisplayingItemImages != null)
        {
            // �A�C�e���摜�̃X�v���C�g���폜����
            m_itemShopUIHandler.m_forDisplayingItemImages.sprite = null;
        }

        // �A�C�e���̖��O���ݒ肳��Ă���ꍇ�̏���
        if (m_itemShopUIHandler.m_itemName != null)
        {
            // �A�C�e���̖��O���폜����
            m_itemShopUIHandler.m_itemName.text = string.Empty;
        }

        // �A�C�e���̒P�����ݒ肳��Ă���ꍇ�̏���
        if (m_itemShopUIHandler.m_itemPerPrice != null)
        {
            // �A�C�e���̒P�����폜����
            m_itemShopUIHandler.m_itemPerPrice.text = string.Empty;
        }

        // �A�C�e���̍��v���z���ݒ肳��Ă���ꍇ�̏���
        if (m_itemShopUIHandler.m_itemTotalPrice != null)
        {
            // �A�C�e���̍��v���z���폜����
            m_itemShopUIHandler.m_itemTotalPrice.text = string.Empty;
        }

        // �A�C�e���̌����ݒ肳��Ă���ꍇ�̏���
        if (m_itemShopUIHandler.m_itemQuantity != null)
        {
            // �A�C�e���̌����폜����
            m_itemShopUIHandler.m_itemQuantity.text = string.Empty; // �A�C�e���̌����N���A
        }

        // ���݂̃A�C�e���I�u�W�F�N�g���ݒ肳��Ă���ꍇ�̏���
        if (m_currentItemObject != null)
        {
            Destroy(m_currentItemObject);
            m_currentItemObject = null;
        }

        // �|�b�v�A�b�v���A�N�e�B�u�ɂ���
        m_isPopupActive = false;

        // �A�C�e���̌��������l�ɂ���
        m_itemQuantity = ITEMQUANTITY_INITIAL;
    }


    // �|�b�v�A�b�v���Ă��邩�ǂ����̃t���O���擾����
    public bool GetPopupFlag()
    {
        // �|�b�v�A�b�v�t���O��Ԃ�
        return m_isPopupActive;
    }
}