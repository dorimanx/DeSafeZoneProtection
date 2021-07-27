using Torch;

namespace DeSafeZoneProtection
{

    public class DeSafeZoneConfig : ViewModel
    {
        private bool _Enabled = false;
        private float _Distance = 2000f;
        private bool _Damage = true;
        private bool _Shooting = true;
        private bool _Welding = true;
        private bool _Building = true;
        private bool _Grinding = true;
        private bool _Drilling = true;
        private bool _VoxelHands = true;
        private bool _LandingGearsLock = true;
        private bool _ConvertToStation = true;
        private bool _BuildingProjections = true;

        public bool Enabled
        {
            get => _Enabled;
            set => SetValue(ref _Enabled, value, "Enabled");
        }

        public float Distance
        {
            get => _Distance;
            set => SetValue(ref _Distance, value, "Distance");
        }

        public bool Damage
        {
            get => _Damage;
            set => SetValue(ref _Damage, value, "Damage");
        }

        public bool Shooting
        {
            get => _Shooting;
            set => SetValue(ref _Shooting, value, "Shooting");
        }

        public bool Welding
        {
            get => _Welding;
            set => SetValue(ref _Welding, value, "Welding");
        }

        public bool Building
        {
            get => _Building;
            set => SetValue(ref _Building, value, "Building");
        }

        public bool Grinding
        {
            get => _Grinding;
            set => SetValue(ref _Grinding, value, "Grinding");
        }

        public bool Drilling
        {
            get => _Drilling;
            set => SetValue(ref _Drilling, value, "Drilling");
        }

        public bool VoxelHands
        {
            get => _VoxelHands;
            set => SetValue(ref _VoxelHands, value, "VoxelHands");
        }

        public bool LandingGearsLock
        {
            get => _LandingGearsLock;
            set => SetValue(ref _LandingGearsLock, value, "LandingGearsLock");
        }

        public bool ConvertToStation
        {
            get => _ConvertToStation;
            set => SetValue(ref _ConvertToStation, value, "ConvertToStation");
        }

        public bool BuildingProjections
        {
            get => _BuildingProjections;
            set => SetValue(ref _BuildingProjections, value, "BuildingProjections");
        }
    }
}
