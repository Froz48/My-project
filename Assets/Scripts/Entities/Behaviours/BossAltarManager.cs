using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class BossAltarManager : NetworkBehaviour
{
    public static BossAltarManager Instance { get; private set; }
    private Dictionary<int, BossAltar> _altars = new Dictionary<int, BossAltar>();

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterAltar(BossAltar altar)
    {
        if (!_altars.ContainsKey(altar.GetAltarId()))
        {
            _altars.Add(altar.GetAltarId(), altar);
        }
    }

    public void UnregisterAltar(BossAltar altar)
    {
        if (_altars.ContainsKey(altar.GetAltarId()))
        {
            _altars.Remove(altar.GetAltarId());
        }
    }

    [ClientRpc]
    public void UpdateAltarStateClientRpc(int altarId, bool isFightInProgress)
    {
        if (_altars.TryGetValue(altarId, out BossAltar altar))
        {
            altar.SetFightState(isFightInProgress);
        }
    }
}