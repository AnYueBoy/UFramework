using UFramework.GameCommon;

public class GameBoard : IView
{
    public string ViewPath => "UI/GameBoard"; 
    public string ViewName { get; }
    public RuntimeComponent UIInstance { get; set; }
    public UILayer UILayer { get; set; }
    public void OnShow(params object[] param)
    {
        
    }

    public void OnClose()
    {
    }
}