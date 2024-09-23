//----------------------------------------------------------------------
// TextFadeAndMove
//
// �w�����̋��z��\������N���X
//
// Data: 17/9/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------
using System.Collections;
using TMPro;
using UnityEngine;

public class TextFadeAndMove : MonoBehaviour
{
    [Header("Fade and Move Duration")]
    public float m_duration = 2.0f;

    [Header("Move Distance")]
    public Vector3 m_moveDistance = new Vector3(0, -50, 0);

    // �A�C�e���V���b�vUI�Ǘ��N���X
    ItemShopUIHandler m_itemShopUIHandler;

    private Vector3 m_initialPosition;

    // �w�����z�\���e�L�X�g�̃R�s�[
    private GameObject m_itemPurchaseDisplayPrefab;

    // ���݂̍w�����z�\���e�L�X�g�̃R�s�[
    private GameObject m_currentitemPurchaseDisplayPrefab;

    void Start()
    {
        // �A�C�e���V���b�vUI�Ǘ��Ǘ��N���X���ݒ肳��Ă��Ȃ��ꍇ�̏���
        if(m_itemShopUIHandler == null)
        {
            // �A�C�e���V���b�vUI�Ǘ��N���X��ݒ肷��
            m_itemShopUIHandler = FindAnyObjectByType<ItemShopUIHandler>();
        }

        // �w�����z�\���e�L�X�g�̂̃R�s�[�����
        m_itemPurchaseDisplayPrefab = m_itemShopUIHandler.m_itemPurchaseDisplay.gameObject;
        // �����̐F�ƈʒu��ۑ�
        m_initialPosition = m_itemShopUIHandler.m_itemPurchaseDisplay.rectTransform.anchoredPosition;

        // �A�C�e���w���z�̕\�����\���ɂ���
        m_itemShopUIHandler.m_itemPurchaseDisplay.gameObject.SetActive(false);
    }

    // �e�L�X�g�̃t�F�[�h
    public IEnumerator FadeMoveAndResetText(string text, float price, string itemUnit)
    {
        // �V�����e�L�X�g�I�u�W�F�N�g�𐶐�
        GameObject newPurchaseDisplay = Instantiate(m_itemPurchaseDisplayPrefab, m_itemShopUIHandler.m_itemPurchaseDisplay.transform.parent);
        var textComponent = newPurchaseDisplay.GetComponent<TextMeshProUGUI>();
        textComponent.text = text + price + itemUnit;
        textComponent.color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, 1f);
        newPurchaseDisplay.SetActive(true);

        // �t�F�[�h�A�E�g���Ȃ���ړ�
        yield return StartCoroutine(FadeAndMoveText(1, 0, newPurchaseDisplay.transform.position, newPurchaseDisplay.transform.position + m_moveDistance, newPurchaseDisplay));

        // �����҂�
        yield return new WaitForSeconds(0.5f);

        // �e�L�X�g���폜
        Destroy(newPurchaseDisplay); // ���������e�L�X�g���폜
    }

    private IEnumerator FadeAndMoveText(float startAlpha, float endAlpha, Vector3 startPosition, Vector3 endPosition, GameObject textObject)
    {
        float elapsedTime = 0f;

        while (elapsedTime < m_duration)
        {
            float t = elapsedTime / m_duration;

            // �e�L�X�g�̈ʒu���Ԃ��Ĉړ�
            if (textObject != null)
            {
                textObject.transform.position = Vector3.Lerp(startPosition, endPosition, t);

                // �e�L�X�g�̓����x��ύX
                Color newColor = textObject.GetComponent<TextMeshProUGUI>().color;
                newColor.a = Mathf.Lerp(startAlpha, endAlpha, t);
                textObject.GetComponent<TextMeshProUGUI>().color = newColor;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // �ŏI�I�Ȉʒu�Ɠ����x��ݒ�
        if (textObject != null)
        {
            textObject.transform.position = endPosition;
            Color finalColor = textObject.GetComponent<TextMeshProUGUI>().color;
            finalColor.a = endAlpha;
            textObject.GetComponent<TextMeshProUGUI>().color = finalColor;
        }
    }
    // �����x�ƈʒu�����Z�b�g����
    private void ResetTextPositionAndAlpha()
    {
        // �ʒu�����ɖ߂�
        m_itemShopUIHandler.m_itemPurchaseDisplay.rectTransform.anchoredPosition = m_initialPosition;

        // �����x�����ɖ߂�
        Color resetColor = m_itemShopUIHandler.m_itemPurchaseDisplay.color;
        resetColor.a = 1f;
        m_itemShopUIHandler.m_itemPurchaseDisplay.color = resetColor;

        // �w�����̋��z���\���ɂ���
        m_itemShopUIHandler.m_itemPurchaseDisplay.gameObject.SetActive(false);
    }
}