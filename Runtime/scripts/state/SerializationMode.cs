namespace UDrive
{
    public enum SerializationMode
    {
        None,
        /// <summary>
        /// Serializes only vehicle input and rigidbody state (postion, rotation, velocity)
        /// </summary>
        Network,
        /// <summary>
        /// Stores all serializable vehicle data for 1:1 demo recording
        /// </summary>
        Full
    }
}