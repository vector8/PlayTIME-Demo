using UnityEngine;
using System.Collections;

public class FlagPole : CustomAction
{
    private GameObject flag;
    private GameObject player;
    private Vector2 flagStartPos = new Vector2(-0.4f, 2.95f), flagEndPos = new Vector2(-0.4f, -3.3f);
    private bool flagFinished = false, marioFinished = false;
    private Vector2 exitStartPos, exitEndPos;
    private float flagLerpTime = 0f;
    private float FLAG_MOVE_DURATION = 1f;
    private float marioEndYPosition;
    private float marioSlideSpeed = 7f;
    private float marioExitTimer = 0f;
    private float MARIO_EXIT_DURATION = 2f;
    private bool marioCollided = false;
    private Animator playerAnim;

    void Update()
    {
        if(marioCollided)
        {
            // Move flag
            flagLerpTime += Time.deltaTime;

            if (flagLerpTime > FLAG_MOVE_DURATION)
            {
                flagLerpTime = FLAG_MOVE_DURATION;
                flagFinished = true;
            }

            flag.transform.localPosition = Vector2.Lerp(flagStartPos, flagEndPos, flagLerpTime / FLAG_MOVE_DURATION);

            // Move Mario
            if(flagFinished && marioFinished)
            {
                // walk mario towards the exit.
                Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
                rb.isKinematic = false;
                player.GetComponent<BoxCollider2D>().isTrigger = false;
                rb.velocity = new Vector2(5.1f/2f, rb.velocity.y);
                playerAnim.SetBool("HorizontalMovement", true);

                marioExitTimer += Time.deltaTime;
                if(marioExitTimer > MARIO_EXIT_DURATION)
                {
                    LevelManager.instance.revert();
                    LevelManager.instance.paused = true;
                    Time.timeScale = 0;
                    return;
                }

                //player.transform.position = Vector2.Lerp(exitStartPos, exitEndPos, marioExitLerpTime / MARIO_EXIT_DURATION);
            }
            else if(player.transform.position.y > marioEndYPosition)
            {
                Vector2 playerPos = player.transform.position;
                playerPos.y = playerPos.y - marioSlideSpeed * Time.deltaTime;
                if(playerPos.y < marioEndYPosition)
                {
                    playerPos.y = marioEndYPosition;
                    marioFinished = true;
                }
                player.transform.position = playerPos;
            }
        }
    }

    public override void initialize()
    {
        // assert transform.childCount == 1
        flag = transform.GetChild(0).gameObject;
    }

    public override void run(GameObject other = null, int id = 0)
    {
        if(!marioCollided && other != null && other.tag == "Player")
        {
            // Mario has collided into the flagpole, run end of level behaviour.
            marioCollided = true;
            // turn off all player control
            player = other;
            player.GetComponent<Move>().enabled = false;
            player.GetComponent<Jump>().enabled = false;
            player.GetComponent<FirePower>().enabled = false;
            player.GetComponent<Rigidbody2D>().isKinematic = true;
            player.GetComponent<BoxCollider2D>().isTrigger = true;
            player.transform.position = new Vector2(transform.position.x - 0.3f, player.transform.position.y);
            marioEndYPosition = transform.position.y - 3.4f;
            exitStartPos = new Vector2(player.transform.position.x, marioEndYPosition);
            exitEndPos = new Vector2(player.transform.position.x + 5.1f, marioEndYPosition);
            playerAnim = player.GetComponent<Animator>();
            playerAnim.SetBool("HorizontalMovement", false);
            playerAnim.SetBool("Jumping", false);
        }
    }

    public override void reset()
    {
        flag.transform.localPosition = flagStartPos;
        flagLerpTime = 0f;
        marioExitTimer = 0f;
        marioCollided = false;
        flagFinished = false;
        marioFinished = false;
        if(player != null)
        {
            player.GetComponent<Move>().enabled = true;
            player.GetComponent<Jump>().enabled = true;
            player.GetComponent<FirePower>().enabled = true;
            player.GetComponent<Rigidbody2D>().isKinematic = false;
            player.GetComponent<BoxCollider2D>().isTrigger = false;
        }
    }
}
