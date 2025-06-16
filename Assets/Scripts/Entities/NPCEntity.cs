using UnityEngine;
using Unity.Netcode;
using UnityEngine.U2D.Animation;
using System.Collections;

public class NPCEntity : NetworkBehaviour, IDamageable
{
    [Header("Network")]
    public NetworkVariable<int> npcId = new NetworkVariable<int>();
    public NetworkVariable<float> currentHealth = new NetworkVariable<float>();

    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteLibrary spriteLibrary;
    [SerializeField] private BoxCollider2D npcCollider;



    [Header("ServerSide")]
    [SerializeField] private NPCData _monsterDataInstance;
    public Ability[] _abilities;
    [SerializeField] private int _activeStatePosition = 0;
    private Coroutine _behaviorCoroutine;
    public NPCData MonsterData => _monsterDataInstance;

    public override void OnNetworkSpawn()
    {
        npcId.OnValueChanged += OnNpcIdChanged;
    }
    private void OnNpcIdChanged(int oldValue, int newValue)
    {
        if (!IsServer && newValue > 0)
        {
            InitializeClientVisuals(newValue);
        }
    }
    private void InitializeClientVisuals(int dataId)
    {   
        var database = Resources.Load<Database>("NPCDatabase");
        var templateData = database.GetObjectById(dataId) as NPCData;

        if (templateData != null)
        {
            _monsterDataInstance = templateData;
            UpdateVisuals(_monsterDataInstance);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void InitializeWithDataServerRpc(int dataId, ServerRpcParams rpcParams = default)
    {
        var database = Resources.Load<Database>("NPCDatabase");
        var templateData = database.GetObjectById(dataId) as NPCData;

        _monsterDataInstance = Instantiate(templateData);
        _monsterDataInstance.name = $"{templateData.name}_Instance_{NetworkObjectId}";

        npcId.Value = dataId;
        currentHealth.Value = _monsterDataInstance.maxHealth;
        npcCollider.enabled = true;
        InitializeAbilities();
        UpdateVisuals();
        StartBehaviorCheck();
    }


    private void InitializeAbilities()
    {
        if (_monsterDataInstance == null) return;

        _abilities = new Ability[_monsterDataInstance.abilities.Length];
        for (int i = 0; i < _monsterDataInstance.abilities.Length; i++)
        {
            if (_monsterDataInstance.abilities[i] != null)
            {
                _abilities[i] = _monsterDataInstance.abilities[i].CreateInstance();
            }
        }
    }

    private void UpdateVisuals(NPCData data = null)
    {
        var targetData = data ?? _monsterDataInstance;
        if (spriteLibrary != null && targetData != null)
        {
            spriteLibrary.spriteLibraryAsset = targetData.spriteLibraryAsset;
        }
    }

    private void StartBehaviorCheck()
    {
        StopBehaviorCheck();

        if (_monsterDataInstance?.nPCBehaviour != null &&
            _monsterDataInstance.nPCBehaviour.Length > 0)
        {
            _behaviorCoroutine = StartCoroutine(BehaviorCheckRoutine());
        }
        
    }

    private void StopBehaviorCheck()
    {
        if (_behaviorCoroutine != null)
        {
            StopCoroutine(_behaviorCoroutine);
            _behaviorCoroutine = null;
        }
    }

    private IEnumerator BehaviorCheckRoutine()
    {
        while (true)
        {
            if (_monsterDataInstance?.nPCBehaviour == null) continue;
            
            // Проверяем все поведения от самого высокого приоритета (последнего в массиве)
            for (int i = _monsterDataInstance.nPCBehaviour.Length -1; i >= 0 ; i--)
            {
                if (_monsterDataInstance.nPCBehaviour[i].CheckConditions(this))
                {
                    if (_activeStatePosition != i)
                    {
                        _activeStatePosition = i;
                    }
                    break; // Используем первое подходящее поведение
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }

    private void FixedUpdate()
    {
        if (!IsServer || _monsterDataInstance == null) return;
        
        if (_activeStatePosition >= 0 && 
            _activeStatePosition < _monsterDataInstance.nPCBehaviour.Length && 
            _monsterDataInstance.nPCBehaviour[_activeStatePosition] != null)
        {
            _monsterDataInstance.nPCBehaviour[_activeStatePosition].Act(this, animator);
        }
    }

    public void TakeDamage(float damage)
    {
        if (IsServer)
        {
            currentHealth.Value -= damage;
            if (currentHealth.Value <= 0)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        DropLoot();
        NotifySpawner();
        DespawnNPC();
    }

    private void DropLoot()
    {
        if (IsServer)
        {
            if (_monsterDataInstance?.lootTable == null) return;

            foreach (var loot in _monsterDataInstance.lootTable)
            {
                if (Random.value <= loot.dropChance)
                {
                    SpawnLootItem(loot.item);
                }
            }
        }
    }
    private void UseAbilityOnPosition(int index, Vector2 postition){
        UseAbilityServerRpc(postition, _abilities[index].id);
    }

    [ServerRpc]
    public void UseAbilityServerRpc(Vector2 mousePosition, int abilityId){
        UseAbilityClientRpc(mousePosition, abilityId);
    }
    [ClientRpc]
    private void UseAbilityClientRpc(Vector2 mousePosition, int abilityId)
    {
        Debug.Log("UseAbilityClientRpc from " + name);
        ((Resources.Load("AbilityDatabase") as Database).GetObjectById(abilityId) as Ability).AbilityUse(transform.position, mousePosition);
    }
    private void SpawnLootItem(Item item)
    {
        var lootPrefab = Resources.Load<GameObject>("GroundItemPrefab");
        var loot = Instantiate(lootPrefab, transform.position, Quaternion.identity);
        loot.GetComponent<NetworkObject>().Spawn();
        loot.GetComponent<GroundItem>().SetItemClientRpc(item.id);
    }

    private void NotifySpawner()
    {
        FindObjectOfType<EnemySpawner>()?.OnEnemyDied(gameObject);
    }

    private void DespawnNPC()
    {
        GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }

    public float GetPower() => 1f;
}