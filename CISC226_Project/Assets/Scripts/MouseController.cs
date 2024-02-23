using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class MouseController : MonoBehaviour
{

    public float speed;
    [SerializeField] private CharacterInfo character; 
    private LevelManager levelManager;

    [SerializeField] private EnemyMovement enemy1;
    private OverlayTile enemySpawnTile;
    private bool enemySpawned = false;

    private PathFinder pathFinder;
    private List<OverlayTile> path = new List<OverlayTile>();
    private OverlayTile spawnTile;
    private OverlayTile prevTile;
    private bool spawned = false;


    // Start is called before the first frame update
    void Start()
    {
        pathFinder = new PathFinder();

        // Gets the level manager for reseting in case of enemies.
        levelManager = FindObjectOfType<LevelManager>();
        
    }


    // usinh late update so it occurs after overlay update but this is a lazy way
    // so we might need to make an event handler system in the future
    void LateUpdate()
    {
        if (!spawned){
            Debug.Log("spawn");
            var hit = GetTileAtPos(new Vector2(-0.5f, -3.5f));
            
            if (hit.HasValue)
            {
                spawnTile = hit.Value.collider.gameObject.GetComponent<OverlayTile>();
                PositionCharacterOnTile(spawnTile);
                character.onTile = spawnTile;
                prevTile = spawnTile;
                spawned = true;
            }

        }

        
        var focusedTileHit = GetFocusedOnTile();

        if(focusedTileHit.HasValue){

            OverlayTile tile = focusedTileHit.Value.collider.gameObject.GetComponent<OverlayTile>();
            transform.position = tile.transform.position;
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;
            
            // if (character.onTile.gridLocation == character.transform.position){
            //     character.onTile.hideTile();    
            // } else {
            //     character.onTile.showTile();
            // }
            
            //else {
            //     character.onTile.hideTile();
            // }

            // Lighten-up a square of tiles.
            if (spawned) {
                character.onTile.lightUpAllAdjacent(character.spotlightSize);
            }
            
            if (Input.GetMouseButtonDown(0)){
                    // should be this?: dk how to fix: overlayTile.GetComponent<Overlay>().showTile();
                    // just copied and pasted code from the function here lol



                if (spawned && tile != null){
                    path = pathFinder.FindPath(character.onTile, tile);
                }
            }
            
            
        }
        if(path.Count>0){
            MoveAlongPath();
        }
        if (spawned) {
            if (prevTile.gridLocation != character.onTile.gridLocation){
                //Debug.Log("showtile");
                
                // Darken a square of tiles.
                // prevTile.makeDark();
                character.onTile.darkenAllAdjacent(prevTile, character.spotlightSize);
            }
            prevTile=character.onTile;
        }

        
        // ENEMY STUFF WILL NEED TO BE REWORKED FOR MULTIPLE ENEMIES
        // ---------------------------------------------------------
        if (!enemySpawned){
            Debug.Log("enemySpawn");

            //spawning enemy (temp system--we'll prob need a better one)
            var enemyHit = GetTileAtPos(new Vector2(0.5f, 3.5f));
            if (enemyHit.HasValue)
            {
                enemySpawnTile = enemyHit.Value.collider.gameObject.GetComponent<OverlayTile>();
                enemy1.PositionEnemyOnTile(enemySpawnTile);
                enemy1.onTile = enemySpawnTile;

                enemySpawned = true;
            }
            
        }
        
        // for echolocation attracting enemy movement:
        if (Input.GetMouseButtonDown(1))
        {
            //for more than one enemy, could have a list or smth and iterate through
            var enemyHit = GetTileAtPos(enemy1.transform.position);
            OverlayTile enemy1Tile = enemyHit.Value.collider.gameObject.GetComponent<OverlayTile>();
            enemy1.ApproachPlayer(enemy1Tile, character.onTile);
        }

        //checks if enemy is touching player
        if (spawned && enemySpawned) 
        {
            if (character.onTile.Equals(enemy1.onTile))
            {
                Debug.Log("end level: reset");
                levelManager.ResetLevel();
            }
        }
        

    }

    private void MoveAlongPath()
    {
        var step = speed * Time.deltaTime;
        character.transform.position = Vector2.MoveTowards(character.transform.position, path[0].transform.position, step);

        if(Vector2.Distance(character.transform.position, path[0].transform.position) < 0.0001f){
            PositionCharacterOnTile(path[0]);
            path.RemoveAt(0);
        }
    }

    private void PositionCharacterOnTile(OverlayTile tile)
    {
        character.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, tile.transform.position.z);
        character.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;
        character.onTile = tile;
    }


    // rename this one lol
    public RaycastHit2D? GetFocusedOnTile(){
        // get mouse position
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //convert mouse position vector into 2d object
        Vector2 mousePos2d = new Vector2(mousePos.x, mousePos.y);
        // list of objects in raycast
        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2d, Vector2.zero);

        if(hits.Length > 0){
            return hits.OrderByDescending( i => i.collider.transform.position.z).First();
        }

        return null;
    }

    public RaycastHit2D? GetTileAtPos(Vector2 pos){
        // list of objects in raycast
        RaycastHit2D[] hits = Physics2D.RaycastAll(pos, Vector2.zero);

        if(hits.Length > 0){
            return hits.OrderByDescending( i => i.collider.transform.position.z).First();
        }

        return null;
    }

}
