using System.Linq;
using Umbraco.Web;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Core.Persistence;
using System.Web;
using System.Configuration;

namespace umbraco_dls.Common.EventHandlers
{
    public class CustomApplicationEventHandler : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            ContentService.Saving += ContentService_Saving;

            base.ApplicationStarted(umbracoApplication, applicationContext);
        }

        //Web.config settings
        private readonly string settingsNodeId = ConfigurationManager.AppSettings["DLSRoot"];
        private readonly string logoPickerAlias = ConfigurationManager.AppSettings["DLSLogo"];
        private readonly string backgroundPickerAlias = ConfigurationManager.AppSettings["DLSBackground"];

        private void ContentService_Saving(IContentService sender, SaveEventArgs<IContent> e)
        {
            //Umbraco helper
            var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);

            //If settings exist
            if (string.IsNullOrEmpty(settingsNodeId))
                return;

            var settingsNode = umbracoHelper.TypedContent(settingsNodeId);

            var savedNode = e.SavedEntities
                .Where(c => c.Id == settingsNode?.Id)
                .FirstOrDefault() as IContent;

            if (savedNode == null) return;

            var logoPicker = string.IsNullOrEmpty("logoPickerAlias") ? umbracoHelper.TypedMedia(logoPickerAlias) : umbracoHelper.TypedMedia(logoPickerAlias);

            if (logoPicker == null)
                ThrowInvalidAliasError(e, "Logo");



        }

        private void ThrowInvalidAliasError(SaveEventArgs<IContent> e, string pickerTitle){
            e.CancelOperation(new EventMessage("Invalid alias", $"{pickerTitle} alias dosen't exist. Enter a valid alias.", EventMessageType.Error));
        }
    }
}