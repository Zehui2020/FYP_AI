//----------------------------------------------------------------------
// SceneExitHandler
//
// �V�[���I�����Ǘ�����N���X
//
// Data: 23/9/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using UnityEngine;

public class SceneExitHandler : MonoBehaviour
{
    // �A�C�e���V���b�vUI�Ǘ��N���X
    ItemShopUIHandler m_itemShopUIHandler;

    void Start()
    {
        // �A�C�e���V���b�vUI�Ǘ��N���X��ݒ肷��
        m_itemShopUIHandler = FindObjectOfType<ItemShopUIHandler>();

        // m_exitTextButton�ɃC�x���g���X�i�[��ǉ�
        if (m_itemShopUIHandler != null && m_itemShopUIHandler.m_exitTextButton != null)
        {
            m_itemShopUIHandler.m_exitTextButton.onClick.AddListener(OnExitButtonClicked);
        }
    }

    // �{�^���������ꂽ���̏���
    public void OnExitButtonClicked()
    {
        // ���݂̃V�[�����I������
        Application.Quit();

        // �f�o�b�O�Ŋm�F����p
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
