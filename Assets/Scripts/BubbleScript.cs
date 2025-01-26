using UnityEngine;
using UnityEngine.U2D;

public class BubbleScript : MonoBehaviour
{
    public bool active;
    private BoxCollider2D screenCollider;
    private SpriteShapeRenderer spriteShapeRenderer;
    private SpriteShapeController sprite;
    
    private Spline spline;

    private Vector3 center;
    // private Transform centerSprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Debug.Log("Start");
        active = true;
        // centerSprite = transform.GetChild(0);
        // sprite = GetComponent<SpriteShapeController>();
        // spline = sprite.spline;
        // spline.SetPosition(0, new Vector3(-1, -1, 0));  // BL
        // spline.SetPosition(1, new Vector3(-1, 1, 0));   // TL
        // spline.SetPosition(2, new Vector3(1, 1, 0));    // TR
        // spline.SetPosition(3, new Vector3(1, -1, 0)); ; // BR
        // UpdateCenter();

        // Iterate through control points to find max values 
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 mouseScreenPos = Input.mousePosition;
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 0));
                mousePos = new Vector3(mousePos.x-transform.position.x, mousePos.y-transform.position.y, 0);
                MoveBubble(mousePos);
            }
        } else {
            spriteShapeRenderer.enabled = false;
        }
    }

    public void InitBubble(Vector3 newCenter, float size, BoxCollider2D collider)
    {
        // Debug.Log("InitBubble");
        // Debug.Log("New Center "+newCenter.x+" "+newCenter.y);
        transform.position = new Vector3(transform.position.x, transform.position.y, 1.0f);
        screenCollider = collider;
        spriteShapeRenderer = GetComponent<SpriteShapeRenderer>();
        sprite = GetComponent<SpriteShapeController>();
        spline = sprite.spline;
        spline.SetPosition(0, new Vector3(newCenter.x-size, newCenter.y-size, 0));  // BL
        spline.SetPosition(1, new Vector3(newCenter.x-size, newCenter.y+size, 0));   // TL
        spline.SetPosition(2, new Vector3(newCenter.x+size, newCenter.y+size, 0));    // TR
        spline.SetPosition(3, new Vector3(newCenter.x+size, newCenter.y-size, 0)); ; // BR
        UpdateCenter();
    }

    public bool OutsideRect()
    {
        bool outside = true;
        for (int i = 0; i < 4; i++)
        {
            // if (i == 0)
            // {
            //     Debug.Log(screenCollider.bounds + "  " + (spline.GetPosition(i) + transform.position));
            // }
            if (screenCollider.bounds.Contains(spline.GetPosition(i)+ transform.position))
            {
                outside = false;
            }
        }
        return outside;
    }
    
    void UpdateCenter()
    {
        float left = System.Math.Max(TL().x, BL().x);
        float right = System.Math.Min(TR().x, BR().x);
        float top = System.Math.Min(TL().y, TR().y);
        float bottom = System.Math.Max(BL().y, BR().y);

        center = new Vector3(left+(right-left)/2, bottom+(top-bottom)/2, 0);

        // centerSprite.localPosition = center;
    }

    void MoveBubble(Vector3 mousePos)
    {
        int pointCount = spline.GetPointCount();
        for (int i = 0; i < pointCount; i++)
        {
            Vector3 currentPos = spline.GetPosition(i);
            float distFromMouse = Vector3.Distance(mousePos, currentPos);
            // if (i == 0)
            // {
            //     Debug.Log(distFromMouse);
            // }
            
            if (distFromMouse < 0.2f)
            {
                Vector3 norm = Vector3.Normalize(mousePos - currentPos);
                Vector3 newPos = currentPos - (0.1f * norm);
                spline.SetPosition(i, newPos);
                UpdateCenter();
                MoveNeighbors(newPos-currentPos, i);
                UpdateCenter();
                int opposite = GetOpposite(i);
                Vector3 oppositePos = spline.GetPosition(opposite);
                if (!IsValidPointPlacement(oppositePos ,opposite))
                {
                    
                    Vector3 offset = center - oppositePos ;
                    Vector3 newOppositePos = 1.5f * (oppositePos + offset);
                    spline.SetPosition(opposite, newOppositePos);
                    UpdateCenter();
                }
                break;
            }
        }
    }

    int GetOpposite(int pointNumber){
        switch(pointNumber)
        {
            case 0: return 2;
            case 1: return 3;
            case 2: return 0;
            case 3: return 1;
            default: return 0;
        }
    }

    bool IsValidPointPlacement(Vector3 pos, int pointNumber)
    {
        switch(pointNumber)
        {   //BL
            case 0: 
                if(pos.x > center.x || pos.y > center.y){
                    return false;
                }
                return true;
            //TL
            case 1:
                if(pos.x > center.x || pos.y < center.y){
                    return false;
                }
                return true;
            //TR
            case 2:
                if(pos.x < center.x || pos.y < center.y){
                    return false;
                }
                return true;

            //BR
            case 3:
                if(pos.x < center.x || pos.y > center.y){
                    return false;
                }
                return true;

            default:
                return false;
        }
    }

    void MoveNeighbors(Vector3 offset, int pointNumber){
        switch(pointNumber)
        {   //BL
            case 0: 
                if (!IsValidPointPlacement(TL(), 1))
                {
                    spline.SetPosition(1, TL()+offset);
                }
                if (!IsValidPointPlacement(BR(),3))
                {
                    spline.SetPosition(3, BR()+offset);
                }
                break;
            //TL
            case 1:
                if (!IsValidPointPlacement(BL(), 0))
                {
                    spline.SetPosition(0, BL()+offset);
                }
                if (!IsValidPointPlacement(TR(), 2))
                {
                    spline.SetPosition(2, TR()+offset);
                }
                break;
            //TR
            case 2:
                if (!IsValidPointPlacement(TL(), 1))
                {
                    spline.SetPosition(1, TL()+offset);
                }
                if (!IsValidPointPlacement(BR(), 3))
                {
                    spline.SetPosition(3, BR()+offset);
                }
                break;

            //BR
            case 3:
                if (!IsValidPointPlacement(TR(), 2))
                {
                    spline.SetPosition(2, TR()+offset);
                }
                if (!IsValidPointPlacement(BL(), 0))
                {
                    spline.SetPosition(0, BL()+offset);
                }
                break;
        }
    }

    Vector3 BL()
    {
        return spline.GetPosition(0);
    }

    Vector3 TL()
    {
        return spline.GetPosition(1);
    }

    Vector3 TR()
    {
        return spline.GetPosition(2);
    }

    Vector3 BR()
    {
        return spline.GetPosition(3);
    }
}
