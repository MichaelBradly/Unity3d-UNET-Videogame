using UnityEngine;
using UnityEngine.Networking;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] int maxHealth = 3;

    //Only Server can do SyncVar but client will call Hook method
    [SyncVar (hook = "OnHealthChanged")] int health;

    Player player;
    

    void Awake()
    {
        player = GetComponent<Player>();
    }

    [ServerCallback] //attribute can only run on the server
    void OnEnable()
    {
        health = maxHealth;
    }

    [Server]
    public bool TakeDamage()
    {
        bool died = false;

        if (health <= 0)
            return died;

        health--;
        died = health <= 0;

        RpcTakeDamage(died);

        return died;
    }

    [ClientRpc]
    void RpcTakeDamage(bool died)
    {
        if (isLocalPlayer)
        {
            PlayerCanvasScript.canvas.FlashDamageEffect();
        }

        if (died)
            player.Die();
    }

    void OnHealthChanged(int value)
    {
        health = value;
        if (isLocalPlayer)
        {
            try
            {
                PlayerCanvasScript.canvas.SetHealth(value);
            }
            catch { }; 
        }
    }
}