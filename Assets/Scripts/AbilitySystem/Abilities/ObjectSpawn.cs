using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
[CreateAssetMenu(fileName = "ObjectSpawn", menuName = "Abilities/ObjectSpawn")]
public class ObjectSpawn : Ability
{
    public GameObject objectPrefab;
    public bool spawnOnCaster = false;
    public bool spawnInMelee = false;

    [Header("Warning Settings (Optional)")]
    public GameObject warningPrefab; // Сюда перетаскиваем CircleWarningPrefab или LineWarningPrefab
    public float warningDuration = 2f;

    [Header("Warning Visuals")]
    public float warningRadius = 1f; // Для CircleWarning
    public Vector2 warningLineSize = new Vector2(5f, 1f); // Для LineWarning
    public override void AbilityUse(Vector2 playerPosition, Vector2 targetPosition)
    {
        if (warningPrefab == null)
        {
            SpawnMainPrefab(playerPosition, targetPosition);
        }
        else
        {
            GameManager.Instance.StartCoroutine(SpawnWithWarning(playerPosition, targetPosition));
        }
    }
    private void SpawnMainPrefab(Vector2 playerPosition, Vector2 targetPosition)
    {
        Vector2 startingPos = spawnOnCaster ? playerPosition : targetPosition;
        GameObject spawnedObject = Instantiate(objectPrefab, startingPos, Quaternion.identity);
        spawnedObject.GetComponent<EffectController>()?.Initialize(playerPosition, targetPosition);
    }
    
    private IEnumerator SpawnWithWarning(Vector2 playerPosition, Vector2 targetPosition)
    {
        Vector2 direction = (targetPosition - playerPosition).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle - 90);
        GameObject warningGO = Instantiate(warningPrefab, targetPosition, rotation);
        
        if (warningGO.TryGetComponent<Effect_CircleWarning>(out var circleWarning))
        {
            warningGO.transform.localScale = Vector3.one * warningRadius;
            circleWarning.SetDuration(warningDuration);
        }
        else if (warningGO.TryGetComponent<LineWarning>(out var lineWarning))
        {
            warningGO.transform.position = playerPosition;
            lineWarning.StartWarning(warningDuration, warningLineSize);
        }

        yield return new WaitForSeconds(warningDuration);

        SpawnMainPrefab(playerPosition, targetPosition);
    }
}