    using Unity.Mathematics;
    using UnityEngine;
    using UnityEngine.AI;
    using System.Collections;

    public enum Demon
    {
        idle,
        walk,
        attack,
    }

    public class Enemy : MonoBehaviour
    {
        [SerializeField] Animator anim;
        [SerializeField] Demon currentState;
        [SerializeField] DamageText damageText;
        //[SerializeField] ParticleSystem bloodEffect;

        [Header("Gizmos")]
        [SerializeField] float range;
        [SerializeField] float smallrange;
        [SerializeField] LayerMask playerMask;

        [Header("Ai")]
        [SerializeField] NavMeshAgent nav;
        [SerializeField] Transform target;

        [Header("Info")]
        [SerializeField] public float hp = 60;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            nav = GetComponent<NavMeshAgent>();

            nav.speed = 7f;
            nav.acceleration = 10f;
            nav.angularSpeed = 800f;
            nav.stoppingDistance = 2f;

            //bloodEffect.Stop();
            RegisterToMaps();
        }

        void OnDestroy()
        {
            UnregisterFromMaps();
        }

        /// <summary> 미니맵/전체맵 매니저에 몬스터 아이콘 등록 (비활성 오브젝트도 찾기) </summary>
        void RegisterToMaps()
        {
            var minimap = FindFirstObjectByType<MinimapManeger>(FindObjectsInactive.Include);
            if (minimap != null) minimap.RegisterMonster(this);

            var fullmap = FindFirstObjectByType<FullMapManager>(FindObjectsInactive.Include);
            if (fullmap != null) fullmap.RegisterMonster(this);
        }

        /// <summary> 미니맵/전체맵에서 몬스터 아이콘 제거 </summary>
        void UnregisterFromMaps()
        {
            var minimap = FindFirstObjectByType<MinimapManeger>(FindObjectsInactive.Include);
            if (minimap != null) minimap.UnregisterMonster(this);

            var fullmap = FindFirstObjectByType<FullMapManager>(FindObjectsInactive.Include);
            if (fullmap != null) fullmap.UnregisterMonster(this);
        }

        // Update is called once per frame
        void Update()
        {
            Range();
            SmallRange();
        }

        void Range()
        {
            Collider[] hit = Physics.OverlapSphere(transform.position, range, playerMask);

            if (hit.Length > 0)
            {
                nav.isStopped = false;
                anim.SetBool("isMove", true);
                nav.SetDestination(target.position);
            }
            else
            {
                anim.SetBool("isMove", false);
                nav.isStopped = true;
            } 
        }

        void SmallRange()
        {
            Collider[] hit = Physics.OverlapSphere(transform.position, smallrange, playerMask);

            if (hit.Length > 0)
            {
                nav.isStopped = true;
                anim.SetInteger("Attack", 1);
            }
            else
            {
                anim.SetInteger("Attack", 0);
                nav.isStopped = false;
            }
        }

        [Header("Juice")]
        [SerializeField] private Renderer[] meshRenderers;
        [SerializeField] private Color flashColor = Color.white;
        [SerializeField] private float flashDuration = 0.15f; 
        [SerializeField] private GameObject hitEffectPrefab; 
        private Color[] originalColors;

        public void TakeDamage(float damage)
        {
            hp -= damage;
            if (damageText != null) damageText.SetDamage((int)damage);
            StartCoroutine(FlashRoutine());

            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, transform.position + Vector3.up, Quaternion.identity);
            }

            Effect effects = FindFirstObjectByType<Effect>();
            if (effects != null)
            {
                effects.TriggerHitStop(0.08f);
                // effects.TriggerCameraShake(0.2f, 0.20f);
            }

            if (hp <= 0)
            {
                Destroy(gameObject);
            }
        }

        private IEnumerator FlashRoutine()
        {
            if (meshRenderers == null || meshRenderers.Length == 0)
                meshRenderers = GetComponentsInChildren<Renderer>();

            if (originalColors == null || originalColors.Length != meshRenderers.Length)
            {
                originalColors = new Color[meshRenderers.Length];
                for (int i = 0; i < meshRenderers.Length; i++)
                {
                    if (meshRenderers[i].material.HasProperty("_BaseColor"))
                        originalColors[i] = meshRenderers[i].material.GetColor("_BaseColor");
                    else if (meshRenderers[i].material.HasProperty("_Color"))
                        originalColors[i] = meshRenderers[i].material.color;
                    else
                        originalColors[i] = Color.white;
                }
            }

            foreach (var renderer in meshRenderers)
            {
                if (renderer.material.HasProperty("_BaseColor")) renderer.material.SetColor("_BaseColor", flashColor);
                else if (renderer.material.HasProperty("_Color")) renderer.material.color = flashColor;
            }

            yield return new WaitForSeconds(flashDuration);

            for (int i = 0; i < meshRenderers.Length; i++)
            {
                if (meshRenderers[i].material.HasProperty("_BaseColor")) meshRenderers[i].material.SetColor("_BaseColor", originalColors[i]);
                else if (meshRenderers[i].material.HasProperty("_Color")) meshRenderers[i].material.color = originalColors[i];
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, range);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, smallrange);
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Bullet"))
            {
                Bullet bullet = collision.gameObject.GetComponent<Bullet>();
                if (bullet != null) TakeDamage(bullet.Damage);
                else TakeDamage(20f);
            }
        }
        }
