using UFramework.GameCommon;

public class GameBoard : IView
{
    public string ViewPath => "UI/GameBoard";
    public BindUI UIInstance { get; set; }
    public UILayer UILayer { get; set; }

    public void OnInit()
    {
    }

    public void OnShow(params object[] param)
    {
        (UIInstance as GameBoardUI).text.text = "测试注入";
    }

    public void LocalUpdate(float dt)
    {
    }

    public void OnClose()
    {
    }
}