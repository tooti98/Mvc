// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.Mvc.Core;
using Microsoft.AspNet.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNet.Mvc
{
    public class RedirectResult : ActionResult, IKeepTempDataResult
    {
        private string _url;

        public RedirectResult(string url)
            : this(url, permanent: false)
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }
        }

        public RedirectResult(string url, bool permanent)
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException(Resources.ArgumentCannotBeNullOrEmpty, nameof(url));
            }

            Permanent = permanent;
            Url = url;
        }

        public bool Permanent { get; set; }

        public string Url
        {
            get
            {
                return _url;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException(Resources.ArgumentCannotBeNullOrEmpty, nameof(value));
                }

                _url = value;
            }
        }

        public IUrlHelper UrlHelper { get; set; }

        public override void ExecuteResult(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var urlHelper = GetUrlHelper(context);

            // IsLocalUrl is called to handle  Urls starting with '~/'.
            var destinationUrl = Url;
            if (urlHelper.IsLocalUrl(destinationUrl))
            {
                destinationUrl = urlHelper.Content(Url);
            }

            context.HttpContext.Response.Redirect(destinationUrl, Permanent);
        }

        private IUrlHelper GetUrlHelper(ActionContext context)
        {
            return UrlHelper ?? context.HttpContext.RequestServices.GetRequiredService<IUrlHelper>();
        }
    }
}