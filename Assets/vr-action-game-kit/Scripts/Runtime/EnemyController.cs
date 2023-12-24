using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
using AnnulusGames.LucidTools.RandomKit;


public class EnemyController : MonoBehaviour
{
    [Header("敵が吹っ飛ぶ高さ")] [SerializeField] private float enemyAddForcePowerY;
    [Header("敵が吹っ飛ぶ奥行き")] [SerializeField] private float enemyAddForcePowerZ;
    [Header("敵が吹っ飛ぶ音")] [SerializeField] private AudioClip seEnemyStrike;
    
    [Header("プレイヤーの座標")] [SerializeField] private Transform playerTransform;
    [SerializeField] private NavMeshAgent enemyNavMeshAgent;
    [SerializeField] private Animator enemyAnimator;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Rigidbody enemyRigidbody;
    
    /// <Summary>
    /// 敵の行動をランダムにする変数です
    /// </Summary>
    [SerializeField] private WeightedList<string> meleeActionWeightedList = new WeightedList<string>(MeleeAttackPattern, GuardWhileMovingPattern);

    /// <Summary>
    /// 敵の攻撃をランダムにする変数です
    /// </Summary>
    [SerializeField] private WeightedList<int> meleeAttackWeightedList = new WeightedList<int>(AttackFromLeftPattern, AttackFromFrontPattern, AttackFromRightPattern);

    /// <Summary>
    /// 敵の移動をランダムにする変数です
    /// </Summary>
    [SerializeField] private WeightedList<string> moveWeightedList = new WeightedList<string>(MoveFrontPattern, MoveBackPattern);

    private bool beatFlag = false;

    private const string MeleeAttackPattern = "MeleeAttackPattern";
    private const string GuardWhileMovingPattern = "GuardWhileMovingPattern";
    private const string MoveFrontPattern = "MoveFrontPattern";
    private const string MoveBackPattern = "MoveBackPattern";
    
    private const int AttackFromLeftPattern = 1;
    private const int AttackFromRightPattern = 2;
    private const int AttackFromFrontPattern = 3;

    //文字列をハッシュという数字に予め変換しておくことで、処理の度に文字列化を行ないでよいようにして負荷を軽減します
    //また、文字列の打ち間違いをしないようにします
    private static readonly int AnimationLocomotionHash = Animator.StringToHash("Locomotion");
    private static readonly int AnimationWalkBackHash = Animator.StringToHash("WalkBack");

    //敵がリセットされた際の処理です
    public void Reset()
    {
        beatFlag = false;
    }

    //敵が倒されているかどうかを確認します
    public bool CheckBeated()
    {
        return beatFlag;
    }
    
    // 敵が倒された際の処理を行います
    public void Beat()
    {
        beatFlag = true;
        
        //NavmeshAgentが有効だと上に飛ばないので無効化します
        enemyNavMeshAgent.enabled = false;
        
        //敵のヒットストップ処理を行います
        
        //敵がやられた際の音を鳴らします
        audioSource.PlayOneShot(seEnemyStrike);
        
        //敵を吹っ飛ばします
        enemyRigidbody.AddForce(0, enemyAddForcePowerY, enemyAddForcePowerZ);
    }

    private void FixedUpdate()
    {
        //プレイヤーの位置まで移動します
        if (Vector3.Distance(playerTransform.position, enemyNavMeshAgent.transform.position) >
            enemyNavMeshAgent.stoppingDistance)
        {
            enemyNavMeshAgent.SetDestination(playerTransform.position);
            enemyAnimator.SetFloat("Speed", 1.0f);
        }
        else
        {
            enemyAnimator.SetFloat("Speed", 0);
            
            //行動パターンを決定します
            var meleeActionPattern = meleeActionWeightedList.RandomElement();
            switch (meleeActionPattern)
            {
                case MeleeAttackPattern:
                    //攻撃パターンを決定します
                    var meleeAttackPattern = meleeAttackWeightedList.RandomElement();
                    enemyAnimator.SetInteger(MeleeAttackPattern, meleeAttackPattern);
           
                    break;

                case GuardWhileMovingPattern:
                    //ガードのアクションを再生します
                    
                    break;
            }
        }

    }
}
