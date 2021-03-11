namespace WorkstationDesigner.Workstation.Substations
{
    public class TableSawSubstation : PrefabSubstationBase
    {
        private const string PrefabPath = "Prefabs/MODS/Substations/TableSawTable";

        public TableSawSubstation(string name, int footprintLength1, int footprintLength2) : base(name, footprintLength1, footprintLength2, PrefabPath) { }
    }
}
