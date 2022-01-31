"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
Object.defineProperty(exports, "__esModule", { value: true });
const lockstep_sdk_1 = require("lockstep-sdk");
console.log("Creating client");
//Api-key
var client = lockstep_sdk_1.LockstepApi.withEnvironment("sbx").withApiKey("Api-key");
console.log("About to call ping");
console.log("Started ping call");
//writing to csv file
const createCsvWriter = require('csv-writer').createObjectCsvWriter;
const csvWriter = createCsvWriter({
    path: './companyData.csv',
    header: [
        { id: 'companyName', title: 'Company Name' },
        { id: 'phone', title: 'Phone Number' },
        { id: 'apEmail', title: 'apEmailAddress' },
        { id: 'arEmail', title: 'arEmailAddress' }
    ]
});
var records = [];
//function to fetch companyNames
function company() {
    return __awaiter(this, void 0, void 0, function* () {
        try {
            var companies = yield client.Companies.queryCompanies("companyName startswith 'a' OR companyName startswith 'b'", "Invoices", "ASC", 100, 0);
        }
        catch (error) {
            console.error(error);
        }
        var pageNumbers = 0;
        while (pageNumbers < 10) {
            var companies = yield client.Companies.queryCompanies("companyName startswith 'a' OR companyName startswith 'b'", "Invoices", "ASC", 100, pageNumbers);
            if (!companies.success || companies.value.records.length == 0) {
                break;
            }
            companies.value.records.forEach((company) => __awaiter(this, void 0, void 0, function* () {
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
                yield csvWriter.writeRecords(records);
            }));
            pageNumbers++;
        }
    });
}
company();
