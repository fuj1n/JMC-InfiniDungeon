using UnityEngine;
using UnityEngine.UI;

public class SpriteAnimation : MonoBehaviour
{
    public Sprite[] frames;
    public float fps = 60;

    private SpriteRenderer spr;
    private Image img;

    private float frameDelay;
    private float timer;

    private int currentFrame = 0;

    private void Start()
    {
        if (frames.Length <= 0) {
            Destroy(this);
            return;
        }

        frameDelay = (1F / fps);

        spr = GetComponent<SpriteRenderer>();

        if (!spr)
        {
            img = GetComponent<Image>();
            if (!img)
            {
                Destroy(this);
                return;
            }
        }

        if (spr)
            spr.sprite = frames[0];
        else
            img.sprite = frames[0];

        timer = frameDelay;
    }

    private void Update()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
            return;
        }

        currentFrame++;

        if (currentFrame >= frames.Length)
            currentFrame = 0;

        if (spr)
            spr.sprite = frames[currentFrame];
        else
            img.sprite = frames[currentFrame];

        timer = frameDelay;
    }


}
