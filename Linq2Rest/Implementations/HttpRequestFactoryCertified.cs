// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpRequestFactoryCertified.cs">
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the public interface for an HTTP request.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LinqCovertTools.Implementations
{
    using Provider;
    using System;
    using System.Security.Cryptography.X509Certificates;

    /// <summary>
    /// Creates an IHttpRequest with the given certificate attached to it
    /// </summary>
    class HttpRequestFactoryCertified : IHttpRequestFactory
    {
        private readonly X509Certificate _clientCertificate;

        public HttpRequestFactoryCertified(X509Certificate clientCertificate)
        {
            _clientCertificate = clientCertificate;
        }

        public IHttpRequest Create(Uri uri, HttpMethod method, string acceptMimeType, string requestMimeType = null)
        {
            var httpWebRequest = HttpWebRequestAdapter.CreateHttpWebRequest(uri, method, acceptMimeType, requestMimeType);

            httpWebRequest.ClientCertificates.Add(_clientCertificate);

            return new HttpWebRequestAdapter(httpWebRequest);
        }
    }
}
