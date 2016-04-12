using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Kitty : MonoBehaviour
{
    public int player;
    public Color color;
    public int level = 0;
    public int swagLevel = 0;
    public float speed;
    public Direction dir;
    Vector2 movement;
    SpriteRenderer sr;
    public Text score;


    public void Reset()
    {
        swagLevel = 0;
        transform.position = new Vector3(Random.Range(5, 16), 0, 0);
        level = player;
        UpdateLevel();
        dir = (Direction)Random.Range(0, 2);
        FaceProperly();
        sr.color = color;
        UpdateScore();

    }

    public int RemoveSwag()
    {
        int ret = swagLevel;
        swagLevel = 0;
        UpdateScore();
        return ret;
    }

    public void UpdateScore()
    {
        score.text = "" + swagLevel;
    }

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        Reset();
    }

    void Update()
    {
        switch (dir)
        {
            case Direction.Left: movement = transform.position + (Vector3.left * speed); break;
            case Direction.Right: movement = transform.position + (Vector3.right * speed); break;
            default: Debug.Log("Unknown direction"); break;
        }
        transform.position = movement;
    }

    public void Button(string b)
    {
        switch (b)
        {
            case "a": Movelevel(1); break;
            case "b": Movelevel(-1); break;
            default: Debug.Log("Unknown button: " + b); break;
        }
    }

    void Movelevel(int val)
    {
        if (val == 1) { if (level < GameController.instance.maxLevelCount) { level++; } }
        else { if (level > 0) { level--; } }
        UpdateLevel();
    }

    void UpdateLevel()
    {
        transform.position = new Vector3(transform.position.x, level, transform.position.z);
    }

    void InverseDirection()
    {
        switch (dir)
        {
            case Direction.Left: dir = Direction.Right; break;
            case Direction.Right: dir = Direction.Left; break;
            default: break;
        }
        FaceProperly();
    }

    void FaceProperly()
    {
        switch (dir)
        {
            case Direction.Left: transform.localScale = new Vector3(-1, 1, 1); break;
            case Direction.Right: transform.localScale = Vector3.one; break;
            default: break;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        InverseDirection();
        if (collision.gameObject.tag == "Kitty")
        {
            Kitty other = collision.gameObject.GetComponent<Kitty>();
            if (other.swagLevel < swagLevel)
            {
                Debug.Log("Getting Swag from other kitty");
                swagLevel += other.RemoveSwag();
            }
            else
            {
                Debug.Log("Couldn't get others swag, mine" + swagLevel + " theirs" + other.swagLevel);
            }
        }
    }
}
