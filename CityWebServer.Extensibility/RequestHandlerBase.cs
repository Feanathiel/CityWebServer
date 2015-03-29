﻿using System;
using System.Net;
using CityWebServer.Extensibility.ResponseFormatters;
using CityWebServer.Extensibility.Responses;

namespace CityWebServer.Extensibility
{
    public abstract class RequestHandlerBase : IRequestHandler
    {
        /// <summary>
        /// Gets a unique identifier for this handler.  Only one handler can be loaded with a given identifier.
        /// </summary>
        public abstract Guid HandlerID { get; }

        /// <summary>
        /// Gets the priority of this request handler.  A request will be handled by the request handler with the lowest priority.
        /// </summary>
        public abstract int Priority { get; }

        /// <summary>
        /// Gets the display name of this request handler.
        /// </summary>
        public abstract String Name { get; }

        /// <summary>
        /// Gets the author of this request handler.
        /// </summary>
        public abstract String Author { get; }

        /// <summary>
        /// Gets the absolute path to the main page for this request handler.  Your class is responsible for handling requests at this path.
        /// </summary>
        /// <remarks>
        /// When set to a value other than <c>null</c>, the Web Server will show this url as a link on the home page.
        /// </remarks>
        public abstract String MainPath { get; }

        /// <summary>
        /// Returns a value that indicates whether this handler is capable of servicing the given request.
        /// </summary>
        public abstract Boolean ShouldHandle(IRequestParameters request);

        /// <summary>
        /// Handles the specified request.  The method should not close the stream.
        /// </summary>
        public abstract IResponseFormatter Handle(IRequestParameters request);

        /// <summary>
        /// Returns a response in JSON format.
        /// </summary>
        protected IResponseFormatter JsonResponse<T>(T content, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return new JsonResponseFormatter<T>(content, statusCode);
        }

        /// <summary>
        /// Returns a response in HTML format.
        /// </summary>
        protected IResponseFormatter HtmlResponse(String content, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return new HtmlResponseFormatter(content, statusCode);
        }

        /// <summary>
        /// Returns a response in plain text format.
        /// </summary>
        protected IResponseFormatter PlainTextResponse(String content, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return new PlainTextResponseFormatter(content, statusCode);
        }

        /// <summary>
        /// Redirects the user to the new url.
        /// </summary>
        protected IResponseFormatter RedirectResponse(String url)
        {
            return new RedirectResponseFormatter(url);
        }
    }
}