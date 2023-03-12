using System;
namespace NewsWaffle.Net
{
	public interface IHttpRequestor
	{
		byte[] BodyBytes { get; }

		string BodyText { get; }

        string ErrorMessage { get; }

		/// <summary>
		/// Sends a GET to an HTTP(S) URL. If succcessful, populates BodyBytes and BodyText.
		/// BodyText created from bytes and any charset specified in HTTP response. Otherwise UTF-8 is used.
        ///
        /// If fails, more detailed error message available
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
        bool Request(Uri url);

        /// <summary>
        /// Sends a GET to an HTTP(S) URL. If succcessful, populates BodyBytes.
        /// If fails, more detailed error message available
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        bool RequestAsBytes(Uri url);

    }
}

