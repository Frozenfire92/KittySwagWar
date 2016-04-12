using UnityEngine;

public class Swag : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        Kitty k = collision.gameObject.GetComponent<Kitty>();
        if (k != null) { k.swagLevel++; k.UpdateScore(); }
        else { Debug.Log("Trying to give swag to not kitty"); }
        Destroy(gameObject);
    }
}
