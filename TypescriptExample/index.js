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
function invoice() {
    return __awaiter(this, void 0, void 0, function* () {
        try {
            var invoices = yield client.Invoices.queryInvoices("invoiceDate > 2021-12-01", "Customer", "invoiceDate asc", 200, 0);
        }
        catch (error) {
            console.error(error);
        }
        var pageNumbers = 0;
        while (pageNumbers < 10) {
            var invoices = yield client.Invoices.queryInvoices("invoiceDate > 2021-12-01", "Customer", "invoiceDate asc", 200, pageNumbers);
            if (!invoices.success || invoices.value.records.length == 0) {
                break;
            }
            invoices.value.records.forEach(invoice => {
                console.log("Invoice Id:", invoice.invoiceId);
                console.log("OutStanding Amount:", invoice.outstandingBalanceAmount);
                console.log("Customer Company Name:", invoice.company.companyName);
                console.log(" ");
            });
            pageNumbers++;
        }
    });
}
invoice();
