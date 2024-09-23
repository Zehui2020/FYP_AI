//----------------------------------------------------------------------
// ItemEffect
//
// �A�C�e���Ɋ֘A������ʂ��Ǘ�����N���X
//
// Data: 8/28/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using System.Collections;
using UnityEngine;

public class ItemEffect : MonoBehaviour
{
    [Header("Item to apply the effect to / �G�t�F�N�g��K�p����A�C�e��")]
    public Transform m_itemTransform;
    [Header("Scale Amount / �X�P�[����")]
    public float m_scaleAmount = 1.2f;
    [Header("Time it takes to change / �ω�����̂ɂ����鎞��")]
    public float m_changeTime = 0.2f;
    [Header("Original size / ���̑傫��")]
    private Vector3 m_originalScale;

    void Start()
    {
        // ���̑傫����ۑ�����
        m_originalScale = m_itemTransform.localScale;
    }

    // �}�E�X�J�[�\�����I�u�W�F�N�g�ɓ������Ƃ��̏���
    public void OnMouseEnter()
    {
        // �i�s���̑傫���ύX���~����
        StopAllCoroutines();
        // �A�C�e�����g�傷��
        StartCoroutine(ScaleEffect(m_originalScale, m_originalScale * m_scaleAmount));
    }

    // �}�E�X���A�C�e�����痣�ꂽ�ꍇ�̏���
    public void OnMouseExit()
    {
        // �i�s���̑傫���ύX���~����
        StopAllCoroutines();
        // �A�C�e�������̑傫���ɖ߂�
        StartCoroutine(ScaleEffect(m_itemTransform.localScale, m_originalScale));
    }

    // �A�C�e���̃G�t�F�N�g���~�߂鏈��
    public void StopEffect()
    {
        // �A�C�e�������̑傫���ɖ߂�
        m_itemTransform.localScale = m_originalScale;

        // �i�s���̑傫���ύX���~����
        StopAllCoroutines();
    }


    // �A�C�e���̑傫����ύX���鏈��
    private IEnumerator ScaleEffect(Vector3 fromScale, Vector3 toScale)
    {
        float elapsedTime = 0f;
        while (elapsedTime < m_changeTime)
        {
            // ���X�ɃX�P�[����ω�������
            m_itemTransform.localScale = Vector3.Lerp(fromScale, toScale, elapsedTime / m_changeTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // �A�C�e���̑傫���ύX���w�肵���l�ɂ���
        m_itemTransform.localScale = toScale;
    }
}

