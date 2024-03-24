// Source: https://gist.github.com/almirage/e9e4f447190371ee6ce9 (24/03/2024)

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageAnimation : MonoBehaviour
{

    public Sprite[] sprites;
    public int framesPerSprite = 6;
    public bool loop = true;
    public bool destroyOnEnd = false;

    private int index = 0;
    private Image image;
    private int frame = 0;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    void FixedUpdate()
    {
        if (!loop && index == sprites.Length)
            return;
        frame++;
        if (frame < framesPerSprite)
            return;
        image.sprite = sprites[index];
        frame = 0;
        index++;
        if (index >= sprites.Length)
        {
            if (loop)
                index = 0;
            if (destroyOnEnd)
                Destroy(gameObject);
        }
    }
}