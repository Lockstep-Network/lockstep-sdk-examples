import { LockstepApi } from './node_modules/lockstep-sdk/src/LockstepApi';

console.log("Creating client");
var client = LockstepApi.withEnvironment("sbx").withApiKey("Api-key");
console.log("About to call ping");

console.log("Started ping call");

//function to fetch companyNames
async function company() 
{
    try 
    {
        var companies = await client.Companies.queryCompanies("companyName startswith 'a' OR companyName startswith 'b'","Invoices","ASC",100,0);
    }
    catch (error) 
    {
        console.error(error);
    }    
    var pageNumbers = 0;
    while (pageNumbers<10) 
    {
        var companies = await client.Companies.queryCompanies("companyName startswith 'a' OR companyName startswith 'b'","Invoices","ASC",100,pageNumbers);
        if(!companies.success || companies.value.records.length == 0) 
        {
            break;
        }

        companies.value.records.forEach(company => 
        {
            console.log("company Name:", company.companyName);
            console.log("company PhoneNumber:", company.phoneNumber);
            console.log("company apEmailAddress:", company.apEmailAddress);
            console.log("company arEmailAddress:", company.arEmailAddress);
            console.log(" ");
        });

        pageNumbers++;
    }
}
company();