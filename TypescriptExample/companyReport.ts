import { LockstepApi } from 'lockstep-sdk';

console.log("Creating client");
//Api-key
var client = LockstepApi.withEnvironment("sbx").withApiKey("Api-key");
console.log("About to call ping");

console.log("Started ping call");
//writing to csv file
const createCsvWriter = require('csv-writer').createObjectCsvWriter;
const csvWriter = createCsvWriter({
    path: './companyData.csv',
    header: [
        {id: 'companyName', title: 'Company Name'},
        {id: 'phone', title: 'Phone Number'},
        {id: 'apEmail', title: 'apEmailAddress'},
        {id: 'arEmail', title: 'arEmailAddress'}
    ]
});
var records=[];

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

        companies.value.records.forEach(async company => 
        {
            var companyName=company.companyName;
            var phone=company.phoneNumber;
            var apEmail=company.apEmailAddress;
            var arEmail=company.arEmailAddress;

            console.log("company Name:", companyName);
            console.log("company PhoneNumber:", phone);
            console.log("company apEmailAddress:", apEmail);
            console.log("company arEmailAddress:", arEmail);
            console.log(" ");

            records.push({companyName: companyName,  phone:phone, apEmail:apEmail, arEmail:arEmail});
            await csvWriter.writeRecords(records);
        });

        pageNumbers++;
    }
}
company();