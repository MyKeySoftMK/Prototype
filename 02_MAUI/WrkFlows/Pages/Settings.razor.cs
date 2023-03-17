using VxFormGenerator.Core;
using System.Dynamic;
using Radzen;
using Radzen.Blazor;

using KMF.WrkFlows.Domain;
using Microsoft.AspNetCore.Components.Forms;

namespace WrkFlows.Pages
{
    public partial class Settings
    {
        private UserSettings _UserSettings = new UserSettings();
        private ClientApplicationSettings _ClientApplicationSettings = new ClientApplicationSettings();
        private ServerData _ServerSettings = new ServerData();

        /// <summary>
        /// Will handle the submit action of the form
        /// </summary>
        /// <param name="context">The model with values as entered in the form</param>
        private void HandleValidSubmit(EditContext context)
        {
            // save your changes
        }

        private void HandleInValidSubmit(EditContext context)
        {
            // Do something
        }

    }
}
