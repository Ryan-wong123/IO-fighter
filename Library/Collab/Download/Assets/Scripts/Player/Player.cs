using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Photon.Pun.UtilityScripts;

public class Player : MonoBehaviourPun
{
    #region playerController

    [Header("Player Controller")]
    //joystick
    private Joystick joystick;

    //cooldown time to fire next projectile
    public float deltaTime = 0;

    //photonview
    public PhotonView pv;

    //attack button
    public atkBtnScript atkBtn;

    //animator component
    private Animator animator;

    #endregion playerController

    #region playerAttack

    [Tooltip("Determines the interval between bullets fired in seconds")]
    //rate of firing projectile
    public float rateOfFire = 0.5f;

    //Prevents bullet spam
    private bool fireCD = false;

    #endregion playerAttack

    #region playerMovement

    [Header("Player Movement")]
    //rigidbody component
    private Rigidbody rb;

    //speed of player movement
    public float speed = 4f;

    #endregion playerMovement

    #region player ui

    [Header("Player UI")]
    //player's canvas to contain username and healthbar
    public GameObject playerCanvas;

    //username text of player
    public Text username;

    //healthbar of player
    public Image healthBar;

    //player's icon on minimap
    public GameObject playerIcon;

    #endregion player ui

    #region player stats

    [Header("Player Health")]
    //player's current health
    public float currentHealth;

    //player's max health
    private float maxHealth = 100f;

    //amount of damage player will take when hit by projectile
    private float damage = 5f;

    #endregion player stats

    #region leaderboard

    [Header("scoreboard")]
    //number of players in the room
    private int playercount;

    //scoreboard UI
    private GameObject scoreboard;

    //death UI screen
    private GameObject deathScreen;

    private PhotonView incomingProjectile;

    private bool isRegistered = false;

    #endregion leaderboard

    #region startOrAwake

    private void Awake()
    {
        //animator component
        animator = GetComponent<Animator>();
        //rigidbody component
        rb = GetComponent<Rigidbody>();

        //set current health to max health which is 100
        currentHealth = maxHealth;

        //photon send rate for synchronization
        PhotonNetwork.SendRate = 1000;

        //find the joystick gameobject
        GameObject joystickfind = GameObject.FindGameObjectWithTag("joystick") as GameObject;
        //find the joystick script component
        joystick = joystickfind.GetComponent<Joystick>();
        //find attack button
        atkBtn = GameObject.FindGameObjectWithTag("AttackBtn").GetComponent<atkBtnScript>();
        //find scoreboard gameobject
        scoreboard = GameObject.Find("MainCanvas").transform.Find("leaderboard").transform.Find("scorelist").gameObject;
        //find deathscreen UI
        deathScreen = GameObject.FindWithTag("Death Ui");

        //hide player's icon on minimap
        playerIcon.SetActive(false);

        //check if photonview is mine
        if (pv.IsMine)
        {
            //set this player's username
            username.text = PhotonNetwork.NickName;
            //synchronize scene with masterclient
            PhotonNetwork.AutomaticallySyncScene = true;
            //show only this player's icon on minimap on his device only
            playerIcon.SetActive(true);
            //hide the death screen UI
            deathScreen.SetActive(false);

            //update scoreboard
            updateScoreboard();
        }
        else
        {
            //set and show other player's username
            username.text = pv.Owner.NickName;
        }
    }

    #endregion startOrAwake

    #region update

    private void FixedUpdate()
    {
        //check if photonview is mine
        if (pv.IsMine)
        {
            //call out movement method
            movement();
            //call out shoot method
            shoot();
            //call out timer cooldown to shoot method
            timer();
        }
    }

    private void LateUpdate()
    {
        //update the position of all player's health bar and username to face the camera
        playerUI();

        //update scoreboard constantly to show the latest player's score
        updateScoreboard();

        if (!isRegistered)
        {
            if (pv.IsMine)
            {
                Leaderboard.registerPlayer(PhotonNetwork.LocalPlayer.NickName.ToString());
                isRegistered = true;
                return;
            }
        }
    }

    #endregion update

    #region takeDamageMethod

    private void OnTriggerEnter(Collider other)
    {
        //check if photonview is mine
        if (pv.IsMine)
        {
            //if a projectile hit the player
            if (other.gameObject.CompareTag("projectile"))
            {
                //play getting hit animation
                animator.SetTrigger("getHit");

                incomingProjectile = other.gameObject.GetComponent<PhotonView>();

                //minus health and synchronize this player's health to other player's device to show updated health
                pv.RPC("takeDamage", RpcTarget.AllBuffered);
            }
        }
    }

    //minus health method
    [PunRPC]
    private void takeDamage()
    {
        //minus health with the damage amount
        currentHealth -= damage;
        //update player's health
        healthBar.fillAmount = currentHealth / maxHealth;

        //if the player's health reaches 0
        if (currentHealth <= 0)
        {
            //check if photonview is mine
            if (pv.IsMine)
            {
                //destroy this player's gameobject and all its data across the network
                PhotonNetwork.Destroy(this.gameObject);
                //show the death screen
                deathScreen.SetActive(true);
                incomingProjectile.Owner.AddScore(1);
            }
        }
    }

    #endregion takeDamageMethod

    #region movementMethod

    //Player movement input
    private Vector3 GetInputDirection()
    {
        Vector3 direction = new Vector3();

        if (Input.GetKey(KeyCode.W) || joystick.Vertical >= .1f)
        {
            direction += Vector3.forward;
        }

        if (Input.GetKey(KeyCode.S) || joystick.Vertical <= -.1f)
        {
            direction += Vector3.back;
        }

        if (Input.GetKey(KeyCode.A) || joystick.Horizontal <= -.1f)
        {
            direction += Vector3.left;
        }

        if (Input.GetKey(KeyCode.D) || joystick.Horizontal >= .1f)
        {
            direction += Vector3.right;
        }
        return direction;
    }

    //movement of player
    private void movement()
    {
        var playerMovement = GetInputDirection() * speed * Time.deltaTime;
        //update the player's position
        gameObject.transform.position += playerMovement;

        //if player is moving
        if (playerMovement != Vector3.zero)
        {
            //play running animation
            animator.SetBool("run", true);

            //Player will face the direction of movement
            gameObject.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerMovement.normalized), 0.2f);
        }
        else
        {
            //stop running animation
            animator.SetBool("run", false);
        }
    }

    #endregion movementMethod

    #region PlayerUIMethod

    private void playerUI()
    {
        //This line makes sure health bar and username always face the camera
        //Vector3 back because of flipped words
        //IF IT AINT BROKEN, DON'T TOUCH
        playerCanvas.transform.LookAt(playerCanvas.transform.position + Camera.main.transform.rotation * Vector3.back, Camera.main.transform.rotation * Vector3.up);
    }

    #endregion PlayerUIMethod

    #region attackMethod

    private void shoot()
    {
        bool btnPressed = atkBtn.isPressed;

        if (pv.IsMine && !fireCD && btnPressed)
        {
            fireProjectile();
        }
    }

    //shoot projectile
    private void fireProjectile()
    {
        //Prevents bullet spam
        fireCD = true;
        deltaTime = 0;

        //Shoots the bullet
        GameObject projectile = PhotonNetwork.Instantiate("bullet", transform.position + (transform.forward * 20), transform.rotation) as GameObject;
        //add velocity to move the projectile forward
        projectile.GetComponent<Rigidbody>().velocity = transform.forward * 100;
    }

    //timer for cool down of firing the next projectile
    private void timer()
    {
        deltaTime += Time.deltaTime;

        if (deltaTime > rateOfFire && fireCD)
        {
            fireCD = false;
        }
    }

    #endregion attackMethod

    #region leaderboardMethod

    //method for showing scores on the scoreboard
    private void updateScoreboard()
    {
        //show the scores of all players
        foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
        {
            //testing for leaderboard
            Leaderboard.setPosition(p.NickName.ToString(), p.GetScore());
        }
    }

    #endregion leaderboardMethod
}