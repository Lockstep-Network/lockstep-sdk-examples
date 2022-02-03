import * as lockstep from 'lockstep-sdk';

// Note that csv-writer requires ES5 mode
import * as csv from 'csv-writer';

/**
 * 2022-02-03 Lockstep
 * 
 * Example program to generate a report for invoices for your Lockstep Platform account.
 * - Prints output to the console AND writes to a CSV file.
 * - Prints the first ten pages of data.
 * - Uses an environment variable to keep your API key out of the source code.
 * 
 * To run this program, compile it with `tsc --target ES5 companyReport.ts` then run it with `node companyReport.js`.
 * 
 * @param apiKey 
 * @param queryFilter 
 */async function generateCompanyReport(apiKey: string, queryFilter: string) {

    // Create a client
    var client = lockstep.LockstepApi
        .withEnvironment("sbx")
        .withApiKey(apiKey);

    // Create a CSV file for the results
    const csvWriter = csv.createObjectCsvWriter({
        path: './companyData.csv',
        header: [
            { id: 'companyName', title: 'Company Name' },
            { id: 'phone', title: 'Phone Number' },
            { id: 'apEmail', title: 'apEmailAddress' },
            { id: 'arEmail', title: 'arEmailAddress' }
        ]
    });

    // This will be our file format
    var records: {
        companyName: string; phone: string | null; apEmail: string | null; arEmail: string | null;
        }[] = [];

    // Query the first ten pages of results for this filter
    var pageNumber = 0;
    while (pageNumber < 10) {
        var companies = await client.Companies.queryCompanies(queryFilter, undefined, undefined, 100, pageNumber);
        if (!companies.success || !companies?.value?.records) {
            break;
        }

        companies.value.records.forEach(async company => {
            var companyName = company.companyName;
            var phone = company.phoneNumber;
            var apEmail = company.apEmailAddress;
            var arEmail = company.arEmailAddress;

            console.log("company Name:", companyName);
            console.log("company PhoneNumber:", phone);
            console.log("company apEmailAddress:", apEmail);
            console.log("company arEmailAddress:", arEmail);
            console.log(" ");

            records.push({ companyName: companyName, phone: phone, apEmail: apEmail, arEmail: arEmail });
            await csvWriter.writeRecords(records);
        });

        pageNumber++;
    }
}

// Fetch the API key from an environment variable
var apiKey = process.env["LOCKSTEPAPI_SBX"] || "Fill in your API key here";
generateCompanyReport(apiKey, "companyName startswith a OR companyName startswith B");