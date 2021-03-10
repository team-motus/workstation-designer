namespace WorkstationDesigner.Workstation.Substations
{
    class WorkbenchSubstation : PrefabSubstationBase
    {
        private const string PrefabPath = "Prefabs/MODS/Substations/Workbench_5ftx3ft";

        public WorkbenchSubstation(string name, int footprintLength1, int footprintLength2) : base(name, footprintLength1, footprintLength2, PrefabPath) { }
    }
}
