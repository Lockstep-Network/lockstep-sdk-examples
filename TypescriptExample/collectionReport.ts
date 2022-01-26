
import { LockstepApi } from 'lockstep-sdk';

console.log("Creating client");
//Api-key
var client = LockstepApi.withEnvironment("sbx").withApiKey("Api-key");
console.log("About to call ping");

console.log("Started ping call");
//writing to csv file
const createCsvWriter = require('csv-writer').createObjectCsvWriter;
const csvWriter = createCsvWriter({
    path: './collectionReport.csv',
    header: [
        {id: 'invoiceId', title: 'Invoice Id'},
        {id: 'invoiceDate', title: 'Invoice Date'},
        {id: 'outstandingBalance', title: 'Outstanding Balance'},
        {id: 'primaryContact', title: 'Primary Contact'}
    ]
});
var records=[];
async function invoice() {
    var pageNumbers = 0;
    
    while (true) 
    {
        var invoices = await client.Invoices.queryInvoices("invoiceDate < 2021-12-01 AND outstandingBalanceAmount > 0", "Customer", "invoiceDate asc", 100, pageNumbers);
        if(!invoices.success || invoices.value.records.length == 0) 
        {
            break;
        }
        invoices.value.records.forEach(async invoice => 
        {
            
            var invoiceId=invoice.invoiceId;
            var invoiceDate=invoice.invoiceDate;
            var outstandingBalance=invoice.outstandingBalanceAmount;
            var contatcName=invoice.customerPrimaryContact.contactName;

            console.log("Invoice Id:", invoiceId);
            console.log("Invoice Date:", invoiceDate);
            console.log("OutStanding Amount:", outstandingBalance);
            console.log("CustomerId:", contatcName);
            console.log(" ");

            records.push({invoiceId: invoiceId,  invoiceDate:invoiceDate, outstandingBalance:outstandingBalance, contatcName:contatcName});
            await csvWriter.writeRecords(records);
            
        });

        pageNumbers++;
    }
}

invoice();


        


            