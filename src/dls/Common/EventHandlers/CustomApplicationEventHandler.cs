using System.Linq;
using Umbraco.Web;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Core.Persistence;
using System.Web;
using System.Configuration;
using System.Collections.Generic;
using Newtonsoft.Json;

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
                .FirstOrDefault();

            if (savedNode == null) return;

            var css = "";


            //Logotype
            if (!string.IsNullOrEmpty(logoPickerAlias))
            {
                var property = savedNode.Properties[logoPickerAlias];
               
                //Types uid url
                var typedLogo = umbracoHelper.TypedMedia(property.Value);

                if (typedLogo != null)
                    css += ".login-overlay__logo img{content:url('" + typedLogo.Url + "'); height: 40px;}";
                else
                    ThrowInvalidAliasError(e, "Logotype");
            }

            //Background
            if (!string.IsNullOrEmpty(backgroundPickerAlias))
            {
                //Get properties
                var properties = savedNode.Properties[backgroundPickerAlias];
                var propertyEditorAlias = properties.PropertyType.PropertyEditorAlias;
                var uidBackgroundUrl = "";


                if (propertyEditorAlias == "Umbraco.NestedContent")
                {
                    var nestedContentItems = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(savedNode.GetValue<string>(backgroundPickerAlias));

                    foreach (var nestedContentItem in nestedContentItems)
                    {
                        var uidMedia = nestedContentItem.Where(x => $"{x.Value}".Contains("media")).FirstOrDefault();

                        if (uidMedia.Value != null)
                        {
                            uidBackgroundUrl = $"{uidMedia.Value}";
                            break;
                        }
                    }

                    if (uidBackgroundUrl == null)
                    {
                        e.CancelOperation(new EventMessage("No image", "No image was found in the neasted content.", EventMessageType.Error));
                        return;
                    }
                }
                else
                {
                    //Get uid url
                    uidBackgroundUrl = $"{properties.Value}";
                }

                //Types uid url
                var typedBackground = umbracoHelper.TypedMedia(uidBackgroundUrl.Split(',').FirstOrDefault());

                if (typedBackground != null)
                    css += ".login-overlay__background-image { background:url('" + typedBackground.Url + "') !important;background-position: 50% !important;background-repeat: no-repeat !important;background-size: cover !important;opacity:0.5 !important;}";
                else
                    ThrowInvalidAliasError(e, "Background");
            }

            //Path to css file
            var path = HttpContext.Current.Server.MapPath("/App_Plugins/DLS/css/login.css");

            //Writes to css file
            System.IO.File.WriteAllText(path, css);
        }

        private void ThrowInvalidAliasError(SaveEventArgs<IContent> e, string pickerTitle)
        {
            e.CancelOperation(new EventMessage("Invalid alias", $"{pickerTitle} alias dosen't exist. Enter a valid alias.", EventMessageType.Error));
        }
    }
}