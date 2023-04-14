using Sandbox.UI.Tests;

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
		Canvas.Layout.ItemWidth = 91;
		Canvas.Layout.ItemHeight = 91;

		Canvas.OnCreateCell = (cell, data) =>
		{
			var file = (Package)data;
			var btn = cell.Add.Button(file.Title);
			btn.AddClass("icon");
			btn.AddEventListener("onclick", () => ConsoleSystem.Run("spawnmenu_spawn", file.FullIdent, SpawnMenu.Current.TypeSelector.ActiveTab));
			btn.Style.BackgroundImage = Texture.Load(file.Thumb);
		};

		_ = UpdateItems();
	}
	string prevSearch = "";
	string prevtab;
	bool prevshowincomp;
	public override void Tick()
	{
		if (SpawnMenu.Current.MainSelector.ActiveTab != "assetparty") return;
		if (prevSearch == SpawnMenu.Current.SearchQuery && prevtab == SpawnMenu.Current.SelectedTab && prevshowincomp == SpawnMenu.Current.ShowIncompatible) return;
		prevSearch = SpawnMenu.Current.SearchQuery;
		prevtab = SpawnMenu.Current.SelectedTab;
		prevshowincomp = SpawnMenu.Current.ShowIncompatible;
		RefreshItems();
	}
	public async Task UpdateItems(int offset = 0)
	{
		var Type = "type:model";

		switch (SpawnMenu.Current.TypeSelector.ActiveTab)
		{
			case "model":
				Type = "type:model";
				break;
			case "addon":
				Type = "type:addon";
				break;
			case "material":
				Type = "type:material";
				break;
			case "map":
				Type = "type:map";
				break;
			case "sound":
				Type = "type:sound";
				break;
		}


		var Order = "order:newest";
		switch (prevtab)
		{
			case "Most Recent":
				Order = "order:newest";
				break;
			case "Most Popular":
				Order = "order:popular";
				break;
			case "Most Downloads":
				Order = "order:live";
				break;
			case "Trending":
				Order = "order:trending";
				break;
		}
		var Take = 200;
		var Skip = offset;
		var Search = "search:" + SpawnMenu.Current.SearchQuery;

		var found = await Package.FindAsync($"{Order} {Type} {Search}", Take, Skip);
		var foundNew = found.Packages.ToList();
		if (SpawnMenu.Current.TypeSelector.ActiveTab == "addon" && !SpawnMenu.Current.ShowIncompatible)
		{
			foundNew.RemoveAll(x => !(x.GetMeta<string>("ParentPackage") == "shishkabob.hls2" || x.Tags.Contains("game:any") || x.Tags.Contains("game:shishkabob.hls2")));
		}
		Canvas.SetItems(foundNew);

		for (var i = 1; i < 5; i++)
		{
			Take = 200;
			Skip = 200 * i;
			Search = "search:" + SpawnMenu.Current.SearchQuery;

			var found2 = await Package.FindAsync($"{Order} {Type} {Search}", Take, Skip);
			var foundNew2 = found2.Packages.ToList();
			if (SpawnMenu.Current.TypeSelector.ActiveTab == "addon" && !SpawnMenu.Current.ShowIncompatible)
			{
				foundNew2.RemoveAll(x => !(x.GetMeta<string>("ParentPackage") == "shishkabob.hls2" || x.Tags.Contains("game:any") || x.Tags.Contains("game:shishkabob.hls2")));
			}
			Canvas.AddItems(foundNew2.ToArray());
		}

		// TODO - auto add more items here
	}

	public void RefreshItems()
	{
		Canvas.Clear();
		_ = UpdateItems();
	}

	static async Task<string> SpawnPackageModel(string packageName, Vector3 pos, Rotation rotation, Entity source)
	{
		var package = await Package.Fetch(packageName, false);
		if (package == null || package.PackageType != Package.Type.Model || package.Revision == null)
		{
			// spawn error particles
			return null;
		}

		if (!source.IsValid) return null; // source entity died or disconnected or something

		var model = package.GetMeta("PrimaryAsset", "models/dev/error.vmdl");
		var mins = package.GetMeta("RenderMins", Vector3.Zero);
		var maxs = package.GetMeta("RenderMaxs", Vector3.Zero);

		// downloads if not downloads, mounts if not mounted
		await package.MountAsync();

		return model;
	}
	static async Task<string> SpawnPackageSound(string packageName, Vector3 pos, Rotation rotation, Entity source)
	{
		var package = await Package.Fetch(packageName, false);
		if (package == null || package.PackageType != Package.Type.Sound || package.Revision == null)
		{
			// spawn error particles
			return null;
		}

		if (!source.IsValid) return null; // source entity died or disconnected or something

		var asset = package.GetMeta("PrimaryAsset", "sounds/dev/error.sound");

		// downloads if not downloads, mounts if not mounted
		await package.MountAsync();
		Log.Info($"Playing sound {asset}");
		Sound.FromEntity(asset, source);
		return asset;

	}
	static async Task<string> SpawnPackageMaterial(string packageName, Entity source)
	{
		var package = await Package.Fetch(packageName, false);
		if (package == null || package.PackageType != Package.Type.Material || package.Revision == null)
		{
			// spawn error particles
			return null;
		}

		if (!source.IsValid) return null; // source entity died or disconnected or something

		var asset = package.GetMeta("PrimaryAsset", "materials/dev/error.vmat");

		// downloads if not downloads, mounts if not mounted
		await package.MountAsync();
		return asset;

	}

	static async Task<string> SpawnPackageAddon(string packageName, Entity source)
	{
		var package = await Package.Fetch(packageName, false);
		if (package == null || package.PackageType != Package.Type.Addon || package.Revision == null)
		{
			// spawn error particles
			return null;
		}

		if (!source.IsValid) return null; // source entity died or disconnected or something

		var asset = package.GetMeta("PrimaryType", "ErrorType");
		var parentpkg = package.GetMeta("ParentPackage", "all games.");

		// downloads if not downloads, mounts if not mounted
		await package.MountAsync(true);

		Log.Info($"Loading addon \"{packageName}\" for \"{parentpkg}\" with primary type \"{asset}\"");
		return asset;

	}
	static async Task<string> SpawnPackageGeneric(string packageName, Entity source)
	{
		var package = await Package.Fetch(packageName, false);
		if (package == null || package.Revision == null)
		{
			// spawn error particles
			return null;
		}

		if (!source.IsValid) return null; // source entity died or disconnected or something

		var asset = package.GetMeta("PrimaryAsset", "sounds/dev/error");

		// downloads if not downloads, mounts if not mounted
		await package.MountAsync();

		Log.Info($"Loading package {asset}");
		return asset;

	}

	[ConCmd.Server("spawnmenu_spawn")]
	public static void Spawn(string ident, string activetab)
	{
		var owner = ConsoleSystem.Caller?.Pawn as Entity;
		Log.Info(ident);
		if (ConsoleSystem.Caller == null)
			return;
		switch (activetab)
		{
			case "model":
				SpawnModel(ident, owner);
				break;
			case "addon":
				DownloadAddon(ident, owner);
				break;
			case "material":
				SetMaterial(ident, owner);
				break;
			case "map":
				Game.ChangeLevel(ident);
				break;
			case "sound":
				SpawnPackageSound(ident, Vector3.Zero, Rotation.From(Angles.Zero), owner);
				break;
			default:
				SpawnModel(ident, owner);
				break;
		}
	}
	static async void DownloadAddon(string ident, Entity owner)
	{
		var modelname = await SpawnPackageAddon(ident, owner);
	}
	static async void SpawnModel(string modelname, Entity owner)
	{

		var tr = Trace.Ray(owner.AimRay.Position, owner.AimRay.Position + owner.AimRay.Forward * 500)
			.UseHitboxes()
			.Ignore(owner)
			.Run();

		var modelRotation = Rotation.From(new Angles(0, owner.AimRay.Forward.EulerAngles.yaw, 0)) * Rotation.FromAxis(Vector3.Up, 180);

		//
		// Does this look like a package?
		//
		if (modelname.Count(x => x == '.') == 1 && !modelname.EndsWith(".vmdl", System.StringComparison.OrdinalIgnoreCase) && !modelname.EndsWith(".vmdl_c", System.StringComparison.OrdinalIgnoreCase))
		{
			modelname = await SpawnPackageModel(modelname, tr.EndPosition, modelRotation, owner);
			if (modelname == null)
				return;
		}

		var model = Model.Load(modelname);
		if (model == null || model.IsError)
			return;

		var ent = new Prop
		{
			Position = tr.EndPosition + Vector3.Down * model.PhysicsBounds.Mins.z,
			Rotation = modelRotation,
			Model = model
		};

		// Let's make sure physics are ready to go instead of waiting
		ent.SetupPhysicsFromModel(PhysicsMotionType.Dynamic);

		// If there's no physics model, create a simple OBB
		if (!ent.PhysicsBody.IsValid())
		{
			ent.SetupPhysicsFromOBB(PhysicsMotionType.Dynamic, ent.CollisionBounds.Mins, ent.CollisionBounds.Maxs);
		}
	}
	static async void SetMaterial(string matname, Entity owner)
	{

		var tr = Trace.Ray(owner.AimRay.Position, owner.AimRay.Position + owner.AimRay.Forward * 500)
			.UseHitboxes()
			.Ignore(owner)
			.Run();

		//
		// Does this look like a package?
		//
		if (matname.Count(x => x == '.') == 1 && !matname.EndsWith(".vmat", System.StringComparison.OrdinalIgnoreCase) && !matname.EndsWith(".vmat_c", System.StringComparison.OrdinalIgnoreCase))
		{
			matname = await SpawnPackageMaterial(matname, owner);
			if (matname == null)
				return;
		}

		Log.Info($"Setting material {matname}");

		var material = Material.Load(matname);
		//if ( material == null || material.IsPromise )
		//return;

		if (tr.Entity is ModelEntity mdlent)
		{
			mdlent.SetMaterialOverride(material);
		}
	}
}
