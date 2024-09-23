using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneExitHandler : MonoBehaviour
{
    // �A�C�e���V���b�vUI
    ItemShopUIHandler m_itemShopUIHandler;

    // �{�^���������ꂽ���ɌĂ΂�郁�\�b�h
    public void OnExitButtonClicked()
    {
        // ���݂̃V�[�����I������
        Application.Quit();

        // �G�f�B�^�Ńe�X�g����ꍇ�́A�ȉ��̍s��ǉ����܂�
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
