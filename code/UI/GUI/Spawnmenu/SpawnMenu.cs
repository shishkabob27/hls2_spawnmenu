global using Sandbox;
global using System.Linq;
global using System.Threading.Tasks;
global using XeNPC;
global using Sandbox.UI;
global using Sandbox.UI.Tests;
global using Sandbox.UI.Construct;
namespace SpawnMenu;
[Library, UseTemplate("/resource/templates/SpawnMenu.html")]
public partial class SpawnMenu : GUIPanel
{
	public SpawnMenu()
	{
        Style.Left = 0;
        Style.Right = 0;
        Style.Top = 0;
        Style.Bottom = 0;
        Focus();
	}

    public override void Tick()
    {
        base.Tick();
        Drag();
        SetClass("active", MenuOpen);
    }

    [Event.BuildInput]
    public void ProcessClientInput(InputBuilder input)
    {
        if (input.Pressed(InputButton.Grenade))
        {
            Style.Left = (Screen.Width / 2) - (Box.Rect.Width / 2);
            Style.Top = (Screen.Height / 2) - (Box.Rect.Height / 2);
            MenuOpen = !MenuOpen;
        }
    }
}

partial class HLGame
{
	static SMhudpanel sphudpanel { get; set; }

	[ConCmd.Client]
	public static void create_spawnmenu()
	{
		Log.Info("Spawnmenu created");
        sphudpanel = new SMhudpanel();
    }
}

public class SMhudpanel : HudEntity<GUIRootPanel>
{
    public SMhudpanel()
    {
            RootPanel.AddChild<SpawnMenu>();
    }
}
