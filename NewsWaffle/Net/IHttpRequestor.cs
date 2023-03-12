using System;
namespace NewsWaffle.Net
{
	public interface IHttpRequestor
	{
		string ErrorMessage { get; }

		byte[] BodyBytes { get; }

		string BodyText { get; }

		bool Request(Uri url);

		bool RequestAsBytes(Uri url);

    }
}

