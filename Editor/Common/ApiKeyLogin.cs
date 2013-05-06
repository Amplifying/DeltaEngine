using System;

namespace DeltaEngine.Editor.Common
{
	public class ApiKeyLogin : IEquatable<ApiKeyLogin>
	{
		public string ApiKey { get; set; }

		public override bool Equals(object other)
		{
			var otherLogin = other as ApiKeyLogin;
			return otherLogin != null && Equals(otherLogin);
		}

		public bool Equals(ApiKeyLogin other)
		{
			return other.ApiKey == ApiKey;
		}

		public override int GetHashCode()
		{
			return ApiKey != null ? ApiKey.GetHashCode() : 0;
		}
	}
}