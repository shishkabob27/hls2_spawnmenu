global using Sandbox;
global using System.Linq;
global using System.Threading.Tasks;
global using XeNPC;
global using Sandbox.UI;
global using Sandbox.UI.Tests;
global using Sandbox.UI.Construct;
using System;

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
        bool oldm = false;
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
            if ( MenuOpen )
            {
                if ( oldm != MenuOpen && Box.Rect.Width != 0 )
                {
                    oldm = MenuOpen;
                    Position.x = ( Screen.Width / 2 ) - ( Box.Rect.Width / 2 );
                    Position.y = ( Screen.Height / 2 ) - ( Box.Rect.Height / 2 );
                } 
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

        public static bool AlwaysUseRenderedIcons = true;
        public static Texture MakeIcon(TypeDescription type)
        {
            var ImgWidth = 96;
            var ImgHeight = 96;
            var ImgScalar = 3f;

            if ( type == null ) return default;
            var b = type.Create<ModelEntity>();
            if ( b == null ) return default;
            b.Position = new Vector3( 9999999, 9999999, 9999999 );

            b.EnableDrawing = false;
            b.EnableAllCollisions = false;
            b.Transmit = TransmitType.Never;
            if ( b.Model == null ) return default;
            var txt = Texture.CreateRenderTarget().WithHeight( ImgHeight ).WithWidth( ImgWidth );
            var txt2 = txt.Create();
            var scn = new SceneCamera();
            scn.AntiAliasing = true;
            scn.World = new SceneWorld();
            scn.FieldOfView = 10;
            var trn = new Transform( new Vector3( 0, 0, 0 ), Rotation.From( 0, 0, 0 ) );
            var boundmax = b.Model.Bounds.Size.Length;
            var dist = ( boundmax / ImgScalar ) / MathF.Tan( MathX.DegreeToRadian( scn.FieldOfView ) / 2 );//8 * MathF.Sqrt(b.Model.Bounds.Size.Length);
                                                                                                           // Ideal distance  our camera should be to fit our object full on screen
            var scnobj = new SceneObject( scn.World, b.Model, trn );
            var box = ( scnobj.Bounds.Mins + scnobj.Bounds.Maxs ) / 2;
            scn.Position = new Vector3( 1, 1, 0.9f ) * dist;
            scn.Rotation = Rotation.LookAt( -1 * ( scn.Position - box ).Normal );//Rotation.From(30,0,0);
            var scnlight = new SceneLight( scn.World, new Vector3( 32, 128, 128 ), 1024, Color.White );
            scnlight.LightColor = Color.White.Lighten( 2 );
            scnlight.ShadowsEnabled = true;
            scn.AmbientLightColor = Color.FromBytes( 180, 240, 255 ).Darken( 0.5f );
            Graphics.RenderToTexture( scn, txt2 );

            b.Delete();
            scnobj.Delete();
            scn.World.Delete();
            return txt2;
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
