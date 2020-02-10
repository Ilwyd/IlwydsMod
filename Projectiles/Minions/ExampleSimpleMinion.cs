using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace IlwydsMod.Projectiles.Minions.ExampleSimpleMinion
{
	/*
	 * This file contains all the code necessary for a minion
	 * - ModItem
	 *     the weapon which you use to summon the minion with
	 * - ModBuff
	 *     the icon you can click on to despawn the minion
	 * - ModProjectile 
	 *     the minion itself
	 *     
	 * It is not recommended to put all these classes in the same file. For demonstrations sake they are all compacted together so you get a better overwiew.
	 * To get a better understanding of how everything works together, and how to code minion AI, read the guide: https://github.com/tModLoader/tModLoader/wiki/Basic-Minion-Guide
	 * This is NOT an in-depth guide to advanced minion AI
	 */

	public class ExampleMinionBuff : ModBuff
	{
		public override void SetDefaults() {
			DisplayName.SetDefault("Example Minion");
			Description.SetDefault("The example minion will fight for you");
			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex) {
			if (player.ownedProjectileCounts[ProjectileType<ExampleMinion>()] > 0) {
				player.buffTime[buffIndex] = 18000;
			}
			else {
				player.DelBuff(buffIndex);
				buffIndex--;
			}
		}
	}

	public class ExampleMinionItem : ModItem
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Example Minion Item");
			Tooltip.SetDefault("Summons an example minion to fight for you");
			ItemID.Sets.GamepadWholeScreenUseRange[item.type] = true; // This lets the player target anywhere on the whole screen while using a controller.
			ItemID.Sets.LockOnIgnoresCollision[item.type] = true;
		}

		public override void SetDefaults() {
			item.damage = 30;
			item.knockBack = 3f;
			item.mana = 10;
			item.width = 32;
			item.height = 32;
			item.useTime = 36;
			item.useAnimation = 36;
			item.useStyle = 1;
			item.value = Item.buyPrice(0, 30, 0, 0);
			item.rare = 9;
			item.UseSound = SoundID.Item44;

			// These below are needed for a minion weapon
			item.noMelee = true;
			item.summon = true;
			item.buffType = BuffType<ExampleMinionBuff>();
			// No buffTime because otherwise the item tooltip would say something like "1 minute duration"
			item.shoot = ProjectileType<ExampleMinion>();
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack) {
			// This is needed so the buff that keeps your minion alive and allows you to despawn it properly applies
			player.AddBuff(item.buffType, 2);

			// Here you can change where the minion is spawned. Most vanilla minions spawn at the cursor position.
			position = Main.MouseWorld;
			return true;
		}

		public override void AddRecipes() {
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.SoulofFright, 25);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}

	/*
	 * This minion shows a few mandatory things that make it behave properly. 
	 * Its attack pattern is simple: If an enemy is in range of 43 tiles, it will fly to it and deal contact damage
	 * If the player targets a certain NPC with right-click, it will fly through tiles to it
	 * If it isn't attacking, it will float near the player with minimal movement
	 */
	public class ExampleMinion : ModProjectile
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Example Minion");
			// Sets the amount of frames this minion has on its spritesheet
			Main.projFrames[projectile.type] = 1;

			// These below are needed for a minion
			// Denotes that this projectile is a pet or minion
			Main.projPet[projectile.type] = true;
			// This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
			ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
			// Don't mistake this with "if this is true, then it will automatically home". It is just for damage reduction for certain NPCs
			ProjectileID.Sets.Homing[projectile.type] = true;
		}

		public sealed override void SetDefaults() {
			projectile.width = 40;
			projectile.height = 40;
			// Makes the minion go through tiles freely
			projectile.tileCollide = true;

			// These below are needed for a minion weapon
			// Only controls if it deals damage to enemies on contact (more on that later)
			projectile.friendly = true;
			// Only determines the damage type
			projectile.minion = true;
			// Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
			projectile.minionSlots = 1f;
			// Needed so the minion doesn't despawn on collision with enemies or tiles
			projectile.penetrate = -1;
		}

		//public override NewInstance(Projectile projectileClone) {

		//}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
			return true;
		}

		public override bool CanDamage() {
			return true;
		}

		// Here you can decide if your minion breaks things like grass or pots
		public override bool? CanCutTiles() {
			return false;
		}

		public override void AI() {
			Player player = Main.player[projectile.owner];
			Vector2 playerPos = player.Center;
			float locX = projectile.position.X;
			float locY = projectile.position.Y;

			#region Active check
			// This is the "active check", makes sure the minion is alive while the player is alive, and despawns if not
			if (player.dead || !player.active) {
				player.ClearBuff(BuffType<ExampleMinionBuff>());
			}
			if (player.HasBuff(BuffType<ExampleMinionBuff>())) {
				projectile.timeLeft = 2;
			}
			#endregion

			#region General behavior
			Console.WriteLine(locX + ", " + locY + "\n"+
								playerPos.X + ", " + playerPos.Y + "\n" +
								(playerPos.X - locX) + ", " + (playerPos.Y - locY) +
								"\n----------------------");
			LockDistance(playerPos, projectile, 200);

			ProjectileIntersect(projectile);
			#endregion

			#region Animation and visuals
			// So it will lean slightly towards the direction it's moving
			projectile.rotation = projectile.velocity.X * 0.05f;

			// Some visuals here
			Lighting.AddLight(projectile.Center, Color.White.ToVector3() * 0.78f);
			#endregion
		}

		/*
			This method determines if other, hostile, projectiles are 
			intersecting with the minion.
		
			minion: The minion projectile
		*/
		public void ProjectileIntersect(Projectile minion) {
			//Loop through each projectile in the world
			foreach(Projectile proj in Main.projectile) {
				//If the projectile is hostile and its hitbox intersects with the minion
				if(proj.Hitbox.Intersects(minion.Hitbox) && proj.hostile) {
					ProjectileReflect(proj);
				}
			}
		}

		/*
			This method reverses a projectile's velocity.
		*/
		public void ProjectileReflect(Projectile proj) {
			//Make the projectile non-hostile and friendly
			proj.hostile = false;
			proj.friendly = true;

			//Reversing the projectile's velocity
			proj.velocity = proj.velocity.RotatedBy(Math.PI);
		}

		public void LockDistance(Vector2 playerPos, Projectile proj, float maxDistance) {
			float distanceX = playerPos.X - proj.Center.X;
			float distanceY = playerPos.Y - proj.Center.Y;
			
			if(distanceX > maxDistance) {
				proj.position.X = playerPos.X + maxDistance;
			}
			else if(distanceX < -maxDistance) {
				proj.position.X = playerPos.X - maxDistance;
			}
			else {
				proj.position.X = Main.MouseWorld.X;
			}

			if(distanceY > maxDistance) {
				proj.position.Y = playerPos.Y + maxDistance;
			}
			else if(distanceY < -maxDistance) {
				proj.position.Y = playerPos.Y - maxDistance;
			}
			else {
				proj.position.Y = Main.MouseWorld.Y;
			}
		}
	}
}