using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MVVM;
using TeamBuilderWebAPI_Tester.Annotations;

namespace TeamBuilderWebAPI_Tester
{
	internal class ViewModel : INotifyPropertyChanged
    {

	    #region [ Enums ]

	    internal enum HttpVerb
	    {
			GET,
			POST,
			PUT,
			DELETE
	    }

	    #endregion

		#region [ Properties ]


		public string Token
		{
			get => token;
			set
			{
				token = value;
				Notify();
			}
		}
		private string token;


		public string Request
		{
			get => request;
			set
			{
				request = value;
				Notify();
			}
		}
		private string request;


		public HttpVerb Verb
		{
			get => verb;
			set
			{
				verb = value;
				Notify();
			}
		}
		private HttpVerb verb;
		
	    public IEnumerable<HttpVerb> VerbSource { get; } = new ObservableCollection<HttpVerb>((HttpVerb[])Enum.GetValues(typeof(HttpVerb)));


		public string Uri
		{
			get => uri;
			set
			{
				uri = value;
				Notify();
			}
		}
		private string uri;

	    public string ResponseToken
	    {
		    get => responseToken;
		    set
		    {
			    responseToken = value;
			    Notify();
		    }
	    }
	    private string responseToken;

	    public string ResponseBody
	    {
		    get => responseBody;
		    set
		    {
			    responseBody = value;
			    Notify();
		    }
	    }
	    private string responseBody = "null";

	    public bool ButtonActive
	    {
		    get => buttonActive;
		    set
		    {
			    buttonActive = value;
			    Notify();
		    }
	    }
	    private bool buttonActive = true;

		#endregion

		#region [ Commands ]


	    public RelayCommand GoCommand => goCommand ?? (goCommand = new RelayCommand(Go, (o) => ButtonActive && IsValidUri()));

	    private RelayCommand goCommand;

		#endregion

		#region [ Methods ]

	    private async void Go(object param)
	    {
		    ButtonActive = false;
		    await GetResponse(Verb, Uri, Token);
		    ButtonActive = true;
	    }

	    private async Task GetResponse(HttpVerb httpVerb, string requestUri, string loginToken)
	    {
		    var httpRequest = WebRequest.CreateHttp(requestUri);
		    httpRequest.Method = httpVerb.ToString();
		    httpRequest.Headers[TeambuilderLibrary.Contracts.Constants.TokenKey] = loginToken;
		    httpRequest.UserAgent = "This is not the agent you are looking for";
		    httpRequest.AllowAutoRedirect = true;
		    HttpWebResponse response = null;
			try
			{
				response = (HttpWebResponse)await httpRequest.GetResponseAsync();
			}
			catch (WebException ex)
			{
				if (ex.Message.Contains("404"))
				{
					ResponseBody = "404 not found";
					return;
				}
				else if(ex.Message.Contains("401"))
				{
					ResponseBody = "401 Unauthorized";
					return;
				}
				throw;
			}

		    ResponseToken = response.Headers[TeambuilderLibrary.Contracts.Constants.TokenKey];
		    using (var stream = response.GetResponseStream())
		    {
			    ResponseBody = stream != null
				    ? await new StreamReader(stream, Encoding.UTF8, true).ReadToEndAsync()
				    : "null";
		    }
	    }


	    private bool IsValidUri() => System.Uri.IsWellFormedUriString(uri, UriKind.Absolute);

	    #endregion

		#region [ INotifyPropertyChanged ]

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void Notify([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		} 

		#endregion
	}
}
