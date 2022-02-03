import * as lockstep from 'lockstep-sdk';

/**
 * 2022-02-03 Lockstep
 * 
 * Example program to generate a report for invoices for your Lockstep Platform account.
 * - Prints output to the console.
 * - Prints the first ten pages of data.
 * - Uses an environment variable to keep your API key out of the source code.
 * 
 * To run this program, compile it with `tsc index.ts` then run it with `node index.js`.
 * 
 * @param apiKey 
 * @param queryFilter 
 */
async function generateInvoiceReport(apiKey: string, queryFilter: string) {

    // Create a client
    var client = lockstep.LockstepApi
        .withEnvironment("sbx")
        .withApiKey(apiKey);

    // Fetch the first ten pages of data for this query filter
    var pageNumber = 0;
    while (pageNumber < 10) {
        var invoices = await client.Invoices.queryInvoices(queryFilter, "Customer", "invoiceDate asc", 200, pageNumber);
        if (!invoices.success || !invoices?.value?.records) {
            break;
        }

        invoices.value.records.forEach(invoice => {
            console.log("Invoice Id:", invoice.invoiceId);
            console.log("OutStanding Amount:", invoice.outstandingBalanceAmount);
            console.log("Customer Company Name:", invoice?.company?.companyName);
            console.log(" ");
        });

        pageNumber++;
    }
}

// Fetch the API key from an environment variable
var apiKey = process.env["LOCKSTEPAPI_SBX"] || "Fill in your API key here";
generateInvoiceReport(apiKey, "invoiceDate > 2021-12-01");
