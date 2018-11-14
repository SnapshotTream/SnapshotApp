using Snapshot.Server.Service.Infra.Model;

namespace Snapshot.Server.Extention.Sdk {
    public class DefaultCutpoint : IInitPluginCutpoint, IStartCutpoint, ICategoryApiCutpoint, ICreateCategoryCutpoint {
        public void OnInitPlugin (object param) {

        }

        public void OnCreateCategory (ICategory category) {

        }

        public void OnGetCategory (ICategory category) {

        }

        public void Process (object param) {

        }
    }
}