using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using TALXIS.DevKit.Build.Dataverse.Plugin;

namespace Plugins.Warehouse
{
    [PluginStep("Create", "lab_warehousetransaction", PluginStage.PostOperation)]
    public sealed class SubtractQuantityPlugin : PluginBase
    {
        private const int Inbound = 100000000;
        private const int Outbound = 100000001;

        public SubtractQuantityPlugin(string unsecureConfiguration, string secureConfiguration)
            : base(typeof(SubtractQuantityPlugin))
        {
        }

        protected override void ExecuteDataversePlugin(ILocalPluginContext localPluginContext)
        {
            var context = localPluginContext.PluginExecutionContext;
            if (!context.InputParameters.Contains("Target") ||
                !(context.InputParameters["Target"] is Entity target) ||
                target.LogicalName != "lab_warehousetransaction" ||
                !target.Contains("lab_quantity") ||
                !target.Contains("lab_itemid") ||
                !target.Contains("lab_transactiontype"))
            {
                return;
            }

            var quantity = (int)target["lab_quantity"];
            var itemReference = (EntityReference)target["lab_itemid"];
            var transactionType = (OptionSetValue)target["lab_transactiontype"];
            var service = localPluginContext.OrgSvcFactory.CreateOrganizationService(context.UserId);
            var item = service.Retrieve(
                "lab_warehouseitem",
                itemReference.Id,
                new ColumnSet("lab_availablequantity"));
            var available = item.GetAttributeValue<int>("lab_availablequantity");

            if (transactionType.Value == Inbound)
            {
                item["lab_availablequantity"] = available + quantity;
            }
            else if (transactionType.Value == Outbound)
            {
                item["lab_availablequantity"] = available - quantity;
            }
            else
            {
                return;
            }

            service.Update(item);
        }
    }
}
