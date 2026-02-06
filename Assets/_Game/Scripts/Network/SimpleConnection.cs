using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class SimpleConnection : MonoBehaviour
{
    public Button hostBtn;
    public Button clientBtn;

    void Start()
    {
        // 绑定按钮事件
        hostBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
            HideUI();
        });

        clientBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
            HideUI();
        });
    }

    void HideUI()
    {
        hostBtn.gameObject.SetActive(false);
        clientBtn.gameObject.SetActive(false);
    }
}