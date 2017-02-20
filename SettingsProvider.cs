
namespace UCNLSalinity
{
    public abstract class SettingsProvider<T> where T : class, new()
    {
        #region Properties

        public T Data { get; set; }
        public bool isSwallowExceptions { get; set; }

        #endregion

        #region Constructor

        protected SettingsProvider()
        {
            isSwallowExceptions = true;
        }

        #endregion

        #region Methods

        public abstract void Save(string fileName);

        public abstract void Load(string fileName);

        #endregion
    }
}
