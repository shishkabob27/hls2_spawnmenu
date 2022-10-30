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
				btn.Style.BackgroundImage = Texture.Load( FileSystem.Mounted, $"/ui/spawnmenu/{type.ClassName}.png", false );
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
