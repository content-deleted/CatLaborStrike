using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class keySpriteController : MonoBehaviour
{
    public Sprite kitchenSprite;
    void Start()
    {
        switch (LevelController.currentLevel) {
            case "levels/tutorial": {
                break;
            }
            case "levels/kitchen": {
                GetComponent<SpriteRenderer>().sprite = kitchenSprite;
                break;
            }
            default: {
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
