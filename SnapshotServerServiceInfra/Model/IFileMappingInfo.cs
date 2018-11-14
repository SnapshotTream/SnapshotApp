using System;

namespace Snapshot.Server.Service.Infra.Model
{
    public interface IFileMappingInfo : Snapshot.Share.Common.Infra.Model.IFileMappingInfo
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="workspace"></param>
        void SetWorkspace(IWorkspace workspace);

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        IWorkspace GetWorkspace();
    }
}
