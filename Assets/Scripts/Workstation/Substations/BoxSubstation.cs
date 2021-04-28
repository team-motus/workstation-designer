namespace WorkstationDesigner.Workstation.Substations
{
    class BoxSubstation : PrefabSubstationBase
    {
        private const string PrefabPath = "Prefabs/MODS/Substations/CardboardBox";

        public BoxSubstation(string name, int footprintLength1, int footprintLength2) : base(name, footprintLength1, footprintLength2, PrefabPath) { }
    }
}
