﻿using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace Mear.Models.Authentication
{
	public class LoginResult : BaseResult
	{
		[JsonProperty("id")]
		public int UserId { get; set; }
		[JsonProperty("username")]
		public string Username { get; set; }
		[JsonProperty("token")]
		public string Token { get; set; }
		[JsonProperty("token_type")]
		public string TokenType { get; set; }
		[JsonProperty("expiration")]
		public int Expiration { get; set; }
	}
}