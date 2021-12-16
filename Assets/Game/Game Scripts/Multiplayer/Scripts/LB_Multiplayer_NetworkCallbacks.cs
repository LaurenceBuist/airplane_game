
using LaurenceBuist;
using UnityEngine;
using UnityEngine.SceneManagement;


[BoltGlobalBehaviour]
public class LB_Multiplayer_NetworkCallbacks : Bolt.GlobalEventListener
{
    // As soon as the scene has loaded
    public override void SceneLoadLocalDone(string map)
    {
        // Instantiate camera
        LB_Camera_AuthorativeMovement.Instantiate();

        // Instantiate plane on correct position
        Vector3 spawnPosition;
        Quaternion spawnRotation;
        if (SceneManager.GetActiveScene().name == "World2")
        {
            spawnPosition = new Vector3(696, 26.5f, -976);
            spawnRotation = new Quaternion(0, -90, 0, 0);
        }
        else // Default
        {
            spawnPosition = new Vector3(19, 27, -160);
            spawnRotation = Quaternion.identity;
        }

        // Instantiate plane
        BoltEntity player = BoltNetwork.Instantiate(BoltPrefabs.BomberPlayer, spawnPosition, spawnRotation);        //NOTE: It should say 'BoltPrefabs.Indiepixel_Airplane' instead of 'BoltPrefabs.BomberPlayer' but it is mixed up somehow
        
        // Instantiate LogSystem
        //BoltNetwork.Instantiate(BoltPrefabs.Canvas, Vector3.zero, Quaternion.Euler(0,0,0));


        // Set following cam to follow the instantiated plane
        LB_Camera_AuthorativeMovement.instance.SetTarget(player);
    }
}
