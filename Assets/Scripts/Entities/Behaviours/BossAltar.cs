using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems; 
public class BossAltar : MonoBehaviour, IPointerClickHandler
{
    [Header("Settings")]
    [SerializeField] public Item requiredItem;
    [SerializeField] private BossData bossToSummon;
    [SerializeField] private Transform bossSpawnPoint;
    [SerializeField] private GameObject barrierObject;
        [Header("Components to Disable During Fight")]
    [SerializeField] private SpriteRenderer altarSprite;
    [SerializeField] private Collider2D altarCollider;
    private int altarId; 
    private bool isFightInProgress = false;
    public int GetAltarId() => altarId;
    public Item GetRequiredItem() => requiredItem;
    private void Awake()

    {
        if (altarSprite == null) altarSprite = GetComponent<SpriteRenderer>();
        if (altarCollider == null) altarCollider = GetComponent<Collider2D>();
        altarId = (int)transform.position.x * 1000 + (int)transform.position.y;

        BossAltarManager.Instance?.RegisterAltar(this);
    }
    private void OnDestroy()
    {
        BossAltarManager.Instance?.UnregisterAltar(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isFightInProgress) return;

        Player localPlayer = null;
        if(NetworkManager.Singleton.LocalClient != null)
            localPlayer = NetworkManager.Singleton.LocalClient.PlayerObject?.GetComponent<Player>();
        if (localPlayer == null) return;
        
        BossAltarUI.Instance.Open(this, localPlayer);
    }
    
    public void TryStartRitual(Player player)
    {
        if (player.GetInventory().IsHasItem(requiredItem))
        {
            player.RequestStartBossFight(altarId, bossToSummon.GetId(), requiredItem.id, bossSpawnPoint.position);
        }
    }
    public void SetFightState(bool inProgress)
    {
        isFightInProgress = inProgress;
        if (barrierObject != null)
        {
            barrierObject.SetActive(inProgress);
        }
        if (altarSprite != null)
        {
            altarSprite.enabled = !inProgress;
        }
        if (altarCollider != null)
        {
            altarCollider.enabled = !inProgress; 
        }
    }


}