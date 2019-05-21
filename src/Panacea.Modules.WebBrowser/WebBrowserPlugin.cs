﻿using Panacea.Core;
using Panacea.Modularity.UiManager;
using Panacea.Modularity.UiManager.Extensions;
using Panacea.Modularity.WebBrowsing;
using Panacea.Modules.WebBrowser.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panacea.Modules.WebBrowser
{
    public class WebBrowserPlugin : IWebBrowserPlugin, ICallablePlugin
    {
        private readonly PanaceaServices _core;
        private IWebViewManager _webViewManager;
        public WebBrowserPlugin(PanaceaServices core)
        {
            _core = core;
        }
        public Task BeginInit()
        {
            return Task.CompletedTask;
        }

        async Task<bool> GetWebViewManager()
        {
            if (_webViewManager == null)
            {
                var plugin = _core.PluginLoader.GetPlugins<IWebViewPlugin>().FirstOrDefault();
                if (plugin == null)
                {
                    return false;
                }
                _webViewManager = await plugin.GetWebViewManagerAsync();
            }
            return true;
        }

        public async void Call()
        {
            if (await GetWebViewManager())
            {
                _core.GetUiManager()
                    .Navigate(new WebBrowserPageViewModel(_webViewManager));
            }
        }

        public void Dispose()
        {

        }

        public Task EndInit()
        {
            return Task.CompletedTask;
        }

        public void OpenUnmanaged(string url, IWebViewManager manager, bool blockDomains = true, List<string> allowedDomains = null)
        {
            var tab = manager.CreateTab(url);

            var vm = new UnmanagedTabViewModel(tab);

            _core.GetUiManager().Navigate(vm, false);
        }

        public async void OpenUnmanaged(string url, bool blockDomains = true, List<string> allowedDomains = null)
        {
            if (await GetWebViewManager())
            {
                var tab = _webViewManager.CreateTab(url);

                var vm = new UnmanagedTabViewModel(tab);

                _core.GetUiManager().Navigate(vm, false);
            }
        }

        public void OpenUrl(string url)
        {
            throw new NotImplementedException();
        }

        public Task Shutdown()
        {
            return Task.CompletedTask;
        }
    }
}
