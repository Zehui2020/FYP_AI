//----------------------------------------------------------------------
// ItemInteraction
//
// �A�C�e���ƃ��[�U�[�̂������Ǘ�����N���X
//
// Data: 28/8/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using UnityEngine.EventSystems;
using UnityEngine;

public class ItemInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    // �A�C�e���G�t�F�N�g
    private ItemEffect m_itemEffect;
    // �A�C�e���w���\���N���X
    public ItemPurchaseDisplay m_itemPurchaseDisplay;

    void Start()
    {
        // �A�C�e���G�t�F�N�g���ݒ肳��Ă��Ȃ��ꍇ�̏���
        if (m_itemEffect == null)
        {
            // �A�C�e���G�t�F�N�g���擾����
            m_itemEffect = this.GetComponent<ItemEffect>();
        }
        // �A�C�e���w���\���N���X���ݒ肳��Ă��Ȃ��ꍇ�̏���
        if(m_itemPurchaseDisplay == null)
        {
            // �A�C�e���w���\���N���X��ݒ肷��
            m_itemPurchaseDisplay = FindAnyObjectByType<ItemPurchaseDisplay>();
        }
    }

    // �|�C���^�[���A�C�e���ɐG�ꂽ���̏���
    public void OnPointerEnter(PointerEventData eventData)
    {
        // �X�P�[���G�t�F�N�g�𔭓�������
        m_itemEffect.OnMouseEnter();

        // �A�C�e���w����ʂ��\������Ă���ꍇ�̏���
        if(m_itemPurchaseDisplay.GetPopupFlag() == true)
        {
            // �G�t�F�N�g�����Ȃ�
            m_itemEffect.StopEffect();
        }
    }

    // �|�C���^�[���A�C�e�����痣�ꂽ���̏���
    public void OnPointerExit(PointerEventData eventData)
    {
        // �X�P�[���G�t�F�N�g�𔭓�������
        m_itemEffect.OnMouseExit();
        // �A�C�e���w����ʂ��\������Ă���ꍇ�̏���
        if (m_itemPurchaseDisplay.GetPopupFlag() == true)
        {
            // �G�t�F�N�g�����Ȃ�
            m_itemEffect.StopEffect();
        }
    }

    // �|�C���^�[���A�C�e���ƐG��Ă��鎞�ɃN���b�N������������
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // �N���b�N���ꂽ�A�C�e����ItemDisplay���擾����
            ItemDisplay clickedItemDisplay = eventData.pointerPress.GetComponent<ItemDisplay>();

            if (clickedItemDisplay != null)
            {
                // �A�C�e���G�t�F�N�g���J�n����
                m_itemEffect.OnMouseEnter();

                // �N���b�N���ꂽ�A�C�e���̃f�[�^�Ń|�b�v�A�b�v�\������
                m_itemPurchaseDisplay.OnItemClicked(clickedItemDisplay.GetCurrentItemData(), clickedItemDisplay.GetCurrentSpriteDictionary());
                // �A�C�e���w����ʂ��\������Ă���ꍇ�̏���
                if (m_itemPurchaseDisplay.GetPopupFlag() == true)
                {
                    // �G�t�F�N�g�����Ȃ�
                    m_itemEffect.StopEffect();
                }

            }
        }
        else
        {
            // �A�C�e���G�t�F�N�g���I������
            m_itemEffect.OnMouseExit();
        }
    }
}
