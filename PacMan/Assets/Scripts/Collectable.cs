using UnityEngine;

/*
 * Collectable parent class for all collectables
 */
public abstract class Collectable : MonoBehaviour
{
    public abstract void HandleOnCollect();
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player") // determine if collision was with player
        {
            HandleOnCollect();
            this.gameObject.GetComponent<Collider>().enabled = false; //hide the pelletpe
            this.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
