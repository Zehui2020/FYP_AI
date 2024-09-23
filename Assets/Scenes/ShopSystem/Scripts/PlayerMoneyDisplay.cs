//----------------------------------------------------------------------
// PlayerMoneyDisplay
//
// �v���C���[�̏�������\������N���X
//
// Data: 30/8/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using TMPro;
using UnityEngine;

public class PlayerMoneyDisplay : MonoBehaviour
{
    [Header("TextMeshProUGUI for displaying player's money / �v���C���[�̏������\���p")]
    public TextMeshProUGUI m_moneyText;

    // �A�C�e���}�l�[�W���[�N���X
    public ItemShopManager m_itemManager;
    private void Start()
    {
        // �A�C�e���}�l�[�W���[�N���X���ݒ肳��Ă��Ȃ��ꍇ�̏���
        if(m_itemManager == null)
        {
            // �A�C�e���}�l�[�W���[�N���X��ݒ肷��
            m_itemManager = FindAnyObjectByType<ItemShopManager>();
        }
    }

    void Update()
    {
        // �v���C���[�̏��������e�L�X�g�ɐݒ肳��Ă���ꍇ�A�v���C���[�̏������̃C���X�^���X���L���ȏꍇ�̏���
        if (m_moneyText != null && PlayerWallet.Instance != null)
        {
            // �v���C���[�̏�������ݒ肷��
            m_moneyText.text = "Money : " + PlayerWallet.Instance.GetMoney() + m_itemManager.GetItemUnit();
        }
    }
}
