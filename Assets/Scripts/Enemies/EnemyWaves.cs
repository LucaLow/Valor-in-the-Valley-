using TMPro;
using UnityEngine;
using UnityEngine.AI;
using static EnemyWaves;

public class EnemyWaves : DataManager
{
    public struct Minutes
    {
        public float fMinutes;
        public float ToSeconds( )
        {
            return this.fMinutes * 60.0f;
        }
        public bool IsValid( )
        {
            return ( fMinutes != 0.0f );
        }
        public void Set( Minutes mMinutes )
        {
            this = mMinutes;
        }
    }
    public Minutes mMinTimeBetween = new Minutes( );
    public Minutes mMaxTimeBetween = new Minutes( );
    public enum ENavigationPoint : int
    {
        North = 0,
        NorthEast = 1,
        East = 2,
        SouthEast = 3,
        South = 4,
        SouthWest = 5,
        West = 6,
        NorthWest = 7
    }
    ENavigationPoint eNavigationPoint;
    public enum EEnemyWaveType
    {
        Generic = 0,
        PlayerRequested = 1,
        Conditional = 2
    }
    public enum EEnemyWaveDifficulty : int
    {
        None = 0,
        Easy = 1,
        Normal = 2,
        Hard = 3,
        Expert = 4,
        Debug = 5
    }
    public struct UEnemyWaveInformation
    {
        public float fSecondsUntil;
        public float fDuration;
        public EEnemyWaveType eEnemyWaveType;
        public EEnemyWaveDifficulty eEnemyWaveDifficulty;
    }
    UEnemyWaveInformation uEnemyWaveInformation;

    public struct UEnemyInfo
    {
        public GameObject gObject;
        public AIAgent aiAgent;
        public NavMeshAgent nmAgent;
        public Transform tSpawnPoint;
    }
    UEnemyInfo[] uEnemy = new UEnemyInfo[20];

    [SerializeField] TextMeshProUGUI tPhaseIndicator;
    [SerializeField] GameObject[] gSpawnPoints = new GameObject[4];
    [SerializeField] Transform[] vNavPoints = new Transform[2];
    [SerializeField] GameObject gEnemyPrefab;

    bool bCanPerformEnemyWaves = true;
    bool bIsEnemyWaveActive = false;
    bool bFlipBool = true;
    bool bAreEnemiesAlive = false;
    bool bCanReset = false;
    float fCachedTime = 0.0f;

    public int iCurrentPhase = 0;
    int iNumberOfEnemies = 0;
    public bool bShowPhaseIndicator = false;
    Color cRed = new Color( 1.0f, 50.0f / 255.0f, 50.0f / 255.0f, 0 );
    bool bDisplayPhase = false;
    float fCachedIndicatorTime = 0.0f;

    void Start( )
    {
        if ( !this.InitializeDependencies( ) )
        {
            Debug.Log( "<!> Error [EnemyWaveComponent.cs FN:InitializeDependencies]" );
            bCanPerformEnemyWaves = false;
        }
    }

    void Update( )
    {
        if ( !bCanPerformEnemyWaves )
            return;

        float fCurrentTime = Time.time;

        if ( bDisplayPhase )
        {
            if ( fCurrentTime >= ( fCachedIndicatorTime + 1.0f ) )
            {
                cRed.a = UnityEngine.Mathf.Lerp( cRed.a, -0.5f, Time.deltaTime );
               // tPhaseIndicator.color = cRed;

                if ( cRed.a > 0 )
                    return;

                fCachedIndicatorTime = fCurrentTime;
                bDisplayPhase = false;
            }
        }

        if ( this.bFlipBool && ( fCurrentTime >= ( fCachedTime + uEnemyWaveInformation.fSecondsUntil ) ) )
        {
            bFlipBool = false;
            bIsEnemyWaveActive = true;
        }

        if ( bIsEnemyWaveActive )
        {
            if ( !bAreEnemiesAlive )
            {
                if ( !bDisplayPhase )
                {
                   // tPhaseIndicator.text = "Wave " + iCurrentPhase;
                    cRed.a = UnityEngine.Mathf.Lerp( cRed.a, 1.5f, Time.deltaTime );
                   // tPhaseIndicator.color = cRed;

                    if ( cRed.a < 1.0f )
                        return;

                    fCachedIndicatorTime = fCurrentTime;
                    bDisplayPhase = true;
                }

                iCurrentPhase++;

                iNumberOfEnemies = Random.Range( 2 * iCurrentPhase, 4 * iCurrentPhase );
                if ( !this.GetTransforms( ) )
                    return;

                bShowPhaseIndicator = true;

                for ( int i = 0; i < iNumberOfEnemies; i++ )
                {
                    GameObject gEnemy = Instantiate<GameObject>( gEnemyPrefab, uEnemy[i].tSpawnPoint );

                    uEnemy[i].gObject = gEnemy;
                    uEnemy[i].aiAgent = gEnemy.GetComponent<AIAgent>( );
                    uEnemy[i].nmAgent = gEnemy.GetComponent<NavMeshAgent>( );

                    uEnemy[i].aiAgent.wanderingState.wanderRangePosition = vNavPoints[Random.Range( 0, 1 )].position;
                    bAreEnemiesAlive = true;
                }
            }
            else
            {
                bAreEnemiesAlive = false;

                for ( int i = 0; i < iNumberOfEnemies; i++ )
                {
                    if ( uEnemy[i].gObject.activeSelf )
                        bAreEnemiesAlive = true;
                    else
                    {
                        Destroy( uEnemy[i].gObject );

                        uEnemy[i].gObject = null;
                        uEnemy[i].aiAgent = null;
                        uEnemy[i].nmAgent = null;
                        uEnemy[i].tSpawnPoint = null;
                    }
                }

                bCanReset = !bAreEnemiesAlive;
            }

            if ( !bCanReset )
                return;

            //Reset
            uEnemyWaveInformation.fSecondsUntil = Random.Range( mMinTimeBetween.ToSeconds( ), mMaxTimeBetween.ToSeconds( ) );
            bIsEnemyWaveActive = false;
            bFlipBool = true;
            fCachedTime = fCurrentTime;
        }
    }
    bool InitializeDependencies( )
    {
        Minutes mMin = new Minutes( );
        Minutes mMax = new Minutes( );

        switch ( gData.eEnemyWaveDifficulty )
        {
            case EEnemyWaveDifficulty.Easy:
                mMin.fMinutes = 3.0f;
                mMax.fMinutes = 4.0f;
                break;
            case EEnemyWaveDifficulty.Normal:
                mMin.fMinutes = 2.0f;
                mMax.fMinutes = 3.0f;
                break;
            case EEnemyWaveDifficulty.Hard:
                mMin.fMinutes = 1.0f;
                mMax.fMinutes = 2.0f;
                break;
            case EEnemyWaveDifficulty.Expert:
                mMin.fMinutes = 0.5f;
                mMax.fMinutes = 1.0f;
                break;
            case EEnemyWaveDifficulty.Debug:
                mMin.fMinutes = 0.025f;
                mMax.fMinutes = 0.05f;
                break;
            default:
                mMin.fMinutes = 0.0f;
                mMax.fMinutes = 0.0f;
                break;
        }

        if ( mMin.IsValid( ) && mMax.IsValid( ) )
        {
            mMinTimeBetween.Set( mMin );
            mMaxTimeBetween.Set( mMax );
            uEnemyWaveInformation.fSecondsUntil = Random.Range( mMinTimeBetween.ToSeconds( ), mMaxTimeBetween.ToSeconds( ) );
            bFlipBool = true;
        }
        else
            return false;

        uEnemyWaveInformation.eEnemyWaveDifficulty = gData.eEnemyWaveDifficulty;
        if ( uEnemyWaveInformation.eEnemyWaveDifficulty == EEnemyWaveDifficulty.None )
            return false;

        return true;
    }
    public delegate Transform GenerateSpawnPositionDelegate( );
    bool GetTransforms( )
    {
        GenerateSpawnPositionDelegate fGenerateSpawnPosition = ( ) =>
        {
            int iSideOfAttack = Random.Range( 1, 4 );
            switch ( iSideOfAttack )
            {
                case 1:
                    return gSpawnPoints[0].transform;
                case 2:
                    return gSpawnPoints[1].transform;
                case 3:
                    return gSpawnPoints[2].transform;
                case 4:
                    return gSpawnPoints[3].transform;
                default:
                    return null;
            }
        };

        for ( int i = 0; i < iNumberOfEnemies; i++ )
        {
            uEnemy[i].tSpawnPoint = fGenerateSpawnPosition( );
        }

        return true;
    }
}