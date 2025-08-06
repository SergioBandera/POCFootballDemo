using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum TeamType { RedTeam, BlueTeam, None }
    private TeamType TeamWithPossession = TeamType.None;

    // Referencia al jugador que tiene la posesión
    private AttachBall currentPlayerWithBall;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SetPossession(GameObject character)
    {
        // Si hay un jugador con la pelota, se la quitamos
        if (currentPlayerWithBall != null)
        {
            Debug.Log("has dejado la pelota");
            currentPlayerWithBall.Detach();
        }

        if (character == null)
        {
            TeamWithPossession = TeamType.None;
            currentPlayerWithBall = null;
        }
        else
        {
            currentPlayerWithBall = character.GetComponent<AttachBall>();
            if (currentPlayerWithBall != null)
                currentPlayerWithBall.Attach();

            // Usamos el tag para saber el equipo
            if (character.CompareTag("RedTeam"))
                TeamWithPossession = TeamType.RedTeam;
            else if (character.CompareTag("BlueTeam"))
                TeamWithPossession = TeamType.BlueTeam;
            else
                TeamWithPossession = TeamType.None;
        }
    }

    public TeamType GetPossession()
    {
        return TeamWithPossession;
    }

    public AttachBall GetPlayerWithBall()
    {
        return currentPlayerWithBall;
    }
}