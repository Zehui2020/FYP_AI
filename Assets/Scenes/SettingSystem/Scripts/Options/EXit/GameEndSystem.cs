//----------------------------------------------------------------------
// GameEndSystem
//
// �Q�[���I�����Ǘ�����N���X
//
// Data: 2/10/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using UnityEngine;

public class GameEndSystem : MonoBehaviour
{
    // �Q�[�����I������
    public void GameEnd()
    {
        // �Q�[�����I������
        Application.Quit();

        // �f�o�b�O�Ŋm�F����p
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
