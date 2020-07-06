namespace NetCon.export
{
    using System;
    using System.Data.Entity;

    public partial class EntityContainer : DbContext
    {
        /// <summary>
        /// This constructor changes the context's connection string to a custom one, specified in argument.
        /// </summary>
        /// <param name="connectionString">Custom connection string for the DBContext.</param>
        public EntityContainer(String connectionString)
            : base(connectionString)
        {
        }
    }
}