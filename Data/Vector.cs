namespace Data
{
    internal record Vector : IVector
    {
        #region IVector

        public double x { get; init; }

        public double y { get; init; }

        #endregion IVector

        /// Creates new instance of <seealso cref="Vector"/> and initialize all properties
        public Vector(double XComponent, double YComponent)
        {
            x = XComponent;
            y = YComponent;
        }
    }
}