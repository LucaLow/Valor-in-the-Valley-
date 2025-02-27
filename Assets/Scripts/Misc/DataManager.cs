using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public class GameData
    {
        public WaveSystem.EWaveDifficulty eWaveDifficulty = WaveSystem.EWaveDifficulty.EXPERT;
        public void Save( )
        {
            string sJsonString = JsonUtility.ToJson( this );
            using ( StreamWriter sWriter = new StreamWriter( "save.dat" ) )
            {
                sWriter.WriteLine( sJsonString );
            }
        }
        public void Load( GameData gData )
        {
            string sJsonString = File.ReadAllText( "save.dat" );
            gData = JsonUtility.FromJson<GameData>( sJsonString );
        }
    }
    public GameData gData = new GameData( );
}