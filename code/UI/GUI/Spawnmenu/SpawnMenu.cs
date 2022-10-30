global using Sandbox;
global using System.Linq;
global using System.Threading.Tasks;
global using XeNPC;
global using Sandbox.UI;
global using Sandbox.UI.Tests;
global using Sandbox.UI.Construct;
namespace SpawnMenuAddon
{


    [Library, UseTemplate( "/resource/templates/SpawnMenu.html" )]
    public class SpawnMenu : GUIPanel
    {
        public static SpawnMenu Current;
        public string SelectedTab;
        public TabContainer MainSelector { get; set; }
        public TabContainer ModelSelector { get; set; }
        public TabContainer WeaponSelector { get; set; }
        public TabContainer EntitySelector { get; set; }
        public TabContainer NPCSelector { get; set; }
        public SpawnMenu()
        {
            Current = this;
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
            SetClass( "active", MenuOpen );
            switch ( MainSelector.ActiveTab )
            {
                case "models":
                    SelectedTab = ModelSelector.ActiveTab;
                    break;
                case "weapons":
                    SelectedTab = WeaponSelector.ActiveTab;
                    break;
                case "entities":
                    SelectedTab = EntitySelector.ActiveTab;
                    break;
                default:
                    break;
            }
//            SelectedTab =
        }

        [Event.BuildInput]
        public void ProcessClientInput( InputBuilder input )
        {
            if ( input.Pressed( InputButton.Grenade ) )
            {
                Style.Left = ( Screen.Width / 2 ) - ( Box.Rect.Width / 2 );
                Style.Top = ( Screen.Height / 2 ) - ( Box.Rect.Height / 2 );
                MenuOpen = !MenuOpen;
            }
        }
    }
}
partial class GAMER
{
    static SMhudpanel sphudpanel { get; set; }

    [Event.BuildInput]
    public static void InputB( InputBuilder input )
    {
        if ( input.Pressed( InputButton.Grenade ) )
        {
            ConsoleSystem.Run( "openspawnmenu" );
        }
    }
    [ConCmd.Client]
    public static void openspawnmenu()
    {
        if ( GUIRootPanel.Current.ChildrenOfType<SpawnMenuAddon.SpawnMenu>().Count() > 0 )
        {
            Log.Info( "Spawnmenu removed" );
            foreach ( var i in GUIRootPanel.Current.ChildrenOfType<SpawnMenuAddon.SpawnMenu>() )
            {
                i.Delete();
            }
        }
        else
        {
            Log.Info( "Spawnmenu created" );
            var a = GUIRootPanel.Current.AddChild<SpawnMenuAddon.SpawnMenu>();
            a.MenuOpen = true;
        }
    }
}

public class SMhudpanel : HudEntity<GUIRootPanel>
{
    public SMhudpanel()
    {
        RootPanel.AddChild<SpawnMenuAddon.SpawnMenu>();
    }
}
