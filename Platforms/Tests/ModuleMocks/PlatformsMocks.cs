
namespace DeltaEngine.Platforms.Tests.ModuleMocks
{
	public class PlatformsMocks : BaseMocks
	{
		internal PlatformsMocks(AutofacStarterForMockResolver resolver)
			: base(resolver)
		{
			resolver.Register<MockInAppPurchase>();
		}
	}

	internal class MockInAppPurchase : InAppPurchase
	{
		public override bool RequestProductInformationAsync(int[] productIds)
		{
			var product = new ProductData("testId", "testTitle", "testDescription", "testPrice", true);
			InvokeOnReceivedProductInformation(new[] {product});
			return true;
		}

		public override bool PurchaseProductAsync(string productId)
		{
			InvokeOnTransactionFinished(productId, true);
			return true;
		}
	}
}
