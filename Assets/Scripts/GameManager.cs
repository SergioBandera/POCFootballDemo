using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum TeamType { RedTeam, BlueTeam, None }
    private TeamType TeamWithPossession = TeamType.None;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
   
    public void SetPossession(GameObject character)
    {
        if (character == null)
        {
            TeamWithPossession = TeamType.None;
        }
        else
        {   

            if (character.CompareTag("RedTeam"))
            {
            TeamWithPossession = TeamType.RedTeam;
            }
             else if (character.CompareTag("BlueTeam"))
             {
            TeamWithPossession = TeamType.BlueTeam;
             }
        }
 
            
        
    }

    public TeamType GetPossession()
    {
        Debug.Log($"Team with possession: {TeamWithPossession}");
        return TeamWithPossession;
    }
}