import { LockstepApi } from './node_modules/lockstep-sdk/src/LockstepApi';

console.log("Creating client");
var client = LockstepApi.withEnvironment("sbx").withApiKey("Api-key");
console.log("About to call ping");

console.log("Started ping call");
async function invoice() {
    try {
        var pageNumbers = 0;
        var count = 0;
        while (pageNumbers<10) {
            var invoices = await client.Invoices.queryInvoices("invoiceDate > 2021-12-01", "Customer", "invoiceDate asc", 100, pageNumbers);

            if(!invoices.success || invoices.value.records.length == 0) {
                break;
            }

            invoices.value.records.forEach(invoice => {
                console.log("invoiceId:", invoice.invoiceId);
                //console.log("company Name:", invoice.customer.companyName);//undefined
                console.log("outstanding Balance:", invoice.outstandingBalanceAmount);
            });

            pageNumbers++;
        }
    } catch (error) {
        console.error(error);
    }    
}

invoice();

















// //var apiKey=process.
// export function main()
// {
//     var client = LockstepApi.withEnvironment('prd');
//     client.Status.ping().then(result => {
//         console.log('Result: ' + JSON.stringify(result));
//     })
// }
// main();