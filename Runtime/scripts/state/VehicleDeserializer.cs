namespace UDrive
{
    public abstract class VehicleDeserializer
    {
        protected UVehicle Vehicle;

        public virtual void SetVehicle(UVehicle vehicle)
        {
            Vehicle = vehicle;
        }

        public abstract void RunUpdate();
    }
}