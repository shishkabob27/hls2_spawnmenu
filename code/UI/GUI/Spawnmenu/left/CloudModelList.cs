namespace SpawnMenuAddon;

[Library]
public partial class CloudModelList : Panel
{
	VirtualScrollPanel Canvas;

	public CloudModelList()
	{
        AddClass("spawnpage");
        AddChild(out Canvas, "canvas");

        Canvas.Layout.AutoColumns = true;
        Canvas.Layout.ItemWidth = 96;
        Canvas.Layout.ItemHeight = 96;

        Canvas.OnCreateCell = (cell, data) =>
        {
            var file = (Package)data;
            var btn = cell.Add.Button(file.Title);
            btn.AddClass("icon");
            btn.AddEventListener("onclick", () => ConsoleSystem.Run("spawn", file.FullIdent));
            btn.Style.BackgroundImage = Texture.Load(file.Thumb);
        };

        _ = UpdateItems();
    }

	public async Task UpdateItems( int offset = 0 )
	{
		var q = new Package.Query();
		q.Type = Package.Type.Model;
		q.Order = Package.Order.Newest;
		q.Take = 1000;
		q.Skip = offset;

		var found = await q.RunAsync( default );
		Canvas.SetItems( found );

		// TODO - auto add more items here
	}

	public void RefreshItems()
	{
		Canvas.Clear();
		_ = UpdateItems();
	}

	static async Task<string> SpawnPackageModel( string packageName, Vector3 pos, Rotation rotation, Entity source )
	{
		var package = await Package.Fetch( packageName, false );
		if ( package == null || package.PackageType != Package.Type.Model || package.Revision == null )
		{
			// spawn error particles
			return null;
		}

		if ( !source.IsValid ) return null; // source entity died or disconnected or something

		var model = package.GetMeta( "PrimaryAsset", "models/dev/error.vmdl" );
		var mins = package.GetMeta( "RenderMins", Vector3.Zero );
		var maxs = package.GetMeta( "RenderMaxs", Vector3.Zero );

		// downloads if not downloads, mounts if not mounted
		await package.MountAsync();

		return model;
	}

	[ConCmd.Server( "spawn" )]
	public static async Task Spawn( string modelname )
	{
		var owner = ConsoleSystem.Caller?.Pawn;

		if ( ConsoleSystem.Caller == null )
			return;

		var tr = Trace.Ray( owner.EyePosition, owner.EyePosition + owner.EyeRotation.Forward * 500 )
			.UseHitboxes()
			.Ignore( owner )
			.Run();

		var modelRotation = Rotation.From( new Angles( 0, owner.EyeRotation.Angles().yaw, 0 ) ) * Rotation.FromAxis( Vector3.Up, 180 );

		//
		// Does this look like a package?
		//
		if ( modelname.Count( x => x == '.' ) == 1 && !modelname.EndsWith( ".vmdl", System.StringComparison.OrdinalIgnoreCase ) && !modelname.EndsWith( ".vmdl_c", System.StringComparison.OrdinalIgnoreCase ) )
		{
			modelname = await SpawnPackageModel( modelname, tr.EndPosition, modelRotation, owner );
			if ( modelname == null )
				return;
		}

		var model = Model.Load( modelname );
		if ( model == null || model.IsError )
			return;

		var ent = new Prop
		{
			Position = tr.EndPosition + Vector3.Down * model.PhysicsBounds.Mins.z,
			Rotation = modelRotation,
			Model = model
		};

		// Let's make sure physics are ready to go instead of waiting
		ent.SetupPhysicsFromModel( PhysicsMotionType.Dynamic );

		// If there's no physics model, create a simple OBB
		if ( !ent.PhysicsBody.IsValid() )
		{
			ent.SetupPhysicsFromOBB( PhysicsMotionType.Dynamic, ent.CollisionBounds.Mins, ent.CollisionBounds.Maxs );
		}
	}
}
