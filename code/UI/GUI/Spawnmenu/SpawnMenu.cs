global using Sandbox;
global using System.Linq;
global using System.Threading.Tasks;
global using XeNPC;
global using Sandbox.UI;
global using Sandbox.UI.Tests;
global using Sandbox.UI.Construct;
namespace SpawnMenu;
[Library]
[UseTemplate("/resource/templates/SpawnMenu.html")]
public partial class SpawnMenu : GUIPanel
{
	public static SpawnMenu Instance;

	public SpawnMenu()
	{
		Instance = this;

		//StyleSheet.Load("/ui/spawnmenu.scss");

        Style.Left = 0;
        Style.Right = 0;
        Style.Top = 0;
        Style.Bottom = 0;
        Focus();

        /*var left = Add.Panel( "left" );
		{
			var tabs = left.AddChild<ButtonGroup>();
			tabs.AddClass( "tabs" );

			var body = left.Add.Panel( "body" );

			{
				var npc = body.AddChild<NPCList>();
				tabs.SelectedButton = tabs.AddButtonActive( "NPC", ( b ) => npc.SetClass( "active", b ) );

				var weapon = body.AddChild<WeaponList>();
				tabs.AddButtonActive( "Weapons", ( b ) => weapon.SetClass( "active", b ) );

				var ents = body.AddChild<EntityList>();
				tabs.AddButtonActive( "Entities", ( b ) => ents.SetClass( "active", b ) );

				var models = body.AddChild<CloudModelList>();
				tabs.AddButtonActive( "asset.party", ( b ) => models.SetClass( "active", b ) );
			}
		};*/

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
    public static SMhudpanel Current;


    public SMhudpanel()
    {
            RootPanel.AddChild<SpawnMenu>();
    }
}
