namespace Foxpict.Service.Extention.Sdk
{
    public interface IInitPluginCutpoint : ICutpoint
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="param"></param>
        void OnInitPlugin(object param);
    }
}
