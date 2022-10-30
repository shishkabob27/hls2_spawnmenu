using System;
using System.Collections.Generic;
using System.Linq;
namespace SpawnMenuAddon;

[Library]
public partial class WeaponList : Panel
{
	VirtualScrollPanel Canvas;
	public WeaponList()
	{
		AddClass( "spawnpage" );
		AddChild( out Canvas, "canvas" );

		Canvas.Layout.AutoColumns = true;
		Canvas.Layout.ItemWidth = 96;
		Canvas.Layout.ItemHeight = 96;
		Canvas.OnCreateCell = ( cell, data ) =>
		{
			if ( data is TypeDescription type )
			{
				var btn = cell.Add.Button( type.Title );
				btn.AddClass( "icon" );
				btn.AddEventListener( "onclick", () => ConsoleSystem.Run( "ent_create", type.ClassName ) );
				var t = Texture.Load( FileSystem.Mounted, $"/ui/spawnmenu/{type.ClassName}.png", false );
				if (t == null || t.IsPromise || true)
				{ 
                    var ImgScalar = 3f;

                    var b = type.Create<ModelEntity>();
                    b.Transmit = TransmitType.Always;
                    var txt = Texture.CreateRenderTarget().WithHeight( Canvas.Layout.ItemHeight.Value.FloorToInt() ).WithWidth( Canvas.Layout.ItemWidth.Value.FloorToInt() );
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
                    scn.Position = new Vector3(1,1,0.9f) * dist;
                    scn.Rotation = Rotation.LookAt( -1 * (scn.Position  - box).Normal );//Rotation.From(30,0,0);
                    var scnlight = new SceneLight( scn.World, new Vector3( 32, 128, 128 ), 1024, Color.White );
                    scnlight.LightColor = Color.White.Lighten(2);
                    scnlight.ShadowsEnabled = true;
                    scn.AmbientLightColor = Color.FromBytes(180,240,255).Darken(0.5f);
                    Graphics.RenderToTexture( scn, txt2 ); 
                    btn.Style.BackgroundImage = txt2;

                    b.Delete();
                    scnobj.Delete();
                    scn.World.Delete();
                } else
                {
                    btn.Style.BackgroundImage = t;
                    btn.Style.BackgroundPositionX = Length.Auto;
                    btn.Style.BackgroundPositionY = Length.Auto;
                }
				var x = type.GetType();

               
			}
		};



    }
	string[] HL1Weapons = new string[] { "weapon_crowbar", "weapon_9mmhandgun", "weapon_357", "weapon_mp5", "weapon_shotgun", "weapon_crossbow", "weapon_rpg", "weapon_gauss", "weapon_egon", "weapon_hornet","weapon_handgrenade","weapon_tripmine","weapon_satchel","weapon_snark" };
    string prevtab;
    [Event.Tick]
	void update()
	{
		if ( prevtab == SpawnMenu.Current.SelectedTab ) return;
		prevtab = SpawnMenu.Current.SelectedTab;
		
        Canvas.Clear();
        //GARBAGE

        var ents = TypeLibrary.GetDescriptions<Weapon>()
                                    //.Where( x => x.HasTag( "spawnable" ) )
                                    .OrderBy( x => x.Title )
                                    .ToList();
		if ( SpawnMenu.Current.SelectedTab == "other" )
		{
			ents.RemoveAll( x => HL1Weapons.Any( x.ClassName.Contains ) );
		} else
		{

            ents.RemoveAll( x => !HL1Weapons.Any( x.ClassName.Contains ) );
        }
        foreach ( var entry in ents )
        {
            Canvas.AddItem( entry );
        }
    }

}
