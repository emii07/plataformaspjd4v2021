using System.Collections;
using UnityEngine;

namespace AI
{
    public class FollowEnemyController : MonoBehaviour, IDamageable
    {
        
        public int maxEnergy;
        public int damage;
        public float moveSpeed;
        
        public Transform playerTransform;

        public Transform[] patrolPoints;

        [SerializeField] private Animator childAnim;
        [SerializeField] private int blinkHitTimes;
        [SerializeField] private float blinkHitDuration;

        public Vector3 patrolCenter;

        private Animator _animator;
        
        private int _currentEnergy;
        private Collider2D _collider2D;
    
        private AudioSource _audioSource;
        private SpriteRenderer _spriteRenderer;


        // Start is called before the first frame update
        void Start()
        {
            
            playerTransform = GameObject.Find("Player").transform;
            
            _collider2D = GetComponent<Collider2D>();
            _audioSource = GetComponentInChildren<AudioSource>();
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            _animator = GetComponent<Animator>();
            
            patrolCenter = patrolPoints[0].position;
            for (var index = 1; index < patrolPoints.Length; index++)
            {
                Vector3 patrolPoint = patrolPoints[index].position;
                patrolCenter += patrolPoint;
            }

            patrolCenter /= patrolPoints.Length;
            
            _currentEnergy = maxEnergy;
        }

        // Update is called once per frame
        void Update()
        {
            _animator.SetFloat("Dist2Player", Vector3.Distance(transform.position, playerTransform.position));
            _animator.SetFloat("Dist2Patrol", Vector3.Distance(transform.position, patrolCenter));
        }

        public void TakeEnergy(int damage)
        {
            _currentEnergy -= damage;

            StartCoroutine(HitBlink());

            if (_currentEnergy <= 0)
            {
                //TODO: Gerenciar morte  do inimigo
                _currentEnergy = 0;
                //Destroy(gameObject);
                _collider2D.enabled = false;
                childAnim.Play("Dead");
                _animator.Play("Dead");
                _audioSource.Play();
            }

            if (_currentEnergy > maxEnergy) _currentEnergy = maxEnergy;
        }
        
        private IEnumerator HitBlink()
        {
            _spriteRenderer.color = Color.red;
            for (int i = 0; i < blinkHitTimes-1; i++)
            {
                yield return new WaitForSeconds(blinkHitDuration);
                _spriteRenderer.color = Color.white;
                yield return new WaitForSeconds(blinkHitDuration);
                _spriteRenderer.color = Color.red;
            }
            yield return new WaitForSeconds(blinkHitDuration);
            _spriteRenderer.color = Color.white;
        }
        
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.GetComponent<IDamageable>().TakeEnergy(damage);
            }
        }
    
        private void OnCollisionStay2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.GetComponent<IDamageable>().TakeEnergy(damage);
            }
        }
        
        private void OnDrawGizmos()
        {
            Debug.DrawLine(transform.position, patrolPoints[0].position, Color.red);
            for (var index = 1; index < patrolPoints.Length; index++)
            {
                var patrolPoint = patrolPoints[index];
                Debug.DrawLine(patrolPoints[index-1].position, patrolPoints[index].position, Color.red);
            }
            Debug.DrawLine(patrolPoints[^1].position, patrolPoints[0].position, Color.red);

            
            patrolCenter = patrolPoints[0].position;
            for (var index = 1; index < patrolPoints.Length; index++)
            {
                Vector3 patrolPoint = patrolPoints[index].position;
                patrolCenter += patrolPoint;
            }

            patrolCenter /= patrolPoints.Length;
            
            
            Gizmos.DrawSphere(patrolCenter, 0.5f);
            
            Gizmos.DrawWireSphere(transform.position, 5f);
        }
    }
}
