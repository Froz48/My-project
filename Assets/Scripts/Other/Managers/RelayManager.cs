using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;

public class RelayManager : MonoBehaviour
{
    // [SerializeField] TextMeshProUGUI joinInputText;
    // [SerializeField] private TMP_InputField joinCodeInputField;
    // [SerializeField] Button joinButton;
    Allocation allocation;
    [SerializeField] string joinCode;
    async void Start()
    {
        // joinCodeInputField.onValidateInput = (text, charIndex, addedChar) =>
        // {
        //     string allowedChars = "6789BCDFGHJKLMNPQRTWbcdfghjklmnpqrtw";
        //     return allowedChars.Contains(char.ToUpper(addedChar)) ? addedChar : '\0';
        // };
        // joinCodeInputField.onValueChanged.AddListener((text) =>
        // {
        //     joinCodeInputField.text = text.ToUpper();
        // });
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        DontDestroyOnLoad(gameObject);
    }

    public void SetHostButton(Button button)
    {
        button?.onClick.AddListener(CreateRelay);
    }

    public async void CreateRelay()
    {
        allocation = await RelayService.Instance.CreateAllocationAsync(4);

        var relayServerData = AllocationUtils.ToRelayServerData(allocation, "dtls");
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        NetworkManager.Singleton.StartHost();
    }
    public async Task SetupRelay()
    {
        allocation = await RelayService.Instance.CreateAllocationAsync(4);
        var relayServerData = AllocationUtils.ToRelayServerData(allocation, "dtls");
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
    }

    // public async void JoinRelay()
    // {
    //     try
    //     {
    //         string joinCode = SanitizeJoinCode(joinInputText.text);

    //         if (string.IsNullOrEmpty(joinCode) || joinCode.Length < 6 || joinCode.Length > 12)
    //         {
    //             Debug.LogError("Invalid join code length");
    //             return;
    //         }
    //         Debug.Log($"Attempting to join with code: '{joinCode}'");

    //         var JoinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

    //         var relayServerData = AllocationUtils.ToRelayServerData(JoinAllocation, "dtls");

    //         NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

    //         NetworkManager.Singleton.StartClient();
    //     }
    //     catch (RelayServiceException e)
    //     {
    //         Debug.LogError($"Relay join failed: {e.Message}");
    //         return;
    //     }
    // }
    public async void JoinRelay(string joinCode)
    {
        try
        {
            string sanitizedCode = SanitizeJoinCode(joinCode);
            
            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(sanitizedCode);
            var relayServerData = AllocationUtils.ToRelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            
            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.LogError($"Relay join failed: {e.Message}");
        }
    }
    private string SanitizeJoinCode(string rawCode)
    {
        if (string.IsNullOrEmpty(rawCode))
            return "";

        string cleaned = new string(rawCode.Where(c => !char.IsWhiteSpace(c)).ToArray());

        string allowedChars = "6789BCDFGHJKLMNPQRTWbcdfghjklmnpqrtw";
        cleaned = new string(cleaned.Where(c => allowedChars.Contains(c)).ToArray());
        cleaned = cleaned.ToUpper();

        return cleaned;
    }
    public async void ShowCode(TextMeshProUGUI textMeshPro)
    {
        joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        textMeshPro.text = joinCode;
    }
}