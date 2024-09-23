//----------------------------------------------------------------------
// ItemShopUIHandler
//
// �A�C�e���V���b�vUI�Ǘ��N���X
//
// Data: 19/9/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemShopUIHandler : MonoBehaviour
{
    // GameObject start here ----------------------------------------------

    [Header("�A�C�e����Prefab")]
    public GameObject m_itemPrefab;

    [Header("�w����ʂ̔w�i�p�l��")]
    public GameObject m_purchaseScreenBackground;

    [Header("�A�C�e���V���b�v")]
    public GameObject m_itemShop;

    [Header("�A�C�e���V���b�v�̔w�i")]
    public GameObject m_itemShopBackground;

    // GameObject end here ------------------------------------------------





    // Button start here --------------------------------------------------

    [Header("�o���{�^��")]
    public Button m_exitTextButton;

    [Header("�f�o�b�O�p�{�^��")]
    public Button m_debugButton;

    [Header("�A�C�e���̌��𑝂₷�{�^��")]
    public Button m_plusButton;

    [Header("�A�C�e���̌������炷�{�^��")]
    public Button m_minusButton;

    [Header("�w���{�^��")]
    public Button m_purchaseButton;

    [Header("�L�����Z���{�^��")]
    public Button m_cancelButton;

    // Button end here ----------------------------------------------------





    // Image start here ---------------------------------------------------

    //[Header("�A�C�e���̃C���[�W�摜�\���p")]
    public Image m_forDisplayingItemImages;

    // Image end here -----------------------------------------------------





    // TextMeshProUGUI start here -----------------------------------------

    [Header("�A�C�e���̖��O")]
    public TextMeshProUGUI m_itemName;

    [Header("�A�C�e���̒P��")]
    public TextMeshProUGUI m_itemPerPrice;

    [Header("�A�C�e���̍��v���z")]
    public TextMeshProUGUI m_itemTotalPrice;

    [Header("�A�C�e���̌�")]
    public TextMeshProUGUI m_itemQuantity;

    [Header("�A�C�e���w�����̃��b�Z�[�W")]
    public TextMeshProUGUI m_itemPurchaseMessage;

    //[Header("�A�C�e���w���z�̕\��")]
    public TextMeshProUGUI m_itemPurchaseDisplay;

    // TextMeshProUGUI end here -------------------------------------------

}
