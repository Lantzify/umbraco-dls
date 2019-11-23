using System.Linq;
using Umbraco.Web;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Core.Persistence;
using System.Web;
using System;

//TODO: NAMESPACE ON INSTALL

namespace umbraco_dls.App_Plugins.DLS.Common.EventHandlers
{
    public class CustomApplicationEventHandler : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            ContentService.Saving += ContentService_Saving;

            base.ApplicationStarted(umbracoApplication, applicationContext);
        }

        private void ContentService_Saving(IContentService sender, SaveEventArgs<IContent> e)
        {
            //Umbraco helper
            var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);

            //Settings node
            var settingsNode = umbracoHelper
                .TypedContentAtRoot()
                .ToList()
                .Where(x => x.DocumentTypeAlias == "umbracoLoginScreen")
                .FirstOrDefault();
            
            var savedSettingNode = e.SavedEntities
                .Where(x => x.Id == settingsNode.Id)
                .FirstOrDefault() as IContent;

            //Textfield alias, check what node is saved
            var alias = savedSettingNode == null ?
                //If not settings node get as usual
                settingsNode.GetPropertyValue<string>("alias") :
                //If settings node saved get updated value
                savedSettingNode.Properties["alias"].Value.ToString();


            //Content picker, check what node is saved
            var contentPicker = savedSettingNode == null ?
                //If not settings node get as usual
                settingsNode.GetPropertyValue<IPublishedContent>("node") :
                //If settings node saved get updated value
                umbracoHelper.TypedContent(savedSettingNode.Properties["node"].Value);

            //Checks if given alias exist
            if (savedSettingNode != null && contentPicker != null && !string.IsNullOrWhiteSpace(alias))
            {
                if (!contentPicker.HasValue(alias))
                {
                    e.CancelOperation(new EventMessage("Invalid alias", "The given alias dosen't exist. Enter a valid alias.", EventMessageType.Error));

                    return;
                }
            }

            //Picked node saved
            var savedNode = e.SavedEntities
                .Where(c => c.Id == contentPicker?.Id)
                .FirstOrDefault() as IContent;

            if (savedNode == null) return;

            var css = "";
            
            //Path to css file
            var path = HttpContext.Current.Server.MapPath("/App_Plugins/DLS/css/loginbackground.css");


            //If no alias is assigned, find first media picker of selected node
            if (string.IsNullOrWhiteSpace(alias))
            {
                alias = savedNode.PropertyGroups
                .OrderBy(x => x.SortOrder)
                .SelectMany(x => x.PropertyTypes.OrderBy(c => c.SortOrder))
                .Where(x => x.PropertyEditorAlias == "Umbraco.MediaPicker2")
                .FirstOrDefault().Alias;
            }

            //Udi Url
            var dataType = savedNode.Properties[alias].Value.ToString();
            
            //Typed Media
            var background = umbracoHelper.TypedMedia(dataType);
            
            //Background
            if (background != null)
            {
                css = ".login-overlay__background-image { background:url('" + background.Url + "') !important;background-position: 50% !important;background-repeat: no-repeat !important;background-size: cover !important;opacity:0.5 !important;}";
            }

            //Writes to css file
            System.IO.File.WriteAllText(path, css);
        }
    }
}