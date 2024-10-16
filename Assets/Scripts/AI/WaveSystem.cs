using System.Collections;
using UnityEngine;
using TMPro;
public class WaveSystem : DataManager
{
    public enum EWaveDifficulty : int
    {
        EXPERT = 1,
        HARD = 2,
        NORMAL = 3,
        EASY = 4,
    }
    public GameObject gEnemyPrefab;
    [SerializeField] public Transform[] tSpawns = new Transform[4];
    public TextMeshProUGUI tmpWaveIndicator;
    public int iCurrentWave = 0;
    private void Start( )
    {
        iCurrentWave = 1;
        tmpWaveIndicator.text = "Enemy Wave: " + ( iCurrentWave - 1 ).ToString( );
        StartCoroutine( EnemyWaveRoutine( ) );
    }
    IEnumerator EnemyWaveRoutine( )
    {
        float fTime = 0.0f, fMinutesToWait = 0.0f;
        float fMinTime = 1.0f * ( float )gData.eWaveDifficulty;
        float fMaxTime = 2.0f * ( float )gData.eWaveDifficulty;

        if ( fMinTime != 0.0f && fMaxTime != 0.0f )
            fMinutesToWait = UnityEngine.Random.Range( fMinTime, fMaxTime );
        else
            yield return null;

        while ( fTime <= ( 5.0f ) )
        {
            fTime += Time.deltaTime;
            yield return new WaitForEndOfFrame( );
        }

        int iNumberOfEnemies = UnityEngine.Random.Range(
            1 * iCurrentWave,                     // Min
            3 * iCurrentWave                      // Max
        );

        for ( int i = 0; i < iNumberOfEnemies; i++ )
        {
            Transform tParent = tSpawns[UnityEngine.Random.Range( 0, 4 )];

            GameObject gObject = Instantiate(
                gEnemyPrefab,       // Object
                tParent             // Parent
            );

            gObject.transform.position = tParent.position;

            AIAgent aiAgent = gObject.GetComponent<AIAgent>( );
            if ( aiAgent )
                aiAgent.wanderingState.wanderRangePosition = TownHallManager.Instance.transform.position;
        }

        iCurrentWave++;
        tmpWaveIndicator.text = "Enemy Wave: " + ( iCurrentWave - 1 ).ToString( );
        StartCoroutine( EnemyWaveRoutine( ) );
    }
}
    