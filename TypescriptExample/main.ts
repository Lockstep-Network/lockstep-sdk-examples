import { LockstepApi } from 'lockstep-sdk';

console.log("Creating client");
var client = LockstepApi.withEnvironment("sbx").withApiKey("Api-key");
console.log("About to call ping");

console.log("Started ping call");
async function invoice() {
    try 
    {
        var invoices = await client.Invoices.queryInvoices("invoiceDate > 2021-12-01", "Customer", "invoiceDate asc", 200, 0);
    }
    catch (error) 
    {
        console.error(error);
    }    
    var pageNumbers = 0;
    while (pageNumbers<10) 
    {
        var invoices = await client.Invoices.queryInvoices("invoiceDate > 2021-12-01", "Customer", "invoiceDate asc", 200, pageNumbers);
        if(!invoices.success || invoices.value.records.length == 0) 
        {
            break;
        }

        invoices.value.records.forEach(invoice => 
        {
            console.log("Invoice Id:", invoice.invoiceId);
            console.log("OutStanding Amount:", invoice.outstandingBalanceAmount);
            console.log("Customer Company Name:", invoice.company.companyName);
            console.log(" ");
        });

        pageNumbers++;
    }
}

invoice();