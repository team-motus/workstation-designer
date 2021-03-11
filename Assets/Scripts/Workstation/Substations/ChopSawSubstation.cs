namespace WorkstationDesigner.Workstation.Substations
{
    public class ChopSawSubstation : PrefabSubstationBase
    {
        private const string PrefabPath = "Prefabs/MODS/Substations/ChopsawTable";

        public ChopSawSubstation(string name, int footprintLength1, int footprintLength2) : base(name, footprintLength1, footprintLength2, PrefabPath) { }
    }
}
