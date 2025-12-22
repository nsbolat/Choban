using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ConnectionManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_InputField ipInputField; // IP Adresi girilecek alan
    public Button hostButton;           // Host olma butonu
    public Button clientButton;         // Client olma butonu

    private void Start()
    {
        // Butonlara tıklama olaylarını ekle
        if (hostButton != null) hostButton.onClick.AddListener(StartHost);
        if (clientButton != null) clientButton.onClick.AddListener(StartClient);

        // InputField boşsa varsayılan olarak localhost yaz
        if (ipInputField != null && string.IsNullOrEmpty(ipInputField.text))
        {
            ipInputField.text = "127.0.0.1";
        }
    }

    public void StartHost()
    {
        // Host başlatırken "0.0.0.0" yaparsak tüm ağlardan gelen bağlantıyı kabul eder (LAN için gerekli)
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        if (transport != null)
        {
            transport.SetConnectionData("0.0.0.0", 7777);
        }

        NetworkManager.Singleton.StartHost();
        Debug.Log("Host Başlatıldı (Listening on 0.0.0.0).");
        HideUI();
    }

    public void StartClient()
    {
        string ipAddress = "127.0.0.1";

        if (ipInputField != null && !string.IsNullOrEmpty(ipInputField.text))
        {
            ipAddress = ipInputField.text;
        }

        // UnityTransport'a girilen IP adresini ata
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        if (transport != null)
        {
            // Port varsayılan 7777 kalır, sadece IP değişir
            transport.SetConnectionData(ipAddress, 7777);
            Debug.Log($"Client Bağlanıyor: {ipAddress}:7777");
        }

        NetworkManager.Singleton.StartClient();
        HideUI();
    }

    private void HideUI()
    {
        // Bağlantı kurulunca bu paneli gizle
        gameObject.SetActive(false);
    }
}
