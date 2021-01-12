using System.Collections;
using System.Linq;
using UnityEngine;

namespace Match
{
    public class Pawn : MonoBehaviour
    {
        public bool king = false;
        public bool playerPawn = true;
        public Vector3 forwardLook = Vector3.forward;

        private Animator _animator;
        private SkinnedMeshRenderer _meshRenderer;
        [SerializeField] private Color selectionColor;
        private Material _material;
        private int _colorShaderPropertyId;
        private int _outlineColorShaderPropertyId;
        private int _outlineWidthShaderPropertyId;
        private Color _colorShaderDefaultValue;


        private bool _isMoving = false;
        private float _moveSpeed = 2f;
        private float _moveTimer = 0f;
        private Vector3 _moveStartPos;
        private Vector3 _moveTargetPos;

        private PawnMove[] _movesToExecute;

        // Start is called before the first frame update
        void Start()
        {
            _meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
            _animator = GetComponent<Animator>();
            _animator.fireEvents = false;//todo
            _material = _meshRenderer.material;
            //_material.shader.Property
            _colorShaderPropertyId = Shader.PropertyToID("_Color");
            _outlineColorShaderPropertyId = Shader.PropertyToID("_OutlineColor");
            _outlineWidthShaderPropertyId = Shader.PropertyToID("_Outline");
        
            _colorShaderDefaultValue = _material.GetColor(_colorShaderPropertyId);

            if(playerPawn == false)
                ChangeOutlineToEnemyColor();
        }

        private void Update()
        {
            if (_isMoving)
            {
                UpdateMovePosition();
                _moveTimer += Time.deltaTime;
            }
        }

        public void ChangeOutlineToDefault()
        {
            _material.SetColor(_colorShaderPropertyId, _colorShaderDefaultValue);
        }

        public void ChangeOutlineToSelection()
        {
            _material.SetColor(_colorShaderPropertyId, selectionColor);
        }

        public void SetAsEnemyPawn()
        {
            playerPawn = false;
        }
    
        public void ExecuteMoves(PawnMove[] orderedMoves)
        {
            _movesToExecute = orderedMoves;
            StopCoroutine(nameof(Foo));
            StartCoroutine(nameof(Foo));
        }

        private IEnumerator Foo()
        {
            foreach (var nextMove in _movesToExecute)
            {
                if (nextMove.MoveType == MoveType.Move)
                {
                    MoveTo(nextMove.Tile.Transform.position);
                    yield return new WaitUntil(() => _isMoving == false);
                }
                else if (nextMove.MoveType == MoveType.Attack)
                {
                    Vector3 victimAttackerVectorNormalized = (nextMove.EnemyTile.Transform.position - transform.position).normalized;
                    MoveTo(nextMove.EnemyTile.Transform.position - (victimAttackerVectorNormalized / 2));
                    yield return new WaitUntil(() => _isMoving == false);
                
                    transform.LookAt(nextMove.EnemyTile.Transform.position, Vector3.up);
                    _animator.SetTrigger("Attack");
                    yield return new WaitForSeconds(0.15f);
                
                    Pawn enemyPawn = GameObject.FindObjectsOfType<Pawn>().First(pawn => pawn.transform.position == nextMove.EnemyTile.Transform.position);//todo
                    enemyPawn?.StartCoroutine(enemyPawn.PlayKillAnimation());
                    yield return new WaitForSeconds(1f);
                
                    MoveTo(nextMove.Tile.Transform.position);
                    yield return new WaitUntil(() => _isMoving == false);
                }
            }
        }

        private IEnumerator PlayKillAnimation()
        {
            _animator.SetBool("Dead", true);
            yield return new WaitForSeconds(1f);
            transform.Translate(Vector3.down * 3);
        }

        private void ChangeOutlineToEnemyColor()
        {
            _material.SetColor(_outlineColorShaderPropertyId, Color.red);
            _material.SetFloat(_outlineWidthShaderPropertyId, 7f);
        }
    
        public void MoveTo(Vector3 pos)
        {
            transform.LookAt(pos, Vector3.up);
            _animator.SetFloat("MoveSpeed", 0.66f);

            _moveStartPos = transform.position;
            _moveTargetPos = pos;
            _moveTimer = 0;
            _isMoving = true;
        }

        private void UpdateMovePosition()
        {
            float requiredTimeInSecondsToArriveAtTarget = Vector3.Distance(_moveStartPos, _moveTargetPos) / _moveSpeed;
            float moveCompletion = _moveTimer / requiredTimeInSecondsToArriveAtTarget;
        
            transform.position = Vector3.Lerp(_moveStartPos, _moveTargetPos, moveCompletion);

            if (moveCompletion >= 1)
            {
                _isMoving = false;
                OnMovingEnd();
            }
        }

        private void OnMovingEnd()
        {
            _animator.SetFloat("MoveSpeed", 0f);
            transform.forward = forwardLook;

            //todo
            if (transform.position.z == -3.5 && forwardLook == Vector3.back)
                king = true;
            else if (transform.position.z == 3.5 && forwardLook == Vector3.forward)
                king = true;
            //todo
        }
    }
}
