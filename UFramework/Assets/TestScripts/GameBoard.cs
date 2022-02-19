using UFramework.GameCommon;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void backHallBoard(){
        SceneLoadManager.GetInstance().LoadAppointScene("HallBoard");
    }
}
