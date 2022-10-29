global using Sandbox;
global using System.Linq;
global using System.Threading.Tasks;
global using XeNPC;
using Sandbox.UI;

[Library]
public partial class SpawnMenu : Panel
{
	public static SpawnMenu Instance;
 	public bool MenuOpen;

	public SpawnMenu()
	{
		Instance = this;

		StyleSheet.Load("/ui/spawnmenu.scss");

        var left = Add.Panel( "left" );
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
		};

	}

	public override void Tick()
	{
		base.Tick();
		Parent.SetClass( "spawnmenuopen", MenuOpen );
	}

	[Event.BuildInput]
    public void ProcessClientInput( InputBuilder input )
    {
        if ( input.Pressed( InputButton.Grenade ) )
        {
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
		//if (Host.IsClient)
		//{
			Log.Info("Spawnmenu created");
            sphudpanel = new SMhudpanel();
        //}
    }
}

public class SMhudpanel : HudEntity<HudRootPanel>
{
    public static SMhudpanel Current;


    public SMhudpanel()
    {
            RootPanel.AddChild<SpawnMenu>();
    }
}
