using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public OverlayTile onTile;
    
    public float speed;
    OverlayTile target;
    bool isMoving = false;

    private PathFinder pathFinder;
    //private List<OverlayTile> path = new List<OverlayTile>();

    // Start is called before the first frame update
    void Start()
    {
        pathFinder = new PathFinder();
    }

    // Update is called once per frame
    void Update()
    {
        
        //triggered in ApproachPlayer method below, called from MouseController
        if (isMoving)
        {
            
            MoveToTile(target);

            if (transform.position == target.gridLocation)
            {
                isMoving = false;
            }
        }
        
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("game over");
        if (other.gameObject.CompareTag("Player")) 
        {
            
            //SceneManager.LoadScene(SceneManager.GetActiveScene());
        }
    }

    //called by MouseController on rightclick
    public void ApproachPlayer(OverlayTile enemyTile, OverlayTile playerTile)
    {

        List<OverlayTile> path = pathFinder.FindPath(enemyTile, playerTile);
        target = path[0];

        isMoving = true;
        
    }

    //altered version of the one in MouseController
    private void MoveToTile(OverlayTile tile)
    {
        var step = speed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, tile.transform.position, step);

        if(Vector2.Distance(transform.position, tile.transform.position) < 0.0001f){
            PositionEnemyOnTile(tile);
        }
    }

    //also altered verson of the one in MouseController
    public void PositionEnemyOnTile(OverlayTile tile)
    {
        transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, tile.transform.position.z);
        GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;
        onTile = tile;
    }
    

}
