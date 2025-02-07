﻿using AnimalBehaviours;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using VFE.Mechanoids.Needs;
using VFECore;

namespace VFE.Mechanoids
{
    public class CompMachine : CompDependsOnBuilding, PawnGizmoProvider
    {
        public ThingDef turretAttached = null;
        public float turretAngle = 0f; //Purely cosmetic, don't need to save it
        public float turretAnglePerFrame = 0.1f;

        public static Dictionary<PawnRenderer, CompMachine> cachedMachines = new Dictionary<PawnRenderer, CompMachine>();
        public static Dictionary<CompMachine, Pawn> cachedPawns = new Dictionary<CompMachine, Pawn>();
        public static Dictionary<Pawn, CompMachine> cachedMachinesPawns = new Dictionary<Pawn, CompMachine>();

        public override void OnBuildingDestroyed(CompPawnDependsOn compPawnDependsOn)
        {
            base.OnBuildingDestroyed(compPawnDependsOn);
            if (compPawnDependsOn.Props.killPawnAfterDestroying)
            {
                parent.Kill();
            }
        }

        public new CompProperties_Machine Props
        {
            get
            {
                return (CompProperties_Machine)this.props;
            }
        }

        public void AttachTurret(ThingDef turret)
        {
            if(turretAttached!=null)
            {
                foreach(ThingDefCountClass stack in turretAttached.costList)
                {
                    Thing thing = ThingMaker.MakeThing(stack.thingDef);
                    thing.stackCount = stack.count;
                    GenPlace.TryPlaceThing(thing, parent.Position, parent.Map, ThingPlaceMode.Near);
                }
                ((Pawn)parent).equipment.DestroyAllEquipment();
            }
            turretAttached = turret;
            Thing turretThing = ThingMaker.MakeThing(turret.building.turretGunDef);
            ((Pawn)parent).equipment.AddEquipment((ThingWithComps)turretThing);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Defs.Look<ThingDef>(ref turretAttached, "turretAttached");
        }

        public override void CompTick()
        {
            base.CompTick();
            if(turretAttached!=null)
            {
                turretAngle += turretAnglePerFrame;
            }
        }

        public override void CompTickRare()
        {
            base.CompTickRare();
            turretAnglePerFrame = Rand.Range(-0.5f, 0.5f);
        }
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            cachedMachines.Add(((Pawn)parent).Drawer.renderer, this);
            cachedPawns.Add(this, (Pawn)parent);
            cachedMachinesPawns.Add((Pawn)parent, this);
        }

        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);
            cachedMachines.Remove(((Pawn)parent).Drawer.renderer);
            cachedPawns.Remove(this);
            cachedMachinesPawns.Remove((Pawn)parent);
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);
            cachedMachines.Remove(((Pawn)parent).Drawer.renderer);
            cachedPawns.Remove(this);
            cachedMachinesPawns.Remove((Pawn)parent);
        }

        public IEnumerable<Gizmo> GetGizmos()
        {
            if (Prefs.DevMode)
            {
                yield return new Command_Action
                {
                    defaultLabel = "Recharge fully",
                    action = delegate ()
                    {
                        (this.parent as Pawn).needs.TryGetNeed<Need_Power>().CurLevel = 1;
                    }
                };
            }
        }
    }
}
