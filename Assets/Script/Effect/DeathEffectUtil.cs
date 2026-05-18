using UnityEngine;

public static class DeathEffectUtil
{
    public static void SpawnDeathEffects(Vector3 position, GameObject smokePrefab, GameObject splatterPrefab)
    {
        // 1. Spawn blood smoke/explosion particles
        if (smokePrefab != null)
        {
            Object.Instantiate(smokePrefab, position + Vector3.up, Quaternion.identity);
        }

        // 2. Spawn floor splatter (Decal/Projector)
        if (splatterPrefab != null)
        {
            if (Physics.Raycast(position + Vector3.up, Vector3.down, out RaycastHit hit, 5f))
            {
                // Offset a bit for projectors to have depth
                Vector3 spawnPos = hit.point + hit.normal * 0.5f; 
                
                // For Projectors, forward (Z) is the projection direction
                Quaternion spawnRot = Quaternion.LookRotation(-hit.normal);
                
                GameObject splatter = Object.Instantiate(splatterPrefab, spawnPos, spawnRot);
                
                // Random rotation around the projection axis
                splatter.transform.Rotate(Vector3.forward, UnityEngine.Random.Range(0, 360), Space.Self);
                
                // Random scale
                float scale = UnityEngine.Random.Range(0.8f, 1.5f);
                
                Projector proj = splatter.GetComponent<Projector>();
                if (proj != null)
                {
                    proj.orthographicSize *= scale;
                }
                else
                {
                    splatter.transform.localScale = Vector3.one * scale;
                }
            }
        }
    }
}
