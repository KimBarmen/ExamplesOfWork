// <auto-generated/>
#pragma warning disable 1591
#pragma warning disable 0414
#pragma warning disable 0649
#pragma warning disable 0169

namespace ElectronBoilerplate.Pages
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Components;
#nullable restore
#line 1 "C:\Users\kim.gundersen\source\Observer 2.0\Code\_Imports.razor"
using System.Net.Http;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\kim.gundersen\source\Observer 2.0\Code\_Imports.razor"
using Microsoft.AspNetCore.Authorization;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "C:\Users\kim.gundersen\source\Observer 2.0\Code\_Imports.razor"
using Microsoft.AspNetCore.Components.Authorization;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "C:\Users\kim.gundersen\source\Observer 2.0\Code\_Imports.razor"
using Microsoft.AspNetCore.Components.Forms;

#line default
#line hidden
#nullable disable
#nullable restore
#line 5 "C:\Users\kim.gundersen\source\Observer 2.0\Code\_Imports.razor"
using Microsoft.AspNetCore.Components.Routing;

#line default
#line hidden
#nullable disable
#nullable restore
#line 6 "C:\Users\kim.gundersen\source\Observer 2.0\Code\_Imports.razor"
using Microsoft.AspNetCore.Components.Web;

#line default
#line hidden
#nullable disable
#nullable restore
#line 7 "C:\Users\kim.gundersen\source\Observer 2.0\Code\_Imports.razor"
using Microsoft.AspNetCore.Components.Web.Virtualization;

#line default
#line hidden
#nullable disable
#nullable restore
#line 8 "C:\Users\kim.gundersen\source\Observer 2.0\Code\_Imports.razor"
using Microsoft.JSInterop;

#line default
#line hidden
#nullable disable
#nullable restore
#line 9 "C:\Users\kim.gundersen\source\Observer 2.0\Code\_Imports.razor"
using ElectronBoilerplate;

#line default
#line hidden
#nullable disable
#nullable restore
#line 10 "C:\Users\kim.gundersen\source\Observer 2.0\Code\_Imports.razor"
using ElectronBoilerplate.Shared;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "C:\Users\kim.gundersen\source\Observer 2.0\Code\Pages\Settings.razor"
using ElectronBoilerplate.Data;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "C:\Users\kim.gundersen\source\Observer 2.0\Code\Pages\Settings.razor"
using System.IO;

#line default
#line hidden
#nullable disable
    [Microsoft.AspNetCore.Components.RouteAttribute("/settings")]
    public partial class Settings : Microsoft.AspNetCore.Components.ComponentBase
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder __builder)
        {
        }
        #pragma warning restore 1998
#nullable restore
#line 64 "C:\Users\kim.gundersen\source\Observer 2.0\Code\Pages\Settings.razor"
       
    private SettingsDto settingsCollection;
    IBrowserFile selectedFile;

    private void FormatTextArea()
    {

    }

    protected override async Task OnInitializedAsync()
    {
        settingsCollection = await new SettingsService().GetSettings("");
    }

    private void OnInputFileChange(InputFileChangeEventArgs e)
    {
        selectedFile = e.GetMultipleFiles().FirstOrDefault();
        this.StateHasChanged();
    }

    private async void LoadSettingsFromFile()
    {
        var fileName = selectedFile?.Name ?? settingsCollection.PathToSettingsJson;
        settingsCollection = await new SettingsService().GetSettings(fileName);
    }

    private void SaveSettingsToFile()
    {
        new SettingsService().PostSettings(
            settingsCollection,
            selectedFile?.Name ?? settingsCollection.PathToSettingsJson
        );
    }

    private void AddNewSetting()
    {
        var tmpList = settingsCollection.SettingList.ToList();
        tmpList.Add(new() { Name = "NewItem", Value = "0", Unit = "Int" });
        settingsCollection.SettingList = tmpList.ToArray();
    }

#line default
#line hidden
#nullable disable
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private SettingsService SettingsService { get; set; }
    }
}
#pragma warning restore 1591